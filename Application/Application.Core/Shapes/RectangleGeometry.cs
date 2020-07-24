using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace Application.Core
{
    public class RectangleGeometry : ShapeGeometry
    {
        private Rectangle Rect => Visual as Rectangle;
        public override Shape RebuildVisual()
        {
            Visual = new Rectangle
            {
                Height = this.Height,
                Width = this.Width,
                Stroke = Brushes.Green,
                Fill = new SolidColorBrush { Color = Colors.LightSeaGreen, Opacity = 0.15 },
                StrokeThickness = 3
            };
            Canvas.SetTop(Visual, Vertices[0].Y);
            Canvas.SetLeft(Visual, Vertices[0].X);
            return Visual;
        }
        public RectangleGeometry(Rectangle rect)
        {
            Visual = rect;
            UpdateVertices();
        }

        public RectangleGeometry()
        {
            Visual = new Rectangle();
        }

        public double Height
        {
            get => Visual.Height;
            set
            {
                if (Visual?.Height == value)
                    return;
                Visual.Height = value;
            }
        }

        public double Width
        {
            get => Visual.Width;
            set
            {
                if (Visual?.Width == value)
                    return;
                Visual.Width = value;
            }
        }

        public override void Translate(double x, double y)
        {
            Canvas.SetTop(Visual, Canvas.GetTop(Visual) + y);
            Canvas.SetLeft(Visual, Canvas.GetLeft(Visual) + x);
            UpdateVertices();
        }

        [XmlIgnore]
        public Point TopLeft => new Point(Canvas.GetLeft(Visual), Canvas.GetTop(Visual));

        private void UpdateVertices()
        {
            var topLeft = TopLeft;
            Vertices = new List<Point>
            {
                topLeft,
                new Point(topLeft.X + Width, topLeft.Y),
                new Point(topLeft.X + Width, topLeft.Y + Height),
                new Point(topLeft.X, topLeft.Y + Height)
            };
        }
    }
}
