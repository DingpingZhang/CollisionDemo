namespace PhysicsEngine2D.Net.Basic
{
    public class Rectangle : Shape
    {
        public float Width { get; set; }

        public float Height { get; set; }

        public override float Left => Position.X - Width / 2;

        public override float Top => Position.Y - Height / 2;

        public override float Right => Position.X + Width / 2;

        public override float Bottom => Position.Y + Height / 2;

        public override void Accept(IShapeVisitor visitor) => visitor.Visit(this);

        public override CollisionResult Collide(ICollideable shape) => shape.CollidedBy(this);

        public override CollisionResult CollidedBy(Circle circle) => Collision.Detect(this, circle);

        public override CollisionResult CollidedBy(Rectangle rectangle) => Collision.Detect(this, rectangle);

    }
}
