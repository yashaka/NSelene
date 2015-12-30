using NUnit.Framework;
using System;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium;
using static NSelene.Utils;
using NSeleneExamples.Pages;


namespace NSeleneExamples
{
    namespace TodoMVC
    {
        [TestFixture ()]
        public class Test : BaseTest
        {
            [Test ()]
            public void FilterTasks ()
            {
                Tasks.Visit ();

                Tasks.Add ("a", "b", "c");
                Tasks.ShouldBe ("a", "b", "c");

                Tasks.Toggle ("b"); 

                Tasks.FilterActive ();
                Tasks.ShouldBe ("a", "c");

                Tasks.FilterCompleted ();
                Tasks.ShouldBe ("b");
            }
        }
    }

}
