using System.Linq;

namespace NSelene
{
    // TODO: consider refactoring "describing" logic for conditions... 
    namespace Conditions
    {

        // TODO: find the way to DRY conditions:) Generics? keep it simple enough though...

        public abstract class DescribedResult<TEntity>
        {

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

            /*
             * TODO: think on better names:)
             */
            public virtual string Explain()
            {
                var expected = this.DescribeExpected();
                return expected == true.ToString() ? this.GetType().Name 
                    : expected;
            }

            public override string ToString()
            {
                //return this.GetType().Name +
                //           "\n    Expected: " + DescribeExpected() +
                //           (DescribeActual() ==  "" ? "" : "\n    " + DescribeActual());
                return string.Format("{0}" +
                    // "\n  Expected : {1}" +
                    "\n       Actual: {1}\n", 
                    this.Explain(), 
                    // this.DescribeExpected(), 
                    this.DescribeActual());
            }
        }

        public abstract class Condition<TEntity> : DescribedResult<TEntity>
        {

            public abstract bool Apply(TEntity entity);

            // todo: review and once finalized make public 
            public Condition<TEntity> Not 
                => new Not<TEntity>(this); 

        }

        // todo: mark it as obsolete
        public abstract class DescribedCondition<TEntity> : Condition<TEntity>
        {

        }

    }

}
