using System;
using System.Xml.Serialization;

namespace PuzzleGraphGenerator.Models
{
    [XmlType(TypeName = "section", Namespace = "Point")]
    [Serializable]
    public class Point : Section
    {
        private Point(double x, double y)
        {
            Name = "point";

            AddGraphObject(Attribute.CreateAttribute("x", "double", x));
            AddGraphObject(Attribute.CreateAttribute("y", "double", y));
        }

        public static Point CreatePoint(double x, double y)
        {
            return new Point(x, y);
        }
    }
}