using NUnit.Framework;
using static NSelene.Selene;
using NSelene.Tests.Integration.SharedDriver.Harness;

namespace NSelene.Tests.Integration.SharedDriver
{
    [TestFixture]
    public class SeleneElementActions_Specs : BaseTest
    {
        //TODO: consider not using shoulds here...

        //TODO: check "waiting InDom/Visible" aspects 

        [Test]
        public void SElementClear()
        {
            Given.OpenedPageWithBody("<input type='text' value='ku ku'/>");
            S("input").Clear().Should(Be.Blank);
        }
    }
}
