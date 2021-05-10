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
            // TODO: should we bother here explicitely by being not overlapped like we do in JsSetValue below?
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
            if (element.config.WaitForNoOverlayByJs ?? false)
            {
                element.ExecuteScript(@"

                    // REQUIRE is not overlapped

                    var centerXOffset = 0;
                    var centerYOffset = 0;

                    var isVisible = !!( 
                        element.offsetWidth 
                        || element.offsetHeight 
                        || element.getClientRects().length 
                    ) && window.getComputedStyle(element).visibility !== 'hidden'

                    if (!isVisible) {
                        throw 'element is not visible'
                    }

                    var rect = element.getBoundingClientRect();
                    var x = rect.left + rect.width/2 + centerXOffset;
                    var y = rect.top + rect.height/2 + centerYOffset;

                    // TODO: now we return [element, null] in case of elementFromPoint returns null
                    //       (kind of – if we don't know what to do, let's at least not block the execution...)
                    //       rethink this... and handle the iframe case
                    //       read more in https://developer.mozilla.org/en-US/docs/Web/API/Document/elementFromPoint

                    var elementByXnY = document.elementFromPoint(x,y);
                    if (elementByXnY == null) { // TODO: do we even need it here?
                        throw 'could not find element by its coordinates'
                    }

                    var isNotOverlapped = element.isSameNode(elementByXnY);
                    if (!isNotOverlapped) {
                        throw 'element is overlapped by: ' + elementByXnY.getAttribute('outerHTML')
                    }

                    // THEN set value

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
            }
            else
            {
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
            }
            
            
            return element;
        }

        public static SeleneElement JsType(this SeleneElement element, string value)
        {
            // TODO: should we call here and above the Should(Be.Visible) ?
            // element.Should(Be.Visible);

            if (element.config.WaitForNoOverlayByJs ?? false)
            {
                element.ExecuteScript(@"

                    // REQUIRE is not overlapped

                    var centerXOffset = 0;
                    var centerYOffset = 0;

                    var isVisible = !!( 
                        element.offsetWidth 
                        || element.offsetHeight 
                        || element.getClientRects().length 
                    ) && window.getComputedStyle(element).visibility !== 'hidden'

                    if (!isVisible) {
                        throw 'element is not visible'
                    }

                    var rect = element.getBoundingClientRect();
                    var x = rect.left + rect.width/2 + centerXOffset;
                    var y = rect.top + rect.height/2 + centerYOffset;

                    // TODO: now we return [element, null] in case of elementFromPoint returns null
                    //       (kind of – if we don't know what to do, let's at least not block the execution...)
                    //       rethink this... and handle the iframe case
                    //       read more in https://developer.mozilla.org/en-US/docs/Web/API/Document/elementFromPoint

                    var elementByXnY = document.elementFromPoint(x,y);
                    if (elementByXnY == null) { // TODO: do we even need it here?
                        throw 'could not find element by its coordinates'
                    }

                    var isNotOverlapped = element.isSameNode(elementByXnY);
                    if (!isNotOverlapped) {
                        throw 'element is overlapped by: ' + elementByXnY.getAttribute('outerHTML')
                    }

                    // THEN type value

                    var value = args[0];

                    const text = element.getAttribute('value').concat(value);
                    const maxlength = element.getAttribute('maxlength') === null
                        ? -1
                        : parseInt(element.getAttribute('maxlength'));
                    element.value = maxlength === -1
                        ? text
                        : text.length <= maxlength
                            ? text
                            : text.substring(0, maxlength);
                    ",
                    value
                );
            }
            else
            {
                element.ExecuteScript(@"
                    var value = args[0];

                    const text = element.getAttribute('value').concat(value);
                    const maxlength = element.getAttribute('maxlength') === null
                        ? -1
                        : parseInt(element.getAttribute('maxlength'));
                    element.value = maxlength === -1
                        ? text
                        : text.length <= maxlength
                            ? text
                            : text.substring(0, maxlength);
                    ",
                    value
                );
            }
            
            return element;
        }
    }
}