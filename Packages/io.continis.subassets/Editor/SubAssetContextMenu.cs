using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SubAssetsToolbox.Editor
{
    public static class SubAssetContextMenu
    {
#if UNITY_6000
        private const int MenuPriority = 20;
#else
        private const int MenuPriority = 19;
#endif
        
        [MenuItem("Assets/Delete Sub-Asset(s)", false, MenuPriority)]
        private static void RenameSubAsset()
        {
            bool isOne = Selection.objects.Length == 1;

            bool decision = EditorUtility.DisplayDialog(
                isOne ? "Delete Sub-Asset" : "Delete Sub-Assets",
                isOne
                    ? $"Are you sure you want to delete {Selection.objects[0].name}?"
                    : $"Are you sure you want to delete {Selection.objects[0].name}, {Selection.objects[1].name}, (...)?",
                isOne ? "Delete" : "Delete all",
                "Cancel");

            if (!decision) return;
            
            foreach (Object obj in Selection.objects)
            {
                string parentObjectPath = AssetDatabase.GetAssetPath(obj);
                SubAssetsToolbox.RemoveSubAsset(parentObjectPath, obj);
            }
                            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem("Assets/Delete Sub-Asset(s)", true)]
        private static bool ValidateRenameSubAsset()
        {
            return Selection.objects.All(AssetDatabase.IsSubAsset);
        }
    }
}