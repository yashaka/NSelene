using NUnit.Framework;
using static NSelene.Selene;

namespace NSelene.Tests.Integration.SharedDriver.SeleneSpec
{
    using Harness;

    [TestFixture]
    public class SeleneElement_Click_Specs : BaseTest
    {
        [Test]
        public void Click_WaitsForVisibility_OfInitiialyAbsent()
        {
            Configuration.Timeout = 0.6;
            Configuration.PollDuringWaits = 0.1;
            Given.OpenedEmptyPage();
            Given.OpenedPageWithBodyTimedOut(
                @"
                <a href='#second'>go to Heading 2</a>
                <h2 id='second'>Heading 2</h2>
                ",
                300
            );

            S("a").Click();

            Assert.IsTrue(Configuration.Driver.Url.Contains("second"));
        }

        [Test]
        public void Click_WaitsForVisibility_OfInitiialyHidden()
        {
            Configuration.Timeout = 0.6;
            Configuration.PollDuringWaits = 0.1;
            Given.OpenedPageWithBody(
                @"
                <a id='link' href='#second' style='display:none'>go to Heading 2</a>
                <h2 id='second'>Heading 2</h2>
                "
            );
            Given.ExecuteScriptWithTimeout(
                @"
                document.getElementById('link').style.display = 'block';
                ",
                300
            );

            S("a").Click();

            Assert.IsTrue(Configuration.Driver.Url.Contains("second"));
        }

        [Test]
        public void Click_Waits_For_NoOverlay()
        {
            Configuration.Timeout = 0.6;
            Configuration.PollDuringWaits = 0.1;
            Given.OpenedPageWithBody(
                @"
                <div 
                    id='overlay' 
                    style='
                        display:block;
                        position: fixed;
                        display: block;
                        width: 100%;
                        height: 100%;
                        top: 0;
                        left: 0;
                        right: 0;
                        bottom: 0;
                        background-color: rgba(0,0,0,0.1);
                        z-index: 2;
                        cursor: pointer;
                    '
                >
                </div>

                <a id='link' href='#second' style='display:block'>go to Heading 2</a>
                <h2 id='second'>Heading 2</h2>
                "
            );
            Given.ExecuteScriptWithTimeout(
                @"
                document.getElementById('overlay').style.display = 'none';
                ",
                300
            );

            S("a").Click();

            Assert.IsTrue(Configuration.Driver.Url.Contains("second"));
        }
    }
}

