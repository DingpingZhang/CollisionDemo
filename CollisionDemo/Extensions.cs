using System.Numerics;
using System.Windows;
using System.Windows.Media;
using PhysicsEngine2D.Net;
using Vector = System.Windows.Vector;

namespace CollisionDemo
{
    public static class Extensions
    {
        private static readonly Brush CircleBrush;
        private static readonly Pen VelocityPen = new Pen(Brushes.Blue, 2);

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

        public static void Draw(this Ball ball, DrawingContext dc)
        {
            dc.DrawEllipse(
                CircleBrush,
                null,
                ball.Position.ToPoint(),
                ball.Radius,
                ball.Radius);
        }

        public static void DrawVelocity(this IParticle particle, DrawingContext dc)
        {
            dc.DrawLine(
                VelocityPen,
                particle.Position.ToPoint(),
                (particle.Position + particle.Velocity).ToPoint());
        }
    }
}
