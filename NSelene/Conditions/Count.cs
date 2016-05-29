using System.Linq;

namespace NSelene
{
    namespace Conditions
    {
        public class Count : DescribedCondition<SCollection>
        {

            protected int expectedCount;
            protected int actualCount;

            public Count(int count)
            {
                this.expectedCount = count;
            }

            public override bool Apply(SCollection entity)
            {
                this.actualCount = entity.ActualWebElements.Count;
                return this.actualCount == this.expectedCount;
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

        public class CountAtLeast: Count
        {
            public CountAtLeast(int count) : base (count) {}

            public override bool Apply(SCollection entity)
            {
                this.actualCount = entity.ActualWebElements.Count;
                return this.actualCount >= this.expectedCount;
            }
        }

    }

    public static partial class Have
    {
        public static Conditions.Condition<SCollection> Count(int count)
        {
            return new Conditions.Count(count);
        }

        public static Conditions.Condition<SCollection> CountAtLeast(int count)
        {
            return new Conditions.CountAtLeast(count);
        }
    }

    public static partial class Be
    {
        public static Conditions.Condition<SCollection> Empty = new Conditions.Count(0);
    }

}
