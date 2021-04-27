using System;
using NSelene.Conditions;

namespace NSelene.Tests.Integration.SharedDriver.OldConditionsSpecs
{
    namespace Conditions
    {
        public class InDom : DescribedCondition<SeleneElement>
        {

            public override bool Apply(SeleneElement entity)
            {
                var webelement = entity.ActualWebElement;
                return true;
            }

            public override string DescribeActual()
            {
                return false.ToString();
            }

            public override string DescribeExpected()
            {
                return true.ToString();
            }
        }

    }

    public static partial class Be
    {
        [Obsolete("Be.InDOM is deprecated and will be removed in next version, please use Be.InDom method instead.")]
        public static Condition<SeleneElement> InDOM => new Conditions.InDom();

        public static Condition<SeleneElement> InDom => new Conditions.InDom();

        public static partial class Not
        {
            public static Condition<SeleneElement> InDom => new Conditions.InDom().Not;
        }
    }

}
