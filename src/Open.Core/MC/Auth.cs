namespace Open.MC.Auth;

/// <summary>
/// baseUrlResponseBody
/// </summary>
/// <see cref="https://developer.salesforce.com/docs/marketing/marketing-cloud/references/mc_rest_auth?meta=type%3AauthTokenErrorResponseBody"/>
public class authTokenErrorResponseBody {
    /// <summary>
    /// The error message.
    /// </summary>
    public string? error { get; set; }
    /// <summary>
    /// A description of the error that occurred.
    /// </summary>
    public string? error_description { get; set; }
    /// <summary>
    /// A link to the Salesforce developer documentation.
    /// </summary>
    public string? error_uri { get; set; }
    //[JsonExtensionData] public Dictionary<string, JsonElement>? _unmapped_ { get; set; }
}

/// <summary>
/// baseUrlResponseBody
/// </summary>
/// <see cref="https://developer.salesforce.com/docs/marketing/marketing-cloud/references/mc_rest_auth?meta=type%3AbaseUrlResponseBody"/>
public class baseUrlResponseBody {
    /// <summary>
    /// The username of the user that you requested base URLs for.
    /// </summary>
    public required string subject { get; set; }
    /// <summary>
    /// An array that contains the base URLs.
    /// </summary>
    public required string[] links { get; set; }
}

/// <summary>
/// httpErrorResponseBody
/// </summary>
public class httpErrorResponseBody {
    /// <summary>
    /// Contains a link to the Marketing Cloud Developer Documentation page about handling REST API errors.
    /// </summary>
    public required string documentation { get; set; }
    /// <summary>
    /// The HTTP error code.
    /// </summary>
    public required int errorcode { get; set; }
    /// <summary>
    /// The error message.
    /// </summary>
    public required string message { get; set; }
}

/// <summary>
/// tokenRequestBody
/// </summary>
/// <see cref="https://developer.salesforce.com/docs/marketing/marketing-cloud/references/mc_rest_auth?meta=type%3AtokenRequestBody"/>
public class tokenRequestBody {
    /// <summary>
    /// The type of access grant. Specify client_credentials as the value for this property.
    /// </summary>
    public required string grant_type { get; set; }
    /// <summary>
    /// The client ID of your Marketing Cloud account.
    /// </summary>
    public required string client_id { get; set; }
    /// <summary>
    /// The client secret for your Marketing Cloud account.
    /// </summary>
    public string? client_secret { get; set; }
    /// <summary>
    /// The member ID (MID) of your Marketing Cloud account.
    /// </summary>
    public string? account_id { get; set; }
    public required string code { get; set; }
    public required string redirect_uri { get; set; }
    public required string scope { get; set; }
}

/// <summary>
/// tokenResponseBody
/// </summary>
/// <see cref="https://developer.salesforce.com/docs/marketing/marketing-cloud/references/mc_rest_auth?meta=type%3AtokenResponseBody"/>
public class tokenResponseBody {
    /// <summary>
    /// The API access token. The access token contains 512 alphanumeric characters.
    /// </summary>
    public required string access_token { get; set; }
    public required string refresh_token { get; set; }
    /// <summary>
    /// The type of token that was retrieved. This property always has the value Bearer.
    /// </summary>
    public required string token_type { get; set; }
    /// <summary>
    /// The amount of time (in seconds) that the token remains valid. This property always has the value 1079.
    /// </summary>
    public required int expires_in { get; set; }
    /// <summary>
    /// The permissions that have been granted to your user. Multiple permission types are separated with a space.
    /// </summary>
    public required string scope { get; set; }
    /// <summary>
    /// The application-specific SOAP API URL.
    /// </summary>
    public required string soap_instance_url { get; set; }
    /// <summary>
    /// The application-specific REST API URL.
    /// </summary>
    public required string rest_instance_url { get; set; }
}

public class timezoneBody {
    public required string longName { get; set; }
    public required string shortName { get; set; }
    public required decimal offset { get; set; }
    public required bool dst { get; set; }
}

public class userBody {
    public required string sub { get; set; }
    public required string name { get; set; }
    public required string preferred_username { get; set; }
    public required string email { get; set; }
    public required string locale { get; set; }
    public required string zoneinfo { get; set; }
    public required timezoneBody timezone { get; set; }
}

public class organizationBody {
    public required int member_id { get; set; }
    public required int enterprise_id { get; set; }
    public required string enterprise_name { get; set; }
    public required string account_type { get; set; }
    public required string stack_key { get; set; }
    public required string locale { get; set; }
    public required string zoneinfo { get; set; }
    public required timezoneBody timezone { get; set; }
}

public class restBody {
    public required string rest_instance_url { get; set; }
    public required string soap_instance_url { get; set; }
}

public class applicationBody {
    public required string id { get; set; }
    public required string name { get; set; }
    public required string[] redirectUrl { get; set; }
    public required string[] appScopes { get; set; }
}

public class permissionsBody {
    public required string objectTypeName { get; set; }
    public required string operationName { get; set; }
    public required string name { get; set; }
    public required int id { get; set; }
}

public class userInfoResponseBody {
    /// <summary>
    /// A Unix epoch timestamp that indicates when the API access token expires.
    /// </summary>
    public required int exp { get; set; }
    /// <summary>
    /// The URL of the issuer of the API access token.
    /// </summary>
    public required string iss { get; set; }
    /// <summary>
    /// Information about the user that the access token is associated with.
    /// </summary>
    public required userBody user { get; set; }
    /// <summary>
    /// An object that contains information about the user’s organization.
    /// </summary>
    public required organizationBody organization { get; set; }
    /// <summary>
    /// An object that contains API base URLs.
    /// </summary>
    public required restBody rest { get; set; }
    /// <summary>
    /// Information about the application.
    /// </summary>
    public required applicationBody application { get; set; }
    public required permissionsBody[] permissions { get; set; }
}
