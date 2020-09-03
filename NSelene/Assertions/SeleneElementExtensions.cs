using System;
using System.Collections.Generic;
using System.Text;

namespace NSelene.Assertions
{
    public static class SeleneElementExtensions
    {
        public static SeleneElementAssertions Should(this SeleneElement element)

        {
            return new SeleneElementAssertions(element);
        }
    }
}
