﻿using System.Linq;

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

    }

    public static partial class Have
    {
        public static Conditions.Condition<SeleneElement> Attribute(string name, string value)
        {
            return new Conditions.Attribute(name, value);
        }

        public static Conditions.Condition<SeleneElement> Value(string expected)
        {
            return new Conditions.Attribute("value", expected);
        }
    }

    public static partial class Be
    {
        public static Conditions.Condition<SeleneElement> Blank()
        {
            return new Conditions.Attribute("value", "");
        }
    }

}
