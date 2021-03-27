using NUnit.Framework;

[assembly: LevelOfParallelism(2)]
[assembly: Parallelizable(ParallelScope.Fixtures)]
[assembly: FixtureLifeCycle(LifeCycle.SingleInstance)]