using NUnit.Framework;

[assembly: LevelOfParallelism(1)]
[assembly: Parallelizable(ParallelScope.Fixtures)]