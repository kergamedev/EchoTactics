using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

using Object = UnityEngine.Object;

namespace Echo.Common
{
    public static class Extensions
    {
        #region Scene management

        public static bool IsLoaded(this SceneInstance? instance)
        {
            if (instance == null)
                return false;

            var scene = instance.Value.Scene;
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

        public static Task StartAsTask(this LoadingTransition loadingTransition)
        {
            loadingTransition.Begin();
            var task = new Task(() =>
            {
                while (loadingTransition.CurrentState != LoadingTransition.State.Loading)
                    continue;
            });

            task.Start();
            return task;
        }
        public static Task EndAsTask(this LoadingTransition loadingTransition)
        {
            loadingTransition.End();
            var task = new Task(() =>
            {
                while (loadingTransition.CurrentState != LoadingTransition.State.None)
                    continue;
            });

            task.Start();
            return task;
        }

        #endregion

        #region Animation curve

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

        public static TComponent[] GetComponentsInRoots<TComponent>(this Scene scene) where TComponent : class
        {
            var components = new List<TComponent>();
            foreach (var root in scene.GetRootGameObjects())
                components.AddRange(root.GetComponentsInChildren<TComponent>());

            return components.ToArray();
        }

        #endregion

        #region String

        public static bool IsNullOrEmpty(this string value)
        {
            return value == null || value == string.Empty;
        }

        #endregion

        #region Network

        public static TComponent Spawn<TComponent>(this NetworkManager networkManager, TComponent prefab) where TComponent : NetworkBehaviour
        {
            var instanceObject = Object.Instantiate(networkManager.GetNetworkPrefabOverride(prefab.gameObject));
            var instanceNetworkObject = instanceObject.GetComponent<NetworkObject>();
            instanceNetworkObject.Spawn();

            return instanceObject.GetComponent<TComponent>();
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

        #region Rect

        public static Rect Pad(this Rect rect, float padding)
        {
            var twice = padding * 2f;
            return new Rect(rect.x + padding, rect.y + padding, rect.width - twice, rect.height - twice);
        }

        #endregion
    }
}