using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using PhysicsEngine2D.Net;

namespace CollisionDemo
{
    public class DrawingControl : FrameworkElement
    {
        private const float Interval = 1000f / 60;
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

            //CollisionDetection.DetectByForce(balls);
            CollisionDetection.DetectByBroadAndNarrowPhase(balls);

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
                    dc.DrawEllipse(_brush, null, ball.Position.ToPoint(), ball.Radius, ball.Radius);
                    //dc.DrawLine(_pen, ball.Position.ToPoint(), (ball.Position + ball.Velocity).ToPoint());
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
