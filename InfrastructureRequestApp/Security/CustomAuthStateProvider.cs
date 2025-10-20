using InfrastructureRequestApp.Data.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace InfrastructureRequestApp.Security
{
	public class CustomAuthStateProvider : AuthenticationStateProvider
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private User? _currentUser;

		public CustomAuthStateProvider(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		public override Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			var httpContext = _httpContextAccessor.HttpContext;

			if (httpContext?.User?.Identity?.IsAuthenticated == true)
			{
				// user already signed in via cookie
				return Task.FromResult(new AuthenticationState(httpContext.User));
			}

			// fall back to in-memory user for Blazor session
			var identity = new ClaimsIdentity();
			if (_currentUser is not null)
			{
				identity = new ClaimsIdentity(new[]
				{
					new Claim(ClaimTypes.NameIdentifier, _currentUser.UserId.ToString()),
					new Claim(ClaimTypes.Name, _currentUser.UserName),
					new Claim(ClaimTypes.Role, _currentUser.UserType)
				}, "CustomAuth");
			}

			return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
		}

		public async Task SignInAsync(User user)
		{
			_currentUser = user;
			NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
			await Task.CompletedTask;
		}

		public async Task SignOutAsync()
		{
			_currentUser = null;
			NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
			await Task.CompletedTask;
		}

		public Guid? CurrentUserId => _currentUser?.UserId;
		public string? CurrentUserRole => _currentUser?.UserType;
	}
}
