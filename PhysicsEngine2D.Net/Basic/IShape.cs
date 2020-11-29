using System.Numerics;

namespace PhysicsEngine2D.Net.Basic
{
    public interface IShape : ICollideable
    {
        Vector2 Position { get; set; }

        void Accept(IShapeVisitor visitor);
    }
}
