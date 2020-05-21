namespace NSelene
{
    namespace Conditions
    {
        public class Selected : DescribedCondition<SeleneElement>
        {

            public override bool Apply(SeleneElement entity)
            {
                return entity.ActualWebElement.Selected;
            }
        }

    }

    public static partial class Be
    {
        public static Conditions.Condition<SeleneElement> Selected { get { return new Conditions.Selected(); } }
    }

}
