using System;

namespace PhysicsEngine2D.Net.Basic
{
    public class Circle : Shape
    {
        public float Radius { get; set; }

        public override float Left => Position.X - Radius;

        public override float Top => Position.Y - Radius;

        public override float Right => Position.X + Radius;

        public override float Bottom => Position.Y + Radius;

        public override void Accept(IShapeVisitor visitor) => visitor.Visit(this);

        public override CollisionResult Collide(ICollideable shape) => shape.CollidedBy(this);

        public override CollisionResult CollidedBy(Circle circle) => Collision.Detect(this, circle);

        public override CollisionResult CollidedBy(Rectangle rectangle) => Collision.Detect(rectangle, this);
    }
}
