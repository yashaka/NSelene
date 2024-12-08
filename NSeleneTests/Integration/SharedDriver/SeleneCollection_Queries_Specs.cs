namespace NSelene.Tests.Integration.SharedDriver.SeleneCollectionQueriesSpecs
{
    [TestFixture]
    public class SeleneCollection_Count_Specs : BaseTest
    {
        [Test]
        public void CountInvisible()
        {
            Given.OpenedEmptyPage();
            var elements = SS(".will-appear");
            When.WithBody(@"
                <p>
                    <ul>Hello to:
                        <li class='will-appear'>Bob</li>
                        <li class='will-appear' style='display:none'>Kate</li>
                    </ul>
                </p>"
            );
            Assert.That(elements, Has.Count.EqualTo(2));
        }
    }
}

