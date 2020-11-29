using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using BenchmarkDotNet.Attributes;
using PhysicsEngine2D.Net;
using PhysicsEngine2D.Net.Basic;

namespace BenchmarkTest
{
    public class ForceDetectVsBroadPhase
    {
        private static readonly Random Random = new Random();
        private IReadOnlyList<Body> _bodies;

        [Params(10, 100, 2000, 5000)]
        public int N;

        [Params(4, 10)]
        public float MeanRadius;

        public float Width = 1000;

        public float Height = 1000;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _bodies = Enumerable.Range(0, N)
                .Select(i =>
                {
                    var weight = GetRandom(MeanRadius / 2, 3 * MeanRadius / 2);
                    return new Body
                    {
                        MassData = new MassData { Mass = (float)Math.Sqrt(weight) },
                        Shape = new Circle
                        {
                            Position = new Vector2(GetRandom(0, Width), GetRandom(0, Height)),
                            Radius = weight,
                        },
                        Velocity = new Vector2(GetRandom(-100, 100), GetRandom(-100, 100)),
                    };
                })
                .ToList();
        }

        [Benchmark(Baseline = true)]
        public void DetectByForce()
        {
            CollisionDetection.DetectByForce(_bodies);
        }

        [Benchmark]
        public void DetectByBroadAndNarrowPhase()
        {
            CollisionDetection.DetectByBroadAndNarrowPhase(_bodies);
        }

        private static float GetRandom(float a, float b)
        {
            return a + (b - a) * (float)Random.NextDouble();
        }
    }
}
