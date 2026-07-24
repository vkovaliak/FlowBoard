using FlowBoard.Domain.DTOs.Boards;
using FlowBoard.Domain.Enums;

namespace FlowBoard.Domain.Authorization;

public static class BoardPermissions
{
    public static bool CanManageBoard(BoardRole role) =>
        role == BoardRole.Owner;

    public static bool CanInviteMembers(BoardRole role) =>
        role is BoardRole.Owner or BoardRole.Admin;

    public static bool CanRemoveMembers(BoardRole role) =>
        role is BoardRole.Owner or BoardRole.Admin;

    public static bool CanModifyContent(BoardRole role) =>
        role != BoardRole.Viewer;

    public static bool CanAssignRole(
        BoardRole actorRole, BoardRole targetRole)
    {
        return actorRole switch
        {
            BoardRole.Owner => targetRole is BoardRole.Admin
                or BoardRole.Member or BoardRole.Viewer,
            BoardRole.Admin => targetRole is BoardRole.Member
                or BoardRole.Viewer,
            _ => false
        };
    }

    public static bool CanTransferOwnership(BoardRole role) =>
        role == BoardRole.Owner;
    
    public static BoardRole? ResolveEffectiveRole(BoardAccessDto access)
    {
        if (access.MemberRole.HasValue)
        {
            return access.MemberRole.Value;
        }

        if (access.IsPublic)
        {
            return BoardRole.Viewer;
        }

        return null;
    }
};