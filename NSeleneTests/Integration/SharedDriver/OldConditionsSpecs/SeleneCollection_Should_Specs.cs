namespace NSelene.Tests.Integration.SharedDriver.OldConditionsSpecs
{
    [TestFixture]
    public class SeleneCollection_Should_OldStyleConditions_Specs : BaseTest
    {
        // TODO: improve coverage and consider breaking down into separate test classes

        [Test]
        public void Should_HaveCount_WaitsForPresenceInDom_OfInitialyAbsent()
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
                SS("p").Should(Have.Count(2));
            };

            Assert.That(act, Does.PassAfter(ShortTimeoutSpan));
        }

        [Test]
        public void Should_HaveNoCount_WaitsForAbsenceInDom_OfInitialyPresent()
        {
            Given.OpenedPageWithBody(
                @"
                <p style='display:none'>a</p>
                <p style='display:none'>b</p>
                "
            );
            Given.WithBodyTimedOut(
                @"
                ",
                ShortTimeoutSpan
            );

            var act = () =>
            {
                SS("p").Should(Have.No.Count(2));
            };

            Assert.That(act, Does.PassAfter(ShortTimeoutSpan));
        }
        
        [Test]
        public void Should_HaveCount_IsRenderedInError_OnAbsentElementTimeoutFailure()
        {
            Configuration.Timeout = BaseTest.ShortTimeout;
            Given.OpenedEmptyPage();

            var act = () => {
                SS("p").Should(Have.Count(2));
            };

            Assert.That(act, Does.Timeout($$"""
                Browser.All(p).Should(count = 2)
                Reason:
                    actual: count = 0
                """));
        }
        
        [Test]
        public void Should_HaveNoCount_IsRenderedInError_OnInDomElementsTimeoutFailure()
        {
            Configuration.Timeout = BaseTest.ShortTimeout;
            Given.OpenedPageWithBody(
                @"
                <p style='display:none'>a</p>
                <p style='display:none'>b</p>
                "
            );

            var act = () => {
                SS("p").Should(Have.No.Count(2));
            };

            Assert.That(act, Does.Timeout($$"""
                Browser.All(p).Should(Not.count = 2)
                Reason:
                    condition not matched
                """));
        }

        [Test]
        public void Should_HaveCount_WaitsForAsked_OfInitialyOtherVisibleCount()
        {
            Given.OpenedPageWithBody(
                @"
                <p>a</p>
                "
            );
            Given.OpenedPageWithBodyTimedOut(
                @"
                <p>a</p>
                <p>b</p>
                ",
                ShortTimeoutSpan
            );

            var act = () =>
            {
                SS("p").Should(Have.Count(2));
            };

            Assert.That(act, Does.PassAfter(ShortTimeoutSpan));
        }
    }
}

