using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using BenchmarkDotNet.Attributes;
using PhysicsEngine2D.Net;

namespace BenchmarkTest
{
    public class ForceDetectVsBroadPhase
    {
        private static readonly Random Random = new Random();
        private IReadOnlyList<Ball> _balls;

        [Params(10, 100, 2000, 5000)]
        public int N;

        [Params(4, 10)]
        public float MeanRadius;

        public float Width = 1000;

        public float Height = 1000;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _balls = Enumerable.Range(0, N)
                .Select(i =>
                {
                    var weight = GetRandom(MeanRadius / 2, 3 * MeanRadius / 2);
                    return new Ball
                    {
                        Mass = (float)Math.Sqrt(weight),
                        Position = new Vector2(GetRandom(0, Width), GetRandom(0, Height)),
                        Radius = weight,
                        Velocity = new Vector2(GetRandom(-100, 100), GetRandom(-100, 100)),
                    }.SetBound(0, 0, Width, Height);
                })
                .ToList();
        }

        [Benchmark(Baseline = true)]
        public void DetectByForce()
        {
            CollisionDetection.DetectByForce(_balls);
        }

        [Benchmark]
        public void DetectByBroadAndNarrowPhase()
        {
            CollisionDetection.DetectByBroadAndNarrowPhase(_balls);
        }

        private static float GetRandom(float a, float b)
        {
            return a + (b - a) * (float)Random.NextDouble();
        }
    }
}
