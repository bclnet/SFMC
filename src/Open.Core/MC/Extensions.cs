using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace Open.MC;

/// <summary>
/// Extensions
/// </summary>
//public static class Extensions {
//    static MethodInfo GetContentStreamAsyncInfo = typeof(HttpContentJsonExtensions).GetMethod("GetContentStreamAsync", BindingFlags.Static | BindingFlags.NonPublic)!;
//    public static Task<(T?, TError?)> ReadFromJsonAsync<T, TError>(this HttpContent content, CancellationToken cancellationToken = default) => ReadFromJsonAsync<T, TError>(content, options: null, cancellationToken: cancellationToken);
//    public static async Task<(T?, TError?)> ReadFromJsonAsync<T, TError>(this HttpContent content, JsonSerializerOptions? options, CancellationToken cancellationToken = default) {
//        var b = new byte[20];
//        using var ms = new MemoryStream();
//        using var s = await GetContentStreamAsync(content, cancellationToken).ConfigureAwait(false);
//        await s.CopyToAsync(ms, cancellationToken);
//        ms.Position = 0; var readString = Encoding.UTF8.GetString(b, 0, ms.Read(b, 0, 20)); ms.Position = 0;
//        var errorAt = readString.IndexOf("\"error\":");
//        return errorAt == -1 || errorAt > 5
//            ? (await JsonSerializer.DeserializeAsync<T>(ms, options ?? JsonSerializerOptions.Web, cancellationToken).ConfigureAwait(false), default)
//            : (default, await JsonSerializer.DeserializeAsync<TError>(ms, options ?? JsonSerializerOptions.Web, cancellationToken).ConfigureAwait(false));
//    }
//    static ValueTask<Stream> GetContentStreamAsync(HttpContent content, CancellationToken cancellationToken) => (ValueTask<Stream>)GetContentStreamAsyncInfo.Invoke(null, [content, cancellationToken])!;
//}
