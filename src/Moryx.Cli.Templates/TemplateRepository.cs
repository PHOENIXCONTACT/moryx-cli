using Moryx.Cli.Templates.Models;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Moryx.Cli.Templates
{
    public class TemplateRepository
    {
        public static void Clone(TemplateSettings settings)
        {
            Clone(settings, null);
        }

        public static int Clone(TemplateSettings settings, Action<string>? onStatus = null)
        {
            var targetDir = settings.SourceDirectory;

            if (!Directory.Exists(targetDir) || !Directory.EnumerateFiles(targetDir, "*", SearchOption.AllDirectories).Any())
            {
                return ExecCommandLine($"git clone {settings.Repository} -b {settings.Branch} --depth 1 --single-branch {targetDir}", _ => onStatus?.Invoke(_));
            }
            else if (settings.Pull)
            {
                return ExecCommandLine($"git -C {targetDir} pull", _ => onStatus?.Invoke(_));
            }
            return 0;
        }

        public static int ExecCommandLine(string command, Action<string> onStatus)
        {
            var (fileName, arguments) = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? ("cmd.exe", "/c " + command)
                : ("/bin/sh", "-c " + command);

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = fileName,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                },
            };
            process.OutputDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                {
                    onStatus(e.Data);
                }
            };
            process.Start();
            process.BeginOutputReadLine();
            process.WaitForExit();
            return process.ExitCode;
        }
    }
}
