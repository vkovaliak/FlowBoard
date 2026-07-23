namespace FlowBoard.Application.Abstractions;

public interface IUnitOfWork : IDisposable
{
    IActivityRepository ActivityRepository { get; }
    IListRepository ListRepository { get; }
    IUserRepository UserRepository { get; }
    IUserSessionRepository UserSessionRepository { get; }
    IBoardRepository BoardRepository { get; }
    ICardRepository CardRepository { get; }
    ICommentRepository CommentRepository { get; }
    ICardAssigneeRepository CardAssigneeRepository { get; }
    ILabelRepository LabelRepository { get; }
    ICardLabelRepository CardLabelRepository { get; }
    IChecklistItemRepository ChecklistItemRepository { get; }
    ICardAttachmentRepository CardAttachmentRepository { get; }
    
    void Commit();
    void Rollback();
}