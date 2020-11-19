using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

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
#pragma warning disable 4014
                            self.LoopDraw();
#pragma warning restore 4014
                        }
                    }
                }));

        public ObservableCollection<Ball> Balls
        {
            get => (ObservableCollection<Ball>)GetValue(BallsProperty);
            set => SetValue(BallsProperty, value);
        }

        private readonly Brush _brush;
        private readonly Pen _pen = new Pen(Brushes.Blue, 2);

        public DrawingControl()
        {
            _drawingVisual = new DrawingVisual();
            AddVisualChild(_drawingVisual);
            var brush = new RadialGradientBrush { GradientOrigin = new Point(0.7, 0.3) };
            brush.RadiusX = brush.RadiusY = 1;
            brush.GradientStops.Add(new GradientStop(Colors.White, 0));
            brush.GradientStops.Add(new GradientStop(Colors.Black, 1));
            brush.Freeze();
            _brush = brush;
        }

        private async Task LoopDraw()
        {
            while (true)
            {
                Draw();

                await Task.Delay(TimeSpan.FromMilliseconds(Interval));
            }
        }

        private void Draw()
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

            foreach (var ball in balls)
            {
                ball.CollideHorizontalBound();
                ball.CollideVerticalBound();
            }

            Dispatcher.Invoke(() =>
            {
                var dc = _drawingVisual.RenderOpen();
                foreach (var ball in Balls)
                {
                    dc.DrawEllipse(_brush, null, ball.Position, ball.Radius, ball.Radius);
                    dc.DrawLine(_pen, ball.Position, ball.Position + ball.Speed);
                }

                dc.Close();
            });

            foreach (var ball in balls)
            {
                ball.NextFrame(Interval);
            }
        }

        protected override int VisualChildrenCount => 1;

        protected override Visual GetVisualChild(int index)
        {
            return _drawingVisual;
        }
    }
}
