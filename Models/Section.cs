using PuzzleGraphGenerator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace PuzzleGraphGenerator.Models
{
    [XmlInclude(typeof(GraphContainer))]
    [XmlInclude(typeof(Graph))]
    [XmlInclude(typeof(Node))]
    [XmlType(TypeName = "section", Namespace = "Section" )]
    [Serializable]
    public class Section : IGraphObject
    {
        private List<IGraphObject> Nodes { get; set; } = new List<IGraphObject>();

        [XmlElement("attribute")]
        public List<Attribute> Attributes
        {
            get
            {
                return Nodes.Where(x => x is Attribute)
                            .Select(x => x as Attribute).ToList();
            }
        }

        [XmlElement("section")]
        public List<Section> Sections
        {
            get
            {
                return Nodes.Where(x => x is Section)
                            .Select(x => x as Section).ToList();
            }
        }

        [XmlAttribute("name")]
        public string Name { get; set; } = string.Empty;

        public IGraphObject AddGraphObject(IGraphObject graphObject)
        {
            Nodes.Add(graphObject);
            return graphObject;
        }

        public static Section CreateSection(string name)
        {
            return new Section
            {
                Name = name
            };
        }
    }
}
