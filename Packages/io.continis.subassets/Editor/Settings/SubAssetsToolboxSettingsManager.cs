using UnityEditor;
using UnityEditor.SettingsManagement;

namespace SubAssetsToolbox.Editor
{
    static class SubAssetsToolboxSettingsManager
    {
        private static Settings instance;

        internal static Settings settings
        {
            get
            {
                if (instance == null)
                    instance = new Settings(Constants.packageName, "Settings");

                return instance;
            }
        }
    }

    public class PackageSetting<T> : UserSetting<T>
    {
        public PackageSetting(string key, T value)
            : base(SubAssetsToolboxSettingsManager.settings, key, value, SettingsScope.Project) { }
    }

    public class UserPref<T> : UserSetting<T>
    {
        public UserPref(string key, T value)
            : base(SubAssetsToolboxSettingsManager.settings, key, value, SettingsScope.User) { }
    }
}