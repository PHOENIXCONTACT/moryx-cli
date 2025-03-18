using Moryx.Cli.Templates.Extensions;
using Moryx.Cli.Templates.Models;
using System.Globalization;
using System.Text.RegularExpressions;


namespace Moryx.Cli.Templates
{
    public class Template
    {
        private const string IdentifierKey = "{id}";
        private const string ResourceKey = "{resource}";
        private const string SolutionNameKey = "{solutionname}";
        private const string TemplateFileExtension = ".moryxtpl";
        private readonly List<string> _fileNames;
        private readonly TemplateSettings _settings;
        private readonly TemplateConfiguration _configuration;

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

        /// <summary>
        /// Loads a `Template` with provided `TemplateSettings` and
        /// `TemplateConfiguration`
        /// </summary>
        /// <returns><see cref="Template"/></returns>
        public static Template Load(TemplateSettings settings, TemplateConfiguration configuration)
        {
            return new Template(
                settings,
                configuration,
                SearchSourceFiles(settings)
            );
        }

        /// <summary>
        /// Loads a `Template` with provided <paramref name="settings"/> and
        /// <paramref name="configuration"/>. It takes a custom list of template <paramref name="fileNames"/>
        /// that are to be used. By default, those <paramref name="fileNames"/> will be 
        /// searched from the filesystem depending on <paramref name="settings"/>.
        /// </summary>
        /// <returns><see cref="Template"/></returns>
        public static Template Load(TemplateSettings settings, TemplateConfiguration configuration, List<string> fileNames)
        {
            var template = new Template(settings, configuration, fileNames);
            return template;
        }

        public static List<string> SearchSourceFiles(TemplateSettings settings)
        {
            var files = Directory.GetFiles(
                Path.Combine(settings.SourceDirectory),
                "*",
                new EnumerationOptions
                {
                    ReturnSpecialDirectories = false,
                    RecurseSubdirectories = true
                });

            return [.. files];
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

        private Dictionary<string, string> FilteredFileStructure(ConfigurationPattern pattern, string identifier = "", Dictionary<string, string>? placeholders = null)
        {
            pattern = new ConfigurationPattern
            {
                Files = pattern.Files.Select(f => f.OsAware()).ToList(),
                Replacements = pattern.Replacements,
            };
            var fileNames = FilterByPattern(_settings.SourceDirectory, pattern);
            return PrepareFileStructure(fileNames, pattern, identifier, placeholders);
        }

        /// <summary>
        /// Returns a dictionary of template files containing
        /// their source location (key) inside the template repository
        /// and their prepared/renamed target location (value) for
        /// products
        /// </summary>
        /// <param name="name">Product name</param>
        public Dictionary<string, string> Product(string name)
            => FilteredFileStructure(_configuration.Add.Product, name);


        /// <summary>
        /// Returns a dictionary of template files containing
        /// their source location (key) inside the template repository
        /// and their prepared/renamed target location (value) for
        /// a `StateBase` file
        /// </summary>
        /// <param name="name">Resource implementing `IStateContext`</param>
        public Dictionary<string, string> StateBaseFile(string resourceName)
            => FilteredFileStructure(_configuration.Add.StateBase, "", new() { { ResourceKey, resourceName } });


        /// <summary>
        /// Returns a dictionary of template files containing
        /// their source location (key) inside the template repository
        /// and their prepared/renamed target location (value) for
        /// states
        /// </summary>
        /// <param name="name">State name</param>
        public Dictionary<string, string> StateFile(string name, string resourceName)
            => FilteredFileStructure(_configuration.Add.State, name, new() { { ResourceKey, resourceName } });


        /// <summary>
        /// Returns a dictionary of template files containing
        /// their source location (key) inside the template repository
        /// and their prepared/renamed target location (value) for
        /// steps
        /// </summary>
        /// <param name="name">Step name</param>
        public Dictionary<string, string> Step(string name)
            => FilteredFileStructure(_configuration.Add.Step, name);


        /// <summary>
        /// Returns a dictionary of template files containing
        /// their source location (key) inside the template repository
        /// and their prepared/renamed target location (value) for
        /// modules
        /// </summary>
        /// <param name="name">Module name</param>
        public Dictionary<string, string> Module(string name)
            => FilteredFileStructure(_configuration.Add.Module, name);


        /// <summary>
        /// Returns a dictionary of template files containing
        /// their source location (key) inside the template repository
        /// and their prepared/renamed target location (value) for
        /// resources
        /// </summary>
        /// <param name="name">Resource name</param>
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

        /// <summary>
        /// Prepares a dictionary for a list of filenames and their
        /// destinations based on the provided pattern, optionally
        /// an <paramref name="identifier"/> (e.g. a product name), 
        /// and a custom dictionary of placeholders to be applied.
        /// </summary>
        public Dictionary<string, string> PrepareFileStructure(List<string> fileNames, ConfigurationPattern pattern, string identifier = "", Dictionary<string, string>? placeholders = null)
        {
            var patterns = ReplaceVariables(pattern, identifier, placeholders);
            return PrepareFileStructure(fileNames, patterns);
        }

        /// <summary>
        /// Prepares a dictionary for a list of filenames and prepares
        /// their destinations based on the provided patterns
        /// 
        /// For example: 
        /// `<"src/MyApplication.Products/MyProduct", "src/BigFactory.Products/Battery">`
        /// </summary>
        public Dictionary<string, string> PrepareFileStructure(List<string> fileNames, Dictionary<string, string> patterns)
            => fileNames
                .ToDictionary(
                    f => f,
                    f => ReplacePlaceholders(f, patterns)
                );

        public Dictionary<string, string> ReplaceVariables(ConfigurationPattern pattern, string identifier = "", Dictionary<string, string>? variables = null)
        {
            var replacements = pattern.Replacements;
            foreach (var replacement in _configuration.New.Replacements)
            {
                replacements.TryAdd(replacement.Key, replacement.Value);
            }

            variables = variables ?? [];
            variables.TryAdd(IdentifierKey, identifier);
            variables.TryAdd(SolutionNameKey, _settings.AppName);

            return replacements
                .ToDictionary(
                    r => r.Key,
                    r => ApplyVariables(r.Value, variables)
                );
        }

        private static string ApplyVariables(string str, Dictionary<string, string> placeholders, int index = 0)
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