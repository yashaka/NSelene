namespace NSelene
{
    namespace Conditions
    {
        public class Enabled : DescribedCondition<SeleneElement>
        {

            public override bool Apply(SeleneElement entity)
            {
                return entity.ActualWebElement.Enabled;
            }
        }

    }

    public static partial class Be
    {
        public static Conditions.Condition<SeleneElement> Enabled { get { return new Conditions.Enabled(); } }
    }

}
