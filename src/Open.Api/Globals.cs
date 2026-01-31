namespace Open;

public class Globals {
    public const string HOST = "https://localhost:7019";
    public const string NGROK = "https://propublicity-nondiathermanous-liv.ngrok-free.dev";

    public readonly static string CLIENT_ID = Environment.GetEnvironmentVariable("MC_CLIENT_ID")!;
    public readonly static string CLIENT_SECRET = Environment.GetEnvironmentVariable("CLIENT_SECRET")!;
    public readonly static string SUBDOMAIN = Environment.GetEnvironmentVariable("SUBDOMAIN")!;
    public const string SCOPE = "data_extensions_read offline"; // https://developer.salesforce.com/docs/marketing/marketing-cloud/references/mc_rest_rest_permission_scopes/rest-permissions-and-scopes.html
}
