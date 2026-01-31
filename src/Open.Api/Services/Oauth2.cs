using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using static Open.Globals;
using static Open.Services.Globals;
using static Open.Services.Oauth2.TokenController;
namespace Open.Services.Oauth2;

/// <summary>
/// AuthorizeController
/// </summary>
/// <see cref="https://developer.salesforce.com/docs/marketing/marketing-cloud/guide/authorization-code.html"/>
[ApiController, Route("services/oauth2/[controller]")]
public class AuthorizeController : ControllerBase {
    public class Req {
        public required string state { get; set; }          // f42f1ebcb90e
        public required string prompt { get; set; }         // login
        public required string scope { get; set; }          // refresh_token api web
        public required string code_challenge { get; set; } // 59P9WmIiMyjmfBYb2mEiRC5Y44giSIcGDlLYrIbKd5Y
        public required string response_type { get; set; }  // code
        public required string client_id { get; set; }      // PlatformCLI
        public required string? client_secret { get; set; } // {optional}
        public required Uri redirect_uri { get; set; }      // http://localhost:1717/OauthRedirect

        public override string ToString() => $"{state}|{scope}|{code_challenge}|{client_id}|{client_secret}|{redirect_uri}";
        public static Req FromString(string s) {
            var p = s.Split(['|'], 6);
            return new Req { state = p[0], prompt = "", scope = p[1], code_challenge = p[2], response_type = "", client_id = p[3], client_secret = p[4], redirect_uri = new Uri(p[5]) };
        }
    }

    [HttpGet]
    public IActionResult Get([FromQuery] Req req) {
        req.client_id = CLIENT_ID;
        req.client_secret = CLIENT_SECRET;
        req.scope = SCOPE;

        // build redirect
        var host = NGROK ?? HttpContext.Request.ToString();
        var b = new UriBuilder($"https://{SUBDOMAIN}.auth.marketingcloudapis.com/v2/authorize");
        var q = HttpUtility.ParseQueryString(b.Query);
        q["client_id"] = req.client_id;
        q["response_type"] = "code";
        q["scope"] = req.scope;
        q["state"] = req.ToString();
        q["redirect_uri"] = $"{host}/oauth2/mc";
        b.Query = q.ToString();
        return Redirect(b.Uri.ToString());
    }
}

/// <summary>
/// McController
/// </summary>
[ApiController, Route("oauth2/[controller]")]
public class McController(IConnectionMultiplexer muxer) : ControllerBase {
    readonly IDatabase _redis = muxer.GetDatabase();

    public class Req {
        public required string state { get; set; }
        // primary
        public string? code { get; set; }
        public string? tssd { get; set; }
        // error
        public string? error { get; set; }
        public string? error_description { get; set; }
    }

    public class Spike {
        [JsonPropertyName("c")] public required string code { get; set; }
        [JsonPropertyName("i")] public required string clientId { get; set; }
        [JsonPropertyName("s")] public string? clientSecret { get; set; }
        [JsonPropertyName("r")] public required string redirectUri { get; set; }
        [JsonPropertyName("k")] public required string scope { get; set; }
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] Req req) {
        // decode state
        var auth = AuthorizeController.Req.FromString(req.state);

        // store spike
        var host = NGROK ?? HttpContext.Request.ToString();
        if (req.error == null)
            await Task.WhenAll(
                _redis.StringSetAsync(auth.code_challenge, JsonSerializer.Serialize(new Spike {
                    code = req.code!,
                    clientId = auth.client_id,
                    clientSecret = auth.client_secret!,
                    redirectUri = $"{host}/oauth2/mc",
                    scope = auth.scope
                })),
                _redis.KeyExpireAsync(req.state, TimeSpan.FromSeconds(3600)));

        // build redirect
        var b = new UriBuilder(auth.redirect_uri);
        var q = HttpUtility.ParseQueryString(b.Query);
        if (req.error != null) { q["error"] = HttpUtility.UrlDecode(req.error); q["error_description"] = HttpUtility.UrlDecode(req.error_description); }
        else { q["code"] = auth.code_challenge; q["state"] = auth.state; }
        b.Query = q.ToString();
        return Redirect(b.Uri.ToString());
    }
}

/// <summary>
/// TokenController
/// </summary>
/// <see cref="https://developer.salesforce.com/docs/marketing/marketing-cloud/guide/access-token-app.html"/>
[ApiController, Route("services/oauth2/[controller]")]
public class TokenController(HttpClient client, IConnectionMultiplexer muxer) : ControllerBase {
    readonly HttpClient _client = client;
    readonly IDatabase _redis = muxer.GetDatabase();

    public class Req {
        public required string grant_type { get; set; }         // authorization_code
        public required string code { get; set; }               // AE_NN0M7sO7BYOsLxeAJQHFqiVtYVrW6xtVY74m6n2s
        public required string client_id { get; set; }          // PlatformCLI
        public required string? client_secret { get; set; }     // {optional}
        public required Uri redirect_uri { get; set; }          // http://localhost:1717/OauthRedirect
        public required string code_verifier { get; set; }      // z8ko1KPq4jHbAE_fMbW3dMPMoa0ViOCdtsEV7WoNstXQ6dxke-A4EiMbimOy9a4m-p_Zo0od72b8xWnE4NQTvhjhlgBCNDTpEWQnaZy6Kbd1FDaXRwOLl6ILTkNvy365Zm-kEcu2QRErGYYhWXSwhhRv8sl4ynGxPzRp7hbvERU
    }

