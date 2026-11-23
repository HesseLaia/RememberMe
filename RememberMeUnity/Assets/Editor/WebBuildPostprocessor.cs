using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace RememberMe.Editor
{
    public sealed class WebBuildPostprocessor : IPostprocessBuildWithReport
    {
        public int callbackOrder => 1000;

        public void OnPostprocessBuild(BuildReport report)
        {
            if (report.summary.platform != BuildTarget.WebGL)
                return;

            string indexPath = Path.Combine(report.summary.outputPath, "index.html");
            string html = File.ReadAllText(indexPath);
            if (html.Contains("body.embedded", StringComparison.Ordinal))
                return;

            const string embeddedCss = @"
    body.embedded { min-height: 100dvh; display: block; overflow: hidden; padding: 0; }
    body.embedded main { width: 100vw; height: 100dvh; display: grid; grid-template-rows: minmax(0, 1fr) auto; }
    body.embedded header { display: none; }
    body.embedded #stage { min-height: 0; aspect-ratio: auto; border: 0; }
    body.embedded footer { margin: 0; padding: 6px 10px; background: #111a17; }
";
            const string embeddedScript = @"
    document.body.classList.toggle('embedded', new URLSearchParams(location.search).get('embedded') === '1');
";

            html = ReplaceOnce(html, "</style>", embeddedCss + "  </style>");
            html = ReplaceOnce(html, "<script>", "<script>" + embeddedScript);
            File.WriteAllText(indexPath, html);
        }

        static string ReplaceOnce(string value, string needle, string replacement)
        {
            int index = value.IndexOf(needle, StringComparison.Ordinal);
            if (index < 0)
                throw new InvalidOperationException("RememberMePages template marker not found: " + needle);
            return value.Substring(0, index) + replacement + value.Substring(index + needle.Length);
        }
    }
}
