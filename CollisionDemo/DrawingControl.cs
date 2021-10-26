using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PhysicsEngine2D.Net;
using Brushes = System.Windows.Media.Brushes;
using Matrix = System.Windows.Media.Matrix;
using Pen = System.Windows.Media.Pen;
using Point = System.Drawing.Point;
using Size = System.Windows.Size;

namespace CollisionDemo
{
    public class DrawingControl : FrameworkElement
    {
        private readonly DrawingVisual _drawingVisual;

        public static readonly DependencyProperty BallsProperty = DependencyProperty.Register(
            "Balls", typeof(ObservableCollection<Circle>), typeof(DrawingControl), new PropertyMetadata(default(ObservableCollection<Circle>),
                (o, args) =>
                {
                    if (args.NewValue != null)
                    {
                        if (o is DrawingControl self)
                        {
#pragma warning disable 4014
                            //self.LoopDraw();
#pragma warning restore 4014
                        }
                    }
                }));

        public ObservableCollection<Circle> Balls
        {
            get => (ObservableCollection<Circle>)GetValue(BallsProperty);
            set => SetValue(BallsProperty, value);
        }

        private static readonly Pen Pen = new(Brushes.White, 0.2);

        public DrawingControl()
        {
            _drawingVisual = new DrawingVisual();
            AddVisualChild(_drawingVisual);
            Pen.Freeze();
            TimeSpan lastRenderTime = new TimeSpan();
            CompositionTarget.Rendering += (sender, args) =>
            {
                if (args is RenderingEventArgs renderingEventArgs && renderingEventArgs.RenderingTime != lastRenderTime)
                {
                    Draw((float)(renderingEventArgs.RenderingTime.TotalSeconds - lastRenderTime.TotalSeconds));
                    //DrawByGdiPlus((float)(renderingEventArgs.RenderingTime.TotalSeconds - lastRenderTime.TotalSeconds));
                    lastRenderTime = renderingEventArgs.RenderingTime;
                }
            };
            //InitializeGdiPlus();
        }

        private void Draw(float duration)
        {
            var balls = Dispatcher.Invoke(() => Balls);
            if (balls == null || !balls.Any()) return;

            //CollisionDetection.DetectByForce(balls);
            CollisionDetection.DetectByBroadAndNarrowPhase(balls);

            foreach (var ball in balls)
            {
                Collision.DetectAndResolveLeftWall(ball, ball.BoundLeft);
                Collision.DetectAndResolveTopWall(ball, ball.BoundTop);
                Collision.DetectAndResolveRightWall(ball, ball.BoundRight);
                Collision.DetectAndResolveBottomWall(ball, ball.BoundBottom);
            }

            Dispatcher.Invoke(() =>
            {
                var dc = _drawingVisual.RenderOpen();
                //foreach (var ball in Balls)
                //{
                //    ball.Draw(dc);
                //    ball.DrawVelocity(dc);
                //}
                for (int i = 0; i < Balls.Count; i++)
                {
                    for (int j = i + 1; j < Balls.Count; j++)
                    {
                        dc.DrawLine(Pen, Balls[i].Position.ToPoint(), Balls[j].Position.ToPoint());
                    }
                }

                dc.Close();
            });

            foreach (var ball in balls)
            {
                ball.NextFrame(duration);
            }
        }

        private const int WIDTH = 1400;
        private const int HEIGHT = 800;

        private Graphics _bitmapGraphics = null!;
        private WriteableBitmap _writeableBitmap = null!;
        private System.Drawing.Pen _pen = null!;

        private void InitializeGdiPlus()
        {
            System.Drawing.Color c = System.Drawing.Color.White;
            _pen = new System.Drawing.Pen(c, 0.2f);

            // get DPI for this window
            //Matrix m = PresentationSource.FromVisual(Application.Current.MainWindow!)!.CompositionTarget!.TransformToDevice;
            double dpiX = /*m.M11 **/ 96.0;
            double dpiY = /*m.M22 **/ 96.0;

            _writeableBitmap = new WriteableBitmap(WIDTH, HEIGHT, dpiX, dpiY, PixelFormats.Pbgra32, null);

            Bitmap b = new Bitmap(WIDTH, HEIGHT, WIDTH * 4, System.Drawing.Imaging.PixelFormat.Format32bppPArgb, _writeableBitmap.BackBuffer);

            _bitmapGraphics = Graphics.FromImage(b);
            _bitmapGraphics.InterpolationMode = InterpolationMode.Default;
            _bitmapGraphics.SmoothingMode = SmoothingMode.Default;

            DrawingContext dc = _drawingVisual.RenderOpen();
            dc.DrawImage(_writeableBitmap, new Rect(0, 0, WIDTH, HEIGHT));
            dc.Close();
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.Linear);
            RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
        }

        public void DrawByGdiPlus(float duration)
        {
            var balls = Dispatcher.Invoke(() => Balls);
            if (balls == null || !balls.Any()) return;

            CollisionDetection.DetectByBroadAndNarrowPhase(balls);

            foreach (var ball in balls)
            {
                Collision.DetectAndResolveLeftWall(ball, ball.BoundLeft);
                Collision.DetectAndResolveTopWall(ball, ball.BoundTop);
                Collision.DetectAndResolveRightWall(ball, ball.BoundRight);
                Collision.DetectAndResolveBottomWall(ball, ball.BoundBottom);
            }

            Dispatcher.Invoke(() =>
            {
                _writeableBitmap.Lock();

                _bitmapGraphics.Clear(System.Drawing.Color.Black);
                for (int i = 0; i < Balls.Count; i++)
                {
                    for (int j = i + 1; j < Balls.Count; j++)
                    {
                        var a = Balls[i].Position;
                        var b = Balls[j].Position;
                        _bitmapGraphics.DrawLines(_pen, new[]
                        {
                            new PointF(a.X, a.Y),
                            new PointF(b.X, b.Y),
                        });
                    }
                }

                _writeableBitmap.AddDirtyRect(new Int32Rect(0, 0, WIDTH, HEIGHT));

                _writeableBitmap.Unlock();

                InvalidateVisual();
                InvalidateMeasure();
                InvalidateArrange();
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

        protected override Geometry GetLayoutClip(Size layoutSlotSize)
        {
            return new RectangleGeometry(new Rect(new Size(ActualWidth, ActualHeight)));
        }
    }
}
