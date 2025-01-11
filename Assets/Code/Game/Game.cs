using Echo.Common;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Echo.Game
{
    public class Game : MonoBehaviour, IGame
    {
        [SerializeField, FoldoutGroup("Transitions")]
        private RoutineBehaviour _splashScreenBehaviour;

        [SerializeField, FoldoutGroup("Transitions")]
        private RoutineBehaviour _quitBehaviour;

        [SerializeField, FoldoutGroup("Scenes")]
        private AssetReference _homeSceneRef;

        [SerializeField, FoldoutGroup("Scenes")]
        private AssetReference _matchSceneRef;

        [SerializeField, FoldoutGroup("Scenes")]
        private AssetReference _earlyOutSceneRef;

        [SerializeField, FoldoutGroup("References")]
        private TweenLibrary _tweenLibrary;

        private PlayerAccount _playerAccount;

        public TweenLibrary TweenLibrary => _tweenLibrary;
        public IPlayerAccount PlayerAccount => _playerAccount;

        private async void Start()
        {
            await InitializeAsync();
            await _splashScreenBehaviour.AsTask();
        }

        #region Initialization

        private async Task InitializeAsync()
        {
            Debug.Log($"[INITIALIZATION] Starting initialization...");
            Global.Game = this;

            try
            {
                await InitializeLocalizationAsync();
                await InitializeServicesAsync();
                await SignInAsync();
            }
            catch (Exception exception)
            {
                Debug.LogError("[INITIALIZATION] Caught an unexpected error while trying to initialize game");
                Debug.LogException(exception);

                await HandleErrorAsync("Couldn't start game", "Please try again later");
                return;
            }

            await GoToHomeAsync();
        }

        private async Task InitializeLocalizationAsync()
        {
            await LocalizationSettings.InitializationOperation.Task;

            ConverterGroups.RegisterGlobalConverter((ref LocalizedString stringRef) => stringRef.GetLocalizedString());
        }

        private async Task InitializeServicesAsync()
        {
            throw new Exception("Fake...");

            try
            {
                Debug.Log($"[INITIALIZATION] Initalizing Unity services...");
                await UnityServices.InitializeAsync();
            }
            catch (Exception exception)
            {
                Debug.LogError("[INITIALIZATION] Failed to intialize Unity services");
                throw exception;
            }
        }

        private async Task SignInAsync()
        {
            Debug.Log($"[SIGN-IN] Starting sign in...");
            _playerAccount = new PlayerAccount();

            if (AuthenticationService.Instance.SessionTokenExists)
            {
                try
                {
                    Debug.Log($"[SIGN-IN] Signing in with session token");
                    await SignInAnonymouslyAsync(withLogging: false);
                    return;
                }
                catch
                {
                    Debug.LogWarning("[SIGN-IN] Session token doesn't seem to be valid anymore");
                    AuthenticationService.Instance.ClearSessionToken();
                }                            
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
                        Debug.Log($"[SIGN-IN] Signing with 'Username={username} and 'Password={password}'");
                        await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
                    }
                    catch (Exception exception)
                    {
                        Debug.LogError($"[SIGN-IN] Failed to authenticate in editor with 'Username={username} and 'Password={password}'");
                        throw exception;
                    }
                    break;
            }

            #else

            await SignInAnonymouslyAsync();

            #endif

            if (AuthenticationService.Instance.PlayerName.IsNullOrEmpty())
            {
                var playerName = string.Empty;

                #if UNITY_EDITOR
                
                playerName = Editor.EditorUtilities.GetAccoutPlayerName();

                #else

                playerName = Echo.Game.PlayerAccount.GeneratePlayerName();

                #endif

                try 
                {
                    Debug.Log($"[SIGN-IN] 'Player={PlayerAccount.GetPlayerId()}' has no name set. Setting it to '{playerName}'");
                    await AuthenticationService.Instance.UpdatePlayerNameAsync(playerName);
                }
                catch (Exception exception)
                {
                    Debug.LogError($"[SIGN-IN] Failed to set player name to '{playerName}'");
                    throw exception;
                }
            }
        }
        private async Task SignInAnonymouslyAsync(bool withLogging = true)
        {
            try
            {
                if (withLogging)
                    Debug.Log($"[SIGN-IN] Signing in anonymously");
                
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            catch (AuthenticationException authenticationException)
            {
                if (withLogging)
                    Debug.LogError($"[SIGN-IN] Encoutered an unexpected error while trying to authenticate anonymously. 'ErrorCode={authenticationException.ErrorCode}'");
                
                throw authenticationException;
            }
            catch (RequestFailedException requestFailedException)
            {
                if (withLogging)
                    Debug.LogError($"[SIGN-IN] Failed to authenticate anonymously. 'ErrorCode={requestFailedException.ErrorCode}'");

                throw requestFailedException;
            }
        }

        private async Task HandleErrorAsync(string reason, string description)
        {
            var operation = Addressables.LoadSceneAsync(_earlyOutSceneRef, LoadSceneMode.Additive);
            await operation.Task;

            var scene = operation.Result.Scene;
            var errorModel = scene.GetComponentInRoots<BlockingErrorModel>();

            if (errorModel != null)
            {
                errorModel.Reason = new LocalizedString("Common.Localization", "ERROR_INITIALIZATION_FAIL_REASON");
                errorModel.Description = new LocalizedString("Common.Localization", "ERROR_INITIALIZATION_FAIL_DESCRIPTION");
            }
        }

        #endregion

        #region Load Home

        public async void GoToHome()
        {
            await GoToHomeAsync();
        }
        private async Task GoToHomeAsync()
        {
            if (_matchSceneRef.IsLoaded())
                await UnloadMatch();

            var operation = Addressables.LoadSceneAsync(_homeSceneRef, LoadSceneMode.Additive);
            await operation.Task;
        }

        private async Task UnloadHome()
        {
            if (!_homeSceneRef.IsLoaded())
                return;

            var handle = _homeSceneRef.OperationHandle.Convert<SceneInstance>();
            var operation = Addressables.UnloadSceneAsync(handle.Result);
            await operation.Task;
        }

        #endregion

        #region Load Match

        public async void GoToMatch()
        {
            await GoToMatchAsync();
        }
        private async Task GoToMatchAsync()
        {
            if (_homeSceneRef.IsLoaded())
                await UnloadHome();

            var operation = Addressables.LoadSceneAsync(_matchSceneRef, LoadSceneMode.Additive);
            await operation.Task;
        }

        private async Task UnloadMatch()
        {
            if (!_matchSceneRef.IsLoaded())
                return;

            var handle = _matchSceneRef.OperationHandle.Convert<SceneInstance>();
            var operation = Addressables.UnloadSceneAsync(handle.Result);
            await operation.Task;
        }

        #endregion

        public void Quit()
        {
            StartCoroutine(QuitAsync());
        }
        public IEnumerator QuitAsync()
        {
            yield return _quitBehaviour.RunAsync();

            #if UNITY_EDITOR

            UnityEditor.EditorApplication.ExitPlaymode();
            
            #else

            Application.Quit();

            #endif
        }

        private void OnDestroy()
        {
            _playerAccount?.Dispose();

            Global.Game = null;
        }
    }
}