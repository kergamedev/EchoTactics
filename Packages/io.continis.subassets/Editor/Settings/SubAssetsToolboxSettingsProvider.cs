using UnityEditor;
using UnityEditor.SettingsManagement;

namespace SubAssetsToolbox.Editor
{
    static class SubAssetsToolboxSettingsProvider
    {
        const string SettingsPath = "Project/SubAssets Toolbox";

        //[SettingsProvider]
        static SettingsProvider CreateSettingsProvider()
        {
            var provider = new UserSettingsProvider(SettingsPath,
                SubAssetsToolboxSettingsManager.settings,
                new [] { typeof(SubAssetsToolboxSettingsProvider).Assembly }, SettingsScope.Project);
            
            return provider;
        }
    }
}