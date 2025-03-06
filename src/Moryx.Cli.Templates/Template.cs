using Moryx.Cli.Templates.Extensions;
using Moryx.Cli.Templates.Models;
using System.Globalization;
using System.Text.RegularExpressions;


namespace Moryx.Cli.Templates
{
    public class Template
    {
        private List<string> _fileNames;
        public const string AppPlaceholder = "MyApplication";
        public const string ProductPlaceholder = "MyProduct";
        public const string ModulePlaceholder = "MyModule";
        public const string StepPlaceholder = "Some";
        public const string CellPlaceholder = "SomeCell";
        public const string StateBasePlaceholder = "SomeStateBase";
        public const string StatePlaceholder = "SpecificState";
        public const string ResourcePlaceholder = "SomeResource";
        public const string ResourcePlaceholder2 = "MyResource";
        private const string ResourceKey = "{resource}";
        private const string TemplateFileExtension = ".moryxtpl";
        private TemplateSettings _settings;
        private TemplateConfiguration _configuration;

        public TemplateSettings Settings { get { return _settings; } }
        public TemplateConfiguration Configuration { get { return _configuration; } }
        public List<string> FileNames { get { return _fileNames; } }

        public string AppName { get { return _settings.AppName; } }

        private Template(TemplateSettings settings, TemplateConfiguration configuration, List<string> files)
        {
            _settings = settings;
            _configuration = configuration;
            _fileNames = files;
        }

        public static Template Load(TemplateSettings settings, TemplateConfiguration configuration)
        {
            return new Template(
                settings,
                configuration,
                GetCleanedResourceNames(settings)
            );
        }

        public static Template Load(TemplateSettings settings, TemplateConfiguration configuration, List<string> files)
        {
            var template = new Template(settings, configuration, files);
            return template;
        }

        public static List<string> GetCleanedResourceNames(TemplateSettings settings)
        {
            var files = Directory.GetFiles(
                Path.Combine(settings.SourceDirectory),
                "*",
                new EnumerationOptions
                {
                    ReturnSpecialDirectories = false,
                    RecurseSubdirectories = true
                });

            return files.ToList();
        }

        public List<string> FilterByPattern(string root, ConfigurationPattern pattern)
        {
            if (!root.EndsWith(Path.DirectorySeparatorChar))
            {
                root += Path.DirectorySeparatorChar;
            }
            var trimmed = _fileNames
                .Select(list => list.TrimStart(root))
                .Where(s => pattern.Files.Any(p => Matches(s, p)) && !s.EndsWith(TemplateFileExtension))
                .ToList();

            return trimmed;
        }

        private static bool Matches(string input, string pattern)
        {
            string separator = $"{Path.DirectorySeparatorChar}";
            if (separator == "\\")
            {
                separator = "\\\\";
            }
            string regexPattern = "^" + Regex.Escape(pattern)
                .Replace(@$"\*\*{separator}", $"(.*{separator})?")
                .Replace(@"\*", @$"[^{separator}]*")
                .Replace(@"\?", @$"[^{separator}]") + "$";
            return Regex.IsMatch(input, regexPattern, RegexOptions.IgnoreCase);
        }

        public Dictionary<string, string> NewProject()
            => FilteredFileStructure(_configuration.New);

        private Dictionary<string, string> FilteredFileStructure(ConfigurationPattern pattern, string identifier = "")
        {
            pattern = new ConfigurationPattern
            {
                Files = pattern.Files.Select(f => f.OsAware()).ToList(),
                Replacements = pattern.Replacements,
            };
            var fileNames = FilterByPattern(_settings.SourceDirectory, pattern);
            return PrepareFileStructure(fileNames, pattern, identifier);
        }

        private Dictionary<string, string> FilteredFileStructure(ConfigurationPattern pattern, string identifier, Dictionary<string, string> placeholders)
        {
            var fileNames = FilterByPattern(_settings.SourceDirectory, pattern);
            return PrepareFileStructure(fileNames, pattern, identifier, placeholders);
        }

