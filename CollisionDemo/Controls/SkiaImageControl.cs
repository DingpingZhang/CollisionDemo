using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PhysicsEngine2D.Net;
using SkiaSharp;

namespace CollisionDemo.Controls
{
    [TemplatePart(Name = ImageName, Type = typeof(Image))]
    public class SkiaImageControl : Control
    {
        private const string ImageName = "PART_Image";
        private static readonly SKPaint Paint = new()
        {
            Color = SKColor.Parse("#FFFFFF"),
            IsAntialias = true,
            StrokeWidth = 0.2f,
        };
        private static readonly SKColor BackgroundColor = SKColor.Parse("#000000");

        public static readonly DependencyProperty ShapesProperty = DependencyProperty.Register(
            "Shapes", typeof(ObservableCollection<Circle>), typeof(SkiaImageControl), new PropertyMetadata(default(ObservableCollection<Circle>)));

        public ObservableCollection<Circle> Shapes
        {
            get { return (ObservableCollection<Circle>)GetValue(ShapesProperty); }
            set { SetValue(ShapesProperty, value); }
        }

        public static readonly DependencyProperty FrameRateProperty = DependencyProperty.Register(
            "FrameRate", typeof(double), typeof(SkiaImageControl), new PropertyMetadata(default(double)));

        public double FrameRate
        {
            get { return (double)GetValue(FrameRateProperty); }
            set { SetValue(FrameRateProperty, value); }
        }

        private Image _image;
        private readonly WriteableBitmap _bitmap = CreateImage(1000, 1000);

        public SkiaImageControl()
        {
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

            Dispatcher.Invoke(() =>
            {
                UpdateImage(_bitmap, canvas =>
                {
                    for (int i = 0; i < Shapes.Count; i++)
                    {
                        for (int j = i + 1; j < Shapes.Count; j++)
                        {
                            float x0 = Shapes[i].Position.X;
                            float y0 = Shapes[i].Position.Y;
                            float x1 = Shapes[j].Position.X;
                            float y1 = Shapes[j].Position.Y;

                            canvas.DrawLine(x0, y0, x1, y1, Paint);
                        }
                    }
                });
            });

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

        private static void UpdateImage(WriteableBitmap writeableBitmap, Action<SKCanvas> draw)
        {
            int width = (int)writeableBitmap.Width;
            int height = (int)writeableBitmap.Height;

            writeableBitmap.Lock();

            SKImageInfo skImageInfo = new(width, height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);

            using (var surface = SKSurface.Create(skImageInfo, writeableBitmap.BackBuffer, writeableBitmap.BackBufferStride))
            {
                SKCanvas canvas = surface.Canvas;

                canvas.Clear(BackgroundColor);
                draw(canvas);
            }

            writeableBitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));
            writeableBitmap.Unlock();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _image = (Image)GetTemplateChild(ImageName);
            _image!.Source = _bitmap;
        }

        static SkiaImageControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SkiaImageControl), new FrameworkPropertyMetadata(typeof(SkiaImageControl)));
        }
    }
}
