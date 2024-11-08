namespace NSelene.Tests.Integration.SharedDriver.SeleneElementSpec
{
    [TestFixture]
    public class SeleneCollection_Matching_Specs : BaseTest
    {
        [Test]
        public void AllwaysReturnsBoolWithoutWaiting()
        {
            var beforeCall = DateTime.Now;
            Given.OpenedPageWithBody("<p id='existing'>Hello!</p>");

            // EXPECT
            Assert.That(SS("#absent").Matching(Have.Count(2)), Is.False);
            Assert.That(SS("#absent").Matching(Have.No.Count(2)), Is.True);

            Assert.That(SS("#existing").Matching(Have.Count(1)), Is.True);
            Assert.That(SS("#existing").Matching(Have.No.Count(1)), Is.False);

            var afterCall = DateTime.Now;
            Assert.That(afterCall, Is.LessThan(beforeCall.AddSeconds(Configuration.Timeout / 2)));
        }
    }
}

