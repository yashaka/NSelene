using NSelene.Tests.Integration.SharedDriver.Harness.Constraints;

namespace NSelene.Tests.Integration.SharedDriver.Harness
{
    internal class Does : NUnit.Framework.Does
    {
        internal static PassWithinConstraint PassBefore(TimeSpan maximumDelay)
        {
            return new PassWithinConstraint(null, BaseTest.DefaultTimeoutSpan);
        }
        internal static PassWithinConstraint PassWithin(TimeSpan minimumDelay, TimeSpan maximumDelay)
        {
            return new PassWithinConstraint(minimumDelay, maximumDelay);
        }

        internal static TimeoutConstraint Timeout(string errorMessage)
        {
            return new TimeoutConstraint(errorMessage);
        }
    }
}
