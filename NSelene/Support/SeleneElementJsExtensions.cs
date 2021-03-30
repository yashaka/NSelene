namespace NSelene.Support.SeleneElementJsExtensions
{
    public static class SeleneElementJsExtensions
    {
        public static SeleneElement JsClick(
            this SeleneElement element, 
            int centerXOffset = 0, 
            int centerYOffset = 0
        )
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
            element.ExecuteScript(
                "element.scrollIntoView({ behavior: 'smooth', block: 'center'})"
            );
            return element;
        }

        public static SeleneElement JsSetValue(this SeleneElement element, string value)
        {
            // TODO: should we call here and above the Should(Be.Visible) ?
            // element.Should(Be.Visible);

            element.ExecuteScript(@"
                var value = args[0];

                var maxlength = element.getAttribute('maxlength') === null
                    ? -1
                    : parseInt(element.getAttribute('maxlength'));
                element.value = maxlength === -1
                    ? value
                    : value.length <= maxlength
                        ? value
                        : value.substring(0, maxlength);
                ",
                value
            );
            
            return element;
        }
    }
}