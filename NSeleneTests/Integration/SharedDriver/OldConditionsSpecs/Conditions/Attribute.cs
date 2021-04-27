using System.Linq;
using NSelene.Conditions;

namespace NSelene.Tests.Integration.SharedDriver.OldConditionsSpecs
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
        public static Condition<SeleneElement> Attribute(string name, string value) => new Conditions.Attribute(name, value);

        public static Condition<SeleneElement> Value(string expected) => new Conditions.Attribute("value", expected);
        public static partial class No
        {
            public static Condition<SeleneElement> Attribute(string name, string value) 
                => new Conditions.NoAttribute(name, value);
            public static Condition<SeleneElement> Value(string expected) 
                => new Conditions.Attribute("value", expected).Not;
        }
    }

    public static partial class Be
    {
        public static Condition<SeleneElement> Blank 
        => new Conditions.Attribute("value", "");
        
        public static partial class Not
        {
            public static Condition<SeleneElement> Blank 
            => new Conditions.Attribute("value", "").Not;
        }
    }
}
