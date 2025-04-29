using System.Xml;
using System.Xml.Linq;

namespace Moryx.Cli.Commands.Components
{
    internal class ProjectFileManipulation
    {
        private const string ProjectReference = "ProjectReference";
        private const string ItemGroup = "ItemGroup";

        public static void AddProjectReference(string targetProjectFileName, string referenceProjectFileName)
        {

            var referencePath = Path.GetRelativePath(Path.GetDirectoryName(targetProjectFileName), Path.GetDirectoryName(referenceProjectFileName));
            referencePath = Path.Combine(referencePath, Path.GetFileName(referenceProjectFileName));

            var projectFile = LoadXml(targetProjectFileName);

            if (projectFile.Root == null)
                return;

            var itemGroup = projectFile.Root
                .Elements(ItemGroup)
                .FirstOrDefault(i => i.Elements(ProjectReference).Any())
                ;

            if (itemGroup == null)
            {
                itemGroup = new XElement(ItemGroup);
                projectFile.Root.Add(itemGroup);
            }

            var nodes = itemGroup.Nodes().Where(n => n.NodeType == XmlNodeType.Element).ToList();

            var projectReference = new XElement(ProjectReference, new XAttribute("Include", referencePath));
            nodes.Add(projectReference);

            itemGroup.RemoveAll();
            foreach (var node in nodes)
            {
                itemGroup.Add(node);
                node.AddBeforeSelf(new XText($"{Environment.NewLine}    "));
            }
            itemGroup.LastNode?.AddAfterSelf(new XText($"{Environment.NewLine}  "));
            
            SaveXml(projectFile, targetProjectFileName);
        }

        private static XDocument LoadXml(string fileName)
        {
            using var reader = XmlReader.Create(fileName);
            return XDocument.Load(reader, LoadOptions.PreserveWhitespace);
        }

        private static void SaveXml(XDocument xmlDocument, string fileName)
        {
            xmlDocument.Save(fileName);
        }
    }
}