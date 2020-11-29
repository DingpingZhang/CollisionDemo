using System.Numerics;

namespace PhysicsEngine2D.Net.Basic
{
    public class Body
    {
        public IShape Shape { get; set; }

        public Material Material { get; set; }

        public MassData MassData { get; set; }

        public Vector2 Velocity { get; set; }

        public Vector2 Force { get; set; }

        public long Layers { get; set; }

        public CollisionInfo Collide(Body other)
        {
            var (penetration, normal) = Shape.Collide(other.Shape);
            return new CollisionInfo(this, other, penetration, normal);
        }
    }
}
