using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PhysicsEngine2D.Net;
using SkiaSharp;

namespace CollisionDemo.Controls
{
    public class SkiaCanvas : CanvasBase
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

        private readonly WriteableBitmap _bitmap = CreateImage(1000, 1000);

        private static WriteableBitmap CreateImage(int width, int height)
        {
            var writeableBitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Pbgra32, null);
            return writeableBitmap;
        }

        private SKSurface? _surface;

        public SkiaCanvas()
        {
            Unloaded += OnUnloaded;
        }

        protected override void Draw(IReadOnlyList<Circle> shapes)
        {
            UpdateImage(_bitmap, canvas =>
            {
                for (int i = 0; i < Shapes.Count; i++)
                {
                    float x0 = Shapes[i].Position.X;
                    float y0 = Shapes[i].Position.Y;
                    for (int j = i + 1; j < Shapes.Count; j++)
                    {
                        float x1 = Shapes[j].Position.X;
                        float y1 = Shapes[j].Position.Y;

                        canvas.DrawLine(x0, y0, x1, y1, Paint);
                    }
                }
            });

            using var ctx = DrawingVisual.RenderOpen();
            ctx.DrawImage(_bitmap, new Rect(0, 0, 1000, 1000));
        }

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

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            _surface?.Dispose();
            _surface = null;
        }
    }
}
