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
        public static Conditions.Condition<SeleneElement> Visible 
            => new Conditions.Visible();

        static partial class Not
        {
            public static Conditions.Condition<SeleneElement> Visible 
                => Be.Visible.Not;
        }
    }

}
