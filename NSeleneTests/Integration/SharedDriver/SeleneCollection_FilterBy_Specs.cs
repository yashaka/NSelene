using NUnit.Framework;
using static NSelene.Selene;
using OpenQA.Selenium;

namespace NSelene.Tests.Integration.SharedDriver.SeleneCollectionSpec
{
    using Harness;

    [TestFixture]
    public class SeleneCollection_FilterBy_Specs : BaseTest
    {

        [TearDown]
        public void TeardownTest()
        {
            Configuration.Timeout = 4;
        }
        
        [Test]
        public void NotStartSearch_OnCreation()
        {
            var nonExistingCollection = SS(".will-exist").FilterBy(Be.Visible);
            Assert.IsNotEmpty(nonExistingCollection.ToString()); 
        }

        [Test]
        public void PostponeSearch_UntilActualActionLikeQuestioiningCount()
        {
            Given.OpenedEmptyPage();
            var elements = SS("li").FilterBy(Be.Visible);
            When.WithBody(@"
                <ul>Hello to:
                    <li class='will-appear' style='display:none'>Bob</li>
                    <li class='will-appear'>Kate</li>
                </ul>"
            ); 
            Assert.AreEqual(1, elements.Count);
        }

        [Test]
        public void UpdateSearch_OnNextActualActionLikeQuestioiningCount()
        {
            Given.OpenedEmptyPage();
            var elements = SS("li").FilterBy(Be.Visible);
            When.WithBody(@"
                <ul>Hello to:
                    <li class='will-appear' style='display:none'>Miller</li>
                    <li class='will-appear' style='display:none'>Julie Mao</li>
                </ul>"
            );
            Assert.AreEqual(0, elements.Count);
            Selene.ExecuteScript(@"
                document.getElementsByTagName('li')[0].style = 'display:block';"
            );
            Assert.AreEqual(1, elements.Count);
        }

        [Test]
        public void WaitNothing()
        {
            Given.OpenedEmptyPage();
            var elements = SS("li").FilterBy(Be.Visible);
            When.WithBodyTimedOut(@"
                <ul>Hello to:
                    <li class='will-appear' style='display:none'>Miller</li>
                    <li class='will-appear' style='display:none'>Julie Mao</li>
                </ul>"
                , 
                500
            );
            When.ExecuteScriptWithTimeout(@"
                document.getElementsByTagName('a')[1].style = 'display:block';
                ", 1000
            );
            Assert.AreEqual(0, elements.Count);
        }
    }
}

