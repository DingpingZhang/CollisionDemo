using System;
using System.Numerics;

namespace PhysicsEngine2D.Net
{
    public class Particle : IParticle, IUpdatable
    {
        public float Mass { get; set; }

        public Vector2 Position { get; set; }

        public Vector2 Velocity { get; set; }

        public Vector2 Acceleration { get; set; } = Vector2.Zero;

        public float Restitution { get; set; } = 1f;

        public virtual void ResolveCollision(IParticle other) => ResolveCollision(this, other);

        public virtual void NextFrame(float duration)
        {
            Position += Velocity * duration + Acceleration * (duration * duration) / 2;
            Velocity += Acceleration * duration;
        }

        private static void ResolveCollision(IParticle p1, IParticle p2)
        {
            // 设：p1 -> p2 为正方向，rel = relative vel = velocity
            var relVel = p2.Velocity - p1.Velocity;
            var normal = p2.Position - p1.Position;
            normal = Vector2.Normalize(normal);
            var relVelAlongNormal = Vector2.Dot(relVel, normal);

            if (relVelAlongNormal > 0) return;

            var restitution = Math.Min(p1.Restitution, p2.Restitution);

            var impulseScalar = -(1 + restitution) * relVelAlongNormal / (1 / p1.Mass + 1 / p2.Mass);
            var impulse = impulseScalar * normal;

            p1.Velocity -= impulse / p1.Mass;
            p2.Velocity += impulse / p2.Mass;
        }
    }
}
