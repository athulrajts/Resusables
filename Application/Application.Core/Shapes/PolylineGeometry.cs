using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Application.Core
{
    public class PolylineGeometry : ShapeGeometry
    {
        public PolylineGeometry()
        {
            Visual = new Polyline();
        }

        public PolylineGeometry(Polyline p)
        {
            Visual = p;
            Vertices = new List<Point>();
            p.Points.ToList().ForEach(point => Vertices.Add(point));
        }

        private Polyline Polyline => Visual as Polyline;

        public bool IsClosed
        {
            get
            {
                if (Vertices.FirstOrDefault() is Point p1 && Vertices.LastOrDefault() is Point pn)
                {
                    return p1 == pn;
                }
                return false;
            }
        }

        public override Shape RebuildVisual()
        {
            Visual = new Polyline
            {
                Stroke = Brushes.Green,
                Fill = new SolidColorBrush { Color = Colors.LightSeaGreen, Opacity = 0.15 },
                StrokeThickness = 3,
            };

            Vertices.ForEach(point => Polyline.Points.Add(point));

            return Visual;
        }

        public override void Translate(double x, double y)
        {
            for (int i = 0; i < Vertices.Count; i++)
            {
                Vertices[i] = new Point(Vertices[i].X + x, Vertices[i].Y + y);
            }

            for (int i = 0; i < Vertices.Count; i++)
            {
                Polyline.Points[i] = Vertices[i];
            }
        }


    }
}
