using System.Numerics;

namespace PhysicsEngine2D.Net
{
    public interface IParticle
    {
        float Mass { get; set; }

        Vector2 Position { get; set; }

        Vector2 Velocity { get; set; }

        Vector2 Acceleration { get; set; }

        float Damping { get; set; }

        void Collide(IParticle other);
    }
}
