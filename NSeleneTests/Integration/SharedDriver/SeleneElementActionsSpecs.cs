using NUnit.Framework;
using static NSelene.Selene;

namespace NSelene.Tests.Integration.SharedDriver
{
    using Harness;

    [TestFixture]
    public class SeleneElementActionsSpecs : BaseTest
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
