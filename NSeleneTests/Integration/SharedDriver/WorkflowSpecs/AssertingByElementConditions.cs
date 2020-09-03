namespace NSelene.Tests.Integration.SharedDriver.WorkflowSpecs
{
    using FluentAssertions;
    using Harness;
    using NUnit.Framework;
    using static NSelene.Selene;
    using NSelene.Assertions;

    class AssertingByElementConditions : BaseTest
    {
        // todo: add more tests
        
        [Test]
        public void BeVisible()
        {
            Given.OpenedPageWithBody("<h1 style='display:none'>ku ku</h1>");
            S("h1").ShouldNot(Be.Visible);
            When.WithBody("<h1 style='display:block'>ku ku</h1>");
            S("h1").Should(Be.Visible);
        }

        [Test]
        public void BeEnabled()
        {
            Given.OpenedPageWithBody("<input type='text' disabled/>");
            S("input").ShouldNot(Be.Enabled);
            When.WithBody("<input type='text'/>");
            S("input").Should(Be.Enabled);
        }

        [Test]
        public void BeInDOM()
        {
            Given.OpenedEmptyPage();
            S("h1").ShouldNot(Be.InDom);
            When.WithBody("<h1 style='display:none'>ku ku</h1>");
            S("h1").Should(Be.InDom);
        }

        [Test]
        public void HaveText()
        {
            Given.OpenedPageWithBody("<h1>Hello Babe!</h1>");
            S("h1").Should(Have.Text("Hello"));
            S("h1").ShouldNot(Have.Text("Hello world!"));
            S("h1").ShouldNot(Have.ExactText("Hello"));
            S("h1").Should(Have.ExactText("Hello Babe!"));
        }

        [Test]
        public void HaveCssClass()
        {
            Given.OpenedPageWithBody("<h1 class='big-title'>Hello Babe!</h1>");
            S("h1").ShouldNot(Have.CssClass("title"));
            When.WithBody("<h1 class='big title'>Hello world!</h1>");
            S("h1").Should(Have.CssClass("title"));
        }

        [Test]
        public void HaveAttribute()
        {
            Given.OpenedPageWithBody("<h1 class='big-title'>Hello Babe!</h1>");
            S("h1").ShouldNot(Have.Attribute("class", "big title"));
            When.WithBody("<h1 class='big title'>Hello world!</h1>");
            S("h1").Should(Have.Attribute("class", "big title"));
        }

        [Test]
        public void HaveValue()
        {
            Given.OpenedEmptyPage();
            S("input").ShouldNot(Have.Value("Yo"));
            When.WithBody("<input value='Yo'></input>");
            S("input").ShouldNot(Have.Value("o_O"));
            S("input").Should(Have.Value("Yo"));
        }

        [Test]
        public void BeBlank()
        {
            Given.OpenedEmptyPage();
            S("input").ShouldNot(Be.Blank); // TODO: sounds crazy, no? :)
            When.WithBody("<input type='text' value='Yo'/>");
            S("input").ShouldNot(Be.Blank);
            When.WithBody("<input type='text'/>");
            S("input").Should(Be.Blank);
        }



        // TODO: add tests for ShouldNot with non-existent element itself... what should the behaviour be? :)


        [Test]
        public void HaveCssClassWithFluentAssertions()
        {
            Given.OpenedPageWithBody("<h1 class='big-title'>Hello Babe!</h1>");
            S("h1").Should().HaveNoCssClass("title");
            When.WithBody("<h1 class='big title'>Hello world!</h1>");
            S("h1").Should().HaveCssClass("title").And.HaveCssClass("big");
        }

    }
}