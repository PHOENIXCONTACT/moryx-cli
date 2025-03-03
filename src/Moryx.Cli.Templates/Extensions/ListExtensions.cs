
namespace Moryx.Cli.Templates.Extensions
{
    public static class ListExtensions
    {
        public static List<string> Intersect(this List<string> list, string filename)
        {
            var whitelist = new List<string>(){
                filename
            };
            return list
                .Intersect(whitelist, new ListComparer())
                .ToList();
        }
    }
}
