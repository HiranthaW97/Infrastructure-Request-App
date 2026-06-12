using InfrastructureRequestApp.Data.Entities;

namespace InfrastructureRequestApp.Services.Interfaces
{
	public interface IAuthService
	{
		Task<User?> ValidateUserAsync(string userName, string password);
	}
}
