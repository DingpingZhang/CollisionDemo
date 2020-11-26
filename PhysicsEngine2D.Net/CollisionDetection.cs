using System.Collections.Generic;

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
                    if (ball1.DetectNarrowPhase(ball2))
                    {
                        ball1.ResolveCollision(ball2);
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
                if (ball1.DetectNarrowPhase(ball2))
                {
                    ball1.ResolveCollision(ball2);
                }
            }
        }
    }
}
