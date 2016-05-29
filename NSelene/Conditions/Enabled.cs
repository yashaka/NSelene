namespace NSelene
{
    namespace Conditions
    {
        public class Enabled : DescribedCondition<SElement>
        {

            public override bool Apply(SElement entity)
            {
                return entity.ActualWebElement.Enabled;
            }
        }

    }

    public static partial class Be
    {
        public static Conditions.Condition<SElement> Enabled = new Conditions.Enabled();
    }

}
