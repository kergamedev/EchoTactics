using Echo.Common;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Multiplayer.Playmode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Matchmaker;
using Unity.Services.Matchmaker.Models;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

using Player = Unity.Services.Matchmaker.Models.Player;

namespace Echo.Game
{
    public class Game : MonoBehaviour, IGame
    {
        #if UNITY_EDITOR

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void OnBeforePlay()
        {
            DoGlobalReset();
        }

        private static void OnPlayModeStateChange(UnityEditor.PlayModeStateChange stateChange)
        {
            if (stateChange == UnityEditor.PlayModeStateChange.ExitingPlayMode)
                DoGlobalReset();
        }

        private static void DoGlobalReset()
        {
            Global.Reset();
        }

        #endif

        [SerializeField, FoldoutGroup("Transitions")]
        private RoutineBehaviour _splashScreenTransition;

        [SerializeField, FoldoutGroup("Transitions")]
        private RoutineBehaviour _quitTransition;

        [SerializeField, FoldoutGroup("Transitions")]
        private LoadingTransition _genericLoadingTransition;

        [SerializeField, FoldoutGroup("Scenes")]
        private AssetReference _homeSceneRef;

        [SerializeField, FoldoutGroup("Scenes")]
        private AssetReference _matchSceneRef;

        [SerializeField, FoldoutGroup("Scenes")]
        private AssetReference _earlyOutSceneRef;

        [SerializeField, FoldoutGroup("References")]
        private TweenLibrary _tweenLibrary;

        private SceneInstance? _homeScene;
        private SceneInstance? _matchScene;
        private PlayerAccount _playerAccount;
        private CloudSaveSystem _saveSystem;

        public TweenLibrary TweenLibrary => _tweenLibrary;
        public IPlayerAccount PlayerAccount => _playerAccount;
        public ISaveSystem SaveSystem => _saveSystem;
        public bool IsMatchmakingOngoing { get; private set; }
        public MultiplayAssignment MatchmakingAssignment { get; set; }

        private async void OnEnable()
        {
            Global.Game = this;

            await InitializeAsync();
            await _splashScreenTransition.AsTask();
        }

        #region Initialization

        private async Task InitializeAsync()
        {
            Debug.Log($"[INITIALIZATION] Starting initialization...");
           
            try
            {
                if (Utilities.IsServer())
                {
                    await InitializeServicesAsync();
                }
                else
                {
                    await InitializeLocalizationAsync();
                    await InitializeServicesAsync();
                    await SignInAsync();
                    await InitializeSaveAsync();
                }              
            }
            catch (Exception exception)
            {
                Debug.LogError("[INITIALIZATION] Caught an unexpected error while trying to initialize game");
                Debug.LogException(exception);

                await HandleErrorAsync();
                return;
            }

            if (Utilities.IsServer())
                await GoToMatchAsync(withTransitions: false);
            else await GoToHomeAsync(withTransition: false);
        }

        private async Task InitializeLocalizationAsync()
        {
            Debug.Log($"[LOCALIZATION] Waiting for localization initialization...");
            await LocalizationSettings.InitializationOperation.Task;
            ConverterGroups.RegisterGlobalConverter((ref LocalizedString stringRef) => stringRef.GetLocalizedString());
        }

