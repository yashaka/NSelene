namespace NSelene
{
    namespace Conditions
    {
        public class Selected : Condition<SeleneElement>
        {
            public override void Invoke(SeleneElement entity)
            {
                var webelement = entity.ActualWebElement;
                if (!webelement.Selected) 
                {
                    throw new ConditionNotMatchedException(() => 
                        "Found element is not selected: "
                        + webelement.GetAttribute("outerHTML")
                    );
                }
            }

            public override string ToString() => "Be.Selected";
        }

    }

    public static partial class Be
    {
        public static Conditions.Condition<SeleneElement> Selected { get { return new Conditions.Selected(); } }
        static partial class Not
        {
            public static Conditions.Condition<SeleneElement> Selected { get { return new Conditions.Not<SeleneElement>(new Conditions.Selected()); } }
        }

    }

}
