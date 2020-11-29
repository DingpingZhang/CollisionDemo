using System.Numerics;

namespace PhysicsEngine2D.Net.Basic
{
    public readonly struct CollisionResult
    {
        public static readonly CollisionResult Empty = default;

        public float Penetration { get; init; }

        public Vector2 Normal { get; init; }

        public void Deconstruct(out float penetration, out Vector2 normal)
        {
            penetration = Penetration;
            normal = Normal;
        }
    }
}
