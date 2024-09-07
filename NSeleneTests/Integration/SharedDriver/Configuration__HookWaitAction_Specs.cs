using NUnit.Framework;
using static NSelene.Selene;
using OpenQA.Selenium;

namespace NSelene.Tests.Integration.SharedDriver.ConfigurationSpec
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Harness;

    [TestFixture]
    public class Configuration__HookWaitAction_Specs : BaseTest
    {
        [Test]
        public void HookWaitAction_SetGlobally_CanLogActionsLikeClickAndShould()
        {
            List<string> log = new List<string>();
            Configuration.Timeout = 0.1;
            Configuration.PollDuringWaits = 0.01;
            Configuration._HookWaitAction = (entityObject, describeComputation, wait) =>
            {
                log.Add($"{entityObject}.{describeComputation()}: STARTED");
                try
                {
                    wait();
                    log.Add($"{entityObject}.{describeComputation()}: PASSED");
                }
                catch (Exception)
                {
                    log.Add($"{entityObject}.{describeComputation()}: FAILED");
                    throw;
                }
            };
            Given.OpenedPageWithBody(@"
                <button>Click me!</button>
            ");

            SS("button").Should(Have.Count(1));
            S("button").Should(Have.ExactText("Click me!"));
            S("button").Click();
            S("button").With(clickByJs: true).Click();
            try { S(".absent").Click(); } catch {}
            try { S(".absent").Should(Have.Text("some")); } catch {}
            try { SS(".absent").Should(Have.Count(1)); } catch {}
            try { S(".parent").All(".child").Should(Have.Count(1)); } catch {}

            Assert.AreEqual(log,
                @"Browser.All(button).Should(Have.Count = 1): STARTED
                Browser.All(button).Should(Have.Count = 1): PASSED
                Browser.Element(button).Should(Have.ExactText(«Click me!»)): STARTED
                Browser.Element(button).Should(Have.ExactText(«Click me!»)): PASSED
                Browser.Element(button).ActualWebElement.Click(): STARTED
                Browser.Element(button).ActualWebElement.Click(): PASSED
                Browser.Element(button).JsClick(centerXOffset: 0, centerYOffset: 0): STARTED
                Browser.Element(button).JsClick(centerXOffset: 0, centerYOffset: 0): PASSED
                Browser.Element(.absent).ActualWebElement.Click(): STARTED
                Browser.Element(.absent).ActualWebElement.Click(): FAILED
                Browser.Element(.absent).Should(Have.TextContaining(«some»)): STARTED
                Browser.Element(.absent).Should(Have.TextContaining(«some»)): FAILED
                Browser.All(.absent).Should(Have.Count = 1): STARTED
                Browser.All(.absent).Should(Have.Count = 1): FAILED
                Browser.Element(.parent).All(.child).Should(Have.Count = 1): STARTED
                Browser.Element(.parent).All(.child).Should(Have.Count = 1): FAILED"
                .Split(Environment.NewLine).Select(item => item.Trim()).ToList()
            );
        }

        [Test]
        public void HookWaitAction_SetPerElements_CanLogActionsLikeClickAndShould()
        {
            List<string> log = new List<string>();
            Configuration.Timeout = 0.1;
            Configuration.PollDuringWaits = 0.01;
            Action<object, Func<string>, Action> logIt = (entityObject, describeComputation, wait) =>
            {
                log.Add($"{entityObject}.{describeComputation()}: STARTED");
                try
                {
                    wait();
                    log.Add($"{entityObject}.{describeComputation()}: PASSED");
                }
                catch (Exception)
                {
                    log.Add($"{entityObject}.{describeComputation()}: FAILED");
                    throw;
                }
            };
            Given.OpenedPageWithBody(@"
                <button>Click me!</button>
            ");
            var button = S("button").With(_hookWaitAction: logIt);
            var allButtons = SS("button").With(_hookWaitAction: logIt);
            var absent = S(".absent").With(_hookWaitAction: logIt);
            var allAbsent = SS(".absent").With(_hookWaitAction: logIt);

            allButtons.Should(Have.Count(1));
            button.Should(Have.ExactText("Click me!"));
            button.Click();
            button.With(clickByJs: true).Click();
            try { absent.Click(); } catch {}
            try { absent.Should(Have.Text("some")); } catch {}
            try { allAbsent.Should(Have.Count(1)); } catch {}
            try { absent.All(".child").Should(Have.Count(1)); } catch {}
            
            Assert.AreEqual(log,
                @"Browser.All(button).Should(Have.Count = 1): STARTED
                Browser.All(button).Should(Have.Count = 1): PASSED
                Browser.Element(button).Should(Have.ExactText(«Click me!»)): STARTED
                Browser.Element(button).Should(Have.ExactText(«Click me!»)): PASSED
                Browser.Element(button).ActualWebElement.Click(): STARTED
                Browser.Element(button).ActualWebElement.Click(): PASSED
                Browser.Element(button).JsClick(centerXOffset: 0, centerYOffset: 0): STARTED
                Browser.Element(button).JsClick(centerXOffset: 0, centerYOffset: 0): PASSED
                Browser.Element(.absent).ActualWebElement.Click(): STARTED
                Browser.Element(.absent).ActualWebElement.Click(): FAILED
                Browser.Element(.absent).Should(Have.TextContaining(«some»)): STARTED
                Browser.Element(.absent).Should(Have.TextContaining(«some»)): FAILED
                Browser.All(.absent).Should(Have.Count = 1): STARTED
                Browser.All(.absent).Should(Have.Count = 1): FAILED
                Browser.Element(.absent).All(.child).Should(Have.Count = 1): STARTED
                Browser.Element(.absent).All(.child).Should(Have.Count = 1): FAILED"
                .Split(Environment.NewLine).Select(item => item.Trim()).ToList()
            );
        }
    }
}

