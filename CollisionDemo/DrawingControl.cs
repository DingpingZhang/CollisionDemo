using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using PhysicsEngine2D.Net;
using PhysicsEngine2D.Net.Basic;

namespace CollisionDemo
{
    public class DrawingControl : FrameworkElement
    {
        private readonly DrawingVisual _drawingVisual;

        public static readonly DependencyProperty BodiesProperty = DependencyProperty.Register(
            "Bodies", typeof(ObservableCollection<Body>), typeof(DrawingControl), new PropertyMetadata(default(ObservableCollection<Body>),
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

        public ObservableCollection<Body> Bodies
        {
            get => (ObservableCollection<Body>)GetValue(BodiesProperty);
            set => SetValue(BodiesProperty, value);
        }

        private readonly DrawingShape _drawingShape = new DrawingShape();

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
            var bodies = Dispatcher.Invoke(() => Bodies);
            if (bodies == null || !bodies.Any()) return;

            //CollisionDetection.DetectByForce(balls);
            CollisionDetection.DetectByBroadAndNarrowPhase(bodies);

            foreach (var body in bodies)
            {
                Collision.DetectAndResolveLeftWall(body);
                Collision.DetectAndResolveTopWall(body);
                Collision.DetectAndResolveRightWall(body, 1000);
                Collision.DetectAndResolveBottomWall(body, 600);
            }

            Dispatcher.Invoke(() =>
            {
                var dc = _drawingVisual.RenderOpen();
                _drawingShape.SetDrawingContext(dc);
                foreach (var body in Bodies)
                {
                    body.Shape.Accept(_drawingShape);
                    //body.DrawVelocity(dc);
                }

                dc.Close();
            });

            foreach (var body in bodies)
            {
                body.Shape.Position += body.Velocity * duration + Gravity * (duration * duration) / 2;
                body.Velocity += Gravity * duration;
            }
        }

        // TODO
        private static readonly Vector2 Gravity = new Vector2(0, 50f);

        protected override int VisualChildrenCount => 1;

        protected override Visual GetVisualChild(int index) => _drawingVisual;
    }
}
