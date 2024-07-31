using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moryx.Cli.Template.Exceptions
{
    /// <summary>
    /// Gets raised when trying to add a state, that already
    /// exists inside the `StateBase` class.
    /// </summary>
    public class StateAlreadyExistsException : Exception
    {
        public StateAlreadyExistsException(string state) : base($"Cannot add {state}. It already exists.")
        {

        }
    }
}
