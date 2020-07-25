using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace PuzzleGraphGenerator.Models
{
    [XmlType(TypeName = "section")]
    [Serializable]
    public class Section
    {
        private List<object> Nodes { get; set; } = new List<object>();

        [XmlElement("attribute")]
        public List<Attribute> Attributes
        {
            get
            {
                return Nodes.Where(x => x.GetType() == typeof(Attribute))
                            .Select(x => x as Attribute).ToList();
            }
        }

        [XmlElement("section")]
        public List<Section> Sections
        {
            get
            {
                return Nodes.Where(x => x.GetType() == typeof(Section))
                            .Select(x => x as Section).ToList();
            }
        }

        [XmlAttribute("name")]
        public string Name { get; set; } = string.Empty;

        public Section AddSection(Section section)
        {
            Nodes.Add(section);
            return section;
        }

        public Attribute AddAttribute(Attribute attribute)
        {
            Nodes.Add(attribute);
            return attribute;
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