        private async Task InitializeServicesAsync()
        {
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

            var canUseSessionToken = true;

            #if UNITY_EDITOR

            if (!Editor.EditorUtilities.IsMainEditorPlayer())
                canUseSessionToken = false;

            #endif

            if (canUseSessionToken && AuthenticationService.Instance.SessionTokenExists)
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
                        Debug.Log($"[SIGN-IN] Trying to sign-in 'Username={username} and 'Password={password}'");
                        await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
                    }
                    catch
                    {
                        Debug.LogWarning($"[SIGN-IN] Failed to sign-in with 'Username={username} and 'Password={password}'. Trying sign-up");
                        try
                        {
                            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
                        }
                        catch (Exception exception)
                        {
                            Debug.LogError($"[SIGN-IN] Failed to authenticate in editor with 'Username={username} and 'Password={password}'");
                            throw exception;
                        }                       
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

        private async Task InitializeSaveAsync()
        {
            Debug.Log($"[SAVE] Initializing Save System...");
            _saveSystem = new CloudSaveSystem();

            var savedLocaleCode = await SaveSystem.LoadKeyValue<string>(SaveKeys.SELECTED_LOCALE);
            if (!savedLocaleCode.IsNullOrEmpty())
            {
                var savedLocale = LocalizationSettings.AvailableLocales.GetLocale(new LocaleIdentifier(savedLocaleCode));
                if (savedLocale != null && LocalizationSettings.SelectedLocale != savedLocale)
                {
                    Debug.LogWarning($"[LOCALIZATION] Correcting mismatch between 'InitialLocale={LocalizationSettings.SelectedLocale.Identifier}' and 'SavedLocale={savedLocale.Identifier}'");

                    LocalizationSettings.SelectedLocale = savedLocale;
                    PlayerPrefs.SetString(SaveKeys.SELECTED_LOCALE, savedLocaleCode);
                }
            }
        }

        private async Task HandleErrorAsync()
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

        public async Task GoToHomeAsync(bool withTransition = true)
        {
            if (withTransition)
                await _genericLoadingTransition.StartAsTask();

            if (_matchScene.IsLoaded())
                await UnloadMatch();

            var operation = Addressables.LoadSceneAsync(_homeSceneRef, LoadSceneMode.Additive);
            await operation.Task;

            _homeScene = operation.Result;
            
            if (withTransition)
                await _genericLoadingTransition.EndAsTask();
        }

        private async Task UnloadHome()
        {
            if (!_homeScene.IsLoaded())
                return;

            var operation = Addressables.UnloadSceneAsync(_homeScene.Value);
            await operation.Task;

            _homeScene = null;
        }

        #endregion

        #region Load Match

        public async Task GoToMatchAsync(bool withTransitions = true)
        {
            MatchmakingAssignment = null;

            if (!Utilities.IsServer())
            {
                #if UNITY_EDITOR

                if (CurrentPlayer.ReadOnlyTags().Any(tag => tag == "Matchmake"))
                    await FindMatchAsync();
                else Debug.Log("[MATCHMAKING] Going into local match");

                #else
                
                await FindMatchAsync();
                
                #endif
            }

            if (withTransitions)
                await _genericLoadingTransition.StartAsTask();

            if (_homeScene.IsLoaded())
                await UnloadHome();

            var operation = Addressables.LoadSceneAsync(_matchSceneRef, LoadSceneMode.Additive);
            await operation.Task;

            _matchScene = operation.Result;
            
            if (withTransitions)
                await _genericLoadingTransition.EndAsTask();
        }

        private async Task FindMatchAsync()
        {
            Debug.Log("[MATCHMAKING] Finding a match via matchmaking...");
            var playersInTicket = new List<Player>
            {
                new (AuthenticationService.Instance.PlayerId, new Dictionary<string, object>())
            };
            var ticketOptions = new CreateTicketOptions("Default", new Dictionary<string, object>());

            IsMatchmakingOngoing = true;
            while (!await FindMatch(playersInTicket, ticketOptions))
                await Awaitable.WaitForSecondsAsync(1.5f);

            IsMatchmakingOngoing = false;
        }
        private async Task<bool> FindMatch(List<Player> playersInTicket, CreateTicketOptions ticketOptions)
        {
            try
            {
                var ticketResponse = await MatchmakerService.Instance.CreateTicketAsync(playersInTicket, ticketOptions);
                while (true)
                {
                    var ticketStatus = await MatchmakerService.Instance.GetTicketAsync(ticketResponse.Id);
                    if (ticketStatus?.Value is not MultiplayAssignment assignment)
                        continue;

                    Debug.Log($"[MATCHMAKING] Status update: '{assignment.Status}'");
                    switch (assignment.Status)
                    {
                        case MultiplayAssignment.StatusOptions.Found:
                            if (!assignment.Port.HasValue)
                            {
                                Debug.LogError("[MATCHMAKING] No port was provided in the response");
                                return false;
                            }

                            Debug.Log("[MATCHMAKING] Found match");
                            MatchmakingAssignment = assignment;
                            return true;

                        case MultiplayAssignment.StatusOptions.Timeout:
                        case MultiplayAssignment.StatusOptions.Failed:
                            Debug.LogError($"[MATCHMAKING] Failed with 'Reason={assignment}'");
                            return false;
                    }

                    await Awaitable.WaitForSecondsAsync(1.5f);
                }
            }
            catch(Exception exception)
            {
                Debug.LogError($"[MATCHMAKING] Encountered an issue...");
                Debug.LogException(exception);
                return false;
            }
        }

        private async Task UnloadMatch()
        {
            if (!_matchScene.IsLoaded())
                return;

            var operation = Addressables.UnloadSceneAsync(_matchScene.Value);
            await operation.Task;

            _matchScene = null;
        }

#endregion

        public async Task ChangeLocaleAsync(Locale locale)
        {
            LocalizationSettings.SelectedLocale = locale;
            var localeCode = locale.Identifier.Code;

            await _saveSystem.SaveKeyValue(SaveKeys.SELECTED_LOCALE, localeCode);
            PlayerPrefs.SetString(SaveKeys.SELECTED_LOCALE, localeCode);
        }

        public void Quit()
        {
            StartCoroutine(QuitAsync());
        }
        public IEnumerator QuitAsync()
        {
            yield return _quitTransition.RunAsync();

#if UNITY_EDITOR

            UnityEditor.EditorApplication.ExitPlaymode();
            
#else

            Application.Quit();

#endif
        }

        private void OnDisable()
        {
            _homeScene = null;
            _matchScene = null;

            _playerAccount?.Dispose();
            _playerAccount = null;

            _saveSystem = null;
            
            Global.Game = null;
        }
    }
}