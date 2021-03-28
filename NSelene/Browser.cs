namespace NSelene
{
    internal class Browser
    {
        public _SeleneSettings_ Config { get; }
        public Browser(_SeleneSettings_ config)
        {
            this.Config = config;
        }

        public Browser() : this(Configuration._New_()) {}

        public Browser With(_SeleneSettings_ config)
        {
            return new Browser(this.Config.With(config));
        }
    }
}