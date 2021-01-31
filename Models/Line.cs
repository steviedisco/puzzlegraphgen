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

        public void AddPoints((double, double) point1, List<(double, double)> points2, int point2index)
        {
            AddGraphObject(Point.Create(point1));

            if (points2.Count > 1 && point2index < points2.Count - 1)
            {
                AddGraphObject(Point.Create(points2[point2index].Item1, point1.Item2));
            }
            else
            {
                AddGraphObject(Point.Create(point1.Item1, points2[point2index].Item2));
            }


            AddGraphObject(Point.Create(points2[point2index]));
        }
    }
}
