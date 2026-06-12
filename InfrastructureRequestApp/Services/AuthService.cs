using InfrastructureRequestApp.Data;
using InfrastructureRequestApp.Data.Entities;
using InfrastructureRequestApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureRequestApp.Services
{
	public class AuthService : IAuthService
	{
		private readonly InfraRequestsDbContext _dbContext;

		public AuthService(InfraRequestsDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<User?> ValidateUserAsync(string userName, string password)
		{
			if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
				return null;

			// Normalize input (trim and lowercase for consistent comparison)
			var normalizedUserName = userName.Trim().ToLowerInvariant();
			var hash = PasswordHasher.Hash(password).ToLowerInvariant();

			// Fetch user and validate in memory (safer across collations)
			var user = await _dbContext.Users
				.AsNoTracking()
				.FirstOrDefaultAsync(u =>
					u.UserName.ToLower() == normalizedUserName && u.IsActive);

			if (user == null)
				return null;

			// Compare hashes in C# to avoid SQL collation mismatches
			if (!string.Equals(user.PasswordHash?.Trim().ToLowerInvariant(), hash, StringComparison.Ordinal))
				return null;

			return user;
		}

	}
}
