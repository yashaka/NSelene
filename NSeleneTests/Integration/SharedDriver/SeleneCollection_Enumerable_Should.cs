using NSelene;
using NSelene.Tests.Integration.SharedDriver.Harness;
using static NSelene.Selene;
using NUnit.Framework;
using System.Linq;

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
        public void SeleneCollection_Enumerable_ShouldReturnFirst()
        {
            Given.OpenedEmptyPage();
            SeleneCollection elements = SS("#will-exist>input");

            When.WithBody(@"
                <p id='will-exist'>
                    <input id='ask' type='submit' value='How r u?' style='display:none'></input>
                    <input id='answer1' type='submit' value='Good!'></input>
                    <input id='answer2' type='submit' value='Great!'></input>
                </p>"
            );
            var element = elements.ActualWebElements.First();

            Assert.IsFalse(element.Displayed);
            Assert.AreEqual("input", element.TagName);
            Assert.IsEmpty(element.Text);
            Assert.AreEqual("ask", element.GetAttribute("id"));
        }
    }
}