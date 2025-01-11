using System;
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

        public static IEnumerator WaitForCompletionAsync(this Task task)
        {
            yield return new WaitUntil(() => task.IsCompleted);
            if (task.IsFaulted)
                throw task.Exception;
        }

        public static Task AsTask(this RoutineBehaviour behaviour)
        {
            behaviour.Run();
            var task = new Task(() =>
            {
                while (behaviour.IsRunning)
                    continue;
            });

            task.Start();
            return task;
        }

        #endregion

        #region Animation Curve

        public static float GetDuration(this AnimationCurve curve)
        {
            return curve.keys[^1].time;
        }

        #endregion

        #region Hierarchy

        public static TComponent GetComponentInRoots<TComponent>(this Scene scene) where TComponent : class
        {
            foreach (var root in scene.GetRootGameObjects())
            {
                var component = root.GetComponentInChildren<TComponent>();
                if (component != null)
                    return component;
            }

            return null;
        }

        #endregion

        #region String

        public static bool IsNullOrEmpty(this string value)
        {
            return value == null || value == string.Empty;
        }

        #endregion

        #region Time

        public static float GetValue(this DeltaTimeKind kind)
        {
            switch (kind)
            {
                case DeltaTimeKind.Normal: return Time.deltaTime;
                case DeltaTimeKind.NormalUnscaled: return Time.unscaledDeltaTime;
                case DeltaTimeKind.Fixed: return Time.fixedDeltaTime;
                case DeltaTimeKind.FixedUnscaled: return Time.fixedUnscaledDeltaTime;
                default: throw new Exception($"No case defined for '{nameof(DeltaTimeKind)}={kind}'");
            }
        }

        #endregion
    }
}