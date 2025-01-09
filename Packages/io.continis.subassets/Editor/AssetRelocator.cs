using System;
using UnityEditor;
using Object = UnityEngine.Object;

namespace SubAssetsToolbox.Editor
{
    /// <summary>
    /// When the user creates a new asset via the Create menu and the selected asset is a main asset,
    /// this class adds it to the target asset as a sub-asset.
    /// </summary>
    public class AssetRelocator : AssetModificationProcessor
    {
        public static Object LastSelection;

        static void OnWillCreateAsset(string path)
        {
            if (path.EndsWith(".meta")) return;
            if (LastSelection != null) EditorApplication.delayCall += () => RelocateAsset(path);
        }

        private static void RelocateAsset(string path)
        {
            Type t = AssetDatabase.GetMainAssetTypeAtPath(path);
            Object justCreatedAsset = AssetDatabase.LoadAssetAtPath(path, t);
            string parentObjectPath = AssetDatabase.GetAssetPath(LastSelection);
            
            SubAssetsToolbox.AddSubAsset(parentObjectPath, justCreatedAsset);            
            
            AssetDatabase.DeleteAsset(path);
            
            AssetDatabase.SaveAssetIfDirty(LastSelection);
            AssetDatabase.Refresh();
        }
    }
}