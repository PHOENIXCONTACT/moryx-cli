using Moryx.Cli.Template.Models;
using System.Diagnostics;

namespace Moryx.Cli.Template
{
    public class TemplateRepository
    {
        public static void Clone(TemplateSettings settings)
        {
            var targetDir = settings.SourceDirectory;

            if (!Directory.Exists(targetDir))
            {
                ExecCommandLine($"git clone {settings.Repository} -b {settings.Branch} --depth 1 --single-branch {targetDir}", _ => { });
            }
            else if(settings.Pull)
            {
                ExecCommandLine($"git -C {targetDir} pull", _ => { });
            }
        }

        public static int ExecCommandLine(string command, Action<string> onStatus)
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