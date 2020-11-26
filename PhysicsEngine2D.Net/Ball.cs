using System.Numerics;

namespace PhysicsEngine2D.Net
{
    public class Ball : Particle, ICircle
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
                Velocity = new Vector2(-Restitution * Velocity.X, Velocity.Y);
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
                Velocity = new Vector2(Velocity.X, -Restitution * Velocity.Y);
            }
        }
    }
}
