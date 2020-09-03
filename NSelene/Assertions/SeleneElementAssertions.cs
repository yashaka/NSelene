using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using System.Linq;

namespace NSelene.Assertions
{
    public class SeleneElementAssertions : ReferenceTypeAssertions<SeleneElement, SeleneElementAssertions>
    {
        public SeleneElementAssertions(SeleneElement subject) : base(subject)
        {
        }
        protected override string Identifier => Subject.ToString();

        public AndConstraint<SeleneElementAssertions> HaveCssClass(string cssClass, string because = "", params object[] becauseArgs)
        {
            using (var scope = new AssertionScope())
            {
                scope.AddReportable("locator", Identifier);
                Execute.Assertion
                            .BecauseOf(because, becauseArgs)
                            .Given(() => Subject.ActualWebElement.GetAttribute("class"))
                            .ForCondition(actual => actual != null && actual.Split(' ').Contains(cssClass))
                            .FailWith("Expected {locator} to contain class {0}{reason}, but found {1}.",
                                _ => cssClass, actual => actual);
            }
            return new AndConstraint<SeleneElementAssertions>(this);
        }
        public AndConstraint<SeleneElementAssertions> HaveNoCssClass(string cssClass, string because = "", params object[] becauseArgs)
        {
            using (var scope = new AssertionScope())
            {
                scope.AddReportable("locator", Identifier);
                Execute.Assertion
                            .BecauseOf(because, becauseArgs)
                            .Given(() => Subject.ActualWebElement.GetAttribute("class"))
                            .ForCondition(actual => actual == null || !actual.Split(' ').Contains(cssClass))
                            .FailWith("Expected {locator} to contain class {0}{reason}, but found {1}.",
                                _ => cssClass, actual => actual);
            }
            return new AndConstraint<SeleneElementAssertions>(this);
        }
    }
}