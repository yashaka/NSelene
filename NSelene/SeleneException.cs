
using System;

namespace NSelene
{
        public class SeleneException : Exception
        {
            public SeleneException(string message) : this(() => message)
            {
            }

            public SeleneException(Func<string> renderMessage) : base("")
            {
                this.RenderMessage = renderMessage;
            }

            public SeleneException(
                string message,
                Exception innerException
            )
            : this(() => message, innerException)
            {
            }

            public SeleneException(
                Func<string> renderMessage,
                Exception innerException
            )
            : base("", innerException)
            {
                this.RenderMessage = renderMessage;
            }

            public Func<string> RenderMessage { get; }

            public override string Message => this.RenderMessage();
        }
}