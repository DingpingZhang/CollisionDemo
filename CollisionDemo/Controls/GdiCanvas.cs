using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PhysicsEngine2D.Net;
using Image = System.Windows.Controls.Image;

namespace CollisionDemo.Controls
{
    [TemplatePart(Name = ImageName, Type = typeof(Image))]
    public class GdiCanvas : Control
    {
        private const string ImageName = "PART_Image";
        private const int WIDTH = 1000;
        private const int HEIGHT = 1000;

        private Image _image;
        private WriteableBitmap _wb;

        private Graphics _bitmapGraphics = null!;
        private System.Drawing.Pen _pen = null!;


        public static readonly DependencyProperty ShapesProperty = DependencyProperty.Register(
            "Shapes", typeof(ObservableCollection<Circle>), typeof(GdiCanvas), new PropertyMetadata(default(ObservableCollection<Circle>)));
        public static readonly DependencyProperty FrameRateProperty = DependencyProperty.Register(
            "FrameRate", typeof(double), typeof(GdiCanvas), new PropertyMetadata(default(double)));

        public ObservableCollection<Circle> Shapes
        {
            get => (ObservableCollection<Circle>)GetValue(ShapesProperty);
            set => SetValue(ShapesProperty, value);
        }

        public double FrameRate
        {
            get => (double)GetValue(FrameRateProperty);
            set => SetValue(FrameRateProperty, value);
        }

        public GdiCanvas()
        {
            TimeSpan lastRenderTime = new TimeSpan();
            CompositionTarget.Rendering += (_, args) =>
            {
                if (args is RenderingEventArgs renderingEventArgs && renderingEventArgs.RenderingTime != lastRenderTime)
                {
                    double duration = renderingEventArgs.RenderingTime.TotalSeconds - lastRenderTime.TotalSeconds;
                    DrawByGdiPlus((float)(renderingEventArgs.RenderingTime.TotalSeconds - lastRenderTime.TotalSeconds));
                    lastRenderTime = renderingEventArgs.RenderingTime;
                    SetCurrentValue(FrameRateProperty, 1 / duration);
                }
            };
        }

        private void InitializeGdiPlus()
        {
            System.Drawing.Color c = System.Drawing.Color.White;
            _pen = new System.Drawing.Pen(c, 0.2f);

            // get DPI for this window
            //Matrix m = PresentationSource.FromVisual(Application.Current.MainWindow!)!.CompositionTarget!.TransformToDevice;
            double dpiX = /*m.M11 **/ 96.0;
            double dpiY = /*m.M22 **/ 96.0;

            _wb = new WriteableBitmap(WIDTH, HEIGHT, dpiX, dpiY, PixelFormats.Pbgra32, null);

            Bitmap b = new Bitmap(WIDTH, HEIGHT, WIDTH * 4, System.Drawing.Imaging.PixelFormat.Format32bppPArgb, _wb.BackBuffer);

            _bitmapGraphics = Graphics.FromImage(b);
            _bitmapGraphics.InterpolationMode = InterpolationMode.Default;
            _bitmapGraphics.SmoothingMode = SmoothingMode.AntiAlias;

            _image.Source = _wb;
            _image.Stretch = Stretch.None;
        }

        public void DrawByGdiPlus(float duration)
        {
            if (_wb is null)
            {
                return;
            }

            if (Shapes == null || !Shapes.Any()) return;

            CollisionDetection.DetectByBroadAndNarrowPhase(Shapes);

            foreach (var ball in Shapes)
            {
                Collision.DetectAndResolveLeftWall(ball, ball.BoundLeft);
                Collision.DetectAndResolveTopWall(ball, ball.BoundTop);
                Collision.DetectAndResolveRightWall(ball, ball.BoundRight);
                Collision.DetectAndResolveBottomWall(ball, ball.BoundBottom);
            }

            _wb.Lock();

            _bitmapGraphics.Clear(System.Drawing.Color.Black);
            for (int i = 0; i < Shapes.Count; i++)
            {
                for (int j = i + 1; j < Shapes.Count; j++)
                {
                    var a = Shapes[i].Position;
                    var b = Shapes[j].Position;
                    _bitmapGraphics.DrawLines(_pen, new[]
                    {
                        new PointF(a.X, a.Y),
                        new PointF(b.X, b.Y),
                    });
                }
            }

            _wb.AddDirtyRect(new Int32Rect(0, 0, WIDTH, HEIGHT));

            _wb.Unlock();

            foreach (var ball in Shapes)
            {
                ball.NextFrame(duration);
            }

            _image.InvalidateMeasure();
            _image.InvalidateVisual();
            _image.InvalidateArrange();
            _image.InvalidateProperty(Image.SourceProperty);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _image = (Image)GetTemplateChild(ImageName);
            InitializeGdiPlus();
        }

        static GdiCanvas()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GdiCanvas), new FrameworkPropertyMetadata(typeof(GdiCanvas)));
        }
    }
}
