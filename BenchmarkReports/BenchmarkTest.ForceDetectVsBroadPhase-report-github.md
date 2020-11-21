``` ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19041.630 (2004/?/20H1)
Intel Core i7-8700K CPU 3.70GHz (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=5.0.100
  [Host]     : .NET Core 5.0.0 (CoreCLR 5.0.20.51904, CoreFX 5.0.20.51904), X64 RyuJIT
  DefaultJob : .NET Core 5.0.0 (CoreCLR 5.0.20.51904, CoreFX 5.0.20.51904), X64 RyuJIT


```
|                      Method |    N | MeanRadius |            Mean |           Error |          StdDev | Ratio | RatioSD |
|---------------------------- |----- |----------- |----------------:|----------------:|----------------:|------:|--------:|
|               **DetectByForce** |   **10** |          **4** |        **318.1 ns** |         **0.80 ns** |         **0.71 ns** |  **1.00** |    **0.00** |
| DetectByBroadAndNarrowPhase |   10 |          4 |      1,359.8 ns |         7.03 ns |         6.23 ns |  4.28 |    0.02 |
|                             |      |            |                 |                 |                 |       |         |
|               **DetectByForce** |   **10** |         **10** |        **322.7 ns** |         **0.95 ns** |         **0.85 ns** |  **1.00** |    **0.00** |
| DetectByBroadAndNarrowPhase |   10 |         10 |      1,229.4 ns |         7.42 ns |         6.94 ns |  3.81 |    0.02 |
|                             |      |            |                 |                 |                 |       |         |
|               **DetectByForce** |  **100** |          **4** |     **29,747.6 ns** |        **47.26 ns** |        **44.21 ns** |  **1.00** |    **0.00** |
| DetectByBroadAndNarrowPhase |  100 |          4 |     19,612.8 ns |       115.54 ns |        90.21 ns |  0.66 |    0.00 |
|                             |      |            |                 |                 |                 |       |         |
|               **DetectByForce** |  **100** |         **10** |     **30,553.4 ns** |        **71.81 ns** |        **63.66 ns** |  **1.00** |    **0.00** |
| DetectByBroadAndNarrowPhase |  100 |         10 |     26,944.4 ns |        74.49 ns |        62.21 ns |  0.88 |    0.00 |
|                             |      |            |                 |                 |                 |       |         |
|               **DetectByForce** | **2000** |          **4** | **11,594,706.1 ns** |    **20,617.43 ns** |    **18,276.82 ns** |  **1.00** |    **0.00** |
| DetectByBroadAndNarrowPhase | 2000 |          4 |  2,951,152.1 ns |     6,589.83 ns |     5,841.71 ns |  0.25 |    0.00 |
|                             |      |            |                 |                 |                 |       |         |
|               **DetectByForce** | **2000** |         **10** | **11,960,062.8 ns** |   **139,298.84 ns** |   **123,484.79 ns** |  **1.00** |    **0.00** |
| DetectByBroadAndNarrowPhase | 2000 |         10 |  6,482,135.9 ns |    66,642.99 ns |    59,077.27 ns |  0.54 |    0.01 |
|                             |      |            |                 |                 |                 |       |         |
|               **DetectByForce** | **5000** |          **4** | **76,871,325.3 ns** | **1,448,520.83 ns** | **2,212,044.30 ns** |  **1.00** |    **0.00** |
| DetectByBroadAndNarrowPhase | 5000 |          4 | 17,417,697.0 ns |   345,910.92 ns |   370,120.81 ns |  0.23 |    0.01 |
|                             |      |            |                 |                 |                 |       |         |
|               **DetectByForce** | **5000** |         **10** | **78,613,654.8 ns** | **1,564,896.17 ns** | **2,861,501.13 ns** |  **1.00** |    **0.00** |
| DetectByBroadAndNarrowPhase | 5000 |         10 | 39,946,127.7 ns |   779,514.88 ns | 1,385,586.91 ns |  0.51 |    0.03 |
