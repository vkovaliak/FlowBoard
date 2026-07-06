using FlowBoard.Domain.Enums;

namespace FlowBoard.Domain.DTOs.Boards;

public record BoardDto(
    Guid Id,
    string Name,
    bool IsPublic,
    string Background,
    Guid CreatedBy,
    DateTime CreatedAt,
    bool IsFavorite,
    BoardRole UserRole);