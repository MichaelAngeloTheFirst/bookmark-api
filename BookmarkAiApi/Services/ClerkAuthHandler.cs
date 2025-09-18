
namespace BookmarkAiApi.Services;

using Clerk.BackendAPI.Helpers.Jwks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

public class ClerkAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    ISystemClock clock,
    IConfiguration config)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder, clock)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var secretKey = config["SECRET_API_KEY"];

        var authorizedParties = config.GetSection("OAUTH_CLIENT_ID").Get<string[]>() 
                                ?? Array.Empty<string>();

        var audiences = config.GetSection("Clerk:Audiences").Get<string[]>();

        var options = new AuthenticateRequestOptions(
            secretKey: secretKey,
            authorizedParties: authorizedParties,
            audiences: audiences
        );

        var state = await AuthenticateRequest.AuthenticateRequestAsync(Context.Request, options);
        
        var clerkClaims = state.Claims!;
        var identity = new ClaimsIdentity(Scheme.Name);
        
        if (!state.IsAuthenticated)
            return AuthenticateResult.Fail($"Clerk auth failed: {state.ErrorReason}");

        var userId = clerkClaims.FindFirst("sub")?.Value
                     ?? clerkClaims.FindFirst("subject")?.Value;
        foreach (var claim in clerkClaims.Claims)
        {
            Logger.LogInformation("Clerk claim: {Type} = {Value}", claim.Type, claim.Value);
        }
        if (!string.IsNullOrEmpty(userId))
        {
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId));
            identity.AddClaim(new Claim(ClaimTypes.Name, userId)); 
        }


        var email = clerkClaims.FindFirst("email")?.Value;
        if (!string.IsNullOrEmpty(email))
        {
            identity.AddClaim(new Claim(ClaimTypes.Email, email));
        }
        
        var orgRole = clerkClaims.FindFirst("org_role")?.Value;
        if (!string.IsNullOrEmpty(orgRole))
        {
            identity.AddClaim(new Claim(ClaimTypes.Role, orgRole)); // so User.IsInRole works
        }
        
        foreach (var c in clerkClaims.Claims)
        {
            identity.AddClaim(c);
        }

        var principal = new ClaimsPrincipal(identity);
        
        var ticket = new AuthenticationTicket(state.Claims!, Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }
}
