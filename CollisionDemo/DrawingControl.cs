﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using PhysicsEngine2D.Net;
using Brushes = System.Windows.Media.Brushes;
using Pen = System.Windows.Media.Pen;
using Size = System.Windows.Size;

namespace CollisionDemo
{
    public class DrawingControl : FrameworkElement
    {
        private readonly DrawingVisual _drawingVisual;

        public static readonly DependencyProperty BallsProperty = DependencyProperty.Register(
            "Balls", typeof(ObservableCollection<Circle>), typeof(DrawingControl), new PropertyMetadata(default(ObservableCollection<Circle>)));

        public ObservableCollection<Circle> Balls
        {
            get => (ObservableCollection<Circle>)GetValue(BallsProperty);
            set => SetValue(BallsProperty, value);
        }

        public static readonly DependencyProperty FrameRateProperty = DependencyProperty.Register(
            "FrameRate", typeof(double), typeof(DrawingControl), new PropertyMetadata(default(double)));

        public double FrameRate
        {
            get { return (double)GetValue(FrameRateProperty); }
            set { SetValue(FrameRateProperty, value); }
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
                    double duration = renderingEventArgs.RenderingTime.TotalSeconds - lastRenderTime.TotalSeconds;
                    Draw((float)(renderingEventArgs.RenderingTime.TotalSeconds - lastRenderTime.TotalSeconds));
                    lastRenderTime = renderingEventArgs.RenderingTime;
                    SetCurrentValue(FrameRateProperty, 1 / duration);
                }
            };
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
