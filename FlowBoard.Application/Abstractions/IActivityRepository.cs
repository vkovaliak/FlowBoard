using FlowBoard.Domain.DTOs.Activities;
using FlowBoard.Domain.Entities;

namespace FlowBoard.Application.Abstractions;
public interface IActivityRepository : IBaseRepository<Activity, Guid>
{
    Task<IEnumerable<ActivityDto>> GetByBoardIdAsync(Guid boardId, int limit = 50);
}