using System.Collections.Generic;
using PhysicsEngine2D.Net.Core;
using static PhysicsEngine2D.Net.Core.CollisionInfo;

namespace PhysicsEngine2D.Net
{
    public static class CollisionDetection
    {
        public static void DetectByForce(IReadOnlyList<Ball> balls)
        {
            for (int i = 0; i < balls.Count; i++)
            {
                for (int j = i + 1; j < balls.Count; j++)
                {
                    var ball1 = balls[i];
                    var ball2 = balls[j];
                    var result = Collision.Detect(ball1, ball2);
                    if (!result.Equals(Empty))
                    {
                        Collision.Resolve(result);
                    }
                }
            }
        }

        public static void DetectByBroadAndNarrowPhase(IReadOnlyList<Ball> balls)
        {
            foreach (var (ball1Index, ball2Index) in BroadPhase.Detect(balls))
            {
                var ball1 = balls[ball1Index];
                var ball2 = balls[ball2Index];
                var result = Collision.Detect(ball1, ball2);
                if (!result.Equals(Empty))
                {
                    Collision.Resolve(result);
                }
            }
        }
    }
}
