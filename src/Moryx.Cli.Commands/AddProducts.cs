using Moryx.Cli.Templates;
using Moryx.Cli.Templates.Extensions;
using Moryx.Cli.Templates.Models;
using Moryx.Products.Management;
using Moryx.Runtime.Kernel;

namespace Moryx.Cli.Commands
{
    public class AddProducts
    {
        public static CommandResult Exec(Template template, IEnumerable<string> products)
        {
            return CommandBase.Exec(template, () =>
            {
                return Add(template, products);
            });
        }

        private static CommandResult Add(Template template, IEnumerable<string> products)
        {
            var msg = new List<string>();
            foreach (var product in products)
            {
                try
                {
                    var dictionary = template.Product(product);

                    var files = template.WriteFilesToDisk(dictionary);
                    Template.ReplacePlaceHoldersInsideFiles(
                        files,
                        template.ReplaceVariables(template.Configuration.Add.Product, product)
                        );

                    UpdateProductConfig(template.Settings, product);

                    msg.Add($"Successfully added {product} product");
                }
                catch (Exception ex)
                {
                    return CommandResult.WithError($"Failed to add product `{product}`!\n" + ex.Message);
                }
            }

            return CommandResult.IsOk(string.Join("\n", msg));
        }

        private static void UpdateProductConfig(TemplateSettings settings, string product)
        {
            var filename = "Moryx.Products.Management.ModuleConfig";
            var files = Directory.GetFiles(settings.TargetDirectory, filename + ".json", SearchOption.AllDirectories);
            var configFilename = files.First();


            var configManager = new ConfigManager
            {
                ConfigDirectory = Path.GetDirectoryName(configFilename)
            };

            var config = (ModuleConfig)configManager.GetConfiguration(typeof(ModuleConfig), filename, false);
            if(config.LoadError == null)
            {
                AddIfNotExists(config.TypeStrategies, BuildProductTypeConfiguration(settings.AppName, product));
                AddIfNotExists(config.InstanceStrategies, BuildProductInstanceConfiguration(settings.AppName, product));

                configManager.SaveConfiguration(config, filename, false);
            }
            else
            {
                Console.WriteLine("Could not update product config!");
            }
        }

        private static void AddIfNotExists<T>(List<T> typeStrategies, T strategy) where T : IProductStrategyConfiguration
        {
            if (!typeStrategies.Any(ts => ts.TargetType == strategy.TargetType))
            {
                typeStrategies.Add(strategy);
            }
        }

        private static GenericTypeConfiguration BuildProductTypeConfiguration(string solutionName, string product)
        => new()
        {
            JsonColumn = "Text8",
            TargetType = $"{solutionName}.Products.{product}Type",
            PropertyConfigs = new List<PropertyMapperConfig>(),
            PluginName = "GenericTypeStrategy",
        };

        private static ProductInstanceConfiguration BuildProductInstanceConfiguration(string solutionName, string product)
        => new()
        {
            TargetType = $"{solutionName}.Products.{product}Instance",
            PluginName = "SkipInstancesStrategy",
        };
    }
}