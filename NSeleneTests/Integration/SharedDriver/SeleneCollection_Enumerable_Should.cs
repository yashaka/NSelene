using NSelene;
using NSelene.Tests.Integration.SharedDriver.Harness;
using static NSelene.Selene;
using NUnit.Framework;
using System.Linq;
using System;

namespace NSeleneTests.Integration.SharedDriver.SeleneCollectionSpec
{
    [TestFixture]
    public class SeleneCollection_Enumerable_Should : BaseTest
    {

        [TearDown]
        public void TeardownTest()
        {
            Configuration.Timeout = 4;
        }

        [Test]
        public void ReturnFirst()
        {
            Given.OpenedPageWithBody(@"
                <p id='will-exist'>
                    <input id='ask' type='submit' value='How r u?' style='display:none'></input>
                    <input id='answer1' type='submit' value='Good!'></input>
                    <input id='answer2' type='submit' value='Great!'></input>
                </p>"
            );

            SeleneCollection elements = SS("#will-exist>input");

            var element = elements.ActualWebElements.First();

            Assert.AreEqual("input", element.TagName);
        }

        [Test]
        public void ReturnDefaultFirst()
        {
            Given.OpenedPageWithBody(@"
                <p id='will-exist'>
                    <input id='ask' type='submit' value='How r u?' style='display:none'></input>
                    <input id='answer1' type='submit' value='Good!'></input>
                    <input id='answer2' type='submit' value='Great!'></input>
                </p>"
            );

            SeleneCollection elements = SS("div");

            var element = elements.ActualWebElements.FirstOrDefault();

            Assert.IsNull(element);
        }

        [Test]
        public void ReturnLast()
        {
            Given.OpenedPageWithBody(@"
                <ul>
                    <li class='item'>a</li>
                    <li class='item'>b</li>
                    <li class='item'>c</li>
                </ul>"
            );

            SeleneCollection elements = SS(".item");

            var element = elements.ActualWebElements.Last();

            Assert.AreEqual("c", element.Text);
        }

        [Test]
        public void ReturnDefaultLast()
        {
            Given.OpenedPageWithBody(@"
                <ul>
                    <li class='item'>a</li>
                    <li class='item'>b</li>
                    <li class='item'>c</li>
                </ul>"
            );

            SeleneCollection elements = SS("div");

            var element = elements.ActualWebElements.LastOrDefault();

            Assert.IsNull(element);
        }

        [Test]
        public void ReturnSingle()
        {
            Given.OpenedPageWithBody(@"
                <ul>
                    <li class='item'>a</li>
                    <li class='item'>b</li>
                    <li class='item'>c</li>
                </ul>"
            );

            SeleneCollection elements = SS(".item");

            var element = elements.ActualWebElements.Single(e => e.Text == "c");

            Assert.AreEqual("c", element.Text);
        }

        [Test]
        public void ReturnSingleThrowException()
        {
            Given.OpenedPageWithBody(@"
                <ul>
                    <li class='item'>c</li>
                    <li class='item'>c</li>
                </ul>"
            );

            SeleneCollection elements = SS(".item");

            Assert.Throws<InvalidOperationException>(() => elements.ActualWebElements.Single(e => e.Text == "c"));
        }

        [Test]
        public void ReturnDefaultSingle()
        {
            Given.OpenedPageWithBody(@"
                <ul>
                    <li class='item'>a</li>
                    <li class='item'>a</li>
                    <li class='item'>a</li>
                </ul>"
            );

            SeleneCollection elements = SS(".item");

            var element = elements.ActualWebElements.SingleOrDefault(e => e.Text == "b");

            Assert.IsNull(element);
        }

        [Test]
        public void ReturnSelect()
        {
            Given.OpenedPageWithBody(@"
                <ul>
                    <li class='item'>a</li>
                    <li class='item'>b</li>
                </ul>"
            );

            SeleneCollection elements = SS(".item");

            var selectedElements = elements.ActualWebElements.Select(e => e.Text == "b");

            Assert.AreEqual(false, selectedElements.First());
            Assert.AreEqual(true, selectedElements.Last());
        }
    }
}