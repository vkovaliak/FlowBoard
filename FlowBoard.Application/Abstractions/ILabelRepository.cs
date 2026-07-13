using FlowBoard.Domain.DTOs.Labels;
using FlowBoard.Domain.Entities;

namespace FlowBoard.Application.Abstractions;

public interface ILabelRepository : IBaseRepository<Label, Guid>
{
    Task<IEnumerable<LabelDto>> GetByBoardIdAsync(Guid boardId);
    Task RemoveByLabelIdAsync(Guid labelId);
}