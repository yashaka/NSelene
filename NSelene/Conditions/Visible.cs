namespace NSelene
{
    namespace Conditions
    {
        public class Visible : DescribedCondition<SeleneElement>
        {
            public override bool Apply(SeleneElement entity)
            {
                return entity.ActualWebElement.Displayed;
            }
        }

    }

    public static partial class Be
    {
        public static Conditions.Condition<SeleneElement> Visible { get { return new Conditions.Visible(); } }
    }

}
