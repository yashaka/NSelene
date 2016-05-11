namespace NSelene
{
    namespace Conditions
    {
        public class Visible : DescribedCondition<SElement>
        {

            public override bool Apply(SElement entity)
            {
                return entity.GetActualWebElement().Displayed;
            }
        }

    }

    public static partial class Be
    {
        public static Conditions.Condition<SElement> Visible = new Conditions.Visible();
    }

}
