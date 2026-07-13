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

    // Install targets the ROOT app: on gateway pages the captured beforeinstallprompt
    // fires directly; on proxied site pages (which own their own PWA manifests) the
    // button routes to /?install=1 and the landing auto-prompts when eligible.
    private const string Actions =
        "<button type=\"button\" id=\"es-install\">Install App</button>" +
        "<button type=\"button\" id=\"es-share\">Share</button>" +
        "<script>(function(){var p=null;" +
        "addEventListener('beforeinstallprompt',function(e){e.preventDefault();p=e;" +
        "if(new URLSearchParams(location.search).get('install')==='1'){p.prompt();p=null;}});" +
        "var ib=document.getElementById('es-install');" +
        "if(ib)ib.addEventListener('click',function(){if(p){p.prompt();p=null;}else{location.href='/?install=1';}});" +
        "var sb=document.getElementById('es-share');" +
        "if(sb)sb.addEventListener('click',function(){var u=location.origin + '/';" +
        "if(navigator.share){navigator.share({title:'EternalSocial',url:u}).catch(function(){});}" +
        "else if(navigator.clipboard){navigator.clipboard.writeText(u);var t=sb.textContent;sb.textContent='Copied!';" +
        "setTimeout(function(){sb.textContent=t;},1500);}});})();</script>";

    private static string Footer(IEnumerable<ProxyRoute> routes)
    {
        var links = new StringBuilder("<a href=\"/\">EternalSocial</a>");
        foreach (var r in routes.Where(r => r.Enabled).OrderBy(r => r.Prefix, StringComparer.Ordinal))
            links.Append($"<a href=\"{r.Prefix}/\">{WebUtility.HtmlEncode(r.Title)}</a>");

        // Publishes --es-footer-h so host templates can reserve space with
        // `var(--es-footer-h, 0rem)` (zero when the page is reached without the gateway).
        // The variable grows at narrow widths where the links wrap to a second row,
        // so reservations stay accurate and no link is ever clipped.
        return
            "<style>" +
            ":root{--es-footer-h:2.6rem}" +
            "@media (max-width:560px){:root{--es-footer-h:4.4rem}}" +
            ".es-footer{position:fixed;left:0;right:0;bottom:0;z-index:2147483000;box-sizing:border-box;min-height:var(--es-footer-h);" +
            "display:flex;gap:.9rem 1.25rem;align-items:center;align-content:center;justify-content:center;flex-wrap:wrap;padding:.45rem .75rem;" +
            "font:500 .8rem/1.2 system-ui,sans-serif;" +
            "background:rgba(16,16,20,.95);color:#9aa0ac;border-top:1px solid #2a2a33;backdrop-filter:blur(6px)}" +
            ".es-footer a{color:#c9cedd;text-decoration:none}.es-footer a:hover{color:#fff;text-decoration:underline}" +
            ".es-footer button{background:none;border:1px solid currentColor;border-radius:999px;color:inherit;cursor:pointer;" +
            "font:inherit;padding:.1rem .6rem}.es-footer button:hover{color:#fff;border-color:#fff}" +
            "@media (prefers-color-scheme:light){.es-footer{background:rgba(255,255,255,.95);color:#5a5f6b;border-top-color:#e2e4ea}" +
            ".es-footer a{color:#3b4252}.es-footer a:hover{color:#000}" +
            ".es-footer button:hover{color:#000;border-color:#000}}" +
            "</style>" +
            $"<footer class=\"es-footer\">{links}{Actions}</footer>";
    }
}
