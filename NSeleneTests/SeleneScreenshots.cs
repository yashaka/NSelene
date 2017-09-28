using NSelene;
using NSelene.Support.Extensions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSeleneTests
{
    [TestFixture]
    public class SeleneScreenshots : BaseTest
    {
        [Test]
        public void GetPageScreenshot()
        {
            string url = "google.com";
            SeleneDriver drv = new SeleneDriver();
            drv.GoToUrl(url);
            SeleneDriverExtensions.TakeScreenshot();
        }
    }
}
