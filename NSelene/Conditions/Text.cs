using System.Linq;

namespace NSelene
{
    namespace Conditions
    {
        public class Text : DescribedCondition<SElement>
        {

            protected string expected;
            protected string actual;

            public Text(string expected)
            {
                this.expected = expected;
            }

            public override bool Apply(SElement entity)
            {
                this.actual = entity.GetActualWebElement().Text;
                return this.actual.Contains(this.expected);
            }

            public override string DescribeActual()
            {
                return this.actual;
            }

            public override string DescribeExpected()
            {
                return this.expected;
            }
        }
        public class ExactText : Text
        {

            public ExactText(string expected) : base(expected) {}

            public override bool Apply(SElement entity)
            {
                this.actual = entity.GetActualWebElement().Text;
                return this.actual.Equals(this.expected);
            }

            public override string DescribeExpected()
            {
                return "contains " + this.expected;
            }
        }

    }

    public static partial class Have
    {
        public static Conditions.Condition<SElement> Text(string expected)
        {
            return new Conditions.Text(expected);
        }

        public static Conditions.Condition<SElement> ExactText(string expected)
        {
            return new Conditions.ExactText(expected);
        }
    }

}
