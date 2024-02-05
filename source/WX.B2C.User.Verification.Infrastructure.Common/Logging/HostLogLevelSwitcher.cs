using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using WX.B2C.User.Verification.Core.Contracts.Configurations.Options;
using WX.B2C.User.Verification.Infrastructure.Contracts;

namespace WX.B2C.User.Verification.Infrastructure.Common.Logging
{
    internal class HostLogLevelSwitcher : LoggingLevelSwitch, IAsyncDisposable
    {
        private readonly IOptionProvider<LogLevelOption> _optionProvider;
        private readonly LogEventLevel _defaultValue;
        private readonly string _host;
        
        private readonly Timer _timer;

        public HostLogLevelSwitcher(IHostSettingsProvider settingsProvider, IOptionProvider<LogLevelOption> optionProvider)
        {
            if (settingsProvider == null)
                throw new ArgumentNullException(nameof(settingsProvider));

            _defaultValue = Enum.Parse<LogEventLevel>(settingsProvider.GetSetting(HostSettingsKey.LogLevel));
            MinimumLevel = _defaultValue;
            
            _host = settingsProvider.GetSetting(HostSettingsKey.ApplicationHost);
            _optionProvider = optionProvider ?? throw new ArgumentNullException(nameof(optionProvider));

            _timer = new Timer(RefreshValueAsync, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        }

        private async void RefreshValueAsync(object state)
        {
            try
            {
                var option = await _optionProvider.GetAsync();
                var newLogLevel = option.Get(_host, _defaultValue);
                if (newLogLevel == MinimumLevel)
                    return;
                
                MinimumLevel = newLogLevel;
                Debug.WriteLine($"Set log level:{MinimumLevel} for {_host}");
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public ValueTask DisposeAsync() =>
            _timer.DisposeAsync();
    }
}