// Script from Lohrion on Unity Answers
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using SyntaxTree.VisualStudio.Unity.Bridge;
using UnityEditor;

namespace Assets.Plugins.Editor
{
    [InitializeOnLoad]
    public class ProjectFileHook
    {
        // necessary for XLinq to save the xml project file in utf8
        class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding
            {
                get { return Encoding.UTF8; }
            }
        }

        static void ProcessNodesWithIncludeAttribute(XDocument document, string localName, string includeValue, Action<XElement> action)
        {
            var nodes = document
                .Descendants()
                .Where(p => p.Name.LocalName == localName);

            foreach (var node in nodes)
            {
                var xa = node.Attribute("Include");
                if (xa != null && !string.IsNullOrEmpty(xa.Value) && string.Equals(xa.Value, includeValue))
                {
                    action(node);
                }
            }
        }

        // Remove System.Data from project (not from file system so Unity can compile properly)
        static void RemoveFileFromProject(XDocument document, string fileName)
        {
            ProcessNodesWithIncludeAttribute(document, "None", fileName, element => element.Remove());
        }

        // Adjust references, by using the default framework assembly instead of local file (remove the HintPath)
        static void RemoveHintPathFromReference(XDocument document, string assemblyName)
        {
            ProcessNodesWithIncludeAttribute(document, "Reference", assemblyName, element => element.Nodes().Remove());
        }

        static ProjectFileHook()
        {
            ProjectFilesGenerator.ProjectFileGeneration += (string name, string content) =>
            {
                var document = XDocument.Parse(content);

                RemoveFileFromProject(document, @"Assets\System.Data.dll");
                RemoveHintPathFromReference(document, "System.Data");

                var str = new Utf8StringWriter();
                document.Save(str);

                return str.ToString();
            };
        }
    }
}