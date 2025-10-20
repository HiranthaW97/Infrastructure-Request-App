using InfrastructureRequestApp.Data.Entities;

namespace InfrastructureRequestApp.Data.Services.Interfaces
{
	public interface IRequestService
	{
		Task<List<Request>> GetMineAsync(Guid userId);
		Task<Request?> GetByIdAsync(Guid requestId);
		Task<Guid> CreateAsync(Request req);
		Task UpdateStatusAsync(Guid requestId, string status, Guid? assignedTo = null);
		Task<List<Request>> GetAllAsync(string? status = null);
		Task AddCommentAsync(Guid requestId, Guid userId, string text);
	}
}
