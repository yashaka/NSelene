using NSelene.Tests.Integration.SharedDriver.Harness.Constraints;

namespace NSelene.Tests.Integration.SharedDriver.Harness
{
    internal class Does : NUnit.Framework.Does
    {
        internal static PassAfterConstraint Pass()
        {
            return new PassAfterConstraint(null);
        }
        internal static PassAfterConstraint PassAfter(TimeSpan delayBeforePass)
        {
            return new PassAfterConstraint(delayBeforePass);
        }

        internal static TimeoutConstraint Timeout(string errorMessage)
        {
            return new TimeoutConstraint(errorMessage);
        }
    }
}
