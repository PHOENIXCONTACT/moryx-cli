using Moryx.Cli.Commands.Options;
using System.Net;
using System.Net.Http.Json;

namespace Moryx.Cli.Commands
{
    public static class ExecCommand
    {
        public static CommandResult Exec(ExecOptions options)
        {
            if (options?.Command?.ToLower() == "post-setup")
            {
                return PostSetup(options);
            }
            else
            {
                return CommandResult.WithError($"Unknown command {options?.Command}");
            }
        }

        internal static CommandResult PostSetup(ExecOptions options)
        {
            var httpClient = new HttpClient(new HttpClientHandler
            {
                Proxy = new WebProxy { BypassProxyOnLocal = true }
            });

            try
            {
                for (int i = 0; i < 2; i++)
                {
                    httpClient.PostAsync($"{options.Endpoint}/databases/createall", null).GetAwaiter().GetResult();
                }

                var httpResponse = httpClient.GetAsync($"{options.Endpoint}/modules").GetAwaiter().GetResult();
                var modules = httpResponse.Content.ReadFromJsonAsync<ModuleResponse[]>().GetAwaiter().GetResult();

                if (modules != null) {
                    var tasks = new List<Task>();
                    foreach (var module in modules.Where(m => m.HealthState?.ToLower() == "failure").ToList())
                    {
                        tasks.Add(httpClient.PostAsync($"{options.Endpoint}/modules/{module.Name}/reincarnate", null));
                    }
                    Task.WaitAll(tasks.ToArray());
                }
                return CommandResult.IsOk("Created databases and restarted modules.");
            }
            catch (Exception ex)
            {
                return CommandResult.WithError(ex.Message);
            }
        }
    }

    internal class ModuleResponse
    {
        public string? Name { get; set; }
        public string? HealthState { get; set; }
    }
}