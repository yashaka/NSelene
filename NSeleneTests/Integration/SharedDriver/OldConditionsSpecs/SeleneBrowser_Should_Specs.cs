
namespace NSelene.Tests.Integration.SharedDriver.OldConditionsSpecs
{
    public class SeleneBrowser_Should_OldStyleConditions_Specs : BaseTest
    {
        // TODO: improve coverage and consider breaking down into separate test classes

        [Test]
        public void SeleneWaitTo_HaveJsReturned_WaitsForPresenceInDom_OfInitialyAbsent()
        {
            Given.OpenedEmptyPage();
            Given.OpenedPageWithBodyTimedOut(
                @"
                <p style='display:none'>a</p>
                <p style='display:none'>b</p>
                ",
                ShortTimeoutSpan
            );

            var act = () =>
            {
                Selene.WaitTo(Have.JSReturnedTrue(
                    @"
                    var expectedCount = arguments[0]
                    return document.getElementsByTagName('p').length == expectedCount
                    ",
                    2
                ));
            };

            Assert.That(act, Does.PassWithin(ShortTimeoutSpan, DefaultTimeoutSpan));
        }

        [Test]
        public void SeleneWaitTo_HaveNoJsReturned_WaitsForAbsenceInDom_OfInitialyPresent()
        {
            Given.OpenedPageWithBody(
                @"
                <p style='display:none'>a</p>
                <p style='display:none'>b</p>
                "
            );
            Selene.SS("p").Should(Have.Count(2));
            Given.WithBodyTimedOut(
                @"
                ",
                ShortTimeoutSpan
            );

            var act = () =>
            {
                Selene.WaitTo(Have.No.JSReturnedTrue(
                    @"
                    var expectedCount = arguments[0]
                    return document.getElementsByTagName('p').length == expectedCount
                    ",
                    2
                ));
            };

            Assert.That(act, Does.PassWithin(ShortTimeoutSpan, DefaultTimeoutSpan));
            Assert.That(Configuration.Driver.FindElements(By.TagName("p")), Is.Empty);
        }

        [Test]
        public void SeleneWaitTo_HaveJsReturned_IsRenderedInError_OnAbsentElementTimeoutFailure()
        {
            Configuration.Timeout = BaseTest.ShortTimeout;
            Given.OpenedEmptyPage();

            var act = () =>
            {
                Selene.WaitTo(Have.JSReturnedTrue(
                    @"
                    var expectedCount = arguments[0]
                    return document.getElementsByTagName('p').length == expectedCount
                    ",
                    2
                ));
            };

            Assert.That(act, Does.Timeout($$"""
                OpenQA.Selenium.Chrome.ChromeDriver.Should(JSReturnedTrue)
                Reason:
                    actual: False
                """,
                after: Configuration.Timeout
            ));
        }

        [Test]
        public void SeleneWaitTo_HaveNoJsReturned_IsRenderedInError_OnInDomElementsTimeoutFailure()
        {
            Configuration.Timeout = BaseTest.ShortTimeout;
            Given.OpenedPageWithBody(
                @"
                <p style='display:none'>a</p>
                <p style='display:none'>b</p>
                "
            );

            var act = () =>
            {
                Selene.WaitTo(Have.No.JSReturnedTrue(
                    @"
                    var expectedCount = arguments[0]
                    return document.getElementsByTagName('p').length == expectedCount
                    ",
                    2
                ));
            };

            Assert.That(act, Does.Timeout($$"""
                OpenQA.Selenium.Chrome.ChromeDriver.Should(Not.JSReturnedTrue)
                Reason:
                    condition not matched
                """,
                after: Configuration.Timeout
            ));
        }

        [Test]
        [Ignore("NOT RELEVANT")]
        public void SeleneWaitTo_HaveNoJsReturned_WaitsForAsked_OfInitialyOtherResult()
        {
        }
    }
}

