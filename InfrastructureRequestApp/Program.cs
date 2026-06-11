using InfrastructureRequestApp.Components;
using InfrastructureRequestApp.Data;
using InfrastructureRequestApp.Data.Services;
using InfrastructureRequestApp.Data.Services.Interfaces;
using InfrastructureRequestApp.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Radzen;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();

builder.Services.AddDbContext<InfraRequestsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.AccessDeniedPath = "/denied";
    });

builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddAuthorizationCore(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole(Roles.Admin));
});

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRequestService, RequestService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddRadzenComponents();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();

// Issues the real auth cookie from a one-time token stored in cache.
// Blazor Server can't write cookies after the WebSocket upgrade, so Login.razor
// stores the validated user ID in cache and redirects here with forceLoad.
app.MapGet("/account/signin", async (
    string token,
    IMemoryCache cache,
    IUserService userService,
    HttpContext httpContext) =>
{
    var cacheKey = $"signin:{token}";
    if (!cache.TryGetValue(cacheKey, out Guid userId))
        return Results.Redirect("/login?error=expired");

    cache.Remove(cacheKey);

    var user = await userService.GetByIdAsync(userId);
    if (user is null || !user.IsActive)
        return Results.Redirect("/login?error=invalid");

    var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
        new(ClaimTypes.Name, user.UserName),
        new(ClaimTypes.GivenName, user.FullName ?? user.UserName),
        new(ClaimTypes.Role, user.UserType)
    };
    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    await httpContext.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        new ClaimsPrincipal(identity));

    return Results.Redirect(user.UserType == Roles.Admin ? "/admin" : "/");
}).AllowAnonymous().DisableAntiforgery();

// Clears the auth cookie and returns to login.
app.MapGet("/account/signout", async (HttpContext httpContext) =>
{
    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/login");
}).DisableAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
