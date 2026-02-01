using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace Open.Services;

public class Globals {
    public static string ORGID = "orgID";
    public static string USERID = "userID";
    public static string USERNAME = "username";
}

public class ApiError(string message, string errorCode) {
    public string message { get; } = message;   // error message
    public string errorCode { get; } = errorCode; // INVALID_TYPE
}

public class SObject : Dictionary<string, object> {
    public string id { get; }

    public SObject(string apiVersion, string sobject, string id) {
        this.id = id;
        this["attributes"] = new Dictionary<string, object> {
            ["type"] = sobject,
            ["url"] = $"/services/data/{apiVersion}/sobjects/{sobject}/{id}"
        };
    }
}

public class CtxClaimsPrincipal(SessionCtx ctx, ClaimsIdentity identity) : ClaimsPrincipal(identity) {
    public readonly SessionCtx Ctx = ctx;
}

public class SessionCtx {
    public AuthenticationHeaderValue Authorization => new("Bearer", accessToken);
    [JsonPropertyName("a")] public required string accessToken { get; set; }
    [JsonPropertyName("b")] public required string baseUrl { get; set; }
    [JsonPropertyName("u")] public required string userName { get; set; }
}
