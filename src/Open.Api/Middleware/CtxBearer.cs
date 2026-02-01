using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Open.Services;
using StackExchange.Redis;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
namespace Open.Middleware;

public class CtxBearerOptions : AuthenticationSchemeOptions {
    public const string DefaultScheme = "CtxBearer";
}

/// <summary>
/// CtxBearerHandler
/// </summary>
/// <see cref="https://dev.to/kazinix/aspnet-core-custom-token-authentication-2j9a"/>
/// <param name="options"></param>
/// <param name="logger"></param>
/// <param name="encoder"></param>
/// <param name="muxer"></param>
public class CtxBearerHandler(IOptionsMonitor<CtxBearerOptions> options, ILoggerFactory logger, UrlEncoder encoder, IConnectionMultiplexer muxer) : AuthenticationHandler<CtxBearerOptions>(options, logger, encoder) {
    readonly IDatabase _redis = muxer.GetDatabase();

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync() {
        if (!Request.Headers.ContainsKey("Authorization")) return AuthenticateResult.NoResult(); //.Fail("Missing Authorization Header");
        var authorization = Request.Headers.Authorization.ToString();
        if (!authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)) return AuthenticateResult.Fail("Invalid Authorization Header Scheme");
        var token = authorization[7..].Trim();

        // get ctx
        string? json;
        json = await _redis.StringGetAsync($"ctx/{token}");
        if (string.IsNullOrEmpty(json)) return AuthenticateResult.Fail("Invalid Token");
        var ctx = JsonSerializer.Deserialize<SessionCtx>(json) ?? throw new NullReferenceException("SessionCtx");
        if (ctx.accessToken != token) return AuthenticateResult.Fail("Mismatched Token");

        // set principal
        var identity = new ClaimsIdentity([
            //new Claim(ClaimTypes.NameIdentifier, ctx.Id),
            new Claim(ClaimTypes.Name, ctx.userName),
            new Claim(ClaimTypes.Role, "Role")], Scheme.Name);
        var principal = new CtxClaimsPrincipal(ctx, identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }
}

//public class AuthMiddleware(IConnectionMultiplexer muxer, RequestDelegate next) {
//    readonly IDatabase _redis = muxer.GetDatabase();
//    readonly RequestDelegate _next = next;

//    public async Task InvokeAsync(HttpContext context) {
//        var authorization = context.Request.Headers.Authorization;
//        if (authorization.Count != 0)
//            try {
//                var authHeader = AuthenticationHeaderValue.Parse(authorization!);
//                if (!authHeader.Scheme.Equals("Bearer", StringComparison.OrdinalIgnoreCase) && authHeader.Parameter != null) return;

//                // get ctx
//                string? json;
//                json = await _redis.StringGetAsync($"ctx/{authHeader.Parameter}");
//                if (string.IsNullOrEmpty(json)) return;
//                var ctx = JsonSerializer.Deserialize<SessionCtx>(json) ?? throw new NullReferenceException("SessionCtx");

//                // set user
//                context.User = new CtxClaimsPrincipal(ctx, new ClaimsIdentity([
//                    new Claim(ClaimTypes.Name, ctx.userName)
//                ], "BearerAuthentication"));
//            }
//            catch { }
//            finally { await _next(context); }
//        else await _next(context);
//    }
//}
