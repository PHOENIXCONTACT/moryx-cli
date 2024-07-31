namespace Moryx.Cli.Template.Extensions
{
    public static class ListExtensions
    {
        public static void Each<T>(this IEnumerable<T> e, Action<T, int> action)
        {
            var i = 0;
            foreach (var item in e)
            {
                action(item, i++);
            }
        }
    }
}
