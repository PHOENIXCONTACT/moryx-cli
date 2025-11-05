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
            var files = Directory.GetFiles(dir, "*.slnx");
            if (files.Length == 0)
            {
                files = Directory.GetFiles(dir, "*.sln");
            }

            if (files.Length > 1)
            {
                if (files.Length == 1)
                {
                    return Path.GetFileNameWithoutExtension(files[0]);
                }
                if (files.Length > 1)
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