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

		/// <summary>
		/// Resets the user's password to a generated temporary one and flags the
		/// account so the next login is forced through the reset-password page.
		/// Returns the matched user and the plain temporary password, or (null, null)
		/// when no active user matches the supplied username.
		/// </summary>
		Task<(User? user, string? temporaryPassword)> StartPasswordRecoveryAsync(string userName);
	}
}
