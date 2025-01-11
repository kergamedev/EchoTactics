using Echo.Common;
using System;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Echo.Game
{
    [Serializable, DisplayName("From Save")]
    public class SelectLocaleFromSave : IStartupLocaleSelector
    {
        Locale IStartupLocaleSelector.GetStartupLocale(ILocalesProvider availableLocales)
        {
            if (!PlayerPrefs.HasKey(SaveKeys.SELECTED_LOCALE))
                return null;

            var code = PlayerPrefs.GetString(SaveKeys.SELECTED_LOCALE);
            if (code.IsNullOrEmpty())
                return null;

            return availableLocales.GetLocale(new LocaleIdentifier(code));
        }
    }
}