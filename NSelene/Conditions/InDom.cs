using System;

namespace NSelene
{
    namespace Conditions
    {
        public class InDom : DescribedCondition<SElement>
        {

            public override bool Apply(SElement entity)
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
        public static Conditions.Condition<SElement> InDOM = new Conditions.InDom();

        public static Conditions.Condition<SElement> InDom = new Conditions.InDom();
    }

}
