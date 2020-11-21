``` ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19041.630 (2004/?/20H1)
Intel Core i7-8700K CPU 3.70GHz (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
.NET Core SDK=5.0.100
  [Host]     : .NET Core 3.1.9 (CoreCLR 4.700.20.47201, CoreFX 4.700.20.47203), X64 RyuJIT
  DefaultJob : .NET Core 3.1.9 (CoreCLR 4.700.20.47201, CoreFX 4.700.20.47203), X64 RyuJIT


```
|                  Method |    N | MeanRadius |            Mean |         Error |          StdDev |          Median | Ratio | RatioSD |
|------------------------ |----- |----------- |----------------:|--------------:|----------------:|----------------:|------:|--------:|
| **CollideTest_ForceDetect** |   **10** |          **4** |        **350.4 ns** |       **1.18 ns** |         **1.10 ns** |        **350.6 ns** |  **1.00** |    **0.00** |
|  CollideTest_BroadPhase |   10 |          4 |      1,385.2 ns |       8.23 ns |         6.87 ns |      1,383.2 ns |  3.95 |    0.03 |
|                         |      |            |                 |               |                 |                 |       |         |
| **CollideTest_ForceDetect** |   **10** |         **10** |        **332.8 ns** |       **2.67 ns** |         **2.37 ns** |        **332.4 ns** |  **1.00** |    **0.00** |
|  CollideTest_BroadPhase |   10 |         10 |      1,517.6 ns |      10.49 ns |         9.30 ns |      1,513.6 ns |  4.56 |    0.05 |
|                         |      |            |                 |               |                 |                 |       |         |
| **CollideTest_ForceDetect** |  **100** |          **4** |     **30,819.8 ns** |      **90.01 ns** |        **79.79 ns** |     **30,837.0 ns** |  **1.00** |    **0.00** |
|  CollideTest_BroadPhase |  100 |          4 |     22,934.6 ns |     136.97 ns |       121.42 ns |     22,891.9 ns |  0.74 |    0.00 |
|                         |      |            |                 |               |                 |                 |       |         |
| **CollideTest_ForceDetect** |  **100** |         **10** |     **32,139.5 ns** |     **111.03 ns** |        **92.72 ns** |     **32,150.4 ns** |  **1.00** |    **0.00** |
|  CollideTest_BroadPhase |  100 |         10 |     32,300.8 ns |     152.47 ns |       135.16 ns |     32,285.7 ns |  1.01 |    0.00 |
|                         |      |            |                 |               |                 |                 |       |         |
| **CollideTest_ForceDetect** | **2000** |          **4** | **11,818,694.5 ns** |  **56,064.80 ns** |    **46,816.65 ns** | **11,802,226.6 ns** |  **1.00** |    **0.00** |
|  CollideTest_BroadPhase | 2000 |          4 |  3,264,494.3 ns |  17,660.31 ns |    16,519.46 ns |  3,267,344.9 ns |  0.28 |    0.00 |
|                         |      |            |                 |               |                 |                 |       |         |
| **CollideTest_ForceDetect** | **2000** |         **10** | **12,284,982.9 ns** |  **57,439.60 ns** |    **53,729.04 ns** | **12,264,982.8 ns** |  **1.00** |    **0.00** |
|  CollideTest_BroadPhase | 2000 |         10 |  6,981,901.6 ns |  38,407.17 ns |    32,071.72 ns |  6,975,121.9 ns |  0.57 |    0.00 |
|                         |      |            |                 |               |                 |                 |       |         |
| **CollideTest_ForceDetect** | **5000** |          **4** | **78,964,490.1 ns** | **497,338.36 ns** |   **415,300.05 ns** | **78,910,242.9 ns** |  **1.00** |    **0.00** |
|  CollideTest_BroadPhase | 5000 |          4 | 20,250,955.4 ns | 245,621.95 ns |   229,754.92 ns | 20,214,912.5 ns |  0.26 |    0.00 |
|                         |      |            |                 |               |                 |                 |       |         |
| **CollideTest_ForceDetect** | **5000** |         **10** | **80,952,501.0 ns** | **317,845.87 ns** |   **281,762.08 ns** | **80,877,214.3 ns** |  **1.00** |    **0.00** |
|  CollideTest_BroadPhase | 5000 |         10 | 40,344,317.9 ns | 890,059.34 ns | 2,610,389.96 ns | 39,582,336.4 ns |  0.55 |    0.03 |
