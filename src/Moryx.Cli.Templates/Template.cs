using Moryx.Cli.Templates.Extensions;
using Moryx.Cli.Templates.Models;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;


namespace Moryx.Cli.Templates
{
    public static class Template
    {
        public const string AppPlaceholder = "MyApplication";
        public const string ProductPlaceholder = "MyProduct";
        public const string ModulePlaceholder = "MyModule";
        public const string StepPlaceholder = "Some";
        public const string CellPlaceholder = "SomeCell";
        public const string StateBasePlaceholder = "SomeStateBase";
        public const string StatePlaceholder = "SpecificState";
        public const string ResourcePlaceholder = "SomeResource";
        public const string ResourcePlaceholder2 = "MyResource";

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

        public static List<string> FilterByPattern(this List<string> list, string root, ConfigurationPattern pattern)
        {
            var trimmed = list.Select(list => list.Replace(root, ""))

                .Where(s => pattern.Files.Any(p => Matches(s, p)))
                .ToList();
            return trimmed;
        }

        private static bool Matches(string input, string pattern)
        {
            string separator = $"{Path.DirectorySeparatorChar}";
            if (separator == "\\") {
                separator = "\\\\";
            }
            string regexPattern = "^" + Regex.Escape(pattern)
                .Replace(@$"\*\*/{separator}", $"(.*{separator})?")
                .Replace(@"\*", @$"[^{separator}]*")
                .Replace(@"\?", @$"[^{separator}]") + "$";
            return Regex.IsMatch(input, regexPattern, RegexOptions.IgnoreCase);
        }

        public static List<string> BareProjectFiles(this List<string> list)
        {
            var keep = list.ResourcesProject();
            var shrinked = list
                .WithoutStep()
                .WithoutProduct()
                .WithoutRecipe()
                .WithoutSetupTrigger()
                .WithoutCellSelector()
                .WithoutModule()
                .WithoutResource()
                .WithoutState()
                ;

            shrinked.AddRange(keep);
            return shrinked;
        }

        public static List<string> WithoutProduct(this List<string> list)
        {
            return list.Except(list.Product()).ToList();
        }

        public static List<string> WithoutStep(this List<string> list)
        {
            return list.Except(list.Step()).ToList();
        }

        public static List<string> WithoutRecipe(this List<string> list)
        {
            return list.Except(list.Recipe()).ToList();
        }

        public static List<string> WithoutSetupTrigger(this List<string> list)
        {
            return list.Except(list.SetupTrigger()).ToList();
        }

        public static List<string> WithoutCellSelector(this List<string> list)
        {
            return list.Except(list.CellSelector()).ToList();
        }

        public static List<string> WithoutModule(this List<string> list)
        {
            return list.Except(list.Module()).ToList();
        }
        
        public static List<string> WithoutState(this List<string> list)
        {
            return list.Except(list.StateFile().Concat(list.StateBaseFile())).ToList();
        }

        public static List<string> WithoutResource(this List<string> list)
        {
            return list.Except(list.Resource()).ToList();
        }


        public static List<string> Product(this List<string> list)
        {
            var whitelist = new List<string>(){
                "MyProductInstance.cs",
                "MyProductType.cs"
            };
            return list
                .Intersect(whitelist, new ListComparer())
                .ToList();
        }

        public static List<string> StateBaseFile(this List<string> list)
            => list.Intersect("StateBase.cs");

        public static List<string> StateFile(this List<string> list)
            => list.Intersect("State.cs");

        public static List<string> Step(this List<string> list)
        {
            var whitelist = list
                .Where(e => e.Contains("Some"))
                .ToList();
            whitelist.Add("SimulatedInOutDriver.cs");
            whitelist.Add("MyApplication.Resources.csproj");

            return list
                .Intersect(whitelist, new ListComparer())
                .ToList();
        }

        public static List<string> ResourcesProject(this List<string> list)
        {
            var whitelist = new List<string>
            {
                "MyApplication.Resources.csproj"
            };

            return list
                .Intersect(whitelist, new ListComparer())
                .ToList();
        }

        public static List<string> Module(this List<string> list)
        {
            var whitelist = list
                .Where(e => e.Contains("MyModule"))
                .ToList();

            return list
                .Intersect(whitelist, new ListComparer())
                .ToList();
        }

        public static List<ProjectFileInfo> InitialProjects(this List<string> list)
        {
            return list
                .Where(s => s.EndsWith(".csproj"))
                .Select(s => ExtractProjectInfo(s))
                .Where(s => s != null)
                .Select(f => new ProjectFileInfo
                {
                    Name = f!.Name,
                    Filename = f.Filename,
                    Extension = f.Extension,
                })
                .ToList();
        }

        public static List<string> Recipe(this List<string> list)
        {
            var whitelist = new List<string>(){
                "MyApplicationRecipe.cs",
            };
            return list
                .Intersect(whitelist, new ListComparer())
                .ToList();
        }

        public static List<string> Resource(this List<string> list)
        {
            var whitelist = list
                .Where(e => e.Contains("ISomeResource") || e.Contains("MyResource"))
                .ToList();

            return list
                .Intersect(whitelist, new ListComparer())
                .ToList();
        }

        public static List<string> SetupTrigger(this List<string> list)
        {
            var whitelist = new List<string>(){
                "MySetupTrigger.cs",
                "MySetupTriggerConfig.cs",
            };
            return list
                .Intersect(whitelist, new ListComparer())
                .ToList();
        }

        public static List<string> CellSelector(this List<string> list)
        {
            var whitelist = new List<string>(){
                "MyCellSelector.cs",
                "MyCellSelectorConfig.cs",
            };
            return list
                .Intersect(whitelist, new ListComparer())
                .ToList();
        }

        private static ProjectFileInfo ExtractProjectInfo(string str)
            => new ProjectFileInfo
            {
                Name = Path.GetFileNameWithoutExtension(str),
                Filename = str,
                Extension = Path.GetExtension(str),
            };

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

        public static Dictionary<ProjectFileInfo, List<string>> PrepareFileStructure(string solution, List<string> cleanedResourceNames, List<ProjectFileInfo> projectFilenames)
        {
            var dictionary = new Dictionary<ProjectFileInfo, List<string>>();

            // Reverse list to avoid problems with similar project names
            // e.g. Moryx.App vs Moryx.App.Plus
            var reverseFilenames = projectFilenames
                .OrderByDescending(x => x.Name)
                .ToList();
            foreach (var project in reverseFilenames)
            {
                var projectRelatedFiles = cleanedResourceNames
                    .Where(f => f.Contains(project.Name) && !f.EndsWith(".sln"))
                    .ToList();
                cleanedResourceNames = cleanedResourceNames
                    .Except(projectRelatedFiles)
                    .ToList();
                dictionary.Add(project, projectRelatedFiles);
            }
            dictionary.Add(new ProjectFileInfo { Extension = "", Filename = "", Name = "" }, cleanedResourceNames);
            return dictionary;
        }

        public static IEnumerable<string> WriteFilesToDisk(Dictionary<ProjectFileInfo, List<string>> dictionary, TemplateSettings settings, Func<string, string> customReplace)
        {
            var result = new List<string>();
            foreach (var pair in dictionary.SelectMany(pair => pair.Value))
            {
                var newFilename = customReplace(pair.Replace(settings.SourceDirectory, settings.TargetDirectory)).Replace(AppPlaceholder, settings.AppName);
                var path = Path.Combine(settings.TargetDirectory, pair);

                Directory.CreateDirectory(Path.GetDirectoryName(newFilename) ?? "");
                File.Copy(pair, newFilename, true);
                result.Add(newFilename);
            }

            return result;
        }

        public static string ReplaceProductName(this string str, string productName)
            => str.Replace(ProductPlaceholder, productName);

        public static string ReplaceStepName(this string str, string stepName)
            => str.Replace(ResourcePlaceholder, stepName);

        public static string GetSolutionName(string dir, Action<string> onError)
        {
            var files = Directory.GetFiles(dir, "*.sln");
            if (files != null)
            {
                if (files.Length == 1)
                {
                    return Path.GetFileNameWithoutExtension(files[0]);
                }
                if (files.Length > 1)
                {
                    onError("Too many `.sln` found. Please make sure, there is only one solution.");
                    return "";
                }
            }
            onError("No `.sln` found. Please make sure, there is a VisualStudio solution in this directory.");
            return "";
        }

        public static void AssertSolution(string dir, Action<string> then, Action<string> onError)
        {
            var solutionName = GetSolutionName(dir, onError);
            if (!string.IsNullOrEmpty(solutionName))
            {
                then(solutionName);
            }
        }
    }

    public class ProjectFileInfo
    {
        public required string Name { get; set; }
        public required string Filename { get; set; }
        public required string Extension { get; set; }


        public override bool Equals(object? obj)
        {
            return obj is ProjectFileInfo other &&
                   Name == other.Name &&
                   Filename == other.Filename;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Filename);
        }
    }


    internal class ListComparer : IEqualityComparer<string>
    {

        public bool Equals(string? x, string? y)
        {
            if (x == null || y == null) 
                return false;
            return y.Contains(x);
        }

        public int GetHashCode([DisallowNull] string obj)
        {
            return 0;
        }
    }
}