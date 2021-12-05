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
            Host.MouseMove += HostOnMouseMove;
            Host.MouseWheel += HostOnMouseWheel;
            Host.MouseEnter += HostOnMouseEnter;

            InitBalls();
        }

        private void HostOnMouseEnter(object sender, MouseEventArgs e)
        {
        }

        private void HostOnMouseWheel(object sender, MouseWheelEventArgs e)
        {
        }

        private void HostOnMouseMove(object sender, MouseEventArgs e)
        {

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

                renderTarget.DrawEllipse(new Ellipse(new RawVector2(circle.Position.X, circle.Position.Y), circle.Radius, circle.Radius), _circleBrush);

                for (int j = i + 1; j < circles.Count; j++)
                {
                    float x1 = circles[j].Position.X;
                    float y1 = circles[j].Position.Y;

                    renderTarget.DrawLine(new RawVector2(x0, y0), new RawVector2(x1, y1), _lineBrush, 0.1f);
                }
            }

            renderTarget.EndDraw();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _lineBrush = new(Host.RenderTarget, new RawColor4(1f, 1f, 1f, 0.2f));
            _circleBrush = new(Host.RenderTarget, new RawColor4(0f, 1f, 0f, 1f));

            CompositionTarget.Rendering += CompositionTargetOnRendering;
        }
    }
}
