using System.Linq;

namespace NSelene
{
    namespace Conditions
    {
        public class Texts : DescribedCondition<SCollection>
        {

            private string[] expectedTexts;
            private string[] actualTexts;

            public Texts(params string[] expectedTexts)
            {
                this.expectedTexts = expectedTexts;
            }

            public override bool Apply(SCollection entity)
            {
                this.actualTexts = entity().Select(element => element.Text).ToArray();
                if (this.actualTexts.Length != this.expectedTexts.Length ||
                !actualTexts.Zip(this.expectedTexts, 
                    (actualText, expectedText) => actualText.Contains(expectedText)).All(res => res))
                {

                    return false;						
                } 
                return true;
            }

            public override string DescribeActual()
            {
                return "[" + string.Join(",", this.actualTexts) + "]";
            }

            public override string DescribeExpected()
            {
                return "[" + string.Join(",", this.expectedTexts) + "]";
            }
        }

        public static partial class Have
        {

            public static Condition<SCollection> Texts(params string[] expectedTexts)
            {
                return new Texts(expectedTexts);
            }
        }

    }

}
