using System.Linq;

namespace NSelene
{
    namespace Conditions
    {
        public class Count : DescribedCondition<SCollection>
        {

            private int expectedCount;
            private int actualCount;

            public Count(int count)
            {
                this.expectedCount = count;
            }

            public override bool Apply(SCollection entity)
            {
                this.actualCount = entity().Count;
                return this.actualCount >= this.expectedCount;
            }

            public override string DescribeActual()
            {
                return this.actualCount.ToString();
            }

            public override string DescribeExpected()
            {
                return this.expectedCount.ToString();
            }
        }

    }

    public static partial class Have
    {
        public static Conditions.Condition<SCollection> Count(int count)
        {
            return new Conditions.CountAtLeast(count);
        }
    }

}
