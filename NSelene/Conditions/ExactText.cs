using System.Linq;

namespace NSelene
{
    namespace Conditions
    {
        public class ExactText : DescribedCondition<SElement>
        {

            private string expectedText;
            private string actualText;

            public ExactText(string expectedText)
            {
                this.expectedText = expectedText;
            }

            public override bool Apply(SElement entity)
            {
                this.actualText = entity().Text;
                return this.actualText.Equals(this.expectedText);
            }

            public override string DescribeActual()
            {
                return this.actualText;
            }

            public override string DescribeExpected()
            {
                return this.expectedText;
            }
        }

    }

    public static partial class Have
    {
        public static Conditions.Condition<SElement> ExactText(string expectedText)
        {
            return new Conditions.ExactText(expectedText);
        }
    }

}
