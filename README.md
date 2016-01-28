# NSelene

So, Here it is... A very draft version, though already published to nuget gallery https://www.nuget.org/packages/NSelene/0.0.0.1.

Sources: https://www.nuget.org/packages/NSelene/0.0.0.1

The version is not tested:)
The project has no tests, no docs yet:)

But has an example tests project: https://github.com/yashaka/NSelene/blob/master/NSeleneExamples

Below you can find a short overview:

NSelene has no automatic driver management, you have to set it up manually, e.g. like here: 
```csharp
        [SetUp]
        public void SetupTest()
        {
            SetDriver (new FirefoxDriver ());
        }

        [TearDown]
        public void TeardownTest()
        {
            GetDriver ().Quit ();
        }
```
(https://github.com/yashaka/NSelene/blob/master/NSeleneExamples/BaseTest.cs)

Tests may look like this, via PageObject implemented in a "Procedural way":

```csharp
        [TestFixture ()]
        public class Test : BaseTest
        {
            [Test ()]
            public void FilterTasks ()
            {
                Tasks.Visit ();

                Tasks.Add ("a", "b", "c");
                Tasks.ShouldBe ("a", "b", "c");

                Tasks.Toggle ("b"); 

                Tasks.FilterActive ();
                Tasks.ShouldBe ("a", "c");

                Tasks.FilterCompleted ();
                Tasks.ShouldBe ("b");
            }
        }
```
(https://github.com/yashaka/NSelene/blob/master/NSeleneExamples/TodoMVC/Tests.cs)

where procedural "PageObject" may look like this:

```csharp
        public static class Tasks
        {
            public static SCollection List = SS ("#todo-list>li"); 

            public static void Visit()
            {
                Open ("https://todomvc4tasj.herokuapp.com/");
            }

            public static void FilterActive ()
            {
                S (By.LinkText ("Active")).Click ();
            }

            public static void FilterCompleted ()
            {
                S (By.LinkText ("Completed")).Click ();
            }

            public static void Add(params string[] taskTexts)
            {
                foreach (var text in taskTexts) 
                {
                    S ("#new-todo").SetValue (text).PressEnter ();
                }
            }

            public static void Toggle (string taskText)
            {
                List.FindBy (Have.ExactText (taskText)).Find (".toggle").Click ();
            }

            public static void ShouldBe(params string[] names)
            {
                List.FilterBy (Be.Visible).Should (Have.Texts (names));
            }
        }
```
So... 
The ported things are: 
- Selenide.$
- SelenideElement#should
- SelenideElement#find
- Selenide.$$
- EllementsCollection#should
- EllementsCollection#findBy
- EllementsCollection#filterBy
- Conditions.visible, Condition.exactText
- CollectionConditions.texts

The first thing I plan to do - is to implement SElement (aka SelenideElement) as something more smart than C# delegate. Because current implementation lacks abilities to support e.g. screenshots in error messages... 

Current implementation is also kind of "experimental one". It is implemented completely in non-OOP way :)
The main answer  to "Why?" - is speed. I implemented it in a few evenings. 
First I will proceed with this style of implementation (using a lot of "extension methods") just for fun and R&D :) And will switch to common OOP implementation via wrapping WebElement (like Selenide does) if needed :)

Since holidays are coming... And my schedule is very tough, I expect to make "more useful" version somewhere till the end of Winter/start of Spring. 

Feel free to share your thoughts and file an issue on github if you need something.