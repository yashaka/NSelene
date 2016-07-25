using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Threading;
using NSelene;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support.PageObjects;

namespace NSeleneTests.SeleniumInteroperability
{
    [TestFixture]
    public class NSeleneBehaviorForPageFactoryIWebElementTests
    {
        IWebDriver driver;

        [OneTimeSetUp]
        public void initDriver()
        {
            driver = new FirefoxDriver();
        }

        [OneTimeTearDown]
        public void disposeDriver()
        {
            driver.Quit();
        }

        [SetUp]
        public void InitPageLoadingForTests()
        {
            Given.OpenedPageWithBody(@"
                <h2 id='second'>Heading 2</h2>"
                , driver
            );
            When.WithBodyTimedOut(@"
                <a href='#second' style='display:none'>go to Heading 2</a>
                <h2 id='second'>Heading 2</h2>"
                , 250, driver
            );
            When.ExecuteScriptWithTimeout(@"
                document.getElementsByTagName('a')[0].style = 'display:block';
                ",
                500, driver
            );
        }
        
        [Test]
        public void ClassicPageWithSeleniumDriverFailsToLocateDeferredElementByAppearance()
        {
            var page = new PageWithDeferredWebElement(driver);
            Assert.Throws(Is.TypeOf(typeof(NoSuchElementException)), () => {
                page.Element.Click();
            });
            Assert.IsFalse(driver.Url.Contains("second"));
        }

        [Test]
        public void ClassicPageWithSeleniumDriverFailsToLocateDeferredElementByVisibility()
        {
            var page = new PageWithDeferredWebElement(driver);
            Thread.Sleep(300);
            Assert.Throws(Is.TypeOf(typeof(TargetInvocationException))
                          .And.InnerException.TypeOf(typeof(ElementNotVisibleException)), () => {
                page.Element.Click();
            });
            Assert.IsFalse(driver.Url.Contains("second"));
        }

        [Test]
        public void ClassicPageWithDecoratedByNSeleneDriverLocatesDeferredElement()
        {
            var page = new PageWithDeferredWebElement(new SDriver(driver));
            page.Element.Click();
            Assert.IsTrue(driver.Url.Contains("second"));
        }

        class PageWithDeferredWebElement
        {
            [FindsBy(How = How.CssSelector, Using = "a")]
            public IWebElement Element;

            public PageWithDeferredWebElement(IWebDriver driver)
            {
                this.driver = driver;
                PageFactory.InitElements(this.driver, this);
            }

            IWebDriver driver;
        }
    }

    [TestFixture]
    public class NSeleneBehaviorForPageFactoryWebElementsListTests
    {
        IWebDriver driver;

        [OneTimeSetUp]
        public void initDriver()
        {
            driver = new FirefoxDriver();
        }

        [OneTimeTearDown]
        public void disposeDriver()
        {
            driver.Quit();
        }

        [SetUp]
        public void InitPageLoadingForTests()
        {
            Given.OpenedPageWithBody(@"
                <a href='#first'>go to Heading 1</a>
                <h2 id='first'>Heading 1</h2>
                <h2 id='second'>Heading 2</h2>"
                , driver
            );
            When.WithBodyTimedOut(@"
                <a href='#first'>go to Heading 1</a>
                <a href='#second' style='display:none'>go to Heading 2</a>
                <h2 id='first'>Heading 1</h2>
                <h2 id='second'>Heading 2</h2>"
                , 250, driver
            );
            When.ExecuteScriptWithTimeout(@"
                document.getElementsByTagName('a')[1].style = 'display:block';
                ",
                500, driver
            );
        }

        [Test]
        public void ClassicPageWithSeleniumDriverFailsToLocateDeferredElementOfListByAppearance()
        {
            var page = PageFactory.InitElements<PageWithDeferredWebElementOfList>(driver);
            Assert.Throws(Is.TypeOf(typeof(TargetInvocationException))
                          .And.InnerException.TypeOf(typeof(ArgumentOutOfRangeException)), () => {
                page.Elements[1].Click();
            });
            Assert.IsFalse(driver.Url.Contains("second"));
        }

        [Test]
        public void ClassicPageWithSeleniumDriverFailsToLocateDeferredElementOfListByVisibility()
        {
            var page = PageFactory.InitElements<PageWithDeferredWebElementOfList>(driver);
            Thread.Sleep(300);
            Assert.Throws(Is.TypeOf(typeof(ElementNotVisibleException)), () => {
                page.Elements[1].Click();
            });
            Assert.IsFalse(driver.Url.Contains("second"));
        }

        [Test]
        public void ClassicPageWithDecoratedByNSeleneDriverFailsToLocateDeferredElementByAppearance()
        {
            var page = PageFactory.InitElements<PageWithDeferredWebElementOfList>(new SDriver(driver));
            /* same as the following
            var page = PageFactory.InitElements<PageWithDeferredWebElementOfList>(new DefaultElementLocator(new SDriver(driver)));
             */
            Assert.Throws(Is.TypeOf(typeof(TargetInvocationException))
                          .And.InnerException.TypeOf(typeof(ArgumentOutOfRangeException)), () => {
                page.Elements[1].Click();
            });
            Assert.IsFalse(driver.Url.Contains("second"));
        }

        [Test]
        public void ClassicPageWithDecoratedByNSeleneDriverLocatesDeferredElementByVisibility()
        {
            var page = PageFactory.InitElements<PageWithDeferredWebElementOfList>(new SDriver(driver));
            /* same as the following
            var page = PageFactory.InitElements<PageWithDeferredWebElementOfList>(new DefaultElementLocator(new SDriver(driver)));
             */
            Thread.Sleep(300);
            page.Elements[1].Click();
            Assert.IsTrue(driver.Url.Contains("second"));
        }

        [Test]
        public void ClassicPageWithDecoratedByNSeleneDriverAndCustomLocatorLocatesDeferredElement()
        {
            var page = new PageWithDeferredWebElementOfListAndCustomFindsBy(new SDriver(driver));
            PageFactory.InitElements(page, new My.CustomElementLocator(new SDriver(driver)), new My.CustomPageObjectMemberDecorator());
            page.Elements[1].Click();
            Assert.IsTrue(driver.Url.Contains("second"));
        }

        class PageWithDeferredWebElementOfList
        {
            [FindsBy(How = How.CssSelector, Using = "a")]
            public IList<IWebElement> Elements;

            public PageWithDeferredWebElementOfList(IWebDriver driver) // Just emulating passing driver to constructor, so 
            {
                //this.driver = driver
            }

            //IWebDriver driver;
        }

        class PageWithDeferredWebElementOfListAndCustomFindsBy
        {
            [My.FindsBy(How = How.CssSelector, Using = "a")]
            public IList<IWebElement> Elements;

            public PageWithDeferredWebElementOfListAndCustomFindsBy(IWebDriver driver)
            {
                //this.driver = driver
            }

            //IWebDriver driver;
        }


    }

    namespace My
    {
        //
        // Code below was "copied&pasted" from selenium src with small changes in just two classes: 
        // CustomElementLocator & WebElementListProxy
        // reason: The implementation of WebElementListProxy was too coupled... 
        //

        public class CustomElementLocator : IElementLocator
        {
            private ISearchContext searchContext;

            public ISearchContext SearchContext {
                get {
                    return this.searchContext;
                }
            }

            public CustomElementLocator (ISearchContext searchContext)
            {
                this.searchContext = searchContext;
            }

            public IWebElement LocateElement (IEnumerable<By> bys)
            {
                if (bys == null) {
                    throw new ArgumentNullException ("bys", "List of criteria may not be null");
                }
                string text = null;
                foreach (By current in bys) {
                    try {
                        return this.searchContext.FindElement (current);
                    }
                    catch (NoSuchElementException) {
                        text = ((text == null) ? "Could not find element by: " : (text + ", or: ")) + current;
                    }
                }
                throw new NoSuchElementException (text);
            }

            public ReadOnlyCollection<IWebElement> LocateElements (IEnumerable<By> bys)
            {
                if (bys == null) {
                    throw new ArgumentNullException ("bys", "List of criteria may not be null");
                }
                return this.searchContext.FindElements (bys.First());
                /* over >
                List<IWebElement> list = new List<IWebElement> ();
                foreach (By current in bys) {
                    ReadOnlyCollection<IWebElement> collection = this.searchContext.FindElements (current);
                    list.AddRange (collection);
                }
                return list.AsReadOnly ();
                 */
            }
        }

        public class CustomPageObjectMemberDecorator : IPageObjectMemberDecorator
        {
            //
            // Static Fields
            //
            private static List<Type> interfacesToBeProxied;

            private static Type interfaceProxyType;

            //
            // Static Properties
            //
            private static Type InterfaceProxyType {
                get {
                    if (CustomPageObjectMemberDecorator.interfaceProxyType == null) {
                        CustomPageObjectMemberDecorator.interfaceProxyType = CustomPageObjectMemberDecorator.CreateTypeForASingleElement ();
                    }
                    return CustomPageObjectMemberDecorator.interfaceProxyType;
                }
            }

            private static List<Type> InterfacesToBeProxied {
                get {
                    if (CustomPageObjectMemberDecorator.interfacesToBeProxied == null) {
                        CustomPageObjectMemberDecorator.interfacesToBeProxied = new List<Type> ();
                        CustomPageObjectMemberDecorator.interfacesToBeProxied.Add (typeof(IWebElement));
                        CustomPageObjectMemberDecorator.interfacesToBeProxied.Add (typeof(ILocatable));
                        CustomPageObjectMemberDecorator.interfacesToBeProxied.Add (typeof(IWrapsElement));
                    }
                    return CustomPageObjectMemberDecorator.interfacesToBeProxied;
                }
            }

            //
            // Static Methods
            //
            protected static ReadOnlyCollection<By> CreateLocatorList (MemberInfo member)
            {
                if (member == null) {
                    throw new ArgumentNullException ("member", "memeber cannot be null");
                }
                Attribute[] customAttributes = Attribute.GetCustomAttributes (member, typeof(FindsBySequenceAttribute), true);
                bool flag = customAttributes.Length > 0;
                Attribute[] customAttributes2 = Attribute.GetCustomAttributes (member, typeof(FindsByAllAttribute), true);
                bool flag2 = customAttributes2.Length > 0;
                if (flag && flag2) {
                    throw new ArgumentException ("Cannot specify FindsBySequence and FindsByAll on the same member");
                }
                List<By> list = new List<By> ();
                Attribute[] customAttributes3 = Attribute.GetCustomAttributes (member, typeof(FindsByAttribute), true);
                if (customAttributes3.Length > 0) {
                    Array.Sort<Attribute> (customAttributes3);
                    Attribute[] array = customAttributes3;
                    for (int i = 0; i < array.Length; i++) {
                        Attribute attribute = array [i];
                        FindsByAttribute findsByAttribute = (FindsByAttribute)attribute;
                        if (findsByAttribute.Using == null) {
                            findsByAttribute.Using = member.Name;
                        }
                        list.Add (findsByAttribute.Finder);
                    }
                    if (flag) {
                        ByChained item = new ByChained (list.ToArray ());
                        list.Clear ();
                        list.Add (item);
                    }
                    if (flag2) {
                        ByAll item2 = new ByAll (list.ToArray ());
                        list.Clear ();
                        list.Add (item2);
                    }
                }
                return list.AsReadOnly ();
            }

            private static object CreateProxyObject (Type memberType, IElementLocator locator, IEnumerable<By> bys, bool cache)
            {
                object result = null;
                if (memberType == typeof(IList<IWebElement>)) {
                    using (List<Type>.Enumerator enumerator = CustomPageObjectMemberDecorator.InterfacesToBeProxied.GetEnumerator ()) {
                        while (enumerator.MoveNext ()) {
                            Type current = enumerator.Current;
                            Type type = typeof(IList<>).MakeGenericType (new Type[] {
                                current
                            });
                            if (type.Equals (memberType)) {
                                result = WebElementListProxy.CreateProxy (memberType, locator, bys, cache);
                                break;
                            }
                        }
                        return result;
                    }
                }
                if (!(memberType == typeof(IWebElement))) {
                    throw new ArgumentException ("Type of member '" + memberType.Name + "' is not IWebElement or IList<IWebElement>");
                }
                result = WebElementProxy.CreateProxy (CustomPageObjectMemberDecorator.InterfaceProxyType, locator, bys, cache);
                return result;
            }

            private static Type CreateTypeForASingleElement ()
            {
                AssemblyName assemblyName = new AssemblyName (Guid.NewGuid ().ToString ());
                AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly (assemblyName, AssemblyBuilderAccess.Run);
                ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule (assemblyName.Name);
                TypeBuilder typeBuilder = moduleBuilder.DefineType (typeof(IWebElement).FullName, TypeAttributes.Public | TypeAttributes.ClassSemanticsMask | TypeAttributes.Abstract);
                foreach (Type current in CustomPageObjectMemberDecorator.InterfacesToBeProxied) {
                    typeBuilder.AddInterfaceImplementation (current);
                }
                return typeBuilder.CreateType ();
            }

            protected static bool ShouldCacheLookup (MemberInfo member)
            {
                if (member == null) {
                    throw new ArgumentNullException ("member", "memeber cannot be null");
                }
                Type typeFromHandle = typeof(CacheLookupAttribute);
                return member.GetCustomAttributes (typeFromHandle, true).Length != 0 || member.DeclaringType.GetCustomAttributes (typeFromHandle, true).Length != 0;
            }

            //
            // Methods
            //
            public object Decorate (MemberInfo member, IElementLocator locator)
            {
                FieldInfo fieldInfo = member as FieldInfo;
                PropertyInfo propertyInfo = member as PropertyInfo;
                Type memberType = null;
                if (fieldInfo != null) {
                    memberType = fieldInfo.FieldType;
                }
                bool flag = false;
                if (propertyInfo != null) {
                    flag = propertyInfo.CanWrite;
                    memberType = propertyInfo.PropertyType;
                }
                if (fieldInfo == null & (propertyInfo == null || !flag)) {
                    return null;
                }
                IList<By> list = CustomPageObjectMemberDecorator.CreateLocatorList (member);
                if (list.Count > 0) {
                    bool cache = CustomPageObjectMemberDecorator.ShouldCacheLookup (member);
                    return CustomPageObjectMemberDecorator.CreateProxyObject (memberType, locator, list, cache);
                }
                return null;
            }
        }

        class WebElementListProxy : WebDriverObjectProxy
        {
            //
            // Fields
            //
            private IList<IWebElement> collection;

            //
            // Properties
            //
            private IList<IWebElement> ElementList {
                get {
                    if (!base.Cache || this.collection == null) {
                        this.collection = base.Locator.LocateElements (base.Bys);
                        /* CHANGED over > 
                        this.collection = new List<IWebElement> ();
                        this.collection.AddRange (base.Locator.LocateElements (base.Bys));
                         */
                    }
                    return this.collection;
                }
            }

            //
            // Constructors
            //
            private WebElementListProxy (Type typeToBeProxied, IElementLocator locator, IEnumerable<By> bys, bool cache) : base (typeToBeProxied, locator, bys, cache)
            {
            }

            //
            // Static Methods
            //
            public static object CreateProxy (Type classToProxy, IElementLocator locator, IEnumerable<By> bys, bool cacheLookups)
            {
                return new WebElementListProxy (classToProxy, locator, bys, cacheLookups).GetTransparentProxy ();
            }

            //
            // Methods
            //
            public override IMessage Invoke (IMessage msg)
            {
                IList<IWebElement> elementList = this.ElementList;
                return WebDriverObjectProxy.InvokeMethod (msg as IMethodCallMessage, elementList);
            }
        }

        sealed class WebElementProxy : WebDriverObjectProxy, IWrapsElement
        {
            //
            // Fields
            //
            private IWebElement cachedElement;

            //
            // Properties
            //
            private IWebElement Element {
                get {
                    if (!base.Cache || this.cachedElement == null) {
                        this.cachedElement = base.Locator.LocateElement (base.Bys);
                    }
                    return this.cachedElement;
                }
            }

            public IWebElement WrappedElement {
                get {
                    return this.Element;
                }
            }

            //
            // Constructors
            //
            private WebElementProxy (Type classToProxy, IElementLocator locator, IEnumerable<By> bys, bool cache) : base (classToProxy, locator, bys, cache)
            {
            }

            //
            // Static Methods
            //
            public static object CreateProxy (Type classToProxy, IElementLocator locator, IEnumerable<By> bys, bool cacheLookups)
            {
                return new WebElementProxy (classToProxy, locator, bys, cacheLookups).GetTransparentProxy ();
            }

            //
            // Methods
            //
            public override IMessage Invoke (IMessage msg)
            {
                IWebElement element = this.Element;
                IMethodCallMessage methodCallMessage = msg as IMethodCallMessage;
                if (typeof(IWrapsElement).IsAssignableFrom ((methodCallMessage.MethodBase as MethodInfo).DeclaringType)) {
                    return new ReturnMessage (element, null, 0, methodCallMessage.LogicalCallContext, methodCallMessage);
                }
                return WebDriverObjectProxy.InvokeMethod (methodCallMessage, element);
            }
        }

        abstract class WebDriverObjectProxy : RealProxy
        {
            //
            // Fields
            //
            private readonly IElementLocator locator;

            private readonly IEnumerable<By> bys;

            private readonly bool cache;

            //
            // Properties
            //
            protected IEnumerable<By> Bys {
                get {
                    return this.bys;
                }
            }

            protected bool Cache {
                get {
                    return this.cache;
                }
            }

            protected IElementLocator Locator {
                get {
                    return this.locator;
                }
            }

            //
            // Constructors
            //
            protected WebDriverObjectProxy (Type classToProxy, IElementLocator locator, IEnumerable<By> bys, bool cache) : base (classToProxy)
            {
                this.locator = locator;
                this.bys = bys;
                this.cache = cache;
            }

            //
            // Static Methods
            //
            protected static ReturnMessage InvokeMethod (IMethodCallMessage msg, object representedValue)
            {
                if (msg == null) {
                    throw new ArgumentNullException ("msg", "The message containing invocation information cannot be null");
                }
                MethodInfo methodInfo = msg.MethodBase as MethodInfo;
                return new ReturnMessage (methodInfo.Invoke (representedValue, msg.Args), null, 0, msg.LogicalCallContext, msg);
            }
        }

        [AttributeUsage (AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
        public sealed class FindsByAttribute : Attribute, IComparable
        {
            //
            // Fields
            //
            private By finder;

            //
            // Properties
            //
            public Type CustomFinderType {
                get;
                set;
            }

            internal By Finder {
                get {
                    if (this.finder == null) {
                        this.finder = ByFactory.From (this);
                    }
                    return this.finder;
                }
                set {
                    this.finder = value;
                }
            }

            [DefaultValue (How.Id)]
            public How How {
                get;
                set;
            }

            [DefaultValue (0)]
            public int Priority {
                get;
                set;
            }

            public string Using {
                get;
                set;
            }

            //
            // Methods
            //
            public int CompareTo (object obj)
            {
                if (obj == null) {
                    throw new ArgumentNullException ("obj", "Object to compare cannot be null");
                }
                FindsByAttribute findsByAttribute = obj as FindsByAttribute;
                if (findsByAttribute == null) {
                    throw new ArgumentException ("Object to compare must be a FindsByAttribute", "obj");
                }
                if (this.Priority != findsByAttribute.Priority) {
                    return this.Priority - findsByAttribute.Priority;
                }
                return 0;
            }

            public override bool Equals (object obj)
            {
                if (obj == null) {
                    return false;
                }
                FindsByAttribute findsByAttribute = obj as FindsByAttribute;
                return !(findsByAttribute == null) && findsByAttribute.Priority == this.Priority && !(findsByAttribute.Finder != this.Finder);
            }

            public override int GetHashCode ()
            {
                return this.Finder.GetHashCode ();
            }

            //
            // Operators
            //
            public static bool operator == (FindsByAttribute one, FindsByAttribute two) {
                return object.ReferenceEquals (one, two) || (one != null && two != null && one.Equals (two));
            }

            public static bool operator > (FindsByAttribute one, FindsByAttribute two) {
                if (one == null) {
                    throw new ArgumentNullException ("one", "Object to compare cannot be null");
                }
                return one.CompareTo (two) > 0;
            }

            public static bool operator != (FindsByAttribute one, FindsByAttribute two) {
                return !(one == two);
            }

            public static bool operator < (FindsByAttribute one, FindsByAttribute two) {
                if (one == null) {
                    throw new ArgumentNullException ("one", "Object to compare cannot be null");
                }
                return one.CompareTo (two) < 0;
            }
        }

        static class ByFactory
        {
            //
            // Static Methods
            //
            public static By From (FindsByAttribute attribute)
            {
                How how = attribute.How;
                string @using = attribute.Using;
                switch (how) {
                    case How.Id:
                    return By.Id (@using);
                    case How.Name:
                    return By.Name (@using);
                    case How.TagName:
                    return By.TagName (@using);
                    case How.ClassName:
                    return By.ClassName (@using);
                    case How.CssSelector:
                    return By.CssSelector (@using);
                    case How.LinkText:
                    return By.LinkText (@using);
                    case How.PartialLinkText:
                    return By.PartialLinkText (@using);
                    case How.XPath:
                    return By.XPath (@using);
                    case How.Custom: {
                        if (attribute.CustomFinderType == null) {
                            throw new ArgumentException ("Cannot use How.Custom without supplying a custom finder type");
                        }
                        if (!attribute.CustomFinderType.IsSubclassOf (typeof(By))) {
                            throw new ArgumentException ("Custom finder type must be a descendent of the By class");
                        }
                        ConstructorInfo constructor = attribute.CustomFinderType.GetConstructor (new Type[] {
                            typeof(string)
                        });
                        if (constructor == null) {
                            throw new ArgumentException ("Custom finder type must expose a public constructor with a string argument");
                        }
                        return constructor.Invoke (new object[] {
                            @using
                            }) as By;
                    }
                    default:
                    throw new ArgumentException (string.Format (CultureInfo.InvariantCulture, "Did not know how to construct How from how {0}, using {1}", new object[] {
                        how,
                        @using
                        }));
                }
            }
        }
    }
}

