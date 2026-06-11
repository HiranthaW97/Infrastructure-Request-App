using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace InfrastructureRequestApp.Security
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        // Captured during construction, when IHttpContextAccessor.HttpContext is
        // still the real HTTP request (before the WebSocket upgrade).
        // This survives page refreshes because each new circuit re-runs the constructor
        // against the fresh HTTP request that carries the auth cookie.
        private ClaimsPrincipal _principal;

        public CustomAuthStateProvider(IHttpContextAccessor httpContextAccessor)
        {
            _principal = httpContextAccessor.HttpContext?.User
                ?? new ClaimsPrincipal(new ClaimsIdentity());
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
            => Task.FromResult(new AuthenticationState(_principal));

        public void NotifySignedOut()
        {
            _principal = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public Guid? CurrentUserId
        {
            get
            {
                var val = _principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return val is null ? null : Guid.Parse(val);
            }
        }

        public string? CurrentUserFullName =>
            _principal.FindFirst(ClaimTypes.GivenName)?.Value
            ?? _principal.FindFirst(ClaimTypes.Name)?.Value;

        public string? CurrentUserRole => _principal.FindFirst(ClaimTypes.Role)?.Value;
    }
}
