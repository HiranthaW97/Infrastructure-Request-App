using InfrastructureRequestApp.Data.Entities;

namespace InfrastructureRequestApp.Data.Services.Interfaces
{
	public interface IAuthService
	{
		Task<User?> ValidateUserAsync(string userName, string password);
	}
}
