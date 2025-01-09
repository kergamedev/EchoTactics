using SubAssetsToolbox.Editor.SubAssetAware;
using UnityEditor;
using UnityEngine;

namespace SubAssetsToolbox.Editor
{
    static class SubAssetsToolbox
    {
        public static void AddSubAsset(string destinationPath, Object objectToClone)
        {
            Object newObject = Object.Instantiate(objectToClone);
            newObject.name = objectToClone.name;
            AssetDatabase.AddObjectToAsset(newObject, destinationPath);

            SubAssetAwareMethods.NotifyAwareObjectOfAddition(destinationPath, newObject);
        }

        public static void RemoveSubAsset(string oldParentPath, Object objectToRemove)
        {
            SubAssetAwareMethods.NotifyAwareObjectOfRemoval(oldParentPath, objectToRemove);
            
            AssetDatabase.RemoveObjectFromAsset(objectToRemove);
        }
    }
}