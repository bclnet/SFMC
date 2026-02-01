using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Open.Globals;
using static Open.Services.Globals;
namespace Open.Services.Data;

/// <summary>
/// DataController
/// </summary>
[ApiController, Route("services/[controller]"), Authorize]
public class DataController(DataX data) : ControllerBase {
    readonly DataX _data = data;

    [HttpGet]
    public async Task<object> Get(string apiVersion) => new Dictionary<string, string> {
        ["metadata"] = "/services/data/v62.0/metadata",
        ["eclair"] = "/services/data/v62.0/eclair",
        ["folders"] = "/services/data/v62.0/folders",
        ["jsonxform"] = "/services/data/v62.0/jsonxform",
        ["appMenu"] = "/services/data/v62.0/appMenu",
        ["iot"] = "/services/data/v62.0/iot",
        ["analytics"] = "/services/data/v62.0/analytics",
        ["smartdatadiscovery"] = "/services/data/v62.0/smartdatadiscovery",
        ["composite"] = "/services/data/v62.0/composite",
        ["parameterizedSearch"] = "/services/data/v62.0/parameterizedSearch",
        ["fingerprint"] = "/services/data/v62.0/fingerprint",
        ["domino"] = "/services/data/v62.0/domino",
        ["serviceTemplates"] = "/services/data/v62.0/serviceTemplates",
        ["recent"] = "/services/data/v62.0/recent",
        ["dedupe"] = "/services/data/v62.0/dedupe",
        ["query"] = "/services/data/v62.0/query",
        ["ai"] = "/services/data/v62.0/ai",
        ["consent"] = "/services/data/v62.0/consent",
        ["omnistudio"] = "/services/data/v62.0/omnistudio",
        ["digitalwallet"] = "/services/data/v62.0/digitalwallet",
        ["compactLayouts"] = "/services/data/v62.0/compactLayouts",
        ["knowledgeManagement"] = "/services/data/v62.0/knowledgeManagement",
        ["actions"] = "/services/data/v62.0/actions",
        ["support"] = "/services/data/v62.0/support",
        ["tooling"] = "/services/data/v62.0/tooling",
        ["prechatForms"] = "/services/data/v62.0/prechatForms",
        ["contact-tracing"] = "/services/data/v62.0/contact-tracing",
        ["chatter"] = "/services/data/v62.0/chatter",
        ["payments"] = "/services/data/v62.0/payments",
        ["tabs"] = "/services/data/v62.0/tabs",
        ["quickActions"] = "/services/data/v62.0/quickActions",
        ["queryAll"] = "/services/data/v62.0/queryAll",
        ["commerce"] = "/services/data/v62.0/commerce",
        ["wave"] = "/services/data/v62.0/wave",
        ["search"] = "/services/data/v62.0/search",
        ["identity"] = $"https://{HOST}/id/{ORGID}/{USERID}",
        ["theme"] = "/services/data/v62.0/theme",
        ["nouns"] = "/services/data/v62.0/nouns",
        ["event"] = "/services/data/v62.0/event",
        ["connect"] = "/services/data/v62.0/connect",
        ["licensing"] = "/services/data/v62.0/licensing",
        ["limits"] = "/services/data/v62.0/limits",
        ["process"] = "/services/data/v62.0/process",
        ["jobs"] = "/services/data/v62.0/jobs",
        ["localizedvalue"] = "/services/data/v62.0/localizedvalue",
        ["mobile"] = "/services/data/v62.0/mobile",
        ["emailConnect"] = "/services/data/v62.0/emailConnect",
        ["tokenizer"] = "/services/data/v62.0/tokenizer",
        ["async"] = "/services/data/v62.0/async",
        ["externalservices"] = "/services/data/v62.0/externalservices",
        ["sobjects"] = "/services/data/v62.0/sobjects"
    };
}

/// <summary>
/// QueryController
/// </summary>
[ApiController, Route("services/data/{apiVersion}/[controller]"), Authorize]
public class QueryController(DataX data) : ControllerBase {
    readonly DataX _data = data;

    public class Res {
        public required int totalSize { get; set; }     // 1
        public required bool done { get; set; }         // true
        public string? nextRecordsUrl { get; set; }     // "/services/data/v51.0/query/0r8xx50ZnWewYSzAUM-2000"
        public required IEnumerable<SObject> records { get; set; }
    }

    [HttpGet]
    public async Task<object> Get(string apiVersion, [FromQuery] string q) {
        var ctx = ((CtxClaimsPrincipal)User).Ctx;
        var res = await _data.QueryAsync(ctx, apiVersion, q, false);
        if (res == null || (res.Length > 0 && res[0] is ApiError)) return NotFound(res ?? [new ApiError("QueryAsync returned null", "NULL")]);
        return new Res() {
            totalSize = res.Length,
            done = true,
            records = res.Cast<SObject>()
        };
    }
}

/// <summary>
/// SObjectsController
/// </summary>
[ApiController, Route("services/data/{apiVersion}/[controller]"), Authorize]
public class SObjectsController(DataX data) : ControllerBase {
    readonly DataX _data = data;

    [HttpGet, Route("{sobject}/{id}")]
    public async Task<object> Get(string apiVersion, string sobject, string id) {
        var ctx = ((CtxClaimsPrincipal)User).Ctx;
        var res = await _data.GetAsync(ctx, apiVersion, sobject, id);
        if (res == null || res[0] is ApiError) return NotFound(res ?? [new ApiError("GetAsync returned null", "NULL")]);
        return res[0];
    }
}

// https://developer.salesforce.com/docs/atlas.en-us.api_meta.meta/api_meta/meta_intro.htm

/// <summary>
/// ToolingController
/// </summary>
/// <see cref="https://developer.salesforce.com/docs/atlas.en-us.api_tooling.meta/api_tooling/intro_rest_resources.htm"/>
[ApiController, Route("services/data/{apiVersion}/[controller]"), Authorize]
public class ToolingController(DataX data) : ControllerBase {
    readonly DataX _data = data;

    public class QueryRes {
        public required int totalSize { get; set; }     // 1
        public required bool done { get; set; }         // true
        public string? nextRecordsUrl { get; set; }     // "/services/data/v51.0/query/0r8xx50ZnWewYSzAUM-2000"
        public required IEnumerable<SObject> records { get; set; }
    }

    [HttpGet, Route("completions")]
    public async Task<object> completions(string apiVersion, [FromQuery] string type) => throw new NotImplementedException();

    [HttpGet, Route("executeAnonymous")]
    public async Task<object> executeAnonymous(string apiVersion, [FromQuery] string anonymousBody) => throw new NotImplementedException();

    [HttpGet, Route("query")]
    public async Task<object> query(string apiVersion, [FromQuery] string q) {
        var ctx = ((CtxClaimsPrincipal)User).Ctx;
        var res = await _data.QueryAsync(ctx, apiVersion, q, true);
        if (res == null || (res.Length > 0 && res[0] is ApiError)) return NotFound(res ?? [new ApiError("QueryAsync returned null", "NULL")]);
        return new QueryRes() {
            totalSize = res.Length,
            done = true,
            records = res.Cast<SObject>()
        };
    }

    [HttpPost, Route("runTestsAsynchronous")]
    public async Task<object> runTestsAsynchronous(string apiVersion) => throw new NotImplementedException();

    [HttpGet, Route("runTestsSynchronous")]
    public async Task<object> runTestsSynchronous(string apiVersion) => throw new NotImplementedException();

    [HttpGet, Route("search")]
    public async Task<object> Get(string apiVersion, [FromQuery] string q) => throw new NotImplementedException();

    [HttpGet, Route("sobjects")]
    public async Task<object> sobjects_list(string apiVersion) => throw new NotImplementedException();

    [HttpGet, HttpPost, Route("sobjects/{sobject}")]
    public async Task<object> sobjects_get(string apiVersion, string sobject) => throw new NotImplementedException();

    [HttpGet, Route("sobjects/{sobject}/describe")]
    public async Task<object> sobjects_describe(string apiVersion, string sobject) => throw new NotImplementedException();

    [HttpGet, HttpPatch, HttpDelete, Route("sobjects/{sobject}/{id}")]
    public async Task<object> sobjects_idget(string apiVersion, string sobject, string id) => throw new NotImplementedException();

    [HttpGet, Route("sobjects/ApexLog/{id}/Body")]
    public async Task<object> sobjects_apexlog(string apiVersion, string sobject, string id) => throw new NotImplementedException();

    [HttpGet, Route("tests")]
    public async Task<object> tests(string apiVersion) => throw new NotImplementedException();
}
