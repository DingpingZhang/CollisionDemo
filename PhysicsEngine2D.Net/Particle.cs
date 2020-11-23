using System;
using System.Diagnostics;
using System.Numerics;

namespace PhysicsEngine2D.Net
{
    public class Particle : IParticle, IUpdatable
    {
        public float Mass { get; set; }

        public Vector2 Position { get; set; }

        public Vector2 Velocity { get; set; }

        public Vector2 Acceleration { get; set; } = Vector2.Zero;

        public float Damping { get; set; } = 1f;

        public virtual void Collide(IParticle other) => Collide(this, other);

        public virtual void NextFrame(float duration)
        {
            Position += Velocity * duration + Acceleration * (duration * duration) / 2;
            var damping = Math.Abs(Damping - 1) < Constants.Tolerance ? 1f : (float) Math.Pow(Damping, duration);
            Velocity = damping * Velocity + Acceleration * duration;
        }

        private static void Collide(IParticle p1, IParticle p2)
        {
            // 设：p1 -> p2 为正方向，将速度延中心连线方向正交分解。
            var positiveVector = p2.Position - p1.Position;
            positiveVector = Vector2.Normalize(positiveVector);
            var tangentVector = new Vector2(positiveVector.Y, -positiveVector.X);

            var vp1 = Vector2.Dot(p1.Velocity, positiveVector);
            var vp2 = Vector2.Dot(p2.Velocity, positiveVector);
            var vt1 = Vector2.Dot(p1.Velocity, tangentVector) * tangentVector;
            var vt2 = Vector2.Dot(p2.Velocity, tangentVector) * tangentVector;

            // 对于中心连线防线上的分速度有：
            // m1v1 + m2v2 = m1v1' + m2v2'
            // m1v1^2 + m2v2^2 = m1v1'^2 + m2v2'^2
            // 联立以上方程，解得：（消元代入，最后可以十字相乘的）
            // => v1' = ((m1v1 + m2v2) + (v2 - v1)m2) / (m1 + m2)
            // => v2' = ((m1v1 + m2v2) + (v1 - v2)m1) / (m1 + m2)
            var initialMomentum = p1.Mass * vp1 + p2.Mass * vp2;
            var totalMass = p1.Mass + p2.Mass;
            var vp1Next = (initialMomentum + (vp2 - vp1) * p2.Mass) / totalMass * positiveVector;
            var vp2Next = (initialMomentum + (vp1 - vp2) * p1.Mass) / totalMass * positiveVector;

            p1.Velocity = vp1Next + vt1;
            p2.Velocity = vp2Next + vt2;
        }
    }
}
