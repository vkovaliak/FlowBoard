namespace FlowBoard.Domain.DTOs.Auth;

public record ExternalUserInfo(
    string ExternalId, 
    string Email, 
    string Name
);
