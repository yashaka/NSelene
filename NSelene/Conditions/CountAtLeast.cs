using System.Linq;

namespace NSelene
{
    namespace Conditions
    {
        public class CountAtLeast : DescribedCondition<SCollection>
        {

            private int expectedMinimumCount;
            private int actualCount;

            public CountAtLeast(int minimumCount)
            {
                this.expectedMinimumCount = minimumCount;
            }

            public override bool Apply(SCollection entity)
            {
                this.actualCount = entity().Count;
                return this.actualCount >= this.expectedMinimumCount;
            }

            public override string DescribeActual()
            {
                return this.actualCount.ToString();
            }

            public override string DescribeExpected()
            {
                return ">= " + this.expectedMinimumCount;
            }
        }

    }

    public static partial class Have
    {
        public static Conditions.Condition<SCollection> CountAtLeast(int minimumCount)
        {
            return new Conditions.CountAtLeast(minimumCount);
        }
    }

}
