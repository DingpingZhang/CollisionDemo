using BenchmarkDotNet.Running;

namespace BenchmarkTest
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<ForceDetectVsBroadPhase>();
        }
    }
}
