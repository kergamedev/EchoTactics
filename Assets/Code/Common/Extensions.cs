using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
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

        #region Async

        public static IEnumerator WaitForCompletion(this Task task)
        {
            yield return new WaitUntil(() => task.IsCompleted);
            if (task.IsFaulted)
                throw task.Exception;
        }

        #endregion

        #region String

        public static bool IsNullOrEmpty(this string value)
        {
            return value == null || value == string.Empty;
        }

        #endregion
    }
}