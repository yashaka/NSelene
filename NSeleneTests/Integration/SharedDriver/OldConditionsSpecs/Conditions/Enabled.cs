using NSelene.Conditions;

namespace NSelene.Tests.Integration.SharedDriver.OldConditionsSpecs
{
    namespace Conditions
    {
        #pragma warning disable 0618
        public class Enabled : DescribedCondition<SeleneElement>
        #pragma warning restore 0618
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
            public static Condition<SeleneElement> Enabled => new Not<SeleneElement>(new Conditions.Enabled());
        }
    }

}
