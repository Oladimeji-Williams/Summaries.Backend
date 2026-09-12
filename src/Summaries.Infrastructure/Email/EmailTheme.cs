namespace Summaries.Infrastructure.Email;

internal static class EmailTheme
{
    private const int ContentWidth = 560;
    private const string Canvas = "#f4efe6";
    private const string Surface = "#fffdf8";
    private const string Ink = "#172032";
    private const string Muted = "#5f6b7a";
    private const string Quiet = "#8b96a3";
    private const string Faint = "#b8c0c8";
    private const string Border = "#e2dcd1";
    private const string Accent = "#e85d3f";

    public static string Apply(string html)
    {
        var themedHtml = html
            .Replace("#f1f5f9", Canvas, StringComparison.Ordinal)
            .Replace("#ffffff", Surface, StringComparison.Ordinal)
            .Replace("#0f172a", Ink, StringComparison.Ordinal)
            .Replace("#475569", Muted, StringComparison.Ordinal)
            .Replace("#94a3b8", Quiet, StringComparison.Ordinal)
            .Replace("#cbd5e1", Faint, StringComparison.Ordinal)
            .Replace("#e2e8f0", Border, StringComparison.Ordinal)
            .Replace("#4f46e5", Accent, StringComparison.Ordinal)
            .Replace("font-family:-apple-system,BlinkMacSystemFont,'Segoe UI',Roboto,Helvetica,Arial,sans-serif", "font-family:'Trebuchet MS',Verdana,sans-serif", StringComparison.Ordinal)
            .Replace("border-radius:12px", "border-radius:4px", StringComparison.Ordinal)
            .Replace("border-radius:10px", "border-radius:4px", StringComparison.Ordinal);

                return themedHtml
                        .Replace("max-width:480px", $"max-width:{ContentWidth}px", StringComparison.Ordinal)
                        .Replace("</head>", $$"""
                        <style>
                            :root { color-scheme: light dark; }
                            @media (prefers-color-scheme: dark) {
                                body, body > table { background-color:#0d1117 !important; }
                                body > table > tbody > tr > td { background-color:#0d1117 !important; }
                                body > table table { background-color:#151b24 !important; }
                                body > table table h1,
                                body > table table div,
                                body > table table p { color:#f4f7fb !important; }
                                body > table table p { color:#b8c2cf !important; }
                                body > table table a span { color:#ff8065 !important; }
                                body > table table td[style*="border-top"] { border-color:#2c3745 !important; }
                            }
                        </style>
                        </head>
                        """, StringComparison.Ordinal);
    }
}