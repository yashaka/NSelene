using OpenQA.Selenium;
using System.Collections.Generic;
using System.Linq;
using NSelene.Conditions;
using System.Collections.ObjectModel;
using System;
using System.Threading;
using System.Collections;

namespace NSelene
{
    public interface WrapsWebElementsCollection
    {
        IReadOnlyCollection<IWebElement> ActualWebElements { get; }
    }

    public sealed class SCollection 
        : WrapsWebElementsCollection, IReadOnlyList<SElement>, IReadOnlyCollection<SElement>, IList<SElement>, ICollection<SElement>, IEnumerable<SElement>, IEnumerable
    {
        readonly SLocator<IReadOnlyCollection<IWebElement>> locator;

        readonly IWebDriver driver;

        public SCollection(SLocator<IReadOnlyCollection<IWebElement>> locator, IWebDriver driver)
        {
            this.locator = locator;
            this.driver = driver;
        }

        public SCollection(By byLocator, IWebDriver driver) 
            : this(new SearchContextWebElementsCollectionSLocator(byLocator, driver), driver) {}

        public SCollection(By byLocator) 
            : this(new SearchContextWebElementsCollectionSLocator(byLocator, PrivateConfiguration.SharedDriver), PrivateConfiguration.SharedDriver) {}

        public SCollection(IList<IWebElement> pageFactoryElements, IWebDriver driver)
            : this(new WrappedWebElementsCollectionSLocator(pageFactoryElements), driver) {}
        
        public IReadOnlyCollection<IWebElement> ActualWebElements
        {
            get {
                return locator.Find();
            }
        }

        SLocator<IReadOnlyCollection<IWebElement>> SLocator 
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

        public SElement FindBy(Condition<SElement> condition)
        {
            return new SElement(new SCollectionWebElementByConditionSLocator(condition, this, this.driver), this.driver);
        }

        public SCollection FilterBy(Condition<SElement> condition)
        {
            return new SCollection(new SCollectionFilteredWebElementsCollectionSLocator(condition, this, this.driver), this.driver);
        }

        public ReadOnlyCollection<IWebElement> ToReadOnlyWebElementsCollection()
        {
            return new ReadOnlyCollection<IWebElement>(
                this.ActualWebElements
                .Select( element => new SElement(element, this.driver))
                .Select( selement => (IWebElement) selement).ToList()
            );
        }

        //
        // IReadOnlyList
        //

        public SElement this [int index] {
            get {
                return new SElement(new SCollectionWebElementByIndexSLocator(index, this), this.driver);
            }
        }

        //
        // IReadOnlyCollection
        //

        public int Count
        {
            get {
                return this.ActualWebElements.Count;
            }
        }

        //
        // IEnumerator
        //

        //TODO: is it stable enought in context of "ajax friendly"?
        public IEnumerator<SElement> GetEnumerator ()
        {
            //TODO: is it lazy, seems like not... because of ToList() conversion? should it be lazy?
            return new ReadOnlyCollection<SElement>(
                this.ActualWebElements.Select(webelement => new SElement(webelement, this.driver)).ToList()).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator ()
        {
            return GetEnumerator();
        }

        //
        // IList
        //

        bool ICollection<SElement>.IsReadOnly {
            get {
                return true;
            }
        }

        SElement IList<SElement>.this [int index] {
            get {
                return this[index];
            }

            set {
                throw new NotImplementedException ();
            }
        }

        int IList<SElement>.IndexOf (SElement item)
        {
            throw new NotImplementedException ();
        }

        void IList<SElement>.Insert (int index, SElement item)
        {
            throw new NotImplementedException ();
        }

        void IList<SElement>.RemoveAt (int index)
        {
            throw new NotImplementedException ();
        }

        void ICollection<SElement>.Add (SElement item)
        {
            throw new NotImplementedException ();
        }

        void ICollection<SElement>.Clear ()
        {
            throw new NotImplementedException ();
        }

        bool ICollection<SElement>.Contains (SElement item)
        {
            throw new NotImplementedException ();
        }

        void ICollection<SElement>.CopyTo (SElement [] array, int arrayIndex)
        {
            throw new NotImplementedException ();
        }

        bool ICollection<SElement>.Remove (SElement item)
        {
            throw new NotImplementedException ();
        }

        //
        // Obsolete Methods
        //

        [Obsolete("GetCount is deprecated and will be removed in next version, please use Count property instead.")]
        public int GetCount()
        {
            return  this.ActualWebElements.Count; // TODO: should we count only visible elements? or all?
        }

        [Obsolete("Get is deprecated and will be removed in next version, please use indexer [] instead.")]
        public SElement Get(int index)
        {
            return this.ElementAt(index);
        }

        [Obsolete("ElementAt is deprecated and will be removed in next version, please use indexer [] instead.")]
        public SElement ElementAt(int index)
        {
            return new SElement(new SCollectionWebElementByIndexSLocator(index, this), this.driver);
        }
    }
}
