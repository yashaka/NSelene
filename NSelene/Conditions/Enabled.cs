namespace NSelene
{
    namespace Conditions
    {
        public class Enabled : Condition<SeleneElement>
        {
            public override void Invoke(SeleneElement entity)
            {
                var webelement = entity.ActualWebElement;
                if (!webelement.Enabled) 
                {
                    throw new ConditionNotMatchedException(() => 
                        "Found element is not enabled: "
                        + webelement.GetAttribute("outerHTML")
                    );
                }
            }

            public override string ToString() => "Be.Enabled";
        }

    }

    public static partial class Be
    {
        public static Conditions.Condition<SeleneElement> Enabled => new Conditions.Enabled();

        public static partial class Not
        {
            public static Conditions.Condition<SeleneElement> Enabled => new Conditions.Not<SeleneElement>(new Conditions.Enabled());
        }
    }

}
