using NUnit.Framework;
using static NSelene.Selene;

namespace NSelene.Tests.Integration.SharedDriver.SeleneElementSpec
{
    using Harness;

    [TestFixture]
    public class SeleneElement_FindAll_Specs : BaseTest
    {

        [TearDown]
        public void TeardownTest()
        {
            Configuration.Timeout = 4;
        }
        
        [Test]
        public void NotStartOnCreation()
        {
            Given.OpenedPageWithBody("<p id='#existing'>Hello!</p>");
            var nonExistingCollection = S("#existing").FindAll(".also-not-existing");
            Assert.IsNotEmpty(nonExistingCollection.ToString());  
            var nonExistingCollection2 = S("#not-existing").FindAll(".also-not-existing"); 
            Assert.IsNotEmpty(nonExistingCollection2.ToString()); 
        }

        [Test]
        public void PostponeSearch_UntilActualActionLikeQuestioiningCount()
        {
            Given.OpenedEmptyPage();
            var elements = S("div").FindAll("li");
            When.WithBody(@"
                <div>
                    <ul>Hello to:
                        <li class='will-appear'>Bob</li>
                        <li class='will-appear'>Kate</li>
                    </ul>
                </div>"
            ); //TODO: consider simplifying example via removing div and using ul instead
            Assert.AreEqual(2, elements.Count);
        }

        [Test]
        public void UpdatedSearch_OnNextActualActionLikeQuestioiningCount()
        {
            Given.OpenedEmptyPage();
            var elements = S("div").FindAll(".will-appear");
            When.WithBody(@"
                <div>
                    <ul>Hello to:
                        <li class='will-appear'>Bob</li>
                        <li class='will-appear'>Kate</li>
                    </ul>
                </div>"
            );
            Assert.AreEqual(2, elements.Count);
            When.WithBody(@"
                <div>
                    <ul>Hello to:
                        <li class='will-appear'>Bob</li>
                        <li class='will-appear'>Kate</li>
                        <li class='will-appear'>Joe</li>
                    </ul>
                </div>"
            );
            Assert.AreEqual(3, elements.Count);
        }

        [Test]
        public void WaitNothing()
        {
            Given.OpenedEmptyPage();
            var elements = S("div").FindAll(".will-appear");
            When.WithBody(@"
                <div>
                    <ul>Hello to:
                        <li class='will-appear'>Bob</li>
                        <li class='will-appear' style='display:none'>Kate</li>
                    </ul>
                </div>"
            );
            When.WithBodyTimedOut(@"
                <div>
                    <ul>Hello to:
                        <li class='will-appear'>Bob</li>
                        <li class='will-appear' style='display:none'>Kate</li>
                        <li class='will-appear'>Bobik</li>
                    </ul>
                </div>",
                500
            );
            Assert.AreEqual(2, elements.Count);
        }
    }
}

