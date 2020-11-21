using System;
using System.Collections.Generic;
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
            var interval = TimeSpan.FromMilliseconds(Interval);
            while (true)
            {
                DateTime timer = DateTime.Now;
                Draw();
                TimeSpan actualInterval = interval - (DateTime.Now - timer);

                if (actualInterval > TimeSpan.Zero)
                {
                    await Task.Delay(actualInterval).ConfigureAwait(false);
                }
                else
                {
                    await Task.Delay(1).ConfigureAwait(false);
                }
            }
        }

        private void Draw()
        {
            var balls = Dispatcher.Invoke(() => Balls);
            if (balls == null || !balls.Any()) return;

            //CollideTest_ForceDetect(balls);
            CollideTest_BroadPhase(balls);

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
                    //dc.DrawLine(_pen, ball.Position, ball.Position + ball.Velocity);
                }

                dc.Close();
            });

            foreach (var ball in balls)
            {
                ball.NextFrame(Interval);
            }
        }

        public static void CollideTest_ForceDetect(IReadOnlyList<Ball> balls)
        {
            for (int i = 0; i < balls.Count; i++)
            {
                for (int j = i + 1; j < balls.Count; j++)
                {
                    balls[i].Collide(balls[j]);
                }
            }
        }

        public static void CollideTest_BroadPhase(IReadOnlyList<Ball> balls)
        {
            foreach (var (ball1Index, ball2Index) in BroadPhase.Detect(balls))
            {
                var ball1 = balls[ball1Index];
                var ball2 = balls[ball2Index];
                if (ball1.DetectNarrowPhase(ball2))
                {
                    ball1.CollideOnly(ball2);
                }
            }
        }

        protected override int VisualChildrenCount => 1;

        protected override Visual GetVisualChild(int index)
        {
            return _drawingVisual;
        }
    }
}
