using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Atc.CodingRules.Updater.CLI
{
    public static class DotnetBuildHelper
    {
        [SuppressMessage("Design", "MA0016:Prefer return collection abstraction instead of implementation", Justification = "OK.")]
        public static Dictionary<string, int> BuildAndCollectErrors(DirectoryInfo rootPath, FileInfo? buildFile)
        {
            if (rootPath == null)
            {
                throw new ArgumentNullException(nameof(rootPath));
            }

            var buildResult = RunBuildCommand(rootPath, buildFile);
            if (!string.IsNullOrEmpty(buildResult.Item2))
            {
                throw new DataException(buildResult.Item2);
            }

            var parsedErrors = ParseBuildOutput(buildResult);
            return parsedErrors;
        }

        [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "OK.")]
        private static Tuple<string, string> RunBuildCommand(DirectoryInfo rootPath, FileInfo? buildFile)
        {
            var arguments = "build -c Release -v q -clp:NoSummary";
            if (buildFile is not null && buildFile.Exists)
            {
                arguments = $"build {buildFile.FullName} -c Release -v q -clp:NoSummary";
            }
            else
            {
                var slnFiles = Directory.GetFiles(rootPath.FullName, "*.sln");
                if (slnFiles.Length > 1)
                {
                    var files = slnFiles.Select(x => new FileInfo(x).Name).ToList();
                    return Tuple.Create(
                        string.Empty,
                        $"Please specify which solution file to use:{Environment.NewLine} - {string.Join($"{Environment.NewLine} - ", files)}{Environment.NewLine} Specify the solution file using this option: --buildFile");
                }

                var csprojFiles = Directory.GetFiles(rootPath.FullName, "*.csproj");
                if (csprojFiles.Length > 1)
                {
                    var files = csprojFiles.Select(x => new FileInfo(x).Name).ToList();
                    return Tuple.Create(
                        string.Empty,
                        $"Please specify which C# project file to use:{Environment.NewLine} - {string.Join($"{Environment.NewLine} - ", files)}{Environment.NewLine} Specify the C# project file using this option: --buildFile");
                }
            }

            var process = new Process
            {
                StartInfo =
                {
                    WorkingDirectory = rootPath.FullName,
                    FileName = "dotnet.exe",
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                },
            };

            process.Start();
            string standardOutput = process.StandardOutput.ReadToEnd();
            string standardError = process.StandardError.ReadToEnd();
            Console.WriteLine(standardError);
            process.WaitForExit();
            return Tuple.Create(standardOutput, standardError);
        }

        [SuppressMessage("Performance", "MA0023:Add RegexOptions.ExplicitCapture", Justification = "OK.")]
        private static Dictionary<string, int> ParseBuildOutput(Tuple<string, string> buildResult)
        {
            const string? regexPattern = @": error ([A-Z]\S+?): (.*) \[";
            var errors = new Dictionary<string, int>(StringComparer.Ordinal);

            var regex = new Regex(
                regexPattern,
                RegexOptions.Multiline | RegexOptions.Compiled,
                TimeSpan.FromMinutes(2));

            var matchCollection = regex.Matches(buildResult.Item1);
            foreach (Match match in matchCollection)
            {
                if (match.Groups.Count != 3)
                {
                    continue;
                }

                var key = match.Groups[1].Value;
                if (errors.ContainsKey(key))
                {
                    errors[key] = errors[key] + 1;
                }
                else
                {
                    errors.Add(key, 1);
                }
            }

            return errors;
        }
    }
}