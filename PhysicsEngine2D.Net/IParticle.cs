using System.Numerics;

namespace PhysicsEngine2D.Net
{
    public interface IParticle
    {
        float Mass { get; set; }

        float InverseMass { get; }

        Vector2 Position { get; set; }

        Vector2 Velocity { get; set; }

        Vector2 Acceleration { get; set; }

        float Restitution { get; set; }
    }

    public interface ICircle : IParticle
    {
        float Radius { get; set; }
    }

    public interface IRectangle : IParticle
    {
        float Width { get; set; }

        float Height { get; set; }
    }
}
