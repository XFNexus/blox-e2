﻿using System.Windows;

namespace Bloxstrap
{
    internal static class Locale
    {
        public static CultureInfo CurrentCulture { get; private set; } = CultureInfo.InvariantCulture;

        public static bool RightToLeft { get; private set; } = false;

        private static readonly List<string> _rtlLocales = new() { "ar", "he", "fa" };

        public static readonly Dictionary<string, string> SupportedLocales = new()
        {
            { "nil", Strings.Common_SystemDefault },
            { "en", "English" },
            { "en-US", "English (United States)" },
            { "ar", "العربية" },
            { "bg", "Български" },
#if QA_BUILD
            { "bn", "বাংলা" },
            { "bs", "Bosanski" },
#endif
            { "cs", "Čeština" },
            { "de", "Deutsch" },
#if QA_BUILD
            { "dk", "Dansk" },
#endif
            { "es-ES", "Español" },
#if QA_BUILD
            { "el", "Ελληνικά" },
            { "fa", "فارسی" },
#endif
            { "fi", "Suomi" },
#if QA_BUILD
            { "fil", "Filipino" },
#endif
            { "fr", "Français" },
            { "he", "עברית‎" },
#if QA_BUILD
            { "hi", "Hindi (Latin)" },
#endif
            { "hr", "Hrvatski" },
            { "hu", "Magyar" },
#if QA_BUILD
            { "id", "Bahasa Indonesia" },
#endif
            { "it", "Italiano" },
            { "ja", "日本語" },
#if QA_BUILD
            { "ko", "한국어" },
#endif
            { "lt", "Lietuvių" },
            { "ms", "Malay" },
#if QA_BUILD
            { "nl", "Nederlands" },
            { "no", "Bokmål" },
#endif
            { "pl", "Polski" },
            { "pt-BR", "Português (Brasil)" },
            { "ro", "Română" },
            { "ru", "Русский" },
            { "sv-SE", "Svenska" },
            { "th", "ภาษาไทย" },
            { "tr", "Türkçe" },
            { "uk", "Українська" },
            { "vi", "Tiếng Việt" },
            { "zh-CN", "中文 (简体)" },
#if QA_BUILD
            { "zh-HK", "中文 (廣東話)" },
#endif
            { "zh-TW", "中文 (繁體)" }
        };

        public static string GetIdentifierFromName(string language) => SupportedLocales.FirstOrDefault(x => x.Value == language).Key ?? "nil";

        public static List<string> GetLanguages()
        {
            var languages = new List<string>();
            
            languages.AddRange(SupportedLocales.Values.Take(3));
            languages.AddRange(SupportedLocales.Values.Where(x => !languages.Contains(x)).OrderBy(x => x));
            languages[0] = Strings.Common_SystemDefault; // set again for any locale changes

            return languages;
        }

        public static void Set(string identifier)
        {
            if (!SupportedLocales.ContainsKey(identifier))
                identifier = "nil";

            if (identifier == "nil")
            {
                CurrentCulture = Thread.CurrentThread.CurrentUICulture;
            }
            else
            {
                CurrentCulture = new CultureInfo(identifier);

                CultureInfo.DefaultThreadCurrentUICulture = CurrentCulture;
                Thread.CurrentThread.CurrentUICulture = CurrentCulture;
            }

            RightToLeft = _rtlLocales.Any(CurrentCulture.Name.StartsWith);
        }

        public static void Initialize()
        {
            Set("nil");

            // https://supportcenter.devexpress.com/ticket/details/t905790/is-there-a-way-to-set-right-to-left-mode-in-wpf-for-the-whole-application
            EventManager.RegisterClassHandler(typeof(Window), FrameworkElement.LoadedEvent, new RoutedEventHandler((sender, _) =>
            {
                var window = (Window)sender;

                if (RightToLeft)
                {
                    window.FlowDirection = FlowDirection.RightToLeft;

                    if (window.ContextMenu is not null)
                        window.ContextMenu.FlowDirection = FlowDirection.RightToLeft;
                }
                else if (CurrentCulture.Name.StartsWith("th"))
                {
                    window.FontFamily = new System.Windows.Media.FontFamily(new Uri("pack://application:,,,/Resources/Fonts/"), "./#Noto Sans Thai");
                }

#if QA_BUILD
                window.BorderBrush = System.Windows.Media.Brushes.Red;
                window.BorderThickness = new Thickness(4);
#endif
            }));
        }
    }
}
