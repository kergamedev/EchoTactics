using Echo.Common;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
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

        private PlayerAccount _playerAccount;

        public IPlayerAccount PlayerAccount => _playerAccount;

        private void Start()
        {
            StartCoroutine(InitalizeAsync());
        }

        #region Initialization

        private IEnumerator InitalizeAsync()
        {
            Global.Game = this;

            yield return Utilities.WaitForCompletion(InitializeServicesAsync);
            yield return Utilities.WaitForCompletion(SignInAsync);
            yield return GoToHomeAsync();
        }

        private async Task InitializeServicesAsync()
        {
            try
            {
                await UnityServices.InitializeAsync();
            }
            catch (Exception exception)
            {
                Debug.LogError("Failed to intialize the unity services");
                throw exception;
            }
        }

        private async Task SignInAsync()
        {
            _playerAccount = new PlayerAccount();

            if (AuthenticationService.Instance.SessionTokenExists)
            {
                await SignInAnonymouslyAsync();
                return;
            }

            #if UNITY_EDITOR

            var method = Editor.EditorUtilities.GetSignInMethod();
            switch (method)
            {
                case Editor.EditorSignInMethod.Anonymous:
                    await SignInAnonymouslyAsync();
                    break;

                case Editor.EditorSignInMethod.UnityAccount:
                    var username = Editor.EditorUtilities.GetUnityAccountUsername();
                    var password = Editor.EditorUtilities.GetUnityAccountPassword();
                    try
                    {
                        await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
                    }
                    catch (Exception exception)
                    {
                        Debug.LogError($"Failed to authenticate in editor with 'Username={username} and 'Password={password}'");
                        throw exception;
                    }
                    break;
            }

            #else

            await SignInAnonymouslyAsync();

            #endif
        }
        private async Task SignInAnonymouslyAsync()
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            catch (AuthenticationException authenticationException)
            {
                Debug.LogError($"Encoutered an unexpected error while trying to authenticate anonymously. 'ErrorCode={authenticationException.ErrorCode}'");
                throw authenticationException;
            }
            catch (RequestFailedException requestFailedException)
            {
                Debug.LogError($"Failed to authenticate anonymously. 'ErrorCode={requestFailedException.ErrorCode}'");
                throw requestFailedException;
            }
        }

        #endregion

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
            _playerAccount.Dispose();

            Global.Game = null;
        }
    }
}