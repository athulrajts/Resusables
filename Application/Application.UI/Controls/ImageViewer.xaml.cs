using Application.Core;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Point = Application.Core.Point;
using RectangleGeometry = Application.Core.RectangleGeometry;

namespace Application.UI.Controls
{
    public enum CanvasAction
    {
        Rectangle,
        Polygon,
        Move,
        None
    }

    /// <summary>
    /// Interaction logic for ImageViewer.xaml
    /// </summary>
    public partial class DrawingBoard : UserControl, INotifyPropertyChanged
    {
        #region Dependancy Properties

        public int MaxObjectsCount
        {
            get { return (int)GetValue(MaxObjectsCountProperty); }
            set { SetValue(MaxObjectsCountProperty, value); }
        }

        public static readonly DependencyProperty MaxObjectsCountProperty =
            DependencyProperty.Register("MaxObjectsCount", typeof(int), typeof(DrawingBoard), new PropertyMetadata(1));

        public double AutoConnectRadius
        {
            get { return (double)GetValue(AutoConnectRadiusProperty); }
            set { SetValue(AutoConnectRadiusProperty, value); }
        }

        public static readonly DependencyProperty AutoConnectRadiusProperty =
            DependencyProperty.Register("AutoConnectRadius", typeof(double), typeof(DrawingBoard), new PropertyMetadata(100.0));

        public bool ShowToolBar
        {
            get { return (bool)GetValue(ShowToolBarProperty); }
            set { SetValue(ShowToolBarProperty, value); }
        }

        public static readonly DependencyProperty ShowToolBarProperty =
            DependencyProperty.Register("ShowToolBar", typeof(bool), typeof(DrawingBoard), new PropertyMetadata(false));


        public ShapeGeometryCollection Geometries
        {
            get { return (ShapeGeometryCollection)GetValue(GeometriesProperty); }
            set { SetValue(GeometriesProperty, value); }
        }

        public static readonly DependencyProperty GeometriesProperty =
            DependencyProperty.Register("Geometries", typeof(ShapeGeometryCollection), typeof(DrawingBoard), new PropertyMetadata(new ShapeGeometryCollection(), GeometriesChanged));

        private static void GeometriesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var board = d as DrawingBoard;
            if (e.NewValue != null)
            {
                foreach (var item in e.NewValue as ShapeGeometryCollection)
                {
                    var shape = item.RebuildVisual();
                    shape.MouseEnter += board.Shape_MouseEnter;
                    shape.MouseLeave += board.Shape_MouseLeave;
                    board.imageViewer.Children.Add(shape);
                } 
            }
        }

        public Brush BoardBackground
        {
            get { return (Brush)GetValue(BoardBackgroundProperty); }
            set { SetValue(BoardBackgroundProperty, value); }
        }

        public static readonly DependencyProperty BoardBackgroundProperty =
            DependencyProperty.Register("BoardBackground", typeof(Brush), typeof(DrawingBoard), new PropertyMetadata(null, OnBackgroundChanged));

        private static void OnBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var board = d as DrawingBoard;

