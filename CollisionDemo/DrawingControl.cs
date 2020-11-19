using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CollisionDemo
{
    public class DrawingControl : FrameworkElement
    {
        private const double Interval = 1000d / 60;
        private readonly DrawingVisual _drawingVisual;

        public static readonly DependencyProperty BallsProperty = DependencyProperty.Register(
            "Balls", typeof(ObservableCollection<Ball>), typeof(DrawingControl), new PropertyMetadata(default(ObservableCollection<Ball>),
                (o, args) =>
                {
                    if (args.NewValue != null)
                    {
                        if (o is DrawingControl self)
                        {
                            //self.LoopDraw();
                        }
                    }
                }));

        public ObservableCollection<Ball> Balls
        {
            get => (ObservableCollection<Ball>)GetValue(BallsProperty);
            set => SetValue(BallsProperty, value);
        }

        private readonly ImageSource _image;
        private readonly Brush _brush;
        private readonly Timer _timer;

        public DrawingControl()
        {
            _timer = new Timer(LoopDraw, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(Interval));
            _drawingVisual = new DrawingVisual();
            new VisualCollection(this) { _drawingVisual };
            _image = new BitmapImage(new Uri("pack://application:,,,/CollisionDemo;component/ball.png"));
            var brush = new RadialGradientBrush { GradientOrigin = new Point(0.7, 0.3) };
            brush.RadiusX = brush.RadiusY = 1;
            brush.GradientStops.Add(new GradientStop(Colors.White, 0));
            brush.GradientStops.Add(new GradientStop(Colors.Black, 1));
            brush.Freeze();
            _brush = brush;
        }

        private void LoopDraw(object? state)
        {
            try
            {
                LoopDraw();
            }
            catch (OperationCanceledException)
            {
                // Ignore
            }
        }

        private void LoopDraw()
        {
            var balls = Dispatcher.Invoke(() => Balls);
            if (balls == null || !balls.Any()) return;

            for (int i = 0; i < balls.Count; i++)
            {
                for (int j = 0; j < balls.Count; j++)
                {
                    if (i == j) continue;

                    balls[i].Collide(balls[j]);
                }
            }

            Dispatcher.Invoke(() =>
            {
                var dc = _drawingVisual.RenderOpen();
                foreach (var ball in Balls)
                {
                    dc.DrawEllipse(_brush, null, ball.Position, ball.Radius, ball.Radius);
                    //dc.DrawImage(_image, new Rect(
                    //    ball.Position.X - ball.Radius,
                    //    ball.Position.Y - ball.Radius,
                    //    ball.Radius * 2,
                    //    ball.Radius * 2));
                }
                dc.Close();
            });

            foreach (var ball in balls)
            {
                ball.CollideHorizontalBound();
                ball.CollideVerticalBound();
                ball.Next(Interval);
            }
        }

        protected override int VisualChildrenCount => 1;

        protected override Visual GetVisualChild(int index)
        {
            return _drawingVisual;
        }
    }
}
