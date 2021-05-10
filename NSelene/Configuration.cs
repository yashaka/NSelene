using System;
using System.Collections.ObjectModel;
using System.Threading;
using NSelene.Helpers;
using OpenQA.Selenium;

namespace NSelene
{

    public interface _SupportsCopyWithOverride_<T>
    where T : _SupportsCopyWithOverride_<T>
    {
        T With(T overrides);
    }

    /// SeleneSettings interface defines a Set of valid settins
    /// Corresponding settings fields are nullable reflecting the fact
    /// that not all settings can be defined for specific context
    public interface _SeleneSettings_ 
    : _SupportsCopyWithOverride_<_SeleneSettings_>
    , SeleneContext // TODO: it's a temporal solution, 
                    // will be removed after Should(Be.Visible) is removed 
                    // from (SeleneElement as SeleneContext).FindElement
    {
        IWebDriver Driver { get; set; }
        double? Timeout { get; set; }
        double? PollDuringWaits { get; set; }
        bool? SetValueByJs { get; set; }
        bool? TypeByJs { get; set; }
        bool? ClickByJs { get; set; }
        bool? WaitForNoOverlayByJs { get; set; }
    }

    /// Configuration is considered as a defined group of all settings
    /// "all settings" are reflected by the _SeleneSettings_ interface
    /// Why would we need both? and separate them in context of naming?
    /// That's becausee historically Configuration was implemented as
    /// a shared static storage of all settings
    /// but then we decided to implement the non-static version
    /// yet keeping the same "already existing class and its fields naming" in use
    /// without adding one more API end point like some "non static new class".
    /// Hence, we yet added SeleneSettings interface, but it's kept under the hood.
    /// The user can freely use old-fashined thread local Configuration.Timeout, etc.
    /// Yet when he needs his own configuration instance he han create it by 
    ///     var configuration = Configuration.New(timeout: 6.0)
    /// 
    /// and yet call the timeout by same name
    ///     configuration.Timeout
    /// 
    /// I.e. by this we achieved creating a property 
    /// with same name for both class (as static) and its objects (as non-static), 
    /// that is not possible in C# ;)
    /// the main tradeoff in context of "quirky style" 
    /// is using Configuration.New(...) over new Configuration(...)
    /// That's why everything below is so complicated and overengineered;p 
    /// Later, we might simplify implementation by actually adding one more 
    /// non-static version of the configuration class, let's see.
    /// That's why we keep current interfaces 
    /// and some methods as "marked as risky API that might change"
    /// by adding _..._ around some names like in _SeleneSettings_ ;)
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

        private Ref<bool?> _refTypeByJs = new Ref<bool?>();
        bool? _SeleneSettings_.TypeByJs
        {
            get
            {
                return this._refTypeByJs.Value;
            }
            set
            {
                this._refTypeByJs.Value = value;
            }
        }

        private Ref<bool?> _refClickByJs = new Ref<bool?>();
        bool? _SeleneSettings_.ClickByJs
        {
            get
            {
                return this._refClickByJs.Value;
            }
            set
            {
                this._refClickByJs.Value = value;
            }
        }

        private Ref<bool?> _refWaitForNoOverlayByJs = new Ref<bool?>();
        bool? _SeleneSettings_.WaitForNoOverlayByJs
        {
            get
            {
                return this._refWaitForNoOverlayByJs.Value;
            }
            set
            {
                this._refWaitForNoOverlayByJs.Value = value;
            }
        }

        private Configuration(
            Ref<IWebDriver> refDriver,
            Ref<double?> refTimeout,
            Ref<double?> refPollDuringWaits,
            Ref<bool?> refSetValueByJs,
            Ref<bool?> refTypeByJs,
            Ref<bool?> refClickByJs,
            Ref<bool?> refWaitForNoOverlayByJs
        )
        {
            _refDriver = refDriver ?? new Ref<IWebDriver>();
            _refTimeout = refTimeout ?? new Ref<double?>();
            _refPollDuringWaits = refPollDuringWaits ?? new Ref<double?>();
            _refSetValueByJs = refSetValueByJs ?? new Ref<bool?>();
            _refTypeByJs = refTypeByJs ?? new Ref<bool?>();
            _refClickByJs = refClickByJs ?? new Ref<bool?>();
            _refWaitForNoOverlayByJs = refWaitForNoOverlayByJs ?? new Ref<bool?>();
        }

        // TODO: consider making public
        //       here we can choose between making it public of finally 
        //       making a public class from "hidden" interface _SeleneSettings_
        internal Configuration() 
        : this(
            refDriver: null,
            refTimeout: null,
            refPollDuringWaits: null,
            refSetValueByJs: null,
            refTypeByJs: null,
            refClickByJs: null,
            refWaitForNoOverlayByJs: null
        ) {}

        public static _SeleneSettings_ _New_(
            IWebDriver driver = null,
            double timeout = 4.0,
            double pollDuringWaits = 0.1,
            bool setValueByJs = false,
            bool typeByJs = false,
            bool clickByJs = false,
            bool waitForNoOverlayByJs = false
        )
        {
            _SeleneSettings_ next = new Configuration();

            next.Driver = driver;
            next.Timeout = timeout;
            next.PollDuringWaits = pollDuringWaits;
            next.SetValueByJs = setValueByJs;
            next.TypeByJs = typeByJs;
            next.ClickByJs = clickByJs;
            next.WaitForNoOverlayByJs = waitForNoOverlayByJs;

            return next;
        }

