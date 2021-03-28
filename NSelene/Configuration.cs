using System;
using System.Threading;
using OpenQA.Selenium;

namespace NSelene
{

    public interface _SupportsCopyWithOverride_<T> 
    where T : _SupportsCopyWithOverride_<T>
    {
        T With(T overrides);
    }

    public interface _SeleneSettings_ : _SupportsCopyWithOverride_<_SeleneSettings_>
    {
        IWebDriver Driver { get; set; }
        double? Timeout { get; set; }
        double? PollDuringWaits { get; set; }
        bool? SetValueByJs { get; set; }
    }

    internal class Ref<T>
    {
        public T Value { get; set; }
        public Ref(T value)
        {
            Value = value;
        }
        public Ref()
        {
        }

    }

    public class Configuration : _SeleneSettings_
    {
        private Ref<IWebDriver> _refDriver;
        IWebDriver _SeleneSettings_.Driver
        { 
            get 
            {
                return this._refDriver.Value;
            } 
            set
            {
                this._refDriver.Value = value;
            } 
        }
        private Ref<double?> _refTimeout = new Ref<double?>();
        double? _SeleneSettings_.Timeout
        { 
            get 
            {
                return this._refTimeout.Value;
            } 
            set
            {
                this._refTimeout.Value = value;
            } 
        }
        private Ref<double?> _refPollDuringWaits = new Ref<double?>();
        double? _SeleneSettings_.PollDuringWaits
        { 
            get 
            {
                return this._refPollDuringWaits.Value;
            } 
            set
            {
                this._refPollDuringWaits.Value = value;
            } 
        }
        private Ref<bool?> _refSetValueByJs = new Ref<bool?>();
        bool? _SeleneSettings_.SetValueByJs
        { 
            get 
            {
                return this._refSetValueByJs.Value;
            } 
            set
            {
                this._refSetValueByJs.Value = value;
            } 
        }

        private Configuration(
            Ref<IWebDriver> refDriver = null, 
            Ref<double?> refTimeout = null,
            Ref<double?> refPollDuringWaits = null, 
            Ref<bool?> refSetValueByJs = null)
        {
            _refDriver = refDriver ?? new Ref<IWebDriver>();
            _refTimeout = refTimeout ?? new Ref<double?>();
            _refPollDuringWaits = refPollDuringWaits ?? new Ref<double?>();
            _refSetValueByJs = refSetValueByJs ?? new Ref<bool?>();
        }

        public static _SeleneSettings_ _New_(
            IWebDriver driver = null,
            double timeout = 4.0,
            double pollDuringWaits = 0.1,
            bool setValueByJs = false)
        {
            _SeleneSettings_ next = new Configuration();
            
            next.Driver = driver;
            next.Timeout = timeout;
            next.PollDuringWaits = pollDuringWaits;
            next.SetValueByJs = setValueByJs;

            return next;
        }

        internal static ThreadLocal<_SeleneSettings_> Shared 
        = new ThreadLocal<_SeleneSettings_>(() => Configuration._New_());

        public static _SeleneSettings_ _With_(
            IWebDriver driver = null,
            double? timeout = null,
            double? pollDuringWaits = null,
            bool? setValueByJs = null)
        {
            _SeleneSettings_ next = new Configuration();

            next.Driver = driver;
            next.Timeout = timeout;
            next.PollDuringWaits = pollDuringWaits;
            next.SetValueByJs = setValueByJs;

            return Configuration.Shared.Value.With(next);
        }

        _SeleneSettings_ _SupportsCopyWithOverride_<_SeleneSettings_>.With(_SeleneSettings_ overrides)
        {
            return new Configuration(
                refDriver: overrides.Driver == null
                ? this._refDriver
                : new Ref<IWebDriver>(overrides.Driver),
                refTimeout: overrides.Timeout == null
                ? this._refTimeout
                : new Ref<double?>(overrides.Timeout),
                refPollDuringWaits: overrides.PollDuringWaits == null
                ? this._refPollDuringWaits
                : new Ref<double?>(overrides.PollDuringWaits),
                refSetValueByJs: overrides.SetValueByJs == null
                ? this._refSetValueByJs
                : new Ref<bool?>(overrides.SetValueByJs)
            );
        }

        public static double Timeout
        {
            get
            {
                return Configuration.Shared.Value.Timeout ?? 4.0;
            }
            set
            {
                Configuration.Shared.Value.Timeout = value;
            }
        }

        public static double PollDuringWaits
        {
            get
            {
                return Configuration.Shared.Value.PollDuringWaits ?? 0.1;
            }
            set
            {
                Configuration.Shared.Value.PollDuringWaits = value;
            }
        }

        public static bool SetValueByJs
        {
            get
            {
                return Configuration.Shared.Value.SetValueByJs ?? false;
            }
            set
            {
                Configuration.Shared.Value.SetValueByJs = value;
            }
        }
        
        public static IWebDriver Driver
        {
            get
            {
                return Configuration.Shared.Value.Driver;
            }
            set
            {
                Configuration.Shared.Value.Driver = value;
            }
        }

        [Obsolete("Use Configuration.Driver over deprecated Configuration.WebDriver")]
        public static IWebDriver WebDriver
        {
            get
            {
                return Configuration.Driver;
            }
            set
            {
                Configuration.Driver = value;
            }
        }
    }
}