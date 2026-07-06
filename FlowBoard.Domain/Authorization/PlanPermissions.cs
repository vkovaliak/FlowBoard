using FlowBoard.Domain.Enums;
using FlowBoard.Domain.Constants;

namespace FlowBoard.Domain.Authorization;

public static class PlanPermissions
{
    public static int MaxBoards(SubscriptionPlan plan) => plan switch
    {
        SubscriptionPlan.Pro => int.MaxValue,
        _ => PlanLimits.FreeMaxBoards
    };

    public static int MaxMembers(SubscriptionPlan plan) => plan switch
    {
        SubscriptionPlan.Pro => int.MaxValue,
        _ => PlanLimits.FreeMaxMembersPerBoard
    };
}