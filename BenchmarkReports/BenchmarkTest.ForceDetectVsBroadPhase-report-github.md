``` ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19041.630 (2004/?/20H1)
Intel Core i5-8300H CPU 2.30GHz (Coffee Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=5.0.100
  [Host]     : .NET Core 3.1.9 (CoreCLR 4.700.20.47201, CoreFX 4.700.20.47203), X64 RyuJIT
  DefaultJob : .NET Core 3.1.9 (CoreCLR 4.700.20.47201, CoreFX 4.700.20.47203), X64 RyuJIT


```
|                  Method |    N | MeanRadius |       Mean |     Error |    StdDev |     Median |
|------------------------ |----- |----------- |-----------:|----------:|----------:|-----------:|
| **CollideTest_ForceDetect** | **1000** |          **4** |   **4.034 ms** | **0.0131 ms** | **0.0110 ms** |   **4.032 ms** |
|  CollideTest_BroadPhase | 1000 |          4 |   1.314 ms | 0.0112 ms | 0.0099 ms |   1.313 ms |
| **CollideTest_ForceDetect** | **1000** |         **10** |   **4.209 ms** | **0.0305 ms** | **0.0255 ms** |   **4.203 ms** |
|  CollideTest_BroadPhase | 1000 |         10 |   2.369 ms | 0.0062 ms | 0.0055 ms |   2.370 ms |
| **CollideTest_ForceDetect** | **2000** |          **4** |  **16.330 ms** | **0.1873 ms** | **0.1752 ms** |  **16.324 ms** |
|  CollideTest_BroadPhase | 2000 |          4 |   4.231 ms | 0.0195 ms | 0.0163 ms |   4.227 ms |
| **CollideTest_ForceDetect** | **2000** |         **10** |  **16.821 ms** | **0.1339 ms** | **0.1187 ms** |  **16.770 ms** |
|  CollideTest_BroadPhase | 2000 |         10 |   9.229 ms | 0.0453 ms | 0.0378 ms |   9.233 ms |
| **CollideTest_ForceDetect** | **5000** |          **4** |  **99.486 ms** | **1.4754 ms** | **1.3079 ms** |  **99.005 ms** |
|  CollideTest_BroadPhase | 5000 |          4 |  26.970 ms | 0.2664 ms | 0.2361 ms |  27.080 ms |
| **CollideTest_ForceDetect** | **5000** |         **10** | **103.673 ms** | **0.3721 ms** | **0.3107 ms** | **103.631 ms** |
|  CollideTest_BroadPhase | 5000 |         10 |  58.019 ms | 1.4755 ms | 4.3506 ms |  56.578 ms |
