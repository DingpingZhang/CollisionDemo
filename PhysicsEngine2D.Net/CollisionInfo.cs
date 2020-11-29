using System.Numerics;
using PhysicsEngine2D.Net.Basic;

namespace PhysicsEngine2D.Net
{
    public readonly struct CollisionInfo
    {
        public static readonly CollisionInfo Empty = default;

        public Body A { get; }

        public Body B { get; }

        public float Penetration { get; }

        public Vector2 Normal { get; }

        public CollisionInfo(Body a, Body b, float penetration, Vector2 normal)
        {
            A = a;
            B = b;
            Penetration = penetration;
            Normal = normal;
        }
    }
}