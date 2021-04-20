using System;
using System.Linq;

namespace NSelene
{
    // TODO: consider refactoring "describing" logic for conditions... 
    namespace Conditions
    {
        public class ConditionNotMatchedException : Exception
        {
            public ConditionNotMatchedException() : base()
            {
            }

            public ConditionNotMatchedException(string message) : base(message)
            {
            }

            public ConditionNotMatchedException(
                string message, 
                Exception innerException
            ) 
            : base(message, innerException)
            {
            }
        }

        // TODO: find the way to DRY conditions:) Generics? keep it simple enough though...

        public abstract class Condition<TEntity> : _Computation<TEntity, object> // object means void here:D 
        {
            public abstract bool Apply(TEntity entity);

            // todo: review and once finalized make public 
            public Condition<TEntity> Not 
                => new Not<TEntity>(this); 

            // OBSOLETE (but we might need to keep it for backwards compatibility)

            public virtual string DescribeActual()
            {
                // TODO: consider providing more "universal" approach... 
                //       because this makes sense only 
                //       in case of failed condition:(
                return false.ToString(); 
            }

            public virtual string DescribeExpected()
            {
                return true.ToString();
            }

            public virtual string Explain()
            {
                var expected = this.DescribeExpected();

                return expected == true.ToString() 
                ? this.GetType().Name 
                : expected;
            }

            public override string ToString()
            {
                //return this.GetType().Name +
                //           "\n    Expected: " + DescribeExpected() +
                //           (DescribeActual() ==  "" ? "" : "\n    " + DescribeActual());
                return this.Explain();
            }

            public virtual _Optionally<object> _Invoke(TEntity entity)
            {
                var result = this.Apply(entity);
                if (!result)
                {
                    throw new ConditionNotMatchedException("actual: " + this.DescribeActual());
                }
                return _Optionally<object>.Undefined;
            }

            // TODO: consider allowing users to define custom conditions with void method
            //       over Invoke with "optional" result
            //       which name then would it have? Match? Pass?
            //       apply we can keep for backwards compatibility... 
        }

        // todo: mark it as obsolete
        [Obsolete("DescribedCondition is obsolete, use Condition class instead")]
        public abstract class DescribedCondition<TEntity> : Condition<TEntity>
        {

        }

    }

}
