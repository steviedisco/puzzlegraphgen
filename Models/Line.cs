using System;
using System.Xml.Serialization;

namespace PuzzleGraphGenerator.Models
{
    [XmlInclude(typeof(Point))]
    [XmlType(TypeName = "section", Namespace = "Line")]
    [Serializable]
    public class Line : Section
    {
        private Line()
        {
            Name = "Line";
        }

        public static Line Create()
        {
            return new Line();
        }

        public void AddPoints((double, double) point1, (double, double) point2)
        {
            AddGraphObject(Point.Create(point1));
            AddGraphObject(Point.Create(point1.Item1, point2.Item2));
            AddGraphObject(Point.Create(point2));
        }
    }
}
