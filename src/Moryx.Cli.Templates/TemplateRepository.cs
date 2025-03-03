using Moryx.Cli.Templates.Models;
using System.Diagnostics;

namespace Moryx.Cli.Templates
{
    public class TemplateRepository
    {
        public static void Clone(TemplateSettings settings)
        {
            var targetDir = settings.SourceDirectory;

            if (!Directory.Exists(targetDir) || Directory.GetFiles(targetDir).Length == 0)
            {
                ExecCommanLine($"git clone {settings.Repository} -b {settings.Branch} --depth 1 --single-branch {targetDir}", _ => { });
            }
            else if(settings.Pull)
            {
                ExecCommanLine($"git -C {targetDir} pull", _ => { });
            }
        }

        public static int ExecCommanLine(string command, Action<string> onStatus)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "cmd.exe",
                    Arguments = "/c " + command,
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