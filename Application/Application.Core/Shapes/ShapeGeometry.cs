using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace Application.Core
{
    [XmlInclude(type: typeof(PolylineGeometry))]
    [XmlInclude(type: typeof(RectangleGeometry))]
    public class ShapeGeometry
    {
        [XmlIgnore]
        [Browsable(false)]
        public Shape Visual { get; set; }
        public List<Point> Vertices { get; set; }
        public virtual void Translate(double x, double y) { return; }
        public virtual Shape RebuildVisual() { return null; }
    }

    public class ShapeGeometryCollection : Collection<ShapeGeometry>
    {
        public ShapeGeometryCollection()
        {

        }
    }

    public struct Point : IEquatable<Point>
    {
        public static implicit operator Point(System.Windows.Point p)
        {
            return new Point { X = p.X, Y = p.Y };
        }

        public static implicit operator System.Windows.Point(Point p)
        {
            return new System.Windows.Point { X = p.X, Y = p.Y };
        }

        public static bool operator == (Point p1, Point p2)
        {
            return p1.X == p2.X && p1.Y == p2.Y;
        }

        public static bool operator !=(Point p1, Point p2)
        {
            return p1.X != p2.X || p1.Y != p2.Y;
        }


        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        [XmlAttribute]
        public double X { get; set; }

        [XmlAttribute]
        public double Y { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is Point))
            {
                return false;
            }

            var point = (Point)obj;
            return X == point.X &&
                   Y == point.Y;
        }

        public bool Equals(Point other)
        {
            return X == other.X &&
                   Y == other.Y;
        }

        public override int GetHashCode()
        {
            var hashCode = 1861411795;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            return hashCode;
        }
    }
}
