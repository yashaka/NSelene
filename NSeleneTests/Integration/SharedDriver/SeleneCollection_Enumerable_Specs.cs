namespace NSeleneTests.Integration.SharedDriver.SeleneCollectionSpec
{
    [TestFixture]
    public class SeleneCollection_Enumerable_Specs : BaseTest
    {
        [Test]
        public void ReturnFirst()
        {
            Given.OpenedPageWithBody(@"
                <p id='will-exist'>
                    <input id='ask' type='submit' value='How r u?' style='display:none'></input>
                    <input id='answer1' type='submit' value='Good!'></input>
                    <input id='answer2' type='submit' value='Great!'></input>
                </p>"
            );

            SeleneCollection elements = SS("#will-exist>input");

            var element = elements.ActualWebElements.First();

            Assert.That(element.TagName, Is.EqualTo("input"));
        }

        [Test]
        public void ReturnDefaultFirst()
        {
            Given.OpenedPageWithBody(@"
                <p id='will-exist'>
                    <input id='ask' type='submit' value='How r u?' style='display:none'></input>
                    <input id='answer1' type='submit' value='Good!'></input>
                    <input id='answer2' type='submit' value='Great!'></input>
                </p>"
            );

            SeleneCollection elements = SS("div");

            var element = elements.ActualWebElements.FirstOrDefault();

            Assert.That(element, Is.Null);
        }

        [Test]
        public void ReturnLast()
        {
            Given.OpenedPageWithBody(@"
                <ul>
                    <li class='item'>a</li>
                    <li class='item'>b</li>
                    <li class='item'>c</li>
                </ul>"
            );

            SeleneCollection elements = SS(".item");

            var element = elements.ActualWebElements.Last();

            Assert.That(element.Text, Is.EqualTo("c"));
        }

        [Test]
        public void ReturnDefaultLast()
        {
            Given.OpenedPageWithBody(@"
                <ul>
                    <li class='item'>a</li>
                    <li class='item'>b</li>
                    <li class='item'>c</li>
                </ul>"
            );

            SeleneCollection elements = SS("div");

            var element = elements.ActualWebElements.LastOrDefault();

            Assert.That(element, Is.Null);
        }

        [Test]
        public void ReturnSingle()
        {
            Given.OpenedPageWithBody(@"
                <ul>
                    <li class='item'>a</li>
                    <li class='item'>b</li>
                    <li class='item'>c</li>
                </ul>"
            );

            SeleneCollection elements = SS(".item");

            var element = elements.ActualWebElements.Single(e => e.Text == "c");

            Assert.That(element.Text, Is.EqualTo("c"));
        }

        [Test]
        public void ReturnSingleThrowException()
        {
            Given.OpenedPageWithBody(@"
                <ul>
                    <li class='item'>c</li>
                    <li class='item'>c</li>
                </ul>"
            );

            SeleneCollection elements = SS(".item");

            Assert.Throws<InvalidOperationException>(() => elements.ActualWebElements.Single(e => e.Text == "c"));
        }

        [Test]
        public void ReturnDefaultSingle()
        {
            Given.OpenedPageWithBody(@"
                <ul>
                    <li class='item'>a</li>
                    <li class='item'>a</li>
                    <li class='item'>a</li>
                </ul>"
            );

            SeleneCollection elements = SS(".item");

            var element = elements.ActualWebElements.SingleOrDefault(e => e.Text == "b");

            Assert.That(element, Is.Null);
        }

        [Test]
        public void ReturnSelect()
        {
            Given.OpenedPageWithBody(@"
                <ul>
                    <li class='item'>a</li>
                    <li class='item'>b</li>
                </ul>"
            );

            SeleneCollection elements = SS(".item");

            var selectedElements = elements.ActualWebElements.Select(e => e.Text == "b");

            Assert.That(selectedElements.First(), Is.EqualTo(false));
            Assert.That(selectedElements.Last(), Is.EqualTo(true));
        }

        [Test]
        public void ReturnWhere()
        {
            Given.OpenedPageWithBody(@"
                <ul>
                    <li class='item'>a</li>
                    <li class='item'>b</li>
                </ul>"
            );

            SeleneCollection elements = SS(".item");

            var selectedElements = elements.ActualWebElements.Where(e => e.Text == "b").ToList();

            Assert.That(selectedElements, Has.Count.EqualTo(1));
        }

        [Test]
        public void ReturnWhereEmpty()
        {
            Given.OpenedPageWithBody(@"
                <ul>
                    <li class='item'>a</li>
                    <li class='item'>b</li>
                </ul>"
            );

            SeleneCollection elements = SS(".item");

            var selectedElements = elements.ActualWebElements.Where(e => e.Text == "c").ToList();

            Assert.That(selectedElements, Has.Count.EqualTo(0));
        }

        [Test]
        public void ReturnCount()
        {
            Given.OpenedPageWithBody(@"
                <ul>
                    <li class='item'>a</li>
                    <li class='item'>a</li>
                    <li class='item'>a</li>
                </ul>"
            );

            SeleneCollection elements = SS(".item");


            Assert.That(elements.ActualWebElements, Has.Count.EqualTo(3));
        }
    }
}