namespace Moryx.Cli.Templates
{
    public static class Solution
    {

        public static void Assert(string dir, Action<string> then, Action<string> onError)
        {
            var solutionName = GetSolutionName(dir, onError);
            if (!string.IsNullOrEmpty(solutionName))
            {
                then(solutionName);
            }
        }

        public static string GetSolutionName(string dir, Action<string> onError)
        {
            return GetSolutionName(Directory.GetFiles(dir, "*.sln*"), onError);
        }

        public static string GetSolutionName(string[] files, Action<string> onError)
        {
            string[] filteredFiles = [.. files.Where(f => f.EndsWith(".slnx"))];
            if (filteredFiles.Length == 0)
            {
                filteredFiles = [.. files.Where(f => f.EndsWith(".sln"))];
            }

            if (filteredFiles.Length > 0)
            {
                if (filteredFiles.Length == 1)
                {
                    return Path.GetFileNameWithoutExtension(filteredFiles[0]);
                }
                if (filteredFiles.Length > 1)
                {
                    onError("Too many _solutions_ found. Please make sure, there is only one solution.");
                    return "";
                }
            }
            onError("No _solutions_ found. Please make sure, there is a VisualStudio solution in this directory.");
            return "";
        }
    }
}