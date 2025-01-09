using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Echo.Common
{
    public static class Extensions
    {
        #region Addressables

        public static bool IsLoaded(this AssetReference assetRef)
        {
            return IsLoaded(assetRef, out _);
        }
        public static bool IsLoaded(this AssetReference assetRef, out Scene scene)
        {
            scene = default;

            if (!assetRef.OperationHandle.IsValid())
                return false;

            var handle = assetRef.OperationHandle.Convert<SceneInstance>();
            if (!handle.IsValid())
                return false;

            scene = handle.Result.Scene;
            return scene.isLoaded;
        }

        #endregion
    }
}