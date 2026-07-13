using System.Net;
using System.Text;

namespace EternalSocial.Proxy;

/// <summary>
/// Replaces the estate footer marker in an HTML document with the pinned footer.
/// Sites opt in by carrying the marker comment in their host page template; the
/// comment is invisible when a site is reached without the gateway, and documents
/// without the marker pass through byte-identical.
/// </summary>
public static class FooterInjector
{
    public const string Marker = "<!--ETERNALSOCIAL-FOOTER-->";

    public static string Inject(string html, IEnumerable<ProxyRoute> routes)
    {
        var at = html.IndexOf(Marker, StringComparison.Ordinal);
        if (at < 0) return html;
        return string.Concat(html.AsSpan(0, at), Footer(routes), html.AsSpan(at + Marker.Length));
    }

    private static string Footer(IEnumerable<ProxyRoute> routes)
    {
        var links = new StringBuilder("<a href=\"/\">EternalSocial</a>");
        foreach (var r in routes.Where(r => r.Enabled).OrderBy(r => r.Prefix, StringComparer.Ordinal))
            links.Append($"<a href=\"{r.Prefix}/\">{WebUtility.HtmlEncode(r.Title)}</a>");

        // Publishes --es-footer-h so host templates can reserve space with
        // `var(--es-footer-h, 0rem)` (zero when the page is reached without the gateway).
        return
            "<style>" +
            ":root{--es-footer-h:2.6rem}" +
            ".es-footer{position:fixed;left:0;right:0;bottom:0;z-index:2147483000;box-sizing:border-box;height:var(--es-footer-h);" +
            "display:flex;gap:1.25rem;align-items:center;justify-content:center;flex-wrap:wrap;padding:.45rem .75rem;" +
            "font:500 .8rem/1.2 system-ui,sans-serif;overflow:hidden;" +
            "background:rgba(16,16,20,.95);color:#9aa0ac;border-top:1px solid #2a2a33;backdrop-filter:blur(6px)}" +
            ".es-footer a{color:#c9cedd;text-decoration:none}.es-footer a:hover{color:#fff;text-decoration:underline}" +
            "@media (prefers-color-scheme:light){.es-footer{background:rgba(255,255,255,.95);color:#5a5f6b;border-top-color:#e2e4ea}" +
            ".es-footer a{color:#3b4252}.es-footer a:hover{color:#000}}" +
            "</style>" +
            $"<footer class=\"es-footer\">{links}</footer>";
    }
}
