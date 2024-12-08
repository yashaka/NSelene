namespace NSelene.Tests.Integration.SharedDriver.SeleneElementSpec
{
    [TestFixture]
    public class SeleneElement_With_Specs: BaseTest
    {
        [Test]
        public void CustomTimeoutIsApplied_When_WaitUntil()
        {
            Configuration.Timeout = 0.5;
            var custom = 1.0;
            var beforeCall = DateTime.Now;

            S("#absent").With(timeout: custom).WaitUntil(Be.Visible);
            var afterCall = DateTime.Now;

            Assert.That(afterCall, Is.GreaterThanOrEqualTo(beforeCall.AddSeconds(custom)));
        }
        
        [Test]
        public void CustomTimeoutDoesNotChangeSharedConfiguration()
        {
            Configuration.Timeout = 0.8;

            // WHEN
            S("#absent").With(timeout: 0.2);

            Assert.That(Configuration.Timeout, Is.EqualTo(0.8));

            // WHEN
            var beforeCall = DateTime.Now;
            S("#absent").WaitUntil(Be.Visible);
            var afterCall = DateTime.Now;

            Assert.That(afterCall, Is.GreaterThanOrEqualTo(beforeCall.AddSeconds(0.8)));
        }
        
        [Test]
        public void CustomTimeoutDoesNotChangeAnotherCustomTimeout()
        {
            Configuration.Timeout = 0.4;
            var another = S("#other-absent").With(timeout: 0.8);

            S("#absent").With(timeout: 0.2);
            var beforeCall = DateTime.Now;
            another.WaitUntil(Be.Visible);
            var afterCall = DateTime.Now;

            Assert.That(afterCall, Is.GreaterThanOrEqualTo(beforeCall.AddSeconds(0.8)));
        }
        
        [Test]
        public void CustomConfigUseLatestSharedSettingsOnInit()
        {
            // default polling is small, let's increase it ...
            var anotherPollingForSharedConfig = 0.6;
            Configuration.PollDuringWaits = anotherPollingForSharedConfig;

            // ... to make waiting even longer than smaller custom timeout
            var beforeCall = DateTime.Now;
            S("#absent").With(timeout: 0.2).WaitUntil(Be.Visible);
            var afterCall = DateTime.Now;

            Assert.That(afterCall, Is.GreaterThanOrEqualTo(beforeCall.AddSeconds(0.6)));
        }
        
        [Test]
        public void CustomConfigDynamicallyReuseSharedSettingsNotOverridenOnInit()
        {
            // GIVEN initialized with latest custom shared setting
            Configuration.PollDuringWaits = 0.3;
            var customized = S("#absent").With(timeout: 0.2);

            // WHEN shared setting updated one more time
            Configuration.PollDuringWaits = 0.6;
            var beforeCall = DateTime.Now;
            customized.WaitUntil(Be.Visible);
            var elapsedTime = DateTime.Now-beforeCall;

            // THEN the updated value is used making waiting longer correspondingly 
            Assert.That(elapsedTime, Is.GreaterThanOrEqualTo(TimeSpan.FromSeconds(0.6)));
        }
        
        [Test]
        public void CustomConfigCanBeFullyDisconnectedFromShared()
        { // TODO: ensure same behavior for Configuration.Driver! it might be different!
            // GIVEN overriden with fresh new Config with custom timeout
            Configuration.PollDuringWaits = 1.0;
            var customized = S("#absent")._With_(Configuration._New_(timeout: 0.2));

            // WHEN shared setting even updated one more time
            Configuration.PollDuringWaits = 2.0;
            var beforeCall = DateTime.Now;
            customized.WaitUntil(Be.Visible);
            var afterCall = DateTime.Now;

            // THEN the default 0.1 polling value is used making waiting shorter correspondingly 
            Assert.That(afterCall, Is.GreaterThanOrEqualTo(beforeCall.AddSeconds(0.2)));
            Assert.That(afterCall, Is.LessThan(beforeCall.AddSeconds(1.0)));
        }
        
        [Test]
        public void CustomizingComplexCollectionElementYetSharedOverride()
        {
            // GIVEN overriden twice for collection element
            Configuration.PollDuringWaits = 1.0;
            var customizedCollection 
            = SS(".absent").With(timeout: 0.2);
            var customized 
            = customizedCollection.ElementBy(Be.Visible).With(timeout: 0.5);

            // WHEN shared setting updated one more time
            Configuration.PollDuringWaits = 0.3;
            var beforeCall = DateTime.Now;
            customized.WaitUntil(Be.Visible);
            var afterCall = DateTime.Now;

            // THEN 
            Assert.That(afterCall, Is.GreaterThanOrEqualTo(beforeCall.AddSeconds(0.6)));
            Assert.That(afterCall, Is.LessThan(beforeCall.AddSeconds(1.0)));
        }
        
        [Test]
        public void CustomizingComplexCollectionElementYetCollectionOverride()
        {
            // GIVEN overriden twice for collection element
            Configuration.PollDuringWaits = 1.0;
            var customizedCollection 
            = SS(".absent").With(timeout: 0.2, pollDuringWaits: 0.3);
            var customized 
            = customizedCollection.ElementBy(Be.Visible).With(timeout: 0.5);

            // WHEN
            var beforeCall = DateTime.Now;
            customized.WaitUntil(Be.Visible);
            var afterCall = DateTime.Now;

            // THEN 
            Assert.That(afterCall, Is.GreaterThanOrEqualTo(beforeCall.AddSeconds(0.6)));
            Assert.That(afterCall, Is.LessThan(beforeCall.AddSeconds(1.0)));
        }
    }
}

