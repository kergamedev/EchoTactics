using Echo.Common;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Echo.Game
{
    public class Game : MonoBehaviour, IGame
    {
        [SerializeField, FoldoutGroup("Scenes")]
        private AssetReference _homeSceneRef;

        [SerializeField, FoldoutGroup("Scenes")]
        private AssetReference _matchSceneRef;

        private void Start()
        {
            StartCoroutine(InitalizeAsync());
        }

        private IEnumerator InitalizeAsync()
        {
            Global.Game = this;
            yield return GoToHomeAsync();
        }

        #region Load Home

        public void GoToHome()
        {
            StartCoroutine(GoToHomeAsync());
        }
        private IEnumerator GoToHomeAsync()
        {
            if (_matchSceneRef.IsLoaded())
                yield return UnloadMatch();

            var operation = Addressables.LoadSceneAsync(_homeSceneRef, LoadSceneMode.Additive);
            yield return operation;
        }

        private IEnumerator UnloadHome()
        {
            if (!_homeSceneRef.IsLoaded())
                yield break;

            var handle = _homeSceneRef.OperationHandle.Convert<SceneInstance>();
            var operation = Addressables.UnloadSceneAsync(handle.Result);
            yield return operation;
        }

        #endregion

        #region Load Match

        public void GoToMatch()
        {
            StartCoroutine(GoToMatchAsync());
        }
        private IEnumerator GoToMatchAsync()
        {
            if (_homeSceneRef.IsLoaded())
                yield return UnloadHome();

            var operation = Addressables.LoadSceneAsync(_matchSceneRef, LoadSceneMode.Additive);
            yield return operation;
        }

        private IEnumerator UnloadMatch()
        {
            if (!_matchSceneRef.IsLoaded())
                yield break;

            var handle = _matchSceneRef.OperationHandle.Convert<SceneInstance>();
            var operation = Addressables.UnloadSceneAsync(handle.Result);
            yield return operation;
        }

        #endregion

        private void OnDestroy()
        {
            Global.Game = null;
        }
    }
}