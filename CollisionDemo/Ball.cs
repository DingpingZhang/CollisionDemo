﻿using System;
using System.Windows;
using Vector = System.Windows.Vector;

namespace CollisionDemo
{
    public class Ball
    {
        private Rect _boundRect;

        public double Mass { get; set; }

        public Vector Velocity { get; set; }

        public Point Position { get; set; }

        public double Radius { get; set; }

        public Ball SetBound(double left, double top, double right, double bottom)
        {
            var x = left + Radius;
            var y = top + Radius;
            _boundRect = new Rect(x, y, right - Radius - x, bottom - Radius - y);

            return this;
        }

        public void Collide(Ball other)
        {
            if (DetectCollision(other))
            {
                Collide(this, other);
            }
        }

        public void CollideOnly(Ball other) => Collide(this, other);

        public void CollideHorizontalBound()
        {
            var isCollide = false;
            if (Position.X <= _boundRect.Left)
            {
                Position = new Point(_boundRect.Left, Position.Y);
                isCollide = true;
            }

            if (Position.X >= _boundRect.Right)
            {
                Position = new Point(_boundRect.Right, Position.Y);
                isCollide = true;
            }

            if (isCollide)
            {
                Velocity = new Vector(-Velocity.X, Velocity.Y);
            }
        }

        public void CollideVerticalBound()
        {
            var isCollide = false;
            if (Position.Y <= _boundRect.Top)
            {
                Position = new Point(Position.X, _boundRect.Top);
                isCollide = true;
            }

            if (Position.Y >= _boundRect.Bottom)
            {
                Position = new Point(Position.X, _boundRect.Bottom);
                isCollide = true;
            }

            if (isCollide)
            {
                Velocity = new Vector(Velocity.X, -Velocity.Y);
            }
        }

        public void NextFrame(double millisecond)
        {
            Position += Velocity * (millisecond / 1000.0);
        }

        public bool DetectNarrowPhase(Ball other)
        {
            var distance = Radius + other.Radius;
            return (Position - other.Position).LengthSquared <= distance * distance;
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
            // 设：b1 -> b2 为正方向，将速度延中心连线方向正交分解。
            var positiveVector = b2.Position - b1.Position;
            var actualDistance = positiveVector.Length;
            positiveVector.Normalize();
            var tangentVector = new Vector(positiveVector.Y, -positiveVector.X);

            var vp1 = b1.Velocity * positiveVector;
            var vp2 = b2.Velocity * positiveVector;
            var vt1 = b1.Velocity * tangentVector * tangentVector;
            var vt2 = b2.Velocity * tangentVector * tangentVector;

            // 对于中心连线防线上的分速度有：
            // m1v1 + m2v2 = m1v1' + m2v2'
            // m1v1^2 + m2v2^2 = m1v1'^2 + m2v2'^2
            // 联立以上方程，解得：（消元代入，最后可以十字相乘的）
            // => v1' = ((m1v1 + m2v2) + (v2 - v1)m2) / (m1 + m2)
            // => v2' = ((m1v1 + m2v2) + (v1 - v2)m1) / (m1 + m2)
            var initialMomentum = b1.Mass * vp1 + b2.Mass * vp2;
            var totalMass = b1.Mass + b2.Mass;
            var vp1Next = (initialMomentum + (vp2 - vp1) * b2.Mass) / totalMass * positiveVector;
            var vp2Next = (initialMomentum + (vp1 - vp2) * b1.Mass) / totalMass * positiveVector;

            b1.Velocity = vp1Next + vt1;
            b2.Velocity = vp2Next + vt2;

            // 对穿模情况的位置进行修正：只能保证这两个小球不穿模，但移动后会不会与其它小球穿模，无法保证。
            // 并不能真正解决穿模问题，只是缓解。
            var delta = b1.Radius + b2.Radius - actualDistance;
            if (delta > 0)
            {
                delta /= 2;
                b1.Position += -delta * positiveVector;
                b2.Position += delta * positiveVector;
            }
        }
    }
}
