using UnityEditor;
using UnityEngine;

namespace SubAssetsToolbox.Editor.SubAssetAware
{
    static class SubAssetAwareMethods
    {
        public static void NotifyAwareObjectOfAddition(string parentObjectPath, Object newObject)
        {
            Object newParentObject = AssetDatabase.LoadAssetAtPath(parentObjectPath, typeof(Object));
            if (newParentObject is ISubAssetAware awareObject)
            {
                awareObject.AddSubAsset(newObject);
            }
        }

        public static void NotifyAwareObjectOfRemoval(string parentObjectPath, Object objectToRemove)
        {
            Object oldParentObject = AssetDatabase.LoadAssetAtPath(parentObjectPath, typeof(Object));
            if (oldParentObject is ISubAssetAware awareObject)
            {
                awareObject.RemoveSubAsset(objectToRemove);
            }
        }
    }
}