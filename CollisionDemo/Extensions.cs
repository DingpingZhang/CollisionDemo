using System.Numerics;
using System.Windows;
using System.Windows.Media;
using PhysicsEngine2D.Net.Basic;
using Vector = System.Windows.Vector;

namespace CollisionDemo
{
    public static class Extensions
    {
        private static readonly Brush CircleBrush;
        private static readonly Pen VelocityPen = new Pen(Brushes.Blue, 1);
        private static readonly Pen CirclePen = new Pen(Brushes.White, 1);

        static Extensions()
        {
            var brush = new RadialGradientBrush { GradientOrigin = new Point(0.7, 0.3) };
            brush.RadiusX = brush.RadiusY = 1;
            brush.GradientStops.Add(new GradientStop(Colors.White, 0));
            brush.GradientStops.Add(new GradientStop(Colors.Black, 1));
            brush.Freeze();
            CircleBrush = brush;
        }

        public static Point ToPoint(this Vector2 vector2)
        {
            return new Point(vector2.X, vector2.Y);
        }

        public static Vector ToVector(this Vector2 vector2)
        {
            return new Vector(vector2.X, vector2.Y);
        }

        public static void Draw(this Rectangle rectangle, DrawingContext dc)
        {
            dc.DrawRectangle(
                Brushes.Transparent,
                CirclePen,
                new Rect(
                    new Point(
                        rectangle.Position.X - rectangle.Width / 2,
                        rectangle.Position.Y - rectangle.Height / 2),
                    new Size(rectangle.Width, rectangle.Height)));
        }

        public static void Draw(this Circle ball, DrawingContext dc)
        {
            dc.DrawEllipse(
                Brushes.Transparent,
                CirclePen,
                ball.Position.ToPoint(),
                ball.Radius,
                ball.Radius);
        }

        public static void DrawVelocity(this Body particle, DrawingContext dc)
        {
            dc.DrawLine(
                VelocityPen,
                particle.Shape.Position.ToPoint(),
                (particle.Shape.Position + particle.Velocity).ToPoint());
        }
    }
}
