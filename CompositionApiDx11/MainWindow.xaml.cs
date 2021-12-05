using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using PhysicsEngine2D.Net;
using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using SharpDX.DirectWrite;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using SolidColorBrush = SharpDX.Direct2D1.SolidColorBrush;

namespace CompositionApiDx11
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private static readonly Random Random = new();
        private SharpDX.Direct2D1.BitmapRenderTarget _bitmap;

        //private WindowRenderTarget _renderTarget = null!;
        private List<Circle> _circles;

        public static readonly DependencyProperty FrameRateProperty = DependencyProperty.Register(
            "FrameRate", typeof(double), typeof(MainWindow), new PropertyMetadata(default(double)));

        public double FrameRate
        {
            get => (double)GetValue(FrameRateProperty);
            set => SetValue(FrameRateProperty, value);
        }

        public MainWindow()
        {
            InitializeComponent();

            Loaded += OnLoaded;
            Host.SizeChanged += HostOnSizeChanged;

            InitBalls();
        }

        private void HostOnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_bitmap is null)
            {
                return;
            }

            //DrawMesh(_bitmap);
        }

        private SharpDX.Direct2D1.BitmapRenderTarget CreateBitmap()
        {
            var renderTarget =
                new SharpDX.Direct2D1.BitmapRenderTarget(Host.RenderTarget, CompatibleRenderTargetOptions.None);

            DrawMesh(renderTarget);

            return renderTarget;
        }

        private static void DrawMesh(RenderTarget renderTarget)
        {
            var random = new Random();
            var width = random.Next(10, 20);
            renderTarget.BeginDraw();

            renderTarget.Clear(new RawColor4(0, 0, 0, 0));
            var brush = new SolidColorBrush(renderTarget, new RawColor4(1f, 0f, 0f, 1));

            for (int i = 0; i < 100; i++)
            {
                renderTarget.DrawLine(new RawVector2(0, i * width), new RawVector2(1000, i * width), brush, 0.5f);
                renderTarget.DrawLine(new RawVector2(i * width, 0), new RawVector2(i * width, 1000), brush, 0.5f);
            }

            renderTarget.EndDraw();
        }

        private void InitBalls()
        {
            const float width = 1000;
            const float height = 1000;
            const float minRadius = 10;
            const float maxRadius = 15;
            _circles = Enumerable.Range(0, 200).Select(_ =>
            {
                var weight = GetRandom(minRadius, maxRadius);
                return new Circle
                {
                    Mass = (float)Math.Sqrt(weight),
                    Position = new Vector2(
                        GetRandom(maxRadius, width - maxRadius),
                        GetRandom(maxRadius, height - maxRadius)),
                    Radius = weight,
                    Velocity = new Vector2(GetRandom(-100, 100), GetRandom(-100, 100)),
                    //Acceleration = new Vector2(0, 100f),
                    Restitution = 1f,
                }.SetBound(0, 0, width, height);
            }).ToList();
        }

        private static float GetRandom(float a, float b)
        {
            return a + (b - a) * (float)Random.NextDouble();
        }

        private TimeSpan _lastRenderTime;
        private SolidColorBrush _lineBrush;
        private SolidColorBrush _circleBrush;
        private int _count;

        private void CompositionTargetOnRendering(object sender, EventArgs e)
        {
            if (e is RenderingEventArgs renderingEventArgs && renderingEventArgs.RenderingTime != _lastRenderTime)
            {
                double duration = renderingEventArgs.RenderingTime.TotalSeconds - _lastRenderTime.TotalSeconds;
                Drive((float)duration);
                _lastRenderTime = renderingEventArgs.RenderingTime;
                if (_count++ == 10)
                {
                    SetCurrentValue(FrameRateProperty, 1 / duration);
                    _count = 0;
                }
            }
        }

        private void Drive(float duration)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (_circles == null || !_circles.Any()) return;

            CollisionDetection.DetectByBroadAndNarrowPhase(_circles);

            foreach (var ball in _circles)
            {
                Collision.DetectAndResolveLeftWall(ball, ball.BoundLeft);
                Collision.DetectAndResolveTopWall(ball, ball.BoundTop);
                Collision.DetectAndResolveRightWall(ball, ball.BoundRight);
                Collision.DetectAndResolveBottomWall(ball, ball.BoundBottom);
            }

            Draw(_circles);

            foreach (var ball in _circles)
            {
                ball.NextFrame(duration);
            }
        }

        private void Draw(IReadOnlyList<Circle> circles)
        {
            RenderTarget renderTarget = Host.RenderTarget;

            renderTarget.BeginDraw();

            renderTarget.Clear(new RawColor4(0f, 0f, 0f, 1f));

            for (int i = 0; i < circles.Count; i++)
            {
                Circle circle = circles[i];

                float x0 = circle.Position.X;
                float y0 = circle.Position.Y;

                for (int j = i + 1; j < circles.Count; j++)
                {
                    float x1 = circles[j].Position.X;
                    float y1 = circles[j].Position.Y;

                    renderTarget.DrawLine(new RawVector2(x0, y0), new RawVector2(x1, y1), _lineBrush, 0.1f);
                }

                renderTarget.DrawEllipse(new Ellipse(new RawVector2(circle.Position.X, circle.Position.Y), circle.Radius, circle.Radius), _circleBrush, 2f);
                renderTarget.DrawLine(new RawVector2(x0, y0), new RawVector2(circle.Velocity.X + x0, circle.Velocity.Y + y0), _circleBrush, 1f);
                renderTarget.DrawText(
                    $"坐标：({x0}, {y0})",
                    new TextFormat(new SharpDX.DirectWrite.Factory(), "Microsoft YaHei", 12f),
                    new RawRectangleF(x0 - 100, y0 - circle.Radius - 18, x0 + 200, y0 + 50), _circleBrush);
            }

            renderTarget.DrawBitmap(_bitmap.Bitmap, 1f, BitmapInterpolationMode.Linear);

            renderTarget.EndDraw();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _lineBrush = new(Host.RenderTarget, new RawColor4(0f, 1f, 0f, 0.2f));
            _circleBrush = new(Host.RenderTarget, new RawColor4(1f, 1f, 1f, 1f));

            _bitmap = CreateBitmap();

            CompositionTarget.Rendering += CompositionTargetOnRendering;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            DrawMesh(_bitmap);
        }
    }
}
