namespace NSelene.Tests.Integration.SharedDriver.OldConditionsSpecs
{
    namespace Conditions
    {
        public class Text : DescribedCondition<SeleneElement>
        {

            protected string expected;
            protected string actual;

            public Text(string expected)
            {
                this.expected = expected;
                this.actual = "";
            }

            public override bool Apply(SeleneElement entity)
            {
                this.actual = entity.ActualWebElement.Text;
                return this.actual.Contains(this.expected);
            }

            public override string DescribeActual()
            {
                return this.actual;
            }

            public override string DescribeExpected()
            {
                return "contains " + this.expected;
            }
        }

        public class ExactText : Text
        {

            public ExactText(string expected) : base(expected) {}

            public override bool Apply(SeleneElement entity)
            {
                this.actual = entity.ActualWebElement.Text;
                return this.actual.Equals(this.expected);
            }

            public override string DescribeExpected()
            {
                return "is " + this.expected;
            }
        }

    }

    public static partial class Have
    {
        public static Condition<SeleneElement> Text(string expected)
        {
            return new Text(expected);
        }

        public static Condition<SeleneElement> ExactText(string expected)
        {
            return new ExactText(expected);
        }
        static partial class No
        {
            public static Condition<SeleneElement> Text(string expected) => new Conditions.Text(expected).Not;

            public static Condition<SeleneElement> ExactText(string expected) => new Conditions.ExactText(expected);
        }
    }

}
