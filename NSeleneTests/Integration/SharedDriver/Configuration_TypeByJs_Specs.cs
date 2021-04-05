using NUnit.Framework;
using static NSelene.Selene;
using OpenQA.Selenium;

namespace NSelene.Tests.Integration.SharedDriver.ConfigurationSpec
{
    using System;
    using Harness;

    [TestFixture]
    public class Configuration_TypeByJs_Specs : BaseTest
    {

        [Test]
        public void Type_ViaElementCustomizedToJs_SetsItFasterThanNormalType()
        {
            Configuration.TypeByJs = false;
            Given.OpenedPageWithBody(@"
                <input id='field1' value='prefix to append to '>
                <input id='field2' value='prefix to append to '>
            ");
            var beforeType = DateTime.Now;
            S("#field1").Type(new string('*', 100));
            var afterType = DateTime.Now;
            var setValueTime = afterType - beforeType;

            var beforeJsSetValue = afterType;
            S("#field2").With(typeByJs: true).Type(new string('*', 100));
            var afterJsSetValue = DateTime.Now;
            
            // THEN
            S("#field1").Should(Have.Value("prefix to append to " + new string('*', 100)));
            S("#field2").Should(Have.Value("prefix to append to " + new string('*', 100)));
            
            var jsTime = afterJsSetValue - beforeJsSetValue;
            Assert.Less(jsTime, setValueTime / 2.5);
        }

        [Test]
        public void Type_ViaConfiguredGloballyToJs_SetsItFasterThanNormalType()
        {
            Configuration.TypeByJs = false;
            Given.OpenedPageWithBody(@"
                <input id='field1' value='prefix to append to '>
                <input id='field2' value='prefix to append to '>
            ");
            var beforeType = DateTime.Now;
            S("#field1").Type(new string('*', 100));
            var afterType = DateTime.Now;
            var setValueTime = afterType - beforeType;

            var beforeJsSetValue = afterType;
            Configuration.TypeByJs = true;
            S("#field2").Type(new string('*', 100));
            var afterJsSetValue = DateTime.Now;
            
            // THEN
            S("#field1").Should(Have.Value("prefix to append to " + new string('*', 100)));
            S("#field2").Should(Have.Value("prefix to append to " + new string('*', 100)));
            
            var jsTime = afterJsSetValue - beforeJsSetValue;
            Assert.Less(jsTime, setValueTime / 2.5);
        }
    }
}

