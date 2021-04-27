using System.Linq;
using NSelene.Conditions;

namespace NSelene.Tests.Integration.SharedDriver.OldConditionsSpecs
{
    namespace Conditions
    {
        public class CssClass : DescribedCondition<SeleneElement>
        {

            private string expected;
            private string actual;

            public CssClass(string expected)
            {
                this.expected = expected;
            }

            public override bool Apply(SeleneElement entity)
            {
                this.actual = entity.ActualWebElement.GetAttribute("class");

                // TODO: do we need this comparison with null? (we needed it in Java)
                return this.actual != null && this.actual.Split(' ').Contains(this.expected); 
            }

            public override string DescribeActual()
            {
                return this.actual;
            }

            public override string DescribeExpected()
            {
                return "has CSS class '" + this.expected + "'";
            }
        }


        #pragma warning disable 0618
        public class NoCssClass : DescribedCondition<SeleneElement>
        #pragma warning restore 0618
        {

            private string expected;
            private string actual;

            public NoCssClass(string expected)
            {
                this.expected = expected;
            }

            public override bool Apply(SeleneElement entity)
            {
                this.actual = entity.ActualWebElement.GetAttribute("class");

                // TODO: do we need this comparison with null? (we needed it in Java)
                return this.actual == null || !this.actual.Split(' ').Contains(this.expected); 
            }

            public override string DescribeActual()
            {
                return this.actual;
            }

            public override string DescribeExpected()
            {
                return "has no'" + this.expected + "'";
            }
        }

    }

    public static partial class Have
    {
        public static Condition<SeleneElement> CssClass(string cssClass) => new Conditions.CssClass(cssClass);

        public static partial class No
        {
            public static Condition<SeleneElement> CssClass(string cssClass) => new Conditions.NoCssClass(cssClass);
         }
    }
}
