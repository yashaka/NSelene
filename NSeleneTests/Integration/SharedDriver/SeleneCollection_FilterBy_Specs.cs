namespace NSelene.Tests.Integration.SharedDriver.SeleneCollectionSpec
{
    [TestFixture]
    public class SeleneCollection_FilterBy_Specs : BaseTest
    {
        [Test]
        public void NotStartSearch_OnCreation()
        {
            var nonExistingCollection = SS(".will-exist").By(Be.Visible);
            Assert.That(nonExistingCollection.ToString(), Is.Not.Empty); 
        }

        [Test]
        public void PostponeSearch_UntilActualActionLikeQuestioiningCount()
        {
            Given.OpenedEmptyPage();
            var elements = SS("li").By(Be.Visible);
            When.WithBody(@"
                <ul>Hello to:
                    <li class='will-appear' style='display:none'>Bob</li>
                    <li class='will-appear'>Kate</li>
                </ul>"
            );
            Assert.That(elements, Has.Count.EqualTo(1));
        }

        [Test]
        public void UpdateSearch_OnNextActualActionLikeQuestioiningCount()
        {
            Given.OpenedEmptyPage();
            var elements = SS("li").By(Be.Visible);
            When.WithBody(@"
                <ul>Hello to:
                    <li class='will-appear' style='display:none'>Miller</li>
                    <li class='will-appear' style='display:none'>Julie Mao</li>
                </ul>"
            );
            Assert.That(elements, Is.Empty);
            Selene.ExecuteScript(@"
                document.getElementsByTagName('li')[0].style = 'display:block';"
            );
            Assert.That(elements, Has.Count);
        }

        [Test]
        public void WaitNothing()
        {
            Given.OpenedEmptyPage();
            var elements = SS("li").By(Be.Visible);
            When.WithBodyTimedOut(@"
                <ul>Hello to:
                    <li class='will-appear' style='display:none'>Miller</li>
                    <li class='will-appear' style='display:none'>Julie Mao</li>
                </ul>"
                ,
                TimeSpan.FromMilliseconds(500)
            );
            When.ExecuteScriptWithTimeout(@"
                document.getElementsByTagName('a')[1].style = 'display:block';
                ",
                TimeSpan.FromMilliseconds(1000)
            );
            Assert.That(elements, Is.Empty);
        }
    }
}

