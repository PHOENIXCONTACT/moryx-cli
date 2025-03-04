using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moryx.Cli.Tests.Extensions
{
    internal static class StringPathExtensions
    {
        internal static string OsAware(this string path)
        {
            return path.Replace('\\', Path.DirectorySeparatorChar);
        }
    }
}
