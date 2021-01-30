using System;
using System.Xml.Serialization;

namespace PuzzleGraphGenerator.Models
{
    [XmlType(TypeName = "section", Namespace = "Point")]
    [Serializable]
    public class Point : Section
    {
        private Point() { }

        private Point(double x, double y)
        {
            Name = "point";

            AddGraphObject(Attribute.Create("x", "double", x));
            AddGraphObject(Attribute.Create("y", "double", y));
        }

        public static Point Create((double, double) point)
        {
            return Create(point.Item1, point.Item2);
        }

        public static Point Create(double x, double y)
        {
            return new Point(x, y);
        }
    }
}