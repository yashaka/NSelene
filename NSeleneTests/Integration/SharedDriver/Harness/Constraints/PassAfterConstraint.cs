using NUnit.Framework.Constraints;

namespace NSelene.Tests.Integration.SharedDriver.Harness.Constraints
{
    internal class PassAfterConstraint(TimeSpan? delayBeforePass) : Constraint
    {
        private readonly TimeSpan _delayBeforePass = delayBeforePass ?? TimeSpan.Zero;

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
            var elapsedTimeLimit = TimeSpan.FromSeconds(Configuration.Timeout);

            return new ConstraintResult(this, elapsedTime, elapsedTime < elapsedTimeLimit && elapsedTime >= _delayBeforePass);
        }

        public override string Description => $"Should not timeout" + (_delayBeforePass == TimeSpan.Zero ? "" : $" or pass in less then {_delayBeforePass}");
    }
}
