using System.Linq;

namespace NSelene
{
	namespace Conditions
	{
		public abstract class Condition<TEntity>
		{

			public abstract bool Apply(TEntity entity);

			public override string ToString ()
			{
				return string.Format ("\n while waiting for condition: {0}", this.GetType ());
			}
		}

		public abstract class DescribedCondition<TEntity> : Condition<TEntity>
		{
			public abstract string DescribeActual ();
			public abstract string DescribeExpected ();

			public override string ToString ()
			{
				return string.Format ("\n while waiting for condition: {0}" +
					"\n Expected: {1}" +
					"\n Actual: {2}", 
					this.GetType (), DescribeExpected (), DescribeActual ());
			}

		}

	}

}