        public Dictionary<string, string> Product(string name)
            => FilteredFileStructure(_configuration.Add.Product, name);
        
        public Dictionary<string, string> StateBaseFile(string resourceName)
            => FilteredFileStructure(_configuration.Add.StateBase, "", new() { { ResourceKey, resourceName } });

        public Dictionary<string, string> StateFile(string name, string resourceName)
            => FilteredFileStructure(_configuration.Add.State, name, new() { { ResourceKey, resourceName } });

        public Dictionary<string, string> Step(string name)
            => FilteredFileStructure(_configuration.Add.Step, name);

        public Dictionary<string, string> Module(string name)
            => FilteredFileStructure(_configuration.Add.Module, name);

        public Dictionary<string, string> Resource(string identifier)
            => FilteredFileStructure(_configuration.Add.Resource, identifier);


        public static void ReplacePlaceHoldersInsideFiles(IEnumerable<string> filenames, Dictionary<string, string> dict)
        {
            var tasks = new List<Task>();
            foreach (var file in filenames)
            {
                tasks.Add(ReplaceInFile(file, dict));
            }

            Task.WaitAll(tasks.ToArray());
        }

        private static async Task ReplaceInFile(string file, Dictionary<string, string> dict)
        {
            var text = await File.ReadAllTextAsync(file);
            foreach (var entry in dict)
            {
                text = text.Replace(entry.Key, entry.Value);
            }
            await File.WriteAllTextAsync(file, text);
        }

        public Dictionary<string, string> PrepareFileStructure(List<string> fileNames, ConfigurationPattern pattern, string identifier = "", Dictionary<string, string> placeholders = null)
        {
            var patterns = ReplaceVariables(pattern, identifier, placeholders);
            return PrepareFileStructure(fileNames, patterns);
        }

        public Dictionary<string, string> PrepareFileStructure(List<string> fileNames, Dictionary<string, string> patterns)
            => fileNames
                .Select(f => new
                {
                    Key = f,
                    Value = ReplacePlaceholders(f, patterns)
                })
                .ToDictionary(x => x.Key, x => x.Value);

        public Dictionary<string, string> ReplaceVariables(ConfigurationPattern pattern, string identifier = "", Dictionary<string, string>? variables = null)
        {
            var replacements = pattern.Replacements;
            foreach(var replacement in _configuration.New.Replacements)
            {
                replacements.TryAdd(replacement.Key, replacement.Value);
            }

            variables = variables ?? [];
            variables.TryAdd("{id}", identifier);
            variables.TryAdd("{solutionname}", _settings.AppName);

            return replacements
                .Select(r => new
                {
                    r.Key,
                    Value = ApplyVariables(r.Value, variables)
                })
                .ToDictionary(x => x.Key, x => x.Value)
                ;
        }

        public static string ApplyVariables(string str, Dictionary<string, string> placeholders, int index = 0)
        {
            if (index >= placeholders.Count)
                return str;

            var placeholder = placeholders.ElementAt(index);
            str = str.Replace(placeholder.Key, placeholder.Value, ignoreCase: true, CultureInfo.CurrentCulture);

            return ApplyVariables(str, placeholders, index + 1);
        }

        private static string ReplacePlaceholders(string f, Dictionary<string, string> patterns)
        {
            var result = f;
            foreach (var p in patterns)
            {
                result = result.Replace(p.Key, p.Value);
            }
            return result;
        }

        public IEnumerable<string> WriteFilesToDisk(Dictionary<string, string> dictionary, bool force = false)
        {
            var result = new List<string>();
            foreach (var pair in dictionary)
            {
                var from = Path.Combine(_settings.SourceDirectory, pair.Key);
                var to = Path.Combine(_settings.TargetDirectory, pair.Value);

                Directory.CreateDirectory(Path.GetDirectoryName(to) ?? "");
                File.Copy(from, to, force);
                result.Add(to);
            }

            return result;
        }
    }
}