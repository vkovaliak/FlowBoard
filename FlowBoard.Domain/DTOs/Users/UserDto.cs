using FlowBoard.Domain.Enums;

namespace FlowBoard.Domain.DTOs.Users;

public record UserDto(
    Guid Id,
    string EmailAddress,
    string UserName,
    string? AvatarUrl,
    SubscriptionPlan SubscriptionPlan);