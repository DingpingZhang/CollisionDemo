using System;
using System.Windows;
using Vector = System.Windows.Vector;

namespace CollisionDemo
{
    public class Ball
    {
        public double Mass { get; set; }

        public Vector Speed { get; set; }

        public Point Position { get; set; }

        public double Radius { get; set; }

        public void Collide(Ball other) => Collide(this, other);

        public void CollideHorizontalBound()
        {
            var isCollide = false;
            if (Position.X - Radius <= 0)
            {
                Position = new Point(Radius, Position.Y);
                isCollide = true;
            }

            if (Position.X + Radius >= 500)
            {
                Position = new Point(500 - Radius, Position.Y);
                isCollide = true;
            }

            if (isCollide)
            {
                Speed = new Vector(-Speed.X, Speed.Y);
            }
        }

        public void CollideVerticalBound()
        {
            var isCollide = false;
            if (Position.Y - Radius <= 0)
            {
                Position = new Point(Position.X, Radius);
                isCollide = true;
            }

            if (Position.Y + Radius >= 500)
            {
                Position = new Point(Position.X, 500 - Radius);
                isCollide = true;
            }

            if (isCollide)
            {
                Speed = new Vector(Speed.X, -Speed.Y);
            }
        }

        public void Next(double millisecond)
        {
            var offset = Speed * (millisecond / 1000.0);
            var x = Position.X + offset.X;
            var y = Position.Y + offset.Y;

            Position = new Point(x, y);
        }

        private bool DetectCollision(Ball other)
        {
            var dx = Position.X - other.Position.X;
            var distance = Radius + other.Radius;
            if (Math.Abs(dx) > distance) return false;

            var dy = Position.Y - other.Position.Y;
            return dx * dx + dy * dy <= distance * distance;
        }

        private static void Collide(Ball b1, Ball b2)
        {
            if (!b1.DetectCollision(b2)) return;

            // 设：b1 -> b2 为正方向，将速度延中心连线方向正交分解。
            var positiveDirection = b2.Position - b1.Position;
            var actualDistance = positiveDirection.Length;
            positiveDirection.Normalize();
            var tangentDirection = new Vector(positiveDirection.Y, -positiveDirection.X);

            var v1p = b1.Speed * positiveDirection;
            var v2p = b2.Speed * positiveDirection;
            var v1t = (b1.Speed * tangentDirection) * tangentDirection;
            var v2t = (b2.Speed * tangentDirection) * tangentDirection;
            // 对于中心连线防线上的分速度有：
            // m1v1 + m2v2 = m1v1' + m2v2'
            // m1v1^2 + m2v2^2 = m1v1'^2 + m2v2'^2
            // 联立以上方程，解得：（消元代入，最后可以十字相乘的）
            // => v1' = ((m1v1 + m2v2) + (v2 - v1)m2) / (m1 + m2)
            // => v2' = ((m1v1 + m2v2) + (v1 - v2)m1) / (m1 + m2)
            var initialMomentum = b1.Mass * v1p + b2.Mass * v2p;
            var totalMass = b1.Mass + b2.Mass;
            var v1pNext = ((initialMomentum + (v2p - v1p) * b2.Mass) / totalMass) * positiveDirection;
            var v2pNext = ((initialMomentum + (v1p - v2p) * b1.Mass) / totalMass) * positiveDirection;

            b1.Speed = v1pNext + v1t;
            b2.Speed = v2pNext + v2t;

            // 对穿模情况的位置进行修正
            var delta = b1.Radius + b2.Radius - actualDistance;
            if (delta > 0)
            {
                delta /= 2;
                b1.Position += -delta * positiveDirection;
                b2.Position += delta * positiveDirection;
            }
        }
    }
}
