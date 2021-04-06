namespace NSelene.Tests.Integration.SharedDriver.WorkflowSpecs
{
    using Harness;
    using NUnit.Framework;
    using static NSelene.Selene;

    class AssertingByElementConditions_Specs : BaseTest
    {
        // todo: add more tests
        
        [Test]
        public void BeVisible()
        {
            Given.OpenedPageWithBody("<h1 style='display:none'>ku ku</h1>");
            S("h1").Should(Be.Not.Visible);
            When.WithBody("<h1 style='display:block'>ku ku</h1>");
            S("h1").Should(Be.Visible);
        }

        [Test]
        public void BeEnabled()
        {
            Given.OpenedPageWithBody("<input type='text' disabled/>");
            S("input").Should(Be.Not.Enabled);
            When.WithBody("<input type='text'/>");
            S("input").Should(Be.Enabled);
        }

        [Test]
        public void BeInDOM()
        {
            Given.OpenedEmptyPage();
            S("h1").Should(Be.Not.InDom);
            When.WithBody("<h1 style='display:none'>ku ku</h1>");
            S("h1").Should(Be.InDom);
        }

        [Test]
        public void HaveText()
        {
            Given.OpenedPageWithBody("<h1>Hello Babe!</h1>");
            S("h1").Should(Have.Text("Hello"));
            S("h1").Should(Have.No.Text("Hello world!"));
            S("h1").Should(Have.No.ExactText("Hello"));
            S("h1").Should(Have.ExactText("Hello Babe!"));
        }

        [Test]
        public void HaveCssClass()
        {
            Given.OpenedPageWithBody("<h1 class='big-title'>Hello Babe!</h1>");
            S("h1").Should(Have.No.CssClass("title"));
            When.WithBody("<h1 class='big title'>Hello world!</h1>");
            S("h1").Should(Have.CssClass("title"));
        }

        [Test]
        public void HaveAttribute()
        {
            Given.OpenedPageWithBody("<h1 class='big-title'>Hello Babe!</h1>");
            S("h1").Should(Have.No.Attribute("class", "big title"));
            When.WithBody("<h1 class='big title'>Hello world!</h1>");
            S("h1").Should(Have.Attribute("class", "big title"));
        }

        [Test]
        public void HaveValue()
        {
            Given.OpenedEmptyPage();
            S("input").Should(Have.No.Value("Yo"));
            When.WithBody("<input value='Yo'></input>");
            S("input").Should(Have.No.Value("o_O"));
            S("input").Should(Have.Value("Yo"));
        }

        [Test]
        public void BeBlank()
        {
            Given.OpenedEmptyPage();
            // TODO: sounds crazy, no? :) it passes... is it good?
            S("input").Should(Be.Not.Blank); 
            When.WithBody("<input type='text' value='Yo'/>");
            S("input").Should(Be.Not.Blank);
            When.WithBody("<input type='text'/>");
            S("input").Should(Be.Blank);
        }

        // TODO: add tests for ShouldNot with non-existent element itself... what should the behaviour be? :)
    }
}