using Moryx.Cli.Commands.Options;
using Moryx.Cli.Templates.Models;

namespace Moryx.Cli.Commands.Extensions
{
    public static class ConfigExtensions
    {
        public static TemplateSettings AsTemplateSettings(this Config.Models.Configuration configuration, string dir, string solutionName, string profile = "default")
            => new()
            {
                Branch = configuration.Profiles[profile].Branch,
                Repository = configuration.Profiles[profile].Repository,
                AppName = solutionName,
                TargetDirectory = dir,
            };

        public static TemplateSettings LoadSettings(string dir, string solutionName, string profile = "default")
            => Config.Models.Configuration.Load(dir).AsTemplateSettings(dir, solutionName, profile);

        public static Config.Models.Configuration ToConfiguration(this NewOptions options, string profile = "default")
        {
            var result = Config.Models.Configuration.DefaultConfiguration();
            result.Profiles[profile].Repository = options.Template ?? DefaultValues.DefaultTemplate;
            result.Profiles[profile].Branch = options.Branch ?? DefaultValues.DefaultBranch;
            return result;
        }
    }
}
