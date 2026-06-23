namespace FlowBoard.Domain.Constants;

public static class ExternalAuthConstants
{
    public const string MicrosoftScheme = "Microsoft";
    public const string ExternalCookieScheme = "ExternalCookie";
    public const string MicrosoftProvider = "Microsoft";

    public const string ResponseTypeCode = "code";

    public const string ScopeOpenId = "openid";
    public const string ScopeProfile = "profile";
    public const string ScopeEmail = "email";

    public const string NameClaimType = "name";
    public const string SubClaim = "sub";
    public const string EmailClaim = "email";
    public const string PreferredUsernameClaim = "preferred_username";
}