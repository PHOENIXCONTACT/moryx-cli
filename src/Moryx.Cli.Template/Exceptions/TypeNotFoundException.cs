namespace Moryx.Cli.Template.Exceptions
{
    /// <summary>
    /// Gets raised when an expected type is not found 
    /// inside a project/file
    /// </summary>
    public class TypeNotFoundException : Exception
    {
        public TypeNotFoundException(string type) : base($"Type {type} could not be found.")
        {

        }
    }
}
