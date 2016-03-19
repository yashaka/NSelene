using System.Linq;

namespace NSelene
{
    namespace Conditions
    {
        public class CssClass : DescribedCondition<SElement>
        {

            private string expected;
            private string actual;

            public CssClass(string expected)
            {
                this.expected = expected;
            }

            public override bool Apply(SElement entity)
            {
                this.actual = entity().GetAttribute("class");

                // TODO: do we need this comparison with null? (we needed it in Java)
                return this.actual != null && this.actual.Split(' ').Contains(this.expected); 
            }

            public override string DescribeActual()
            {
                return this.actual;
            }

            public override string DescribeExpected()
            {
                return "has '" + this.expected + "'";
            }
        }

    }

    public static partial class Have
    {
        public static Conditions.Condition<SElement> CssClass(string cssClass)
        {
            return new Conditions.CssClass(cssClass);
        }
    }

}
