namespace Moryx.Cli.Templates
{
    public static class Solution
    {

        public static void AssertSolution(string dir, Action<string> then, Action<string> onError)
        {
            var solutionName = GetSolutionName(dir, onError);
            if (!string.IsNullOrEmpty(solutionName))
            {
                then(solutionName);
            }
        }

        //public static string ReplaceProductName(this string str, string productName)
        //    => str.Replace(ProductPlaceholder, productName);

        //public static string ReplaceStepName(this string str, string stepName)
        //    => str.Replace(ResourcePlaceholder, stepName);

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
    }
}