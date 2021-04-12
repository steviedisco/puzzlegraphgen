using System;
using System.Collections.Generic;
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

        public (double x, double y) AddPoints((double x, double y) point1, List<(double x, double y)> points2, int point2index, int nextId)
        {
            AddGraphObject(Point.Create(point1));

            (double x, double y) output;

            if (points2.Count > 1 && point2index < points2.Count - 1 &&
                (points2[point2index].x > point1.x || points2[point2index].x < 0))
            {
                output = (points2[point2index].x, point1.y);                
            }
            else
            {
                output = (point1.x, points2[point2index].y);
            }

            AddGraphObject(Point.Create(output, nextId));
            AddGraphObject(Point.Create(points2[point2index]));

            return output;
        }
    }
}
