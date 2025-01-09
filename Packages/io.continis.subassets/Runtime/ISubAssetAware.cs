using UnityEngine;

namespace SubAssetsToolbox
{
    /// <summary>
    /// Implement this interface in your ScriptableObjects, to make them compatible with the editor workflows of SubAssets Toolbox.
    /// </summary>
    public interface ISubAssetAware
    {
        public void AddSubAsset(Object newSubAsset);
        public void RemoveSubAsset(Object removedSubAsset);
    }
}