namespace PhysicsEngine2D.Net.Basic
{
    public interface IShapeVisitor
    {
        void Visit(Circle circle);

        void Visit(Rectangle rectangle);
    }
}