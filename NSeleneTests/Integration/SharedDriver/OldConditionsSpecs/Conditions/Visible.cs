using NSelene.Conditions;

namespace NSelene.Tests.Integration.SharedDriver.OldConditionsSpecs
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
        public static Condition<SeleneElement> Visible 
            => new Conditions.Visible();

        static partial class Not
        {
            public static Condition<SeleneElement> Visible 
                => Be.Visible.Not;
        }
    }

}
