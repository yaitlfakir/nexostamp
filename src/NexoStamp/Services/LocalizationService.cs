using System;
using System.ComponentModel;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;

namespace NexoStamp.Services
{
    /// <summary>
    /// Service for managing application localization and language switching
    /// </summary>
    public class LocalizationService : INotifyPropertyChanged
    {
        private static LocalizationService? _instance;
        private readonly ResourceManager _resourceManager;
        private CultureInfo _currentCulture;

        public static LocalizationService Instance => _instance ??= new LocalizationService();

        public event PropertyChangedEventHandler? PropertyChanged;

        public CultureInfo CurrentCulture
        {
            get => _currentCulture;
            private set
            {
                _currentCulture = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FlowDirection));
                OnPropertyChanged(nameof(IsRightToLeft));
            }
        }

        /// <summary>
        /// Returns the appropriate FlowDirection based on current language
        /// </summary>
        public FlowDirection FlowDirection => IsRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

        /// <summary>
        /// Returns true if current language is RTL (like Arabic, Hebrew, etc.)
        /// </summary>
        public bool IsRightToLeft => _currentCulture.TextInfo.IsRightToLeft;

        private LocalizationService()
        {
            _resourceManager = new ResourceManager("NexoStamp.Resources.Strings", typeof(LocalizationService).Assembly);
            _currentCulture = CultureInfo.CurrentUICulture;
        }

        /// <summary>
        /// Gets a localized string by key
        /// </summary>
        public string GetString(string key)
        {
            try
            {
                return _resourceManager.GetString(key, _currentCulture) ?? key;
            }
            catch
            {
                return key;
            }
        }

        /// <summary>
        /// Changes the current application language
        /// </summary>
        public void SetLanguage(string cultureCode)
        {
            var culture = new CultureInfo(cultureCode);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            CurrentCulture = culture;

            // Notify all bindings to update
            OnPropertyChanged(string.Empty);
        }

        /// <summary>
        /// Changes the current application language to Arabic
        /// </summary>
        public void SetArabic()
        {
            SetLanguage("ar");
        }

        /// <summary>
        /// Changes the current application language to English
        /// </summary>
        public void SetEnglish()
        {
            SetLanguage("en");
        }

        /// <summary>
        /// Gets available languages
        /// </summary>
        public static (string Code, string DisplayName)[] AvailableLanguages => new[]
        {
            ("en", "English"),
            ("ar", "العربية (Arabic)")
        };

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// Markup extension for easy XAML binding to localized strings
    /// </summary>
    public static class Loc
    {
        public static string Get(string key) => LocalizationService.Instance.GetString(key);
    }
}