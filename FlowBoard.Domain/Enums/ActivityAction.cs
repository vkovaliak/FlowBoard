namespace FlowBoard.Domain.Enums;

public enum ActivityAction
{
    BoardCreated = 0,
    BoardUpdated = 1,
    BoardArchived = 2,
    BoardRestored = 3,
    BoardDeleted = 4,

    ListCreated = 10,
    ListUpdated = 11,
    ListMoved = 12,
    ListDeleted = 13,

    CardCreated = 20,
    CardUpdated = 21,
    CardMoved = 22,
    CardDeleted = 23,
    CardCompleted = 24,
    CardDuplicated = 25,

    MemberAdded = 30,
    MemberRemoved = 31,
    MemberLeft = 32,
    MemberRoleChanged = 33,
    OwnershipTransferred = 34,

    CommentCreated = 40,
    CommentUpdated = 41,
    CommentDeleted = 42,

    AttachmentCreated = 50,
    AttachmentDeleted = 51,
}