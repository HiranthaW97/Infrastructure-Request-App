using InfrastructureRequestApp.Data.Entities;
using InfrastructureRequestApp.Data.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureRequestApp.Data.Services
{
	public class RequestService : IRequestService
	{
		private readonly InfraRequestsDbContext _dbContext;

		public RequestService(InfraRequestsDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task AddCommentAsync(Guid requestId, Guid userId, string text)
		{
			var c = new RequestComment
			{
				RequestId = requestId,
				CommentText = text,
				CommentedBy = userId,
				CommentDate = DateTime.UtcNow
			};
			_dbContext.RequestComments.Add(c);
			await _dbContext.SaveChangesAsync();
		}

		public async Task<Guid> CreateAsync(Request req)
		{
			req.Status = "Pending";
			req.CreatedDate = DateTime.UtcNow;
			_dbContext.Requests.Add(req);
			await _dbContext.SaveChangesAsync();
			return req.RequestId;
		}

		public Task<List<Request>> GetAllAsync(string? status = null)
		{
			var requests = _dbContext.Requests.AsQueryable();
			if (!string.IsNullOrWhiteSpace(status)) requests = requests.Where(r => r.Status == status);
			return requests.OrderByDescending(r => r.CreatedDate).ToListAsync();
		}

		public Task<Request?> GetByIdAsync(Guid requestId)
		{
			return _dbContext.Requests
				.Include(r => r.RequestComments)
				.ThenInclude(c => c.CommentedByUser)
				.FirstOrDefaultAsync(r => r.RequestId == requestId);
		}

		public Task<List<Request>> GetMineAsync(Guid userId)
		{
			return _dbContext.Requests
				.AsNoTracking()
				.Where(r => r.CreatedBy == userId)
				.OrderByDescending(r => r.CreatedDate)
				.ToListAsync();
		}

		public async Task UpdateStatusAsync(Guid requestId, string status, Guid? assignedTo = null)
		{
			var r = await _dbContext.Requests.FirstOrDefaultAsync(x => x.RequestId == requestId);
			if (r == null) return;

			r.Status = status;
			r.AssignedTo = assignedTo;
			r.UpdatedDate = DateTime.UtcNow;
			await _dbContext.SaveChangesAsync();
		}
	}
}
