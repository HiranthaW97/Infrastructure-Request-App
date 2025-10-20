using InfrastructureRequestApp.Components;                 // for App.razor
using InfrastructureRequestApp.Data;
using InfrastructureRequestApp.Data.Services;
using InfrastructureRequestApp.Data.Services.Interfaces;
using InfrastructureRequestApp.Security;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

// EF Core DbContext
builder.Services.AddDbContext<InfraRequestsDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Blazor Components (Interactive Server)
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();

// Authentication & Authorization
// Auth services
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication("Cookies")
	.AddCookie("Cookies", options =>
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


// Application services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRequestService, RequestService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Radzen support (Notifications, Dialogs, etc.)
builder.Services.AddRadzenComponents();

var app = builder.Build();

// Middleware pipeline
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

// Map Blazor Components
app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();

// Optional: if hosting under a subpath (e.g., IIS virtual dir)
// app.UsePathBase("/InfrastructureRequestApp");

app.Run();
