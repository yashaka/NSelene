using NUnit.Framework;

[assembly: LevelOfParallelism(2)]
[assembly: Parallelizable(ParallelScope.Fixtures)]