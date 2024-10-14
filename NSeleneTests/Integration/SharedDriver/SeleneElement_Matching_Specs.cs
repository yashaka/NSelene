namespace NSelene.Tests.Integration.SharedDriver.SeleneElementSpec
{
    [TestFixture]
    public class SeleneElement_Matching_Specs : BaseTest
    {
        [Test]
        public void AllwaysReturnsBoolWithoutWaiting()
        {
            var beforeCall = DateTime.Now;
            Given.OpenedPageWithBody("<p id='existing'>Hello!</p>");

            // EXPECT
            Assert.That(S("#absent").Matching(Be.Visible), Is.False);
            Assert.That(S("#absent").Matching(Be.Not.Visible), Is.True);

            Assert.That(S("#existing").Matching(Be.Visible), Is.True);
            Assert.That(S("#existing").Matching(Be.Not.Visible), Is.False);

            var elapsedTime = DateTime.Now-beforeCall;
            Assert.That(elapsedTime, Is.LessThan(TimeSpan.FromSeconds(Configuration.Timeout / 2)));
        }
    }
}

