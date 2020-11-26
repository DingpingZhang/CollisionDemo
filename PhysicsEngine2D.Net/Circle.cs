using System.Numerics;

namespace PhysicsEngine2D.Net
{
    public class Circle : Particle, ICircle
    {
        public float BoundLeft;
        public float BoundTop;
        public float BoundRight;
        public float BoundBottom;

        public float Radius { get; set; }

        public Circle SetBound(float left, float top, float right, float bottom)
        {
            BoundLeft = left;
            BoundTop = top;
            BoundRight = right;
            BoundBottom = bottom;

            return this;
        }
    }
}
