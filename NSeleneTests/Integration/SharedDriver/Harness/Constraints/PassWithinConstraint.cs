using NUnit.Framework.Constraints;

namespace NSelene.Tests.Integration.SharedDriver.Harness.Constraints
{
    internal class PassWithinConstraint(TimeSpan? minimumDelay, TimeSpan maximumDelay) : Constraint
    {
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            var beforeCall = DateTime.Now;
            try
            {
                if (actual is Action act)
                {
                    act.Invoke();
                }
                else
                {
                    return new ConstraintResult(this, "Not a System.Action object passed to the constraint", ConstraintStatus.Error);
                }
            }
            catch (TimeoutException ex)
            {
                return new ConstraintResult(this, ex.Message, ConstraintStatus.Failure);
            }

            var elapsedTime = DateTime.Now.Subtract(beforeCall);
            return new ConstraintResult(
                this, 
                elapsedTime, 
                elapsedTime <= maximumDelay && elapsedTime >= (minimumDelay ?? TimeSpan.Zero)
            );
        }

        public override string Description =>
            $"Should not timeout"
            + (
                minimumDelay.HasValue
                ? $" and pass within: {minimumDelay} <= delay <= {maximumDelay}"
                : ""
            );
    }
}
