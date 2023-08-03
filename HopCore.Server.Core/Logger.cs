using System;
using HopCore.Shared;
using HopCore.Shared.DependencyInjection;

namespace HopCore.Server.Core {
    public sealed class Logger : ILogger {
        private const byte 
            Red = 1,
            Green = 2,
            Yellow = 3,
            Blue = 4,
            LightBlue = 5,
            Violett = 6,
            White = 7,
            DarkRed = 8,
            DarkBlue = 9;
        
        private readonly Config _config;

        public Logger() {
            _config = Dependency.Inject<Config>();
        }
        
        public void Debug(object message) {
            if (!_config.Debug) return;
            CitizenFX.Core.Debug.WriteLine(FormatMessage(LightBlue + "DEBUG", message));
        }

        public void Info(object message) {
            CitizenFX.Core.Debug.WriteLine(FormatMessage(Green + "INFO", message));
        }

        public void Warning(object message) {
            CitizenFX.Core.Debug.WriteLine(FormatMessage(Yellow + "WARNING", message));
        }

        public void Error(object message) {
            CitizenFX.Core.Debug.WriteLine(FormatMessage(Red + "ERROR", message));
        }
        
        public void Fatal(object message) {
            CitizenFX.Core.Debug.WriteLine(FormatMessage(DarkRed + "ERROR", message, Red));
        }

        public void Database(object message, bool newLine = true) {
            if (!_config.Debug) return;
            CitizenFX.Core.Debug.Write(FormatMessage(Violett + "DATABASE", message) + (newLine ? "\n" : ""));
        }

        private string FormatMessage(string modeName, object message, byte messageColor = White) {
            return $"^{White}[^{Blue}{DateTime.Now.ToShortTimeString()}^{White}] [^{modeName}^{White}]^{messageColor} {message}^{White}";
        }
    }
}