using System;
using System.Xml.Serialization;

namespace PuzzleGraphGenerator.Models
{
    [XmlType(TypeName = "section", Namespace = "Point")]
    [Serializable]
    public class Point : Section
    {
        private Point() { }

        private Point(double x, double y, int nextId)
        {
            Name = "point";

            AddGraphObject(Attribute.Create("x", "double", x));
            AddGraphObject(Attribute.Create("y", "double", y));

            if (nextId != 0)
            {
                AddGraphObject(Attribute.Create("nextId", "int", nextId));
            }
        }

        public static Point Create((double x, double y) point, int nextId = 0)
        {
            return Create(point.x, point.y, nextId);
        }

        public static Point Create(double x, double y, int nextId = 0)
        {
            return new Point(x, y, nextId);
        }
    }
}