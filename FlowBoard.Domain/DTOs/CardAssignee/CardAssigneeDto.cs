namespace FlowBoard.Domain.DTOs.CardAssignee;

public record CardAssigneeDto(
    Guid UserId,
    string EmailAddress);