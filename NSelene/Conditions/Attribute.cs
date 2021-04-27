using System.Linq;

namespace NSelene
{
    namespace Conditions
    {
        public class Attribute : DescribedCondition<SeleneElement>
        {
            private string name;
            private string expectedValue;
            private string actualValue;

            public Attribute(string name, string value)
            {
                this.name = name;
                this.expectedValue = value;
            }

            public override bool Apply(SeleneElement entity)
            {
                this.actualValue = entity.ActualWebElement.GetAttribute(this.name);

                // TODO: do we need this comparison with null? (we needed it in Java)
                return this.actualValue != null && this.actualValue == this.expectedValue;
            }

            public override string DescribeActual()
            {
                return this.name + "='" + this.actualValue + "'";
            }

            public override string DescribeExpected()
            {
                return this.name + "='" + this.expectedValue + "'";
            }
        }

        class NoAttribute : DescribedCondition<SeleneElement>
        {

            private string name;
            private string expectedValue;
            private string actualValue;

            public NoAttribute(string name, string value)
            {
                this.name = name;
                this.expectedValue = value;
            }

            public override bool Apply(SeleneElement entity)
            {
                this.actualValue = entity.ActualWebElement.GetAttribute(this.name);

                // TODO: do we need this comparison with null? (we needed it in Java)
                return this.actualValue == null || this.actualValue != this.expectedValue;
            }

            public override string DescribeActual()
            {
                return this.name + "='" + this.actualValue + "'";
            }

            public override string DescribeExpected()
            {
                return this.name + "!='" + this.expectedValue + "'";
            }
        }

    }

    public static partial class Have
    {
        public static Conditions.Condition<SeleneElement> Attribute(string name, string value) => new Conditions.Attribute(name, value);

        public static Conditions.Condition<SeleneElement> Value(string expected) => new Conditions.Attribute("value", expected);
        public static partial class No
        {
            public static Conditions.Condition<SeleneElement> Attribute(string name, string value) 
                => new Conditions.NoAttribute(name, value);
            public static Conditions.Condition<SeleneElement> Value(string expected) 
                => new Conditions.Not<SeleneElement>(new Conditions.Attribute("value", expected));
        }

    }

    public static partial class Be
    {
        public static Conditions.Condition<SeleneElement> Blank => new Conditions.Attribute("value", "");
        public static partial class Not
        {
            public static Conditions.Condition<SeleneElement> Blank => new Conditions.Not<SeleneElement>(new Conditions.Attribute("value", ""));
        }
    }
}
