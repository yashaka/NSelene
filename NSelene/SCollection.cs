using OpenQA.Selenium;
using System.Collections.Generic;
using System.Linq;
using NSelene.Conditions;
using System.Collections.ObjectModel;
using System;
using System.Threading;

namespace NSelene
{
    public interface FindsWebElementsCollection
    {
        IReadOnlyCollection<IWebElement> ActualWebElements { get; }
    }

    // TODO: implement IEnumerable, IEnumerable<T>
    public sealed class SCollection : FindsWebElementsCollection
    {
        readonly SLocator<IReadOnlyCollection<IWebElement>> locator;

        readonly IWebDriver driver;

        public SCollection(SLocator<IReadOnlyCollection<IWebElement>> locator, IWebDriver driver)
        {
            this.locator = locator;
            this.driver = driver;
        }

        public SCollection(By byLocator, IWebDriver driver) 
            : this(new DriverWebElementsCollectionSLocator(byLocator, driver), driver) {}

        public SCollection(By byLocator) 
            : this(new DriverWebElementsCollectionSLocator(byLocator, SharedThreadLocalDriver.Instance), SharedThreadLocalDriver.Instance) {}

        public SCollection(IList<IWebElement> pageFactoryElements, IWebDriver driver)
            : this(new WrappedWebElementsCollectionSLocator(pageFactoryElements), driver) {}

        // todo: would it be better to use method GetActualWebElements() instead?
        public IReadOnlyCollection<IWebElement> ActualWebElements
        {
            get {
                return locator.Find();
            }
        }

        public SLocator<IReadOnlyCollection<IWebElement>> SLocator 
        {
            get {
                return this.locator;
            }
        }

        public override string ToString()
        {
            return this.locator.Description;
        }

        public SCollection Should(Condition<SCollection> condition)
        {
            return Utils.WaitFor(this, condition);
        }

        public SCollection AssertTo(Condition<SCollection> condition)
        {
            return this.Should(condition);
        }

        public SCollection ShouldNot(Condition<SCollection> condition)
        {
            return Utils.WaitForNot(this, condition);
        }

        public SCollection AssertToNot(Condition<SCollection> condition)
        {
            return this.ShouldNot(condition);
        }

        public SElement Get(int index)
        {
            return this.ElementAt(index);
        }

        public SElement ElementAt(int index)
        {
            return new SElement(new SCollectionWebElementByIndexSLocator(index, this), this.driver);
        }

        public SElement FindBy(Condition<SElement> condition)
        {
            return new SElement(new SCollectionWebElementByConditionSLocator(condition, this, this.driver), this.driver);
        }

        public SCollection FilterBy(Condition<SElement> condition)
        {
            return new SCollection(new SCollectionFilteredWebElementsCollectionSLocator(condition, this, this.driver), this.driver);
        }

        public int Count
        {
            get {
                return this.ActualWebElements.Count;
            }
        }

        [Obsolete("GetCount is deprecated and will be removed in next version, please use Count property instead.")]
        public int GetCount()
        {
            return  this.ActualWebElements.Count; // TODO: should we count only visible elements? or all?
        }
    }
}
