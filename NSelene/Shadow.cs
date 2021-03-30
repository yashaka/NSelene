using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace NSelene
{
    public static class Shadow
    {
        public static IWebElement FindShadow(this SeleneElement element, string cssOrXPathSelector)
        {
            // value 'body' is temporary hardcoded 
            var e = Selene.ExecuteScript($"return document.querySelector('body').shadowRoot.querySelector('{cssOrXPathSelector}');");
            return e as IWebElement;
        }
    }
}
