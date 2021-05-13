using System;

namespace NSelene
{
    namespace Conditions
    {
        public class InDom : Condition<SeleneElement>
        {
            public override void Invoke(SeleneElement entity)
            {
                var found = entity.ActualWebElement;
            }

            public override string ToString() => "Be.InDom";
        }

    }

    public static partial class Be
    {
        [Obsolete("Be.InDOM is deprecated and will be removed in next version, please use Be.InDom method instead.")]
        public static Conditions.Condition<SeleneElement> InDOM => new Conditions.InDom();

        public static Conditions.Condition<SeleneElement> InDom => new Conditions.InDom();

        public static partial class Not
        {
            public static Conditions.Condition<SeleneElement> InDom => new Conditions.Not<SeleneElement>(new Conditions.InDom());
        }
    }

}
