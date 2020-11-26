using System.Numerics;

namespace PhysicsEngine2D.Net
{
    public class Particle : IParticle, IUpdatable
    {
        private float _mass;

        public float Mass
        {
            get => _mass;
            set
            {
                _mass = value;
                InverseMass = 1 / value;
            }
        }

        public float InverseMass { get; set; }

        public Vector2 Position { get; set; }

        public Vector2 Velocity { get; set; }

        public Vector2 Acceleration { get; set; } = Vector2.Zero;

        public float Restitution { get; set; } = 1f;

        public virtual void NextFrame(float duration)
        {
            Position += Velocity * duration + Acceleration * (duration * duration) / 2;
            Velocity += Acceleration * duration;
        }
    }
}
