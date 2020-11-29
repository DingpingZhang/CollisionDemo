namespace PhysicsEngine2D.Net.Basic
{
    public interface ICollideable
    {
        CollisionResult Collide(ICollideable shape);

        CollisionResult CollidedBy(Circle circle);

        CollisionResult CollidedBy(Rectangle rectangle);
    }
}