    public class Res {
        public required string token_type { get; set; }
        /// <summary>
        /// Space-separated list of OAuth scopes associated with the access token
        /// For the OAuth 2.0 Web Server Flow, this can be a subset of the registered scopes if specified when requesting the auth code.
        /// </summary>
        /// <see cref="https://help.salesforce.com/s/articleView?id=xcloud.remoteaccess_oauth_tokens_scopes.htm&type=5"/>
        public required string scope { get; set; }
        /// <summary>
        /// Identity URL
        /// The format of the URL is https://login.salesforce.com/id/orgID/userID.
        /// </summary>
        public required string id { get; set; }
        public required string access_token { get; set; }
        public string? refresh_token { get; set; }
        public required string signature { get; set; }
        public required string issued_at { get; set; }
        public required string instance_url { get; set; }
        public string? sfdc_community_url { get; set; }
        public string? sfdc_community_id { get; set; }
    }

    public class TokenReq {
        public required string grant_type { get; set; }
        public required string code { get; set; }
        public required string client_id { get; set; }
        public string? client_secret { get; set; }
        public required string redirect_uri { get; set; }
        public required string scope { get; set; }
    }

    public class TokenRes {
        // primary
        public string? access_token { get; set; }
        public string? refresh_token { get; set; }
        public int? expires_in { get; set; }
        public string? token_type { get; set; }
        public string? rest_instance_url { get; set; }
        public string? soap_instance_url { get; set; }
        public string? scope { get; set; }
        // error
        public string? error { get; set; }
        public string? error_description { get; set; }
    }

    [HttpPost]
    public async Task<Res?> Post([FromForm] Req req) {
        // get spike
        string? json;
        json = await _redis.StringGetAsync(req.code);
        if (string.IsNullOrEmpty(json)) { NotFound(); return default; }
        var spike = JsonSerializer.Deserialize<McController.Spike>(json) ?? throw new NullReferenceException("Spike");

        // get token
        var tokReq = await _client.PostAsJsonAsync($"https://{SUBDOMAIN}.auth.marketingcloudapis.com/v2/token", new TokenReq {
            grant_type = "authorization_code",
            code = spike.code,
            client_id = spike.clientId,
            client_secret = spike.clientSecret,
            redirect_uri = spike.redirectUri,
            scope = spike.scope,
        });
        tokReq.EnsureSuccessStatusCode();
        var tokRes = await tokReq.Content.ReadFromJsonAsync<TokenRes>() ?? throw new NullReferenceException("TokenRes");

        // return res
        return new() {
            token_type = tokRes.token_type!,
            scope = "refresh_token api web", //tokenRes.scope
            id = $"{HOST}/id/{ORGID}/{USERID}",
            access_token = tokRes.access_token!,
            refresh_token = tokRes.refresh_token!,
            signature = "signature",
            issued_at = "issued_at",
            instance_url = HOST,
            sfdc_community_url = tokRes.rest_instance_url,
            sfdc_community_id = "",
        };
    }
}

/// <summary>
/// UserInfoController
/// </summary>
[ApiController, Route("services/oauth2/[controller]")]
public class UserInfoController(HttpClient client) : ControllerBase {
    readonly HttpClient _client = client;

    public class Res {
        public required string preferred_username { get; set; }
        public required string organization_id { get; set; }
        public required string user_id { get; set; }
    }

    public class UserInfoRes {
        public string? exp { get; set; }
        public string? refresh_token { get; set; }
        public int? expires_in { get; set; }
        public string? token_type { get; set; }
        public string? rest_instance_url { get; set; }
        public string? soap_instance_url { get; set; }
        public string? scope { get; set; }
    }

    [HttpGet]
    public Res Get() {
        var hdr = Request.Headers.ToList();
        var auth = Request.Headers["Authorization"];

        //// get token
        //var tokReq = await _client.PostAsJsonAsync($"https://{SUBDOMAIN}.auth.marketingcloudapis.com/v2/userinfo", new TokenReq {
        //    grant_type = "authorization_code",
        //    code = spike.code,
        //    client_id = spike.clientId,
        //    client_secret = spike.clientSecret,
        //    redirect_uri = spike.redirectUri,
        //    scope = spike.scope,
        //});
        //tokReq.EnsureSuccessStatusCode();
        //var tokRes = await tokReq.Content.ReadFromJsonAsync<TokenRes>() ?? throw new NullReferenceException("TokenRes");

        return new() {
            preferred_username = USERNAME,
            organization_id = ORGID,
            user_id = USERID,
        };
    }
}

/// <summary>
/// RevokeController
/// </summary>
[ApiController, Route("services/oauth2/[controller]")]
public class RevokeController : ControllerBase {
    [HttpGet]
    public IActionResult Get() => throw new NotImplementedException();
}