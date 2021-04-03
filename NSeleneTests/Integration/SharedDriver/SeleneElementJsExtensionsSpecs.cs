using NUnit.Framework;
using static NSelene.Selene;
using NSelene.Tests.Integration.SharedDriver.Harness;
using NSelene.Conditions;
using OpenQA.Selenium;
using NSelene.Support.SeleneElementJsExtensions;
using System;

namespace NSelene.Tests.Integration.SharedDriver
{
    [TestFixture]
    public class SeleneElementJsExtensionsSpecs : BaseTest
    {
        private const string ELEMENT_IN_VIEW = @"
                var windowHeight = window.innerHeight;
                var height = document.documentElement.clientHeight;
                var r = arguments[0].getClientRects()[0]; 

                return r.top > 0
                    ? r.top <= windowHeight 
                    : (r.bottom > 0 && r.bottom <= windowHeight);
                ";

        [Test]
        public void JsScrollIntoView()
        {
            Given.OpenedPageWithBody("<input style='margin-top:100cm;' type='text' value='ku ku'/>");
            SeleneElement element = S("input");
            Selene.WaitTo(Have.No.JSReturnedTrue(ELEMENT_IN_VIEW, element.ActualWebElement));

            element.JsScrollIntoView();

            Selene.WaitTo(Have.JSReturnedTrue(ELEMENT_IN_VIEW, element.ActualWebElement));
        }

        [Test]
        public void JsClick_ClicksOnHiddenElement()
        {
            Given.OpenedPageWithBody(@"
                <a href='#second' style='display:none'>go to Heading 2</a>
                <h2 id='second'>Heading 2</h2>"
            );

            S("a").JsClick();
            
            Assert.IsTrue(Configuration.Driver.Url.Contains("second"));
        }

        [Test]
        public void JsSetValue_SetsItFasterThanSendKeysLikeType()
        {
            Given.OpenedPageWithBody(@"
                <input id='field1'>
                <input id='field2'>
            ");

            var beforeType = DateTime.Now;
            S("#field1").Type(new string('*', 100));
            var afterType = DateTime.Now;
            var typeTime = afterType - beforeType;
            var beforeJsSetValue = afterType;
            S("#field2").JsSetValue(new string('*', 100));
            var afterJsSetValue = DateTime.Now;
            var jsTime = afterJsSetValue - beforeJsSetValue;
            
            S("#field1").Should(Have.Value(new string('*', 100)));
            S("#field2").Should(Have.Value(new string('*', 100)));
            
            Assert.Less(jsTime, typeTime / 3);
        }

        // TODO: make it work and pass:)
        // [Test]
        // public void JsClick_ClicksWithOffsetFromCenter()
        // {
        //     Given.OpenedPageWithBody(@"
        //         <a id='to-first' href='#first'>go to Heading 1</a>
        //         </br>
        //         <a id='to-second' href='#second'>go to Heading 2</a>
        //         <h2 id='second'>Heading 1</h2>
        //         <h2 id='second'>Heading 2</h2>"
        //     );

        //     var toSecond = S("#to-second");
            
        //     toSecond.JsClick(0, -toSecond.Size.Height);
            
        //     Assert.IsTrue(Configuration.Driver.Url.Contains("first"));
        // }
    }
}
