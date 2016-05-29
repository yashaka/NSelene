# NSelene - Selenide "from scratch port" to .NET (`C#`, etc.)
A Tool created specifically for Web UI Test Automation

So, Here it is... A base stabilized version, published to nuget gallery https://www.nuget.org/packages/NSelene

The version was covered with integration tests. The coverage was not complete in context of all probable use cases, but the "integration aspect" makes it good enough.

You can use tests as docs ;)

There is also an example tests project: https://github.com/yashaka/NSelene/blob/master/NSeleneExamples

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
        [TestFixture]
        public class Test : BaseTest
        {
            [Test]
            public void FilterTasks()
            {
                Tasks.Visit();

                Tasks.Add("a", "b", "c");
                Tasks.ShouldBe("a", "b", "c");

                Tasks.Toggle("b"); 

                Tasks.FilterActive();
                Tasks.ShouldBe("a", "c");

                Tasks.FilterCompleted();
                Tasks.ShouldBe("b");
            }
        }
```
(https://github.com/yashaka/NSelene/blob/master/NSeleneExamples/TodoMVC/Tests.cs)

where procedural "PageObject" aka "PageModule" may look like this:

```csharp
        public static class Tasks
        {
            public static SCollection List = SS("#todo-list>li"); 

            public static void Visit()
            {
                Open("https://todomvc4tasj.herokuapp.com/");
            }

            public static void FilterActive()
            {
                S (By.LinkText("Active")).Click();
            }

            public static void FilterCompleted()
            {
                S (By.LinkText("Completed")).Click();
            }

            public static void Add(params string[] taskTexts)
            {
                foreach (var text in taskTexts) 
                {
                    S("#new-todo").SetValue(text).PressEnter();
                }
            }

            public static void Toggle(string taskText)
            {
                List.FindBy(Have.ExactText(taskText)).S(".toggle").Click();
            }

            public static void ShouldBe(params string[] names)
            {
                List.FilterBy(Be.Visible).Should(Have.Texts(names));
            }
        }
```

See more examples of other styles, e.g. object oriented PageObjects, examples of how NSelene can be integrated into your current selenium based framework (to make it more stable and efficient) in NSeleneExamples project.

So... 
The ported things are: 
- Selenide.$
- SelenideElement#should
- SelenideElement#shouldNot
- SelenideElement#find / SelenideElement#$
- SelenideElement#findAll / SelenideElement#$$
- Selenide.$$
- EllementsCollection#should
- EllementsCollection#shouldNot
- EllementsCollection#findBy
- EllementsCollection#filterBy
- EllementsCollection#get
- all main Conditions
- all main CollectionConditions

Feel free to share your thoughts and file an issue on github if you need something.
