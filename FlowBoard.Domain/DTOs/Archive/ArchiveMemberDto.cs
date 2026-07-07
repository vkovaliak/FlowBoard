using FlowBoard.Domain.Enums;

namespace FlowBoard.Domain.DTOs.Archive;

public class ArchiveMemberDto
{
    public Guid UserId { get; set; }
    public BoardRole Role { get; set; }
}