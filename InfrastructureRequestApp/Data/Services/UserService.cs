using InfrastructureRequestApp.Data.Entities;
using InfrastructureRequestApp.Data.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace InfrastructureRequestApp.Data.Services
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
			await _dbContext.SaveChangesAsync();
			return true;
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