        internal static _SeleneSettings_ Shared
        = new Configuration(
            refDriver: new Ref<IWebDriver>(
                getter: () => Configuration.Driver,
                setter: value => Configuration.Driver = value
            ),
            refTimeout: new Ref<double?>(
                getter: () => Configuration.Timeout,
                setter: value => Configuration.Timeout = value ?? 4.0
                // TODO: consider moving all these defaults to Configuration.Defaults.*
            ),
            refPollDuringWaits: new Ref<double?>(
                getter: () => Configuration.PollDuringWaits,
                setter: value => Configuration.PollDuringWaits = value ?? 0.1
            ),
            refSetValueByJs: new Ref<bool?>(
                getter: () => Configuration.SetValueByJs,
                setter: value => Configuration.SetValueByJs = value ?? false
            ),
            refTypeByJs: new Ref<bool?>(
                getter: () => Configuration.TypeByJs,
                setter: value => Configuration.TypeByJs = value ?? false
            ),
            refClickByJs: new Ref<bool?>(
                getter: () => Configuration.ClickByJs,
                setter: value => Configuration.ClickByJs = value ?? false
            ),
            refWaitForNoOverlayByJs: new Ref<bool?>(
                getter: () => Configuration.WaitForNoOverlayByJs,
                setter: value => Configuration.WaitForNoOverlayByJs = value ?? false
            )
        );

        // TODO: how to differentiate between _With_(driver: null) and _With_() ?
        //       e.g. what if user want to override with null? i.e. reset? how to do this now?
        //       how to differentiate undefined/not-specified-yet and explicit-nothing?
        public static _SeleneSettings_ _With_(
            IWebDriver driver = null,
            double? timeout = null,
            double? pollDuringWaits = null,
            bool? setValueByJs = null,
            bool? typeByJs = null,
            bool? clickByJs = null,
            bool? waitForNoOverlayByJs = null
        )
        {
            _SeleneSettings_ next = new Configuration();

            next.Driver = driver;
            next.Timeout = timeout;
            next.PollDuringWaits = pollDuringWaits;
            next.SetValueByJs = setValueByJs;
            next.TypeByJs = typeByJs;
            next.ClickByJs = clickByJs;
            next.WaitForNoOverlayByJs = waitForNoOverlayByJs;

            return Configuration.Shared.With(next);
        }

        _SeleneSettings_ _SupportsCopyWithOverride_<_SeleneSettings_>.With(
            _SeleneSettings_ overrides
        )
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
                : new Ref<bool?>(overrides.SetValueByJs),
                refTypeByJs: overrides.TypeByJs == null
                ? this._refTypeByJs
                : new Ref<bool?>(overrides.TypeByJs),
                refClickByJs: overrides.ClickByJs == null
                ? this._refClickByJs
                : new Ref<bool?>(overrides.ClickByJs),
                refWaitForNoOverlayByJs: overrides.WaitForNoOverlayByJs == null
                ? this._refWaitForNoOverlayByJs
                : new Ref<bool?>(overrides.WaitForNoOverlayByJs)
            );
        }

        IWebElement SeleneContext.FindElement(By by)
        {
            _SeleneSettings_ self = this;
            return self.Driver.FindElement(by);
        }

        ReadOnlyCollection<IWebElement> SeleneContext.FindElements(By by)
        {
            _SeleneSettings_ self = this;
            return self.Driver.FindElements(by);
        }

        private static ThreadLocal<IWebDriver> _Driver = new ThreadLocal<IWebDriver>();

        public static IWebDriver Driver
        {
            get
            {
                return Configuration._Driver.Value;
            }
            set
            {
                Configuration._Driver.Value = value;
            }
        }

        private static ThreadLocal<double?> _Timeout = new ThreadLocal<double?>();
        public static double Timeout
        {
            get
            {
                return Configuration._Timeout.Value ?? 4.0;
            }
            set
            {
                Configuration._Timeout.Value = value;
            }
        }

        private static ThreadLocal<double?> _PollDuringWaits = new ThreadLocal<double?>();
        public static double PollDuringWaits
        {
            get
            {
                return Configuration._PollDuringWaits.Value ?? 0.1;
            }
            set
            {
                Configuration._PollDuringWaits.Value = value;
            }
        }

        private static ThreadLocal<bool?> _SetValueByJs = new ThreadLocal<bool?>();
        public static bool SetValueByJs
        {
            get
            {
                return Configuration._SetValueByJs.Value ?? false;
            }
            set
            {
                Configuration._SetValueByJs.Value = value;
            }
        }

        private static ThreadLocal<bool?> _TypeByJs = new ThreadLocal<bool?>();
        public static bool TypeByJs
        {
            get
            {
                return Configuration._TypeByJs.Value ?? false;
            }
            set
            {
                Configuration._TypeByJs.Value = value;
            }
        }

        private static ThreadLocal<bool?> _ClickByJs = new ThreadLocal<bool?>();
        public static bool ClickByJs
        {
            get
            {
                return Configuration._ClickByJs.Value ?? false;
            }
            set
            {
                Configuration._ClickByJs.Value = value;
            }
        }

        private static ThreadLocal<bool?> _WaitForNoOverlayByJs = new ThreadLocal<bool?>();
        public static bool WaitForNoOverlayByJs
        {
            get
            {
                return Configuration._WaitForNoOverlayByJs.Value ?? false;
            }
            set
            {
                Configuration._WaitForNoOverlayByJs.Value = value;
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