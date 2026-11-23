using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace RememberMe.Editor
{
    public static class WebBuild
    {
        public static void Build()
        {
            if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.WebGL, BuildTarget.WebGL))
                throw new InvalidOperationException("Install WebGL Build Support for this Unity Editor.");

            string output = ArgumentValue("-customBuildPath");
            if (string.IsNullOrWhiteSpace(output))
                throw new ArgumentException("Missing required -customBuildPath argument.");

            output = Path.GetFullPath(output);
            const string fontLicense = "Assets/Art/Fonts/Tinos-LICENSE.txt";
            if (!File.Exists(fontLicense))
                throw new FileNotFoundException("The browser typeface license is required.", fontLicense);

            Act1Verification.Verify();
            Act1Builder.Assemble();

            Directory.CreateDirectory(output);
            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Gzip;
            PlayerSettings.WebGL.decompressionFallback = true;
            PlayerSettings.WebGL.template = "PROJECT:RememberMePages";

            var report = BuildPipeline.BuildPlayer(
                new[] { Act1Builder.ScenePath, "Assets/Scenes/Kitchen.unity" }, output, BuildTarget.WebGL, BuildOptions.None);
            if (report.summary.result != BuildResult.Succeeded)
                throw new InvalidOperationException("Web build: " + report.summary.result);

            File.WriteAllText(Path.Combine(output, ".nojekyll"), string.Empty);
            File.Copy(fontLicense, Path.Combine(output, "Tinos-LICENSE.txt"), true);

            var oversized = Directory.EnumerateFiles(output, "*", SearchOption.AllDirectories)
                .FirstOrDefault(file => new FileInfo(file).Length >= 100L * 1024 * 1024);
            if (oversized != null)
                throw new InvalidOperationException("The Web build contains a file at or above 100 MiB: " + oversized);

            Debug.Log("REMEMBER_CLEAN_WEB_BUILD_PASS: " + report.summary.totalSize + " bytes -> " + output);
        }

        static string ArgumentValue(string name)
        {
            string[] args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length - 1; i++)
                if (string.Equals(args[i], name, StringComparison.Ordinal))
                    return args[i + 1];
            return null;
        }
    }
}
