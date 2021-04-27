using NSelene.Conditions;

namespace NSelene.Tests.Integration.SharedDriver.OldConditionsSpecs
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
        public static Condition<SeleneElement> Selected { get { return new Conditions.Selected(); } }
        static partial class Not
        {
            public static Condition<SeleneElement> Selected { get { return new Conditions.Selected().Not; } }
        }

    }

}
