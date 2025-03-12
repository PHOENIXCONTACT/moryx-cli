using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.ComponentModel;

namespace Moryx.Cli.Config.Models
{
    public class Configuration
    {
        private const string DefaultProfileName = "default";

        public required string DefaultProfile { get; set; } = DefaultProfileName;

        public required Dictionary<string, Profile> Profiles { get; set; }

        public static Configuration Load(string directory)
        {
            try
            {
                using var file = File.OpenText(GetFilename(directory));
                var serializer = new JsonSerializer();
                var config = serializer.Deserialize(file, typeof(Configuration)) as Configuration ?? DefaultConfiguration();
                config.DefaultProfile ??= DefaultProfileName;

                return config;
            }
            catch (Exception)
            {
                return DefaultConfiguration();
            }
        }

        public static Configuration DefaultConfiguration()
        {
            return new Configuration
            {
                DefaultProfile = "default",
                Profiles = new Dictionary<string, Profile> { { "default",
                    new Profile
                    {
                        Branch = "machine",
                        Repository = "https://github.com/PHOENIXCONTACT/MORYX-Template.git",
                    }
                }}
            };
        }

        public void Save(string directory)
        {
            using var file = File.CreateText(GetFilename(directory));
            var serializer = new JsonSerializer
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            };
            serializer.Serialize(file, this);
        }

        private static string GetFilename(string directory)
            => Path.Combine(directory, ".moryxcli");
    }
}
