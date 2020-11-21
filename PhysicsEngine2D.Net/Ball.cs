using System;
using System.Numerics;

namespace PhysicsEngine2D.Net
{
    public class Ball
    {
        private float _boundLeft;
        private float _boundTop;
        private float _boundRight;
        private float _boundBottom;

        public float Mass { get; set; }

        public Vector2 Velocity { get; set; }

        public Vector2 Position { get; set; }

        public float Radius { get; set; }

        public Ball SetBound(float left, float top, float right, float bottom)
        {
            _boundLeft = left + Radius;
            _boundTop = top + Radius;
            _boundRight = right - Radius;
            _boundBottom = bottom - Radius;

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
            if (Position.X <= _boundLeft)
            {
                Position = new Vector2(_boundLeft, Position.Y);
                isCollide = true;
            }

            if (Position.X >= _boundRight)
            {
                Position = new Vector2(_boundRight, Position.Y);
                isCollide = true;
            }

            if (isCollide)
            {
                Velocity = new Vector2(-Velocity.X, Velocity.Y);
            }
        }

        public void CollideVerticalBound()
        {
            var isCollide = false;
            if (Position.Y <= _boundTop)
            {
                Position = new Vector2(Position.X, _boundTop);
                isCollide = true;
            }

            if (Position.Y >= _boundBottom)
            {
                Position = new Vector2(Position.X, _boundBottom);
                isCollide = true;
            }

            if (isCollide)
            {
                Velocity = new Vector2(Velocity.X, -Velocity.Y);
            }
        }

        public void NextFrame(float millisecond)
        {
            Position += Velocity * (millisecond / 1000f);
        }

        public bool DetectNarrowPhase(Ball other)
        {
            var distance = Radius + other.Radius;
            return (Position - other.Position).LengthSquared() <= distance * distance;
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
            var actualDistance = positiveVector.Length();
            positiveVector = Vector2.Normalize(positiveVector);
            var tangentVector = new Vector2(positiveVector.Y, -positiveVector.X);

            var vp1 = Vector2.Dot(b1.Velocity, positiveVector);
            var vp2 = Vector2.Dot(b2.Velocity, positiveVector);
            var vt1 = Vector2.Dot(b1.Velocity, tangentVector) * tangentVector;
            var vt2 = Vector2.Dot(b2.Velocity, tangentVector) * tangentVector;

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
