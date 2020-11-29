namespace PhysicsEngine2D.Net.Basic
{
    public class Rectangle : Shape
    {
        public float Width { get; set; }

        public float Height { get; set; }

        public override void Accept(IShapeVisitor visitor) => visitor.Visit(this);

        public override CollisionResult Collide(ICollideable shape) => shape.CollidedBy(this);

        public override CollisionResult CollidedBy(Circle circle) => Collision.Detect(this, circle);

        public override CollisionResult CollidedBy(Rectangle rectangle) => Collision.Detect(this, rectangle);

    }
}
