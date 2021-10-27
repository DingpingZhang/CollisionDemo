using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PhysicsEngine2D.Net;

namespace CollisionDemo.Controls
{
    public class GdiCanvas : CanvasBase
    {
        private const int CanvasWidth = 1000;
        private const int CanvasHeight = 1000;

        private readonly WriteableBitmap _bitmap;
        private readonly Graphics _bitmapGraphics;
        private readonly System.Drawing.Pen _pen;

        public GdiCanvas()
        {
            //Matrix m = PresentationSource.FromVisual(Application.Current.MainWindow!)!.CompositionTarget!.TransformToDevice;
            double dpiX = /*m.M11 **/ 96.0;
            double dpiY = /*m.M22 **/ 96.0;

            _bitmap = new WriteableBitmap(CanvasWidth, CanvasHeight, dpiX, dpiY, PixelFormats.Pbgra32, null);

            Bitmap b = new(CanvasWidth, CanvasHeight, CanvasWidth * 4, System.Drawing.Imaging.PixelFormat.Format32bppPArgb, _bitmap.BackBuffer);

            _bitmapGraphics = Graphics.FromImage(b);
            _bitmapGraphics.InterpolationMode = InterpolationMode.Default;
            _bitmapGraphics.SmoothingMode = SmoothingMode.AntiAlias;
            System.Drawing.Color c = System.Drawing.Color.White;
            _pen = new System.Drawing.Pen(c, 0.2f);
        }

        protected override void Draw(IReadOnlyList<Circle> shapes)
        {
            _bitmap.Lock();

            _bitmapGraphics.Clear(System.Drawing.Color.Black);
            for (int i = 0; i < Shapes.Count; i++)
            {
                var a = Shapes[i].Position;
                for (int j = i + 1; j < Shapes.Count; j++)
                {
                    var b = Shapes[j].Position;
                    _bitmapGraphics.DrawLines(_pen, new[]
                    {
                        new PointF(a.X, a.Y),
                        new PointF(b.X, b.Y),
                    });
                }
            }

            _bitmap.AddDirtyRect(new Int32Rect(0, 0, CanvasWidth, CanvasHeight));
            _bitmap.Unlock();

            using var ctx = DrawingVisual.RenderOpen();
            ctx.DrawImage(_bitmap, new Rect(0, 0, 1000, 1000));
        }
    }
}
