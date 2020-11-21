using System.Numerics;
using System.Windows;
using Vector = System.Windows.Vector;

namespace CollisionDemo
{
    public static class Extensions
    {
        public static Point ToPoint(this Vector2 vector2)
        {
            return new Point(vector2.X, vector2.Y);
        }

        public static Vector ToVector(this Vector2 vector2)
        {
            return new Vector(vector2.X, vector2.Y);
        }
    }
}
