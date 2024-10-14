global using Does = NSelene.Tests.Integration.SharedDriver.Harness.Does;
global using NSelene.Conditions;
global using NSelene.Tests.Integration.SharedDriver.Harness;
global using NSelene;
global using NUnit.Framework;
global using OpenQA.Selenium.Chrome;
global using OpenQA.Selenium;
global using System.Linq;
global using System;
global using static NSelene.Selene;

[assembly: LevelOfParallelism(2)]
[assembly: Parallelizable(ParallelScope.Fixtures)]
[assembly: FixtureLifeCycle(LifeCycle.SingleInstance)]