using System;

namespace PhysicsEngine2D.Net.Basic
{
    public class Circle : Shape
    {
        public float Radius { get; set; }

        public override void Accept(IShapeVisitor visitor) => visitor.Visit(this);

        public override CollisionResult Collide(ICollideable shape) => shape.CollidedBy(this);

        public override CollisionResult CollidedBy(Circle circle) => Collision.Detect(this, circle);

        public override CollisionResult CollidedBy(Rectangle rectangle) => Collision.Detect(rectangle, this);
    }
}
