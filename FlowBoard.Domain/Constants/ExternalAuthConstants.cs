namespace FlowBoard.Domain.Constants;

public static class ExternalAuthConstants
{
    public const string MicrosoftScheme = "Microsoft";
    public const string MicrosoftProvider = "Microsoft";

    public const string OpenIdConfigurationPath = "/.well-known/openid-configuration";

    public const string NameClaimType = "name";
    public const string SubClaim = "sub";
    public const string EmailClaim = "email";
    public const string PreferredUsernameClaim = "preferred_username";
}