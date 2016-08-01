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
        ReadOnlyCollection<IWebElement> ActualWebElements { get; }
    }

    public sealed class SCollection 
        : WrapsWebElementsCollection, IReadOnlyList<SElement>, IReadOnlyCollection<SElement>, IList<SElement>, IList<IWebElement>, ICollection<SElement>, IEnumerable<SElement>, IEnumerable
    {
        readonly SLocator<ReadOnlyCollection<IWebElement>> locator;

        readonly SDriver driver;

        internal SCollection(SLocator<ReadOnlyCollection<IWebElement>> locator, SDriver driver)
        {
            this.locator = locator;
            this.driver = driver;
        }

        internal SCollection(By byLocator, SDriver driver) 
            : this(new SearchContextWebElementsCollectionSLocator(byLocator, driver), driver) {}

        internal SCollection(By byLocator) 
            : this(new SearchContextWebElementsCollectionSLocator(byLocator, PrivateConfiguration.SharedDriver), PrivateConfiguration.SharedDriver) {}

        internal SCollection(IList<IWebElement> elementsListToWrap, IWebDriver driver)
            : this(new WrappedWebElementsCollectionSLocator(elementsListToWrap), new SDriver(driver)) {}
        
        public ReadOnlyCollection<IWebElement> ActualWebElements
        {
            get {
                return locator.Find();
            }
        }

        SLocator<ReadOnlyCollection<IWebElement>> SLocator 
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
            return Selene.WaitFor(this, condition);
        }

        public SCollection ShouldNot(Condition<SCollection> condition)
        {
            return Selene.WaitForNot(this, condition);
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
            return new ReadOnlyCollection<IWebElement>(this);
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
            //TODO: is it lazy? seems like not... because of ToList() conversion? should it be lazy?
            return new ReadOnlyCollection<SElement>(
                this.ActualWebElements.Select(webelement => new SElement(webelement, this.driver)).ToList()).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator ()
        {
            return GetEnumerator();
        }

        //
        // IList<IWebElement> methods
        //

        int ICollection<IWebElement>.Count {
            get {
                return this.Count;
            }
        }

        bool ICollection<IWebElement>.IsReadOnly {
            get {
                return true;
            }
        }

        IWebElement IList<IWebElement>.this [int index] {
            get {
                return this[index];
            }

            set {
                throw new NotImplementedException ();
            }
        }

        int IList<IWebElement>.IndexOf (IWebElement item)
        {
            throw new NotImplementedException ();
        }

        void IList<IWebElement>.Insert (int index, IWebElement item)
        {
            throw new NotImplementedException ();
        }

        void IList<IWebElement>.RemoveAt (int index)
        {
            throw new NotImplementedException ();
        }

        void ICollection<IWebElement>.Add (IWebElement item)
        {
            throw new NotImplementedException ();
        }

        void ICollection<IWebElement>.Clear ()
        {
            throw new NotImplementedException ();
        }

        bool ICollection<IWebElement>.Contains (IWebElement item)
        {
            throw new NotImplementedException ();
        }

        void ICollection<IWebElement>.CopyTo (IWebElement [] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException("arrayIndex");
            if (array.Rank > 1)
                throw new ArgumentException("array is multidimensional.");
            if (array.Length - arrayIndex < Count)
                throw new ArgumentException("Not enough elements after index in the destination array.");

            for (int i = 0; i < Count; ++i)
                array.SetValue(this[i], i + arrayIndex);
        }

        bool ICollection<IWebElement>.Remove (IWebElement item)
        {
            throw new NotImplementedException ();
        }

        IEnumerator<IWebElement> IEnumerable<IWebElement>.GetEnumerator ()
        {
            return this.GetEnumerator();
        }

        //
        // IList<SElement> methods
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



    namespace Support.Extensions 
    {
        public static class SCollectionExtensions 
        {
            public static SCollection AssertTo(this SCollection selements, Condition<SCollection> condition)
            {
                return selements.Should(condition);
            }

            public static SCollection AssertToNot(this SCollection selements, Condition<SCollection> condition)
            {
                return selements.ShouldNot(condition);
            }
        }
    }
}
