using Open.MC.Auth;
using System.Net.Http.Json;
using static Open.Services.Globals;

namespace Open.Services.Data;

public class DataX(HttpClient client) {
    readonly HttpClient _client = client;

    public async Task<object[]> GetAsync(SessionCtx ctx, string apiVersion, string sobject, string id) {
        switch (sobject) {
            case "User":
                // get token
                _client.DefaultRequestHeaders.Authorization = ctx.Authorization;
                var tokReq = await _client.GetAsync($"{ctx.baseUrl}/userinfo");
                if (!tokReq.IsSuccessStatusCode) {
                    var tokErr = await tokReq.Content.ReadFromJsonAsync<authTokenErrorResponseBody>();
                    return [new ApiError(tokErr?.error_description ?? "error", "ERROR")];
                }
                var tokRes = await tokReq.Content.ReadFromJsonAsync<userInfoResponseBody>() ?? throw new ArgumentNullException("userInfoResponseBody");
                return [new SObject(apiVersion, sobject, id) {
                    ["Id"] = id,
                    ["Username"] = tokRes.user.preferred_username
                }];
            default: return [new ApiError("Bad res", "INVALID_TYPE")];
        }
    }

    public async Task<object[]> QueryAsync(SessionCtx ctx, string apiVersion, string q, bool tooling) => q switch {
        "SELECT Id FROM ScratchOrgInfo limit 1" => [new ApiError("Bad res", "INVALID_TYPE")],
        "Select Namespaceprefix FROM Organization" => [new SObject(apiVersion, "Organization", ORGID) { ["NamespacePrefix"] = null! }],
        _ => [new ApiError("Bad res", "INVALID_TYPE")]
    };

    //public async 

        //"SELECT Id,Status,SandboxName,SandboxInfoId,LicenseType,CreatedDate,CopyProgress,SandboxOrganization,SourceId,Description,EndDate,Features FROM SandboxProcess WHERE SandboxOrganization='orgID' ORDER BY CreatedDate DESC" =>
}