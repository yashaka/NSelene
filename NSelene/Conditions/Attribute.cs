using System.Linq;

namespace NSelene
{
    namespace Conditions
    {
        public class Attribute : DescribedCondition<SElement>
        {

            private string name;
            private string expectedValue;
            private string actualValue;

            public Attribute(string name, string value)
            {
                this.name = name;
                this.expectedValue = value;
            }

            public override bool Apply(SElement entity)
            {
                this.actualValue = entity().GetAttribute(this.name);

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
        public static Conditions.Condition<SElement> Attribute(string name, string value)
        {
            return new Conditions.Attribute(name, value);
        }
    }

}
