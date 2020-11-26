using System;
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

        public DrawingControl()
        {
            _drawingVisual = new DrawingVisual();
            AddVisualChild(_drawingVisual);
        }

        private async Task LoopDraw()
        {
            var interval = TimeSpan.FromSeconds(Constants.FrameRate);
            var duration = Constants.FrameRate;
            while (true)
            {
                var timer = DateTime.Now;
                Draw(duration);
                var actualInterval = interval - (DateTime.Now - timer);

                if (actualInterval > TimeSpan.Zero)
                {
                    await Task.Delay(actualInterval).ConfigureAwait(false);
                }
                else
                {
                    await Task.Delay(1).ConfigureAwait(false);
                }

                duration = (float)(DateTime.Now - timer).TotalSeconds;
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private void Draw(float duration)
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
                    ball.Draw(dc);
                    ball.DrawVelocity(dc);
                }

                dc.Close();
            });

            foreach (var ball in balls)
            {
                ball.NextFrame(duration);
            }
        }

        protected override int VisualChildrenCount => 1;

        protected override Visual GetVisualChild(int index)
        {
            return _drawingVisual;
        }
    }
}
