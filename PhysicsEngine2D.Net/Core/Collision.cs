using System;
using System.Numerics;

namespace PhysicsEngine2D.Net.Core
{
    public static class Collision
    {
        public static void Detect()
        {

        }

        public static void Resolve(IParticle p1, IParticle p2)
        {
            // 设：p1 -> p2 为正方向，rel = relative vel = velocity
            // Ref to: https://gamedevelopment.tutsplus.com/tutorials/how-to-create-a-custom-2d-physics-engine-the-basics-and-impulse-resolution--gamedev-6331
            var relVel = p2.Velocity - p1.Velocity;
            var normal = p2.Position - p1.Position;
            normal = Vector2.Normalize(normal);
            var relVelAlongNormal = Vector2.Dot(relVel, normal);

            if (relVelAlongNormal > 0) return;

            var restitution = Math.Min(p1.Restitution, p2.Restitution);

            var impulseScalar = -(1 + restitution) * relVelAlongNormal / (p1.InverseMass + p1.InverseMass);
            var impulse = impulseScalar * normal;

            p1.Velocity -= p1.InverseMass * impulse;
            p2.Velocity += p2.InverseMass * impulse;
        }
    }
}
