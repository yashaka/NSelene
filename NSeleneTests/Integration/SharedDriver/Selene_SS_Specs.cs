namespace NSelene.Tests.Integration.SharedDriver.SeleneSpec
{

    [TestFixture]
    public class Selene_SS_Specs : BaseTest
    {
        [Test]
        public void NotStartOnCreation()
        {
            var nonExistingCollection = SS(".not-existing");
            Assert.That(nonExistingCollection.ToString(), Is.Not.Empty); 
                // TODO: think on improving... actually it does not tell search is not started
                // it would tell if browser is quited at the moment... but it is not...
        }

        [Test]
        public void PostponeTheSearchUntilActualActionLikeQuestioiningCount()
        {
            Given.OpenedEmptyPage();
            var elements = SS(".will-appear");
            When.WithBody(@"
                <ul>Hello to:
                    <li class='will-appear'>Bob</li>
                    <li class='will-appear'>Kate</li>
                </ul>");
            Assert.That(elements, Has.Count.EqualTo(2));
        }

        [Test]
        public void UpdatedSearch_OnNextActualActionLikeQuestioiningCount()
        {
            Given.OpenedEmptyPage();
            var elements = SS(".will-appear");
            When.WithBody(@"
                <ul>Hello to:
                    <li class='will-appear'>Bob</li>
                    <li class='will-appear'>Kate</li>
                </ul>");
            Assert.That(elements, Has.Count.EqualTo(2));
            When.WithBody(@"
                <ul>Hello to:
                    <li class='will-appear'>Bob</li>
                    <li class='will-appear'>Kate</li>
                    <li class='will-appear'>Joe</li>
                </ul>");
            Assert.That(elements, Has.Count.EqualTo(3));
        }

        [Test]
        public void WaitNothing()
        {
            Given.OpenedEmptyPage();
            var elements = SS(".will-appear");
            When.WithBody(@"
                <ul>Hello to:
                    <li class='will-appear'>Bob</li>
                    <li class='will-appear' style='display:none'>Kate</li>
                </ul>"
            );
            When.WithBodyTimedOut(@"
                <ul>Hello to:
                    <li class='will-appear'>Bob</li>
                    <li class='will-appear' style='display:none'>Kate</li>
                    <li class='will-appear'>Bobik</li>
                </ul>",
                TimeSpan.FromMilliseconds(500)
            );
            Assert.That(elements, Has.Count.EqualTo(2));
        }
    }
}

