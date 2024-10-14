namespace NSelene.Tests.Integration.SharedDriver.SeleneElementSpec
{
    [TestFixture]
    public class SeleneElement_FindAll_Specs : BaseTest
    {
        [Test]
        public void NotStartOnCreation()
        {
            Given.OpenedPageWithBody("<p id='#existing'>Hello!</p>");
            var nonExistingCollection = S("#existing").All(".also-not-existing");
            Assert.That(nonExistingCollection.ToString(), Is.Not.Empty);  
            var nonExistingCollection2 = S("#not-existing").All(".also-not-existing");
            Assert.That(nonExistingCollection2.ToString(), Is.Not.Empty); 
        }

        [Test]
        public void PostponeSearch_UntilActualActionLikeQuestioiningCount()
        {
            Given.OpenedEmptyPage();
            var elements = S("div").All("li");
            When.WithBody(@"
                <div>
                    <ul>Hello to:
                        <li class='will-appear'>Bob</li>
                        <li class='will-appear'>Kate</li>
                    </ul>
                </div>"
            ); //TODO: consider simplifying example via removing div and using ul instead
            Assert.That(elements, Has.Count.EqualTo(2));
        }

        [Test]
        public void UpdatedSearch_OnNextActualActionLikeQuestioiningCount()
        {
            Given.OpenedEmptyPage();
            var elements = S("div").All(".will-appear");
            When.WithBody(@"
                <div>
                    <ul>Hello to:
                        <li class='will-appear'>Bob</li>
                        <li class='will-appear'>Kate</li>
                    </ul>
                </div>"
            );
            Assert.That(elements, Has.Count.EqualTo(2));
            When.WithBody(@"
                <div>
                    <ul>Hello to:
                        <li class='will-appear'>Bob</li>
                        <li class='will-appear'>Kate</li>
                        <li class='will-appear'>Joe</li>
                    </ul>
                </div>"
            );
            Assert.That(elements, Has.Count.EqualTo(3));
        }

        [Test]
        public void WaitNothing()
        {
            Given.OpenedEmptyPage();
            var elements = S("div").All(".will-appear");
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
                TimeSpan.FromMilliseconds(500)
            );
            Assert.That(elements, Has.Count.EqualTo(2));
        }
    }
}

