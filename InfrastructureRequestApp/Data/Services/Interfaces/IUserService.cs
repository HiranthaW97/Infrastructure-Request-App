using InfrastructureRequestApp.Data.Entities;

namespace InfrastructureRequestApp.Data.Services.Interfaces
{
	public interface IUserService
	{
		Task<User?> GetByUserNameAsync(string userName);
		Task<User?> GetByIdAsync(Guid userId);
		Task<List<User>> GetAllAsync();
		Task<Guid> CreateAsync(User user, string plainPassword);
		Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, bool force = false);
	}
}
