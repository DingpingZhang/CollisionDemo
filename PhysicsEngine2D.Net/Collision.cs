using System;
using System.Numerics;
using PhysicsEngine2D.Net.Basic;

namespace PhysicsEngine2D.Net
{
    public static class Collision
    {
        public static CollisionResult Detect(Circle c1, Circle c2)
        {
            var r = c1.Radius + c2.Radius;
            var normal = c1.Position - c2.Position;
            var distanceSquared = normal.LengthSquared();
            if (distanceSquared <= r * r)
            {
                var distance = (float)Math.Sqrt(distanceSquared);
                normal /= distance;
                return new CollisionResult { Penetration = r - distance, Normal = normal };
            }

            return CollisionResult.Empty;
        }

        public static CollisionResult Detect(Rectangle r1, Rectangle r2) => CollisionResult.Empty;

        public static CollisionResult Detect(Rectangle r, Circle c) => CollisionResult.Empty;

        public static void DetectAndResolveLeftWall(Body c, float limit = 0)
        {
            var shape = c.Shape;
            if (shape.Left <= limit)
            {
                shape.Position = new Vector2((shape.Right - shape.Left) / 2, shape.Position.Y);
                c.Velocity = new Vector2(-c.Material.Restitution * c.Velocity.X, c.Velocity.Y);
            }
        }

        public static void DetectAndResolveTopWall(Body c, float limit = 0)
        {
            var shape = c.Shape;
            if (shape.Top <= limit)
            {
                shape.Position = new Vector2(shape.Position.X, (shape.Bottom - shape.Top) / 2);
                c.Velocity = new Vector2(c.Velocity.X, -c.Material.Restitution * c.Velocity.Y);
            }
        }

        public static void DetectAndResolveRightWall(Body c, float limit)
        {
            var shape = c.Shape;
            if (shape.Right >= limit)
            {
                shape.Position = new Vector2(limit - (shape.Right - shape.Left) / 2, shape.Position.Y);
                c.Velocity = new Vector2(-c.Material.Restitution * c.Velocity.X, c.Velocity.Y);
            }
        }

        public static void DetectAndResolveBottomWall(Body c, float limit)
        {
            var shape = c.Shape;
            if (shape.Bottom >= limit)
            {
                shape.Position = new Vector2(shape.Position.X, limit - (shape.Bottom - shape.Top) / 2);
                c.Velocity = new Vector2(c.Velocity.X, -c.Material.Restitution * c.Velocity.Y);
            }
        }

        public static void Resolve(in CollisionInfo info)
        {
            // 设：a -> b 为正方向，rel = relative vel = velocity
            // Ref to: https://gamedevelopment.tutsplus.com/tutorials/how-to-create-a-custom-2d-physics-engine-the-basics-and-impulse-resolution--gamedev-6331
            var a = info.A;
            var b = info.B;

            var relVel = b.Velocity - a.Velocity;
            var relVelAlongNormal = Vector2.Dot(relVel, info.Normal);

            if (relVelAlongNormal > 0) return;

            var restitution = Math.Min(a.Material.Restitution, b.Material.Restitution);

            var inverseMassA = a.MassData.InverseMass;
            var inverseMassB = b.MassData.InverseMass;

            var impulseScalar = -(1 + restitution) * relVelAlongNormal / (inverseMassA + inverseMassB);
            var impulse = impulseScalar * info.Normal;

            a.Velocity -= inverseMassA * impulse;
            b.Velocity += inverseMassB * impulse;

            // 位置修正
            const float percent = 0.2f; // usually 20% to 80%
            const float slop = 0.01f; // usually 0.01 to 0.1
            if (info.Penetration > slop)
            {
                var correction = info.Penetration / (inverseMassA + inverseMassB) * percent * info.Normal;
                a.Shape.Position -= inverseMassA * correction;
                b.Shape.Position += inverseMassB * correction;
            }
        }
    }
}
