using System.Numerics;

namespace PhysicsEngine2D.Net.Basic
{
    public abstract class Shape : IShape
    {
        public Vector2 Position { get; set; }

        public abstract void Accept(IShapeVisitor visitor);

        public abstract CollisionResult Collide(ICollideable shape);

        public virtual CollisionResult CollidedBy(Circle circle) => CollisionResult.Empty;

        public virtual CollisionResult CollidedBy(Rectangle rectangle) => CollisionResult.Empty;
    }
}
