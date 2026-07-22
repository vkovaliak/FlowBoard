namespace FlowBoard.Domain.Enums;

public enum ActivityAction
{
    CardCreated = 0,
    CardUpdated = 1,
    CardMoved = 2,
    CardDeleted = 3,
    CardCompleted = 4,
    CardReopened = 5,
    CardDuplicated = 6,

    DueDateSet = 10,
    DueDateRemoved = 11,
    StartDateSet = 12,
    StartDateRemoved = 13,
    LabelAdded = 14,
    LabelRemoved = 15,
    AssigneeAdded = 16,
    AssigneeRemoved = 17,
    ChecklistItemAdded = 18,
    ChecklistItemCompleted = 19,
    ChecklistItemDeleted = 20,
    AttachmentAdded = 21,
    AttachmentRemoved = 22,
    CommentAdded = 23,
    CommentUpdated = 24,
    CommentDeleted = 25,
    CommentAttachmentsAdded = 26,
    CommentAttachmentsRemoved = 27    
}