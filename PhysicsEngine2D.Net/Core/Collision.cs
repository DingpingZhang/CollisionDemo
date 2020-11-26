using System;
using System.Numerics;

namespace PhysicsEngine2D.Net.Core
{
    public readonly struct CollisionInfo
    {
        public static readonly CollisionInfo Empty = default;

        public IParticle A { get; }

        public IParticle B { get; }

        public float Penetration { get; }

        public Vector2 Normal { get; }

        public CollisionInfo(IParticle a, IParticle b, float penetration, Vector2 normal)
        {
            A = a;
            B = b;
            Penetration = penetration;
            Normal = normal;
        }
    }

    public static class Collision
    {
        public static void Detect()
        {

        }

        public static CollisionInfo Detect(ICircle c1, ICircle c2)
        {
            var r = c1.Radius + c2.Radius;
            var normal = c2.Position - c1.Position;
            var distanceSquared = normal.LengthSquared();
            if (distanceSquared <= r * r)
            {
                var distance = (float)Math.Sqrt(distanceSquared);
                normal /= distance;
                return new CollisionInfo(c1, c2, r - distance, normal);
            }

            return CollisionInfo.Empty;
        }

        public static void Resolve(CollisionInfo info)
        {
            // 设：p1 -> p2 为正方向，rel = relative vel = velocity
            // Ref to: https://gamedevelopment.tutsplus.com/tutorials/how-to-create-a-custom-2d-physics-engine-the-basics-and-impulse-resolution--gamedev-6331
            var a = info.A;
            var b = info.B;

            var relVel = b.Velocity - a.Velocity;
            var relVelAlongNormal = Vector2.Dot(relVel, info.Normal);

            if (relVelAlongNormal > 0) return;

            var restitution = Math.Min(a.Restitution, b.Restitution);

            var impulseScalar = -(1 + restitution) * relVelAlongNormal / (a.InverseMass + b.InverseMass);
            var impulse = impulseScalar * info.Normal;

            a.Velocity -= a.InverseMass * impulse;
            b.Velocity += b.InverseMass * impulse;

            // 位置修正
            const float percent = 0.2f; // usually 20% to 80%
            const float slop = 0.01f; // usually 0.01 to 0.1
            if (info.Penetration > slop)
            {
                var correction = info.Penetration / (a.InverseMass + b.InverseMass) * percent * info.Normal;
                a.Position -= a.InverseMass * correction;
                b.Position += b.InverseMass * correction;
            }
        }
    }
}
