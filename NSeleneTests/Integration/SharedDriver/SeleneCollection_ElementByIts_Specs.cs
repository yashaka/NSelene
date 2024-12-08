namespace NSelene.Tests.Integration.SharedDriver.SeleneCollectionSpec
{
    [TestFixture]
    public class SeleneCollection_ElementByIts_Specs : BaseTest
    {
        [Test]
        public void SelectsProperElementByItsInnerLocatedElementMatchingCondition()
        {
            Given.OpenedEmptyPage();
            When.WithBody(@"
                <ul>Hello to:
                    <li><label>Miller</label><a href='#found-miller'>Find</a></li>
                    <li><label>Julie Mao</label><a href='#found-mao'>Find</a></li>
                </ul>"
            );

            SS("li").ElementByIts("label", Have.Text("Mao")).Element("a").Click();

            Should(Have.UrlContaining("#found-mao"));
        }
    }
}

