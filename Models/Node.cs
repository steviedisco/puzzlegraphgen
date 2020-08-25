using System;
using System.Xml.Serialization;

namespace PuzzleGraphGenerator.Models
{
    [XmlType(TypeName = "section", Namespace = "Node")]
    [Serializable]
    public class Node : Section
    {
        public Node()
        {
            Name = "node";

            var section = AddGraphObject(Section.CreateSection("node")) as Section;
            section.AddGraphObject(Attribute.CreateAttribute("id", "int", 0));
            section.AddGraphObject(Attribute.CreateAttribute("label", "String", "Finish"));
        }
    }
}
