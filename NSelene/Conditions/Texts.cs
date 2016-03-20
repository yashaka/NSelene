using System.Linq;

namespace NSelene
{
    namespace Conditions
    {
        // TODO: ensure messages are relevant

        public class Texts : DescribedCondition<SCollection>
        {

            protected string[] expected;
            protected string[] actual;

            public Texts(params string[] expectedTexts)
            {
                this.expected = expectedTexts;
            }

            public override bool Apply(SCollection entity)
            {
                this.actual = entity().Select(element => element.Text).ToArray();
                // TODO: make logic more readable, consider using something based on LINQ
                if (this.actual.Length != this.expected.Length ||
                !actual.Zip(this.expected, 
                    (actualText, expectedText) => actualText.Contains(expectedText)).All(res => res))
                {
                    return false;						
                } 
                return true;
            }

            public override string DescribeActual()
            {
                return "[" + string.Join(",", this.actual) + "]";
            }

            public override string DescribeExpected()
            {
                return "[" + string.Join(",", this.expected) + "]";
            }
        }

        public class ExactTexts : Texts
        {
            public ExactTexts(params string[] expected) : base(expected) {}

            public override bool Apply(SCollection entity)
            {
                this.actual = entity().Select(element => element.Text).ToArray();
                return this.actual.SequenceEqual(this.expected);
            }
        }

    }

    public static partial class Have
    {
        public static Conditions.Condition<SCollection> Texts(params string[] expected)
        {
            return new Conditions.Texts(expected);
        }

        public static Conditions.Condition<SCollection> ExactTexts(params string[] expected)
        {
            return new Conditions.ExactTexts(expected);
        }
    }

}
