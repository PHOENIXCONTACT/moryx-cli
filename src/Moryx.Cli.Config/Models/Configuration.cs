using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Moryx.Cli.Config.Models
{
    public class Configuration
    {
        public Dictionary<string, Profile> Profiles { get; set; }

        public static Configuration Load(string directory) {
            try
            {
                using var file = File.OpenText(GetFilename(directory));
                var serializer = new JsonSerializer();
                return (Configuration)serializer.Deserialize(file, typeof(Configuration))
                    ?? DefaultConfiguration();
            } catch (Exception)
            {
                return DefaultConfiguration();
            }
        }

        public static Configuration DefaultConfiguration()
        {
            return new Configuration
            {
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
