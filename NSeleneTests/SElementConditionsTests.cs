﻿using NUnit.Framework;
using NSelene;
using static NSelene.Selene;

namespace NSeleneTests
{
    [TestFixture]
    public class SElementConditionTests : BaseTest
    {

        // TODO: TBD

        [Test]
        public void SElementShouldBeVisible()
        {
            Given.OpenedPageWithBody("<h1 style='display:none'>ku ku</h1>");
            S("h1").ShouldNot(Be.Visible);
            When.WithBody("<h1 style='display:block'>ku ku</h1>");
            S("h1").Should(Be.Visible);
        }

        [Test]
        public void SElementShouldBeEnabled()
        {
            Given.OpenedPageWithBody("<input type='text' disabled/>");
            S("input").ShouldNot(Be.Enabled);
            When.WithBody("<input type='text'/>");
            S("input").Should(Be.Enabled);
        }

        [Test]
        public void SElementShouldBeInDOM()
        {
            Given.OpenedEmptyPage();
            S("h1").ShouldNot(Be.InDom);
            When.WithBody("<h1 style='display:none'>ku ku</h1>");
            S("h1").Should(Be.InDom);
        }

        [Test]
        public void SElementShouldHaveText()
        {
            Given.OpenedPageWithBody("<h1>Hello Babe!</h1>");
            S("h1").Should(Have.Text("Hello"));
            S("h1").ShouldNot(Have.Text("Hello world!"));
            S("h1").ShouldNot(Have.ExactText("Hello"));
            S("h1").Should(Have.ExactText("Hello Babe!"));
        }

        [Test]
        public void SElementShouldHaveCssClass()
        {
            Given.OpenedPageWithBody("<h1 class='big-title'>Hello Babe!</h1>");
            S("h1").ShouldNot(Have.CssClass("title"));
            When.WithBody("<h1 class='big title'>Hello world!</h1>");
            S("h1").Should(Have.CssClass("title"));
        }

        [Test]
        public void SElementShouldHaveAttribute()
        {
            Given.OpenedPageWithBody("<h1 class='big-title'>Hello Babe!</h1>");
            S("h1").ShouldNot(Have.Attribute("class", "big title"));
            When.WithBody("<h1 class='big title'>Hello world!</h1>");
            S("h1").Should(Have.Attribute("class", "big title"));
        }

        [Test]
        public void SElementShouldHaveValue()
        {
            Given.OpenedEmptyPage();
            S("input").ShouldNot(Have.Value("Yo"));
            When.WithBody("<input value='Yo'></input>");
            S("input").ShouldNot(Have.Value("o_O"));
            S("input").Should(Have.Value("Yo"));
        }

        [Test]
        public void SElementShouldBeBlank()
        {
            Given.OpenedEmptyPage();
            S("input").ShouldNot(Be.Blank()); // TODO: sounds crazy, no? :)
            When.WithBody("<input type='text' value='Yo'/>");
            S("input").ShouldNot(Be.Blank());
            When.WithBody("<input type='text'/>");
            S("input").Should(Be.Blank());
        }

        // TODO: add tests for ShouldNot with non-existent element itself... what should the behaviour be? :)
    }
}

