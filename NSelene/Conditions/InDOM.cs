namespace NSelene
{
    namespace Conditions
    {
        public class InDOM : DescribedCondition<SElement>
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
        public static Conditions.Condition<SElement> InDOM = new Conditions.InDOM(); // TODO: consider renaming to InDom
    }

}
