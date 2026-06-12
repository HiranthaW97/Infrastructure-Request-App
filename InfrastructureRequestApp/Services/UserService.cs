using InfrastructureRequestApp.Data;
using InfrastructureRequestApp.Data.Entities;
using InfrastructureRequestApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Cryptography;
using System.Text;

namespace InfrastructureRequestApp.Services
{
	public class UserService : IUserService
	{
		private readonly InfraRequestsDbContext _dbContext;

		public UserService(InfraRequestsDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, bool force = false)
		{
			var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);

			if (user == null)
			{
				return false;
			}

			if (!force)
			{
				var currentHash = PasswordHasher.Hash(currentPassword);
				if (!string.Equals(currentHash, user.PasswordHash,
				StringComparison.OrdinalIgnoreCase))
					return false;
			}

			user.PasswordHash = PasswordHasher.Hash(newPassword);
			// A successful change clears any pending forced-reset requirement.
			user.MustResetPassword = false;
			await _dbContext.SaveChangesAsync();
			return true;
		}

		public async Task<(User? user, string? temporaryPassword)> StartPasswordRecoveryAsync(string userName)
		{
			if (string.IsNullOrWhiteSpace(userName))
				return (null, null);

			var normalized = userName.Trim().ToLowerInvariant();
			var user = await _dbContext.Users
				.FirstOrDefaultAsync(u => u.UserName.ToLower() == normalized && u.IsActive);

			if (user == null)
				return (null, null);

			var tempPassword = GenerateTemporaryPassword();
			user.PasswordHash = PasswordHasher.Hash(tempPassword);
			user.MustResetPassword = true;
			await _dbContext.SaveChangesAsync();

			return (user, tempPassword);
		}

		private static string GenerateTemporaryPassword()
		{
			// Avoids visually ambiguous characters (0/O, 1/l/I) for readability.
			const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789";
			var bytes = RandomNumberGenerator.GetBytes(10);
			var sb = new StringBuilder(bytes.Length);
			foreach (var b in bytes)
				sb.Append(chars[b % chars.Length]);
			return sb.ToString();
		}

		public async Task<Guid> CreateAsync(User user, string plainPassword)
		{
			user.PasswordHash = PasswordHasher.Hash(plainPassword);
			_dbContext.Users.Add(user);
			await _dbContext.SaveChangesAsync();
			return user.UserId;
		}

		public Task<List<User>> GetAllAsync()
		{
			return _dbContext.Users.Where(u => u.IsActive).ToListAsync();
		}

		public Task<User?> GetByIdAsync(Guid userId)
		{
			return _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId && u.IsActive);
		}

		public Task<User?> GetByUserNameAsync(string userName)
		{
			return _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == userName && u.IsActive);
		}
	}
}
