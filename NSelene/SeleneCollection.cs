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

    public sealed class SeleneCollection 
        :  WrapsWebElementsCollection, IReadOnlyList<SeleneElement>, IReadOnlyCollection<SeleneElement>, IList<SeleneElement>, IList<IWebElement>, ICollection<SeleneElement>, IEnumerable<SeleneElement>, IEnumerable
    {
        readonly SeleneLocator<ReadOnlyCollection<IWebElement>> locator;

        readonly SeleneDriver driver;

        internal SeleneCollection(SeleneLocator<ReadOnlyCollection<IWebElement>> locator, SeleneDriver driver)
        {
            this.locator = locator;
            this.driver = driver;
        }

        internal SeleneCollection(By byLocator, SeleneDriver driver) 
            : this(new SearchContextWebElementsCollectionSLocator(byLocator, driver), driver) {}

        internal SeleneCollection(By byLocator) 
            : this(new SearchContextWebElementsCollectionSLocator(byLocator, Selene.SharedBrowser), Selene.SharedBrowser) {}

        internal SeleneCollection(IList<IWebElement> elementsListToWrap, IWebDriver driver)
            : this(new WrappedWebElementsCollectionSLocator(elementsListToWrap), new SeleneDriver(driver)) {}
        
        public ReadOnlyCollection<IWebElement> ActualWebElements
        {
            get {
                return locator.Find();
            }
        }

        SeleneLocator<ReadOnlyCollection<IWebElement>> SLocator 
        {
            get {
                return this.locator;
            }
        }

        public override string ToString()
        {
            return this.locator.Description;
        }

        public SeleneCollection Should(Condition<SeleneCollection> condition)
        {
            return Selene.WaitFor(this, condition);
        }

        [Obsolete("Use the negative condition instead, e.g. Should(Have.No.Count(0))")]
        public SeleneCollection ShouldNot(Condition<SeleneCollection> condition)
        {
            return Selene.WaitForNot(this, condition);
        }

        public SeleneElement FindBy(Condition<SeleneElement> condition)
        {
            return new SeleneElement(new SCollectionWebElementByConditionSLocator(condition, this, this.driver), this.driver);
        }

        public SeleneCollection FilterBy(Condition<SeleneElement> condition)
        {
            return new SeleneCollection(new SCollectionFilteredWebElementsCollectionSLocator(condition, this, this.driver), this.driver);
        }

        public ReadOnlyCollection<IWebElement> ToReadOnlyWebElementsCollection()
        {
            return new ReadOnlyCollection<IWebElement>(this);
        }

        //
        // IReadOnlyList
        //

        public SeleneElement this [int index] {
            get {
                return new SeleneElement(new SCollectionWebElementByIndexSLocator(index, this), this.driver);
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
        public IEnumerator<SeleneElement> GetEnumerator ()
        {
            //TODO: is it lazy? seems like not... because of ToList() conversion? should it be lazy?
            return new ReadOnlyCollection<SeleneElement>(
                this.ActualWebElements.Select(webelement => new SeleneElement(webelement, this.driver)).ToList()).GetEnumerator();
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

        bool ICollection<SeleneElement>.IsReadOnly {
            get {
                return true;
            }
        }

        SeleneElement IList<SeleneElement>.this [int index] {
            get {
                return this[index];
            }

            set {
                throw new NotImplementedException ();
            }
        }

        int IList<SeleneElement>.IndexOf (SeleneElement item)
        {
            throw new NotImplementedException ();
        }

        void IList<SeleneElement>.Insert (int index, SeleneElement item)
        {
            throw new NotImplementedException ();
        }

        void IList<SeleneElement>.RemoveAt (int index)
        {
            throw new NotImplementedException ();
        }

        void ICollection<SeleneElement>.Add (SeleneElement item)
        {
            throw new NotImplementedException ();
        }

        void ICollection<SeleneElement>.Clear ()
        {
            throw new NotImplementedException ();
        }

        bool ICollection<SeleneElement>.Contains (SeleneElement item)
        {
            throw new NotImplementedException ();
        }

        void ICollection<SeleneElement>.CopyTo (SeleneElement [] array, int arrayIndex)
        {
            throw new NotImplementedException ();
        }

        bool ICollection<SeleneElement>.Remove (SeleneElement item)
        {
            throw new NotImplementedException ();
        }
    }



    namespace Support.Extensions 
    {
        public static class SeleneCollectionExtensions 
        {
            public static SeleneCollection AssertTo(this SeleneCollection selements, Condition<SeleneCollection> condition)
            {
                return selements.Should(condition);
            }

            [Obsolete("Use the negative condition instead, e.g. AssertTo(Have.No.Count(0))")]
            public static SeleneCollection AssertToNot(this SeleneCollection selements, Condition<SeleneCollection> condition)
            {
                return selements.ShouldNot(condition);
            }
        }
    }
}
