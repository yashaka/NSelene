namespace NSelene.Support.SeleneElementJsExtensions
{
    public static class SeleneElementJsExtensions
    {
        public static SeleneElement JsClick(this SeleneElement element, int centerXOffset = 0, int centerYOffset = 0)
        {
            element.ExecuteScript(@"
                    var centerXOffset = args[0];
                    var centerYOffset = args[1];

                    var rect = element.getBoundingClientRect();
                    element.dispatchEvent(new MouseEvent('click', {
                        'view': window,
                        'bubbles': true,
                        'cancelable': true,
                        'clientX': rect.left + rect.width/2 + centerXOffset,
                        'clientY': rect.top + rect.height/2 + centerYOffset 
                    }));
                    ",
                centerXOffset,
                centerYOffset);
            return element;
        }

        public static SeleneElement JsScrollIntoView(this SeleneElement element)
        {
            element.ExecuteScript("element.scrollIntoView({ behavior: 'smooth', block: 'center'})");
            return element;
        }
    }
}