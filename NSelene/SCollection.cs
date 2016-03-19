using OpenQA.Selenium;
using System.Collections.Generic;
using System.Linq;
using NSelene.Conditions;
using System.Collections.ObjectModel;

namespace NSelene
{
    public delegate IReadOnlyCollection<IWebElement> SCollection();

    public static partial class Utils
    {
        public static SCollection SS(By locator)
        {
            return () => FindAll(locator);
        }

        public static SCollection SS(string cssSelector)
        {
            return SS(By.CssSelector(cssSelector));
        }
    }

    public static class SCollectionExtensions
    {

        public static SCollection Should(this SCollection elements, Condition<SCollection> condition)
        {
            return Utils.WaitFor(elements, condition);
        }

        public static SCollection ShouldNot(this SCollection elements, Condition<SCollection> condition)
        {
            return Utils.WaitForNot(elements, condition);
        }

        public static SElement Get(this SCollection elements, int index)
        {
            return elements.ElementAt(index);
        }

//        public static SElement First(this SCollection elements)
//        {
//            return elements.ElementAt(0);
//        }

        public static SElement ElementAt(this SCollection elements, int index)
        {
            return () => elements.Should(Have.CountAtLeast(index+1))().ElementAt(index);
        }

        public static SElement FindBy(this SCollection elements, Condition<SElement> condition)
        {
            return () =>
            {
                var found = elements().ToList().Find(element => condition.Apply(() => element));
                if (found == null) 
                {
                    throw new NotFoundException("element was not found by condition" + condition);
                }
                return found;
            };
        }

        public static SCollection FilterBy(this SCollection elements, Condition<SElement> condition)
        {
            return () => new ReadOnlyCollection<IWebElement>(// TODO: don't we need here ReadONlyCollection<SElement> ?
                elements().Where(element => condition.Apply(() => element)).ToList()
            );
        }

        public static int GetCount(this SCollection elements)
        {
            return  elements().Count; // TODO: should we count only visible elements? or all?
        }

    }
}
