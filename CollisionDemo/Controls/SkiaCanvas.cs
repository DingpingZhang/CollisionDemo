using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PhysicsEngine2D.Net;
using SkiaSharp;

namespace CollisionDemo.Controls
{
    public class SkiaCanvas : FrameworkElement
    {
        private static readonly SKPaint Paint = new()
        {
            Color = SKColor.Parse("#FFFFFF"),
            IsAntialias = true,
            StrokeWidth = 0.2f,
            FilterQuality = SKFilterQuality.High,
            //PathEffect = SKPathEffect.CreateDash(new[] { 10f, 10f }, 0f),
        };
        private static readonly SKColor BackgroundColor = SKColor.Parse("#000000");

        public static readonly DependencyProperty ShapesProperty = DependencyProperty.Register(
            "Shapes", typeof(ObservableCollection<Circle>), typeof(SkiaCanvas), new PropertyMetadata(default(ObservableCollection<Circle>)));

        public ObservableCollection<Circle> Shapes
        {
            get => (ObservableCollection<Circle>)GetValue(ShapesProperty);
            set => SetValue(ShapesProperty, value);
        }

        public static readonly DependencyProperty FrameRateProperty = DependencyProperty.Register(
            "FrameRate", typeof(double), typeof(SkiaCanvas), new PropertyMetadata(default(double)));

        public double FrameRate
        {
            get => (double)GetValue(FrameRateProperty);
            set => SetValue(FrameRateProperty, value);
        }

        private readonly WriteableBitmap _bitmap = CreateImage(1000, 1000);
        private readonly DrawingVisual _drawingVisual;

        public SkiaCanvas()
        {
            _drawingVisual = new DrawingVisual();
            AddVisualChild(_drawingVisual);
            TimeSpan lastRenderTime = new TimeSpan();
            CompositionTarget.Rendering += (sender, args) =>
            {
                if (args is RenderingEventArgs renderingEventArgs && renderingEventArgs.RenderingTime != lastRenderTime)
                {
                    double duration = renderingEventArgs.RenderingTime.TotalSeconds - lastRenderTime.TotalSeconds;
                    Draw((float)duration);
                    //DrawByGdiPlus((float)(renderingEventArgs.RenderingTime.TotalSeconds - lastRenderTime.TotalSeconds));
                    lastRenderTime = renderingEventArgs.RenderingTime;
                    SetCurrentValue(FrameRateProperty, 1 / duration);
                }
            };
        }

        private void Draw(float duration)
        {
            if (Shapes == null || !Shapes.Any()) return;

            CollisionDetection.DetectByBroadAndNarrowPhase(Shapes);

            foreach (var ball in Shapes)
            {
                Collision.DetectAndResolveLeftWall(ball, ball.BoundLeft);
                Collision.DetectAndResolveTopWall(ball, ball.BoundTop);
                Collision.DetectAndResolveRightWall(ball, ball.BoundRight);
                Collision.DetectAndResolveBottomWall(ball, ball.BoundBottom);
            }

            UpdateImage(_bitmap, canvas =>
            {
                for (int i = 0; i < Shapes.Count; i++)
                {
                    float x0 = Shapes[i].Position.X;
                    float y0 = Shapes[i].Position.Y;
                    canvas.DrawCircle(x0, y0, Shapes[i].Radius, Paint);
                    for (int j = i + 1; j < Shapes.Count; j++)
                    {
                        float x1 = Shapes[j].Position.X;
                        float y1 = Shapes[j].Position.Y;

                        canvas.DrawLine(x0, y0, x1, y1, Paint);
                    }
                }
            });

            using (var ctx = _drawingVisual.RenderOpen())
            {
                ctx.DrawImage(_bitmap, new Rect(0, 0, 1000, 1000));
            }

            foreach (var ball in Shapes)
            {
                ball.NextFrame(duration);
            }
        }

        private static WriteableBitmap CreateImage(int width, int height)
        {
            var writeableBitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Pbgra32, null);
            return writeableBitmap;
        }

        private SKSurface? _surface;

        private void UpdateImage(WriteableBitmap writeableBitmap, Action<SKCanvas> draw)
        {
            int width = (int)writeableBitmap.Width;
            int height = (int)writeableBitmap.Height;

            writeableBitmap.Lock();

            SKImageInfo skImageInfo = new(width, height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);

            _surface ??= SKSurface.Create(skImageInfo, writeableBitmap.BackBuffer, writeableBitmap.BackBufferStride);
            SKCanvas canvas = _surface.Canvas;
            canvas.Clear(BackgroundColor);
            draw(canvas);

            writeableBitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));
            writeableBitmap.Unlock();
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
