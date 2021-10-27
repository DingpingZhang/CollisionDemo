using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using PhysicsEngine2D.Net;

namespace CollisionDemo.Controls
{
    public abstract class CanvasBase : FrameworkElement
    {
        public static readonly DependencyProperty ShapesProperty = DependencyProperty.Register(
            "Shapes", typeof(ObservableCollection<Circle>), typeof(CanvasBase), new PropertyMetadata(default(ObservableCollection<Circle>)));
        public static readonly DependencyProperty FrameRateProperty = DependencyProperty.Register(
            "FrameRate", typeof(double), typeof(CanvasBase), new PropertyMetadata(default(double)));

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

        protected DrawingVisual DrawingVisual = new();

        protected CanvasBase()
        {
            AddVisualChild(DrawingVisual);

            TimeSpan lastRenderTime = new TimeSpan();
            CompositionTarget.Rendering += (_, args) =>
            {
                if (args is RenderingEventArgs renderingEventArgs && renderingEventArgs.RenderingTime != lastRenderTime)
                {
                    double duration = renderingEventArgs.RenderingTime.TotalSeconds - lastRenderTime.TotalSeconds;
                    Drive((float)(renderingEventArgs.RenderingTime.TotalSeconds - lastRenderTime.TotalSeconds));
                    lastRenderTime = renderingEventArgs.RenderingTime;
                    SetCurrentValue(FrameRateProperty, 1 / duration);
                }
            };
        }

        private void Drive(float duration)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (Shapes == null || !Shapes.Any()) return;

            CollisionDetection.DetectByBroadAndNarrowPhase(Shapes);

            foreach (var ball in Shapes)
            {
                Collision.DetectAndResolveLeftWall(ball, ball.BoundLeft);
                Collision.DetectAndResolveTopWall(ball, ball.BoundTop);
                Collision.DetectAndResolveRightWall(ball, ball.BoundRight);
                Collision.DetectAndResolveBottomWall(ball, ball.BoundBottom);
            }

            Draw(Shapes);

            foreach (var ball in Shapes)
            {
                ball.NextFrame(duration);
            }
        }

        protected abstract void Draw(IReadOnlyList<Circle> shapes);

        protected override int VisualChildrenCount => 1;

        protected override Visual GetVisualChild(int index) => DrawingVisual;

        protected override Geometry GetLayoutClip(Size layoutSlotSize)
        {
            return new RectangleGeometry(new Rect(new Size(ActualWidth, ActualHeight)));
        }
    }
}
