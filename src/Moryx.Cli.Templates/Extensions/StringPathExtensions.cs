namespace Moryx.Cli.Templates.Extensions
{
    public static class StringPathExtensions
    {
        public static string OsAware(this string path)
        {
            return path.Replace('\\', Path.DirectorySeparatorChar);
        }
    }
}
