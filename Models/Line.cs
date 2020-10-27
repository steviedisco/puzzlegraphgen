using System;
using System.Xml.Serialization;

namespace PuzzleGraphGenerator.Models
{
    [XmlType(TypeName = "section", Namespace = "Line")]
    [Serializable]
    public class Line : Section
    {
        private Line()
        {
            Name = "Line";
        }

        public static Line CreateLine()
        {
            return new Line();
        }

        public Line AddPoint(double x, double y)
        {
            AddGraphObject(Point.CreatePoint(x, y));

            return this;
        }
    }
}
