using System.Linq;

namespace NSelene
{
    // TODO: consider refactoring "describing" logic for conditions... 
    namespace Conditions
    {

        // TODO: find the way to DRY conditions:) Generics? keep it simple enough though...

        public abstract class Condition<TEntity>
        {

            public abstract bool Apply(TEntity entity);

            /*
             * TODO: think on leaving ToString off board, 
             * since better to use Explain and Describe vs Explain and ToString 
             * because of less confusion
             */
            public abstract string Explain();

            public override string ToString()
            {
                return string.Format("{0}", this.GetType());
            }
        }

        public abstract class DescribedCondition<TEntity> : Condition<TEntity>
        {
//            public abstract string DescribeActual();

//            public abstract string DescribeExpected();


            public virtual string DescribeActual()
            {
                return false.ToString(); // TODO: consider providing more "universal" approach... because this makes sense only in case of failed condition:(
            }

            public virtual string DescribeExpected()
            {
                return true.ToString();
            }

            public override string ToString()
            {
                //return this.GetType().Name +
                //           "\n    Expected: " + DescribeExpected() +
                //           (DescribeActual() ==  "" ? "" : "\n    " + DescribeActual());
                return string.Format("{0}" +
                    "\n  Expected : {1}" +
                    "\n  Actual   : {2}", 
                    this.GetType().Name, DescribeExpected(), DescribeActual());
            }

            /*
             * TODO: think on better names:)
             */
            public override string Explain()
            {
                return this.GetType().Name + " " + DescribeExpected();
            }

        }

    }

}
