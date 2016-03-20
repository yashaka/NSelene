using System;
using NSelene;
using static NSelene.Utils;
using NUnit.Framework;

namespace NSeleneTests.WithManagedBrowserBeforAndAfterAllTests
{
    [TestFixture]
    public class SElementActionsTests
    {
        //TODO: consider not using shoulds here...

        [Test]
        public void SElementClear()
        {
            Given.OpenedPageWithBody("<input type='text' value='ku ku'/>");
            S("input").Clear().Should(Be.Blank());
        }

        // TODO: TBD
    }

}

