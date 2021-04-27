using NSelene.Conditions;

namespace NSelene.Tests.Integration.SharedDriver.OldConditionsSpecs
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
        public static Condition<SeleneElement> Enabled => new Conditions.Enabled();

        public static partial class Not
        {
            public static Condition<SeleneElement> Enabled => new Conditions.Enabled().Not;
        }
    }

}
