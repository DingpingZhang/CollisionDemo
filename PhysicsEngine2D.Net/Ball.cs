using System.Numerics;

namespace PhysicsEngine2D.Net
{
    public class Ball : Particle
    {
        private float _boundLeft;
        private float _boundTop;
        private float _boundRight;
        private float _boundBottom;

        public float Radius { get; set; }

        public Ball SetBound(float left, float top, float right, float bottom)
        {
            _boundLeft = left + Radius;
            _boundTop = top + Radius;
            _boundRight = right - Radius;
            _boundBottom = bottom - Radius;

            return this;
        }

        public override void Collide(IParticle other)
        {
            base.Collide(other);

            if (other is Ball ball)
            {
                // 对穿模情况的位置进行修正：只能保证这两个小球不穿模，但移动后会不会与其它小球穿模，无法保证。
                // 并不能真正解决穿模问题，只是缓解。
                var positiveVector = ball.Position - Position;
                var actualDistance = positiveVector.Length();
                positiveVector /= actualDistance;
                var delta = Radius + ball.Radius - actualDistance;
                if (delta > 0)
                {
                    delta /= 2;
                    Position += -delta * positiveVector;
                    ball.Position += delta * positiveVector;
                }
            }
        }

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

        public bool DetectNarrowPhase(Ball other)
        {
            var distance = Radius + other.Radius;
            return (Position - other.Position).LengthSquared() <= distance * distance;
        }
    }
}
