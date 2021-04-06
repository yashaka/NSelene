namespace NSelene.Tests.Integration.SharedDriver.WorkflowSpecs
{
    using Harness;
    using NUnit.Framework;
    using static NSelene.Selene;

    class AssertingByCollectionConditions_Specs : BaseTest
    {
        // todo: add more tests

        [Test]
        public void HaveTextsAndExactTexts()
        { 
            Given.OpenedPageWithBody("<ul>Hello to:<li>Dear Bob</li><li>Lovely Kate</li></ul>");
            SS("li").Should(Have.No.Texts("Kate", "Bob"));
            SS("li").Should(Have.No.Texts("Bob"));
            SS("li").Should(Have.No.Texts("Bob", "Kate", "Joe"));
            SS("li").Should(Have.Texts("Bob", "Kate"));
            SS("li").Should(Have.No.ExactTexts("Bob", "Kate"));
            SS("li").Should(Have.No.ExactTexts("Lovely Kate", "Dear Bob"));
            SS("li").Should(Have.No.ExactTexts("Dear Bob"));
            SS("li").Should(Have.No.ExactTexts("Dear Bob", "Lovely Kate", "Funny Joe"));
            SS("li").Should(Have.ExactTexts("Dear Bob", "Lovely Kate"));
        }

        [Test]
        public void HaveCountAtLeastAndCount()
        {
            Given.OpenedEmptyPage();
            SS("li").Should(Have.No.Count(2));
            When.WithBody("<ul>Hello to:<li>Dear Bob</li><li>Lovely Kate</li></ul>");
            SS("li").Should(Have.No.CountAtLeast(3));
            SS("li").Should(Have.Count(2));
            SS("li").Should(Have.CountAtLeast(1));
        }

        [Test]
        public void BeEmpty()
        {
            Given.OpenedEmptyPage();
            SS("li").Should(Be.Empty);
            When.WithBody("<ul>Hello to:<li>Dear Bob</li><li>Lovely Kate</li></ul>");
            SS("li").Should(Be.Not.Empty);
        }
    }
}