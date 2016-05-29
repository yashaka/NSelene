namespace NSelene
{
    namespace Conditions
    {
        public class Selected : DescribedCondition<SElement>
        {

            public override bool Apply(SElement entity)
            {
                return entity.ActualWebElement.Selected;
            }
        }

    }

    public static partial class Be
    {
        public static Conditions.Condition<SElement> Selected = new Conditions.Selected();
    }

}
