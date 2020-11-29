using System.Collections.Generic;
using System.Linq;
using PhysicsEngine2D.Net.Basic;
using static PhysicsEngine2D.Net.CollisionInfo;

namespace PhysicsEngine2D.Net
{
    public static class CollisionDetection
    {
        public static void DetectByForce(IReadOnlyList<Body> bodies)
        {
            for (int i = 0; i < bodies.Count; i++)
            {
                for (int j = i + 1; j < bodies.Count; j++)
                {
                    var ball1 = bodies[i];
                    var ball2 = bodies[j];
                    var result = ball1.Collide(ball2);
                    if (!result.Equals(Empty))
                    {
                        Collision.Resolve(result);
                    }
                }
            }
        }

        public static void DetectByBroadAndNarrowPhase(IReadOnlyList<Body> bodies)
        {
            foreach (var (ball1Index, ball2Index) in BroadPhase.Detect(bodies.Select(item => item.Shape).ToList()))
            {
                var ball1 = bodies[ball1Index];
                var ball2 = bodies[ball2Index];
                var result = ball1.Collide(ball2);
                if (!result.Equals(Empty))
                {
                    Collision.Resolve(result);
                }
            }
        }
    }
}
