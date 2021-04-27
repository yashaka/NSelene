using System.Linq;
using NSelene.Conditions;

namespace NSelene.Tests.Integration.SharedDriver.OldConditionsSpecs
{
    namespace Conditions
    {
        public class Count : DescribedCondition<SeleneCollection>
        {

            protected int expectedCount;
            protected int actualCount;

            public Count(int count)
            {
                this.expectedCount = count;
            }

            public override bool Apply(SeleneCollection entity)
            {
                this.actualCount = entity.ActualWebElements.Count;
                return this.actualCount == this.expectedCount;
            }

            public override string DescribeActual()
            {
                return "count = " + this.actualCount;
            }

            public override string DescribeExpected()
            {
                return "count = " + this.expectedCount;
            }
        }

        public class CountAtLeast: Count
        {
            public CountAtLeast(int count) : base (count) {}

            public override bool Apply(SeleneCollection entity)
            {
                this.actualCount = entity.ActualWebElements.Count;
                return this.actualCount >= this.expectedCount;
            }
        }

    }

    public static partial class Have
    {
        public static Condition<SeleneCollection> Count(int count) => new Conditions.Count(count);

        public static Condition<SeleneCollection> CountAtLeast(int count) => new Conditions.CountAtLeast(count);
        public static partial class No
        {
            public static Condition<SeleneCollection> Count(int count) => new Conditions.Count(count).Not;

            public static Condition<SeleneCollection> CountAtLeast(int count) => new Conditions.CountAtLeast(count).Not;
        }
    }

    public static partial class Be
    {
        public static Condition<SeleneCollection> Empty => new Conditions.Count(0);
        public static partial class Not
        {
            public static Condition<SeleneCollection> Empty => new Conditions.Count(0).Not;
        }
    }
}