            if (e.NewValue is ImageBrush ib && board.IsLoaded && ib.ImageSource != null)
            {
                var ap = ib.ImageSource.Height / ib.ImageSource.Width;
                board.imageViewer.Height = board.ActualHeight;
                board.imageViewer.Width = board.ActualHeight / ap;
            }
        }

        public bool ShowGeometries
        {
            get { return (bool)GetValue(ShowGeometriesProperty); }
            set { SetValue(ShowGeometriesProperty, value); }
        }

        public static readonly DependencyProperty ShowGeometriesProperty =
            DependencyProperty.Register("ShowGeometries", typeof(bool), typeof(DrawingBoard), new PropertyMetadata(true, OnShowGeometriesChanged));

        private static void OnShowGeometriesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var board = d as DrawingBoard;
            if((bool)e.NewValue == true)
            {
                if (board.cachedGeometries.Count > 0)
                {
                    board.cachedGeometries.ToList().ForEach(g => 
                    {
                        board.Geometries.Add(g);
                        board.imageViewer.Children.Add(g.Visual);
                    });
                    board.cachedGeometries.Clear();
                }
            }
            else
            {
                if (board.Geometries.Count > 0)
                {
                    board.Geometries.ToList().ForEach(g =>
                    {
                        board.cachedGeometries.Add(g);
                        board.imageViewer.Children.Remove(g.Visual);
                    });
                    board.Geometries.Clear();
                }
            }
        }




        #endregion

        private readonly DoubleCollection STROKED = new DoubleCollection { 1, 1 };
        private readonly DoubleCollection SOLID = new DoubleCollection();
        private readonly Point NULL = new Point(-1, -1);
        private readonly SolidColorBrush SHAPE_FILL = new SolidColorBrush { Color = Colors.LightSeaGreen, Opacity = 0.15 };
        private readonly Brush SHAPE_STROKE = Brushes.Green;
        private readonly double SHAPE_STROKE_THICKNESS = 3;
        private Shape selectedShape;
        private Point startPoint;
        private Point endPoint;
        private ShapeGeometryCollection cachedGeometries = new ShapeGeometryCollection();


        private ObservableCollection<Point> vertices = new ObservableCollection<Point>();
        public ObservableCollection<Point> Vertices
        {
            get => vertices;
            set
            {
                vertices = value;
                RaisePropertyChanged(nameof(Vertices));
            }
        }
        public DrawingBoard()
        {
            InitializeComponent();
            Loaded += ImageViewer_Loaded;
        }

        #region Events

        private void ImageViewer_Loaded(object sender, RoutedEventArgs e)
        {
            if (imageViewer.Background is ImageBrush background)
            {
                var ap = background.ImageSource.Height / background.ImageSource.Width;
                imageViewer.Height = this.ActualHeight;
                imageViewer.Width = this.ActualHeight / ap; 
            }
        }


        private void Shape_MouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Shape s)
            {
                s.StrokeDashArray = SOLID;
                Mouse.OverrideCursor = Cursors.Arrow;
                Vertices.Clear();
            }
        }

        private void Shape_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Shape s)
            {
                s.StrokeDashArray = STROKED;
                Mouse.OverrideCursor = Cursors.SizeAll;
                Geometries.First(x => x.Visual == s).Vertices.ForEach(p => Vertices.Add(p));
            }
        }


        private void ImageViewer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(imageViewer);

            if (CurrentAction == CanvasAction.Polygon)
            {
                polylineOverlay.Visibility = Visibility.Visible;
                if (polylineOverlay.Points.Count > 0)
                {
                    double xc_sq = (startPoint.X - polylineOverlay.Points[0].X) * (startPoint.X - polylineOverlay.Points[0].X);
                    double yc_sq = (startPoint.Y - polylineOverlay.Points[0].Y) * (startPoint.Y - polylineOverlay.Points[0].Y);
                    if (xc_sq + yc_sq <= AutoConnectRadius * AutoConnectRadius)
                    {
                        polylineOverlay.Points.Add(polylineOverlay.Points[0]);
                        AddPath();
                    }
                    else
                    {
                        polylineOverlay.Points.Add(startPoint);
                        Vertices.Add(startPoint);
                    }
                }
                else
                {
                    polylineOverlay.Points.Add(startPoint);
                    Vertices.Add(startPoint);
                }
            }
            else if (VisualTreeHelper.HitTest(imageViewer, startPoint)?.VisualHit is Shape shape && ShowToolBar)
            {
                selectedShape = shape;
                CurrentAction = CanvasAction.Move;
            }
            else if (CurrentAction == CanvasAction.Rectangle)
            {
                Mouse.OverrideCursor = Cursors.Cross;
            }

            if (CurrentAction != CanvasAction.None && CurrentAction != CanvasAction.Move)
                ClearExcessShapes();

        }
        
        private void ImageViewer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (CurrentAction == CanvasAction.Rectangle)
            {
                AddRectangle();
                rectangleOverlay.Height = 0;
                rectangleOverlay.Width = 0;
            }
            if( CurrentAction == CanvasAction.Move)
            {
                double xOffset = 0;
                double yOffset = 0;

                if (selectedShape?.RenderTransform is TranslateTransform t)
                {
                    xOffset = t.X;
                    yOffset = t.Y;
                    t.X = 0;
                    t.Y = 0;
                }

                Geometries?.FirstOrDefault(s => s.Visual == selectedShape)?.Translate(xOffset, yOffset);

                CurrentAction = previousAction;
                selectedShape = null;
            }
        }

        private void ImageViewer_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                endPoint = e.GetPosition(imageViewer);
                if (CurrentAction == CanvasAction.Rectangle)
                {
                    DrawRectangle();
                }
                else
                {
                    MoveShape();
                }
            }
        }

        private void ImageViewer_MouseLeave(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ImageViewer_MouseLeftButtonUp(sender, null);
                CurrentAction = CanvasAction.None;
            }
        }

        private void Rectangle_Checked(object sender, RoutedEventArgs e)
        {
            CurrentAction = CanvasAction.Rectangle;
        }

        private void Polygon_Checked(object sender, RoutedEventArgs e)
        {
            CurrentAction = CanvasAction.Polygon;
        }

        private void MousePointer_Checked(object sender, RoutedEventArgs e)
        {
            CurrentAction = CanvasAction.None;
        }

        #endregion

        private CanvasAction previousAction = CanvasAction.Rectangle;
        private CanvasAction currentAction = CanvasAction.None;
        public CanvasAction CurrentAction
        {
            get => currentAction;
            set
            {
                if (currentAction == value)
                    return;
                previousAction = currentAction;
                currentAction = value;

                if (currentAction == CanvasAction.Polygon)
                {
                    polygonBtn.IsChecked = true;
                    rectangleOverlay.Visibility = Visibility.Collapsed;
                }
                else if (currentAction == CanvasAction.Rectangle)
                {
                    rectangleBtn.IsChecked = true;
                    polylineOverlay.Visibility = Visibility.Collapsed;
                }
                else if (currentAction == CanvasAction.None || currentAction == CanvasAction.None)
                {
                    mousePointerBtn.IsChecked = true;
                    polylineOverlay.Visibility = Visibility.Collapsed;
                    rectangleOverlay.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void DrawRectangle()
        {
            rectangleOverlay.Visibility = Visibility.Visible;

            var width = Math.Abs(endPoint.X - startPoint.X);
            var height = Math.Abs(endPoint.Y - startPoint.Y);

            Point topLeft = NULL;
            if (startPoint.X < endPoint.X && startPoint.Y < endPoint.Y)
                topLeft = startPoint;
            else if (startPoint.X > endPoint.X && startPoint.Y > endPoint.Y)
                topLeft = endPoint;
            else if (startPoint.X < endPoint.X && startPoint.Y > endPoint.Y)
                topLeft = new Point(startPoint.X, endPoint.Y);
            else if (startPoint.X > endPoint.X && startPoint.Y < endPoint.Y)
                topLeft = new Point(endPoint.X, startPoint.Y);

            if (height > 0 && width > 0)
            {
                rectangleOverlay.Height = height;
                rectangleOverlay.Width = width;

                Canvas.SetTop(rectangleOverlay, topLeft.Y);
                Canvas.SetLeft(rectangleOverlay, topLeft.X);
            }
        }

        private void AddRectangle()
        {
            if (rectangleOverlay.Height > 0 && rectangleOverlay.Width > 0)
            {
                ClearExcessShapes();

                var rect = new Rectangle
                {
                    Height = rectangleOverlay.Height,
                    Width = rectangleOverlay.Width,
                    Stroke = SHAPE_STROKE,
                    StrokeThickness = SHAPE_STROKE_THICKNESS,
                    Fill = SHAPE_FILL
                };

                Canvas.SetTop(rect, Canvas.GetTop(rectangleOverlay));
                Canvas.SetLeft(rect, Canvas.GetLeft(rectangleOverlay));

                rect.MouseEnter += Shape_MouseEnter;
                rect.MouseLeave += Shape_MouseLeave;

                imageViewer.Children.Add(rect);
                Geometries?.Add(new RectangleGeometry(rect));
            }
        }
        private void AddPath()
        {
            var path = new Polyline
            {
                StrokeThickness = SHAPE_STROKE_THICKNESS,
                Stroke = SHAPE_STROKE,
                Fill = SHAPE_FILL,
                StrokeEndLineCap = PenLineCap.Round,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeLineJoin = PenLineJoin.Round
            };
            polylineOverlay.Points.ToList().ForEach(p => path.Points.Add(p));
            polylineOverlay.Points.Clear();
            Vertices.Clear();


            path.MouseEnter += Shape_MouseEnter;
            path.MouseLeave += Shape_MouseLeave;

            imageViewer.Children.Add(path);

            Geometries?.Add(new PolylineGeometry(path));

            CurrentAction = CanvasAction.None;
        }

        private void MoveShape()
        {
            double xOffset = endPoint.X - startPoint.X;
            double yOffset = endPoint.Y - startPoint.Y;

            if (selectedShape != null)
            {
                if (selectedShape.RenderTransform is MatrixTransform)
                    selectedShape.RenderTransform = new TranslateTransform(xOffset, yOffset);
                else if (selectedShape.RenderTransform is TranslateTransform t)
                {
                    t.X = xOffset;
                    t.Y = yOffset;

                    var temp = new ObservableCollection<Point>();
                    foreach (var item in Geometries.First(s => s.Visual == selectedShape).Vertices)
                    {
                        temp.Add(new Point(item.X + xOffset, item.Y + yOffset));
                    }
                    Vertices = temp;

                } 
            }
        }

        private void ClearExcessShapes()
        {
            if(Geometries.Count >= MaxObjectsCount)
            {
                var firstChild = Geometries[0];
                firstChild.Visual.MouseEnter -= Shape_MouseEnter;
                firstChild.Visual.MouseLeave -= Shape_MouseEnter;
                imageViewer.Children.Remove(firstChild.Visual);
                Geometries?.RemoveAt(0);
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string property) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
    }
}
