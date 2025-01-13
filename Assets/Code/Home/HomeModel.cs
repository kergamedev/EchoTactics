using Echo.Common;
using Sirenix.OdinInspector;
using System;
using System.Threading;
using Unity.Properties;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.UIElements;

namespace Echo.Home
{
    public class HomeModel : Model
    {
        [SerializeField, DrawWithUnity, FoldoutGroup("Texts")]
        private LocalizedString _playOnlineText;

        [SerializeField, DrawWithUnity, FoldoutGroup("Texts")]
        private LocalizedString _cancelText;

        private Label _playerNameLabel;
        private VisualElement _content;
        private VisualElement _playSoloButton;
        private VisualElement _playOnlineButton;

        private CancellationTokenSource _cancelMatchmaking;
        private float? _matchmakingStartTimestamp;

        [CreateProperty, ReadOnly, ShowInInspector, FoldoutGroup("Data"), HideInEditorMode]
        public bool IsMatchmakingOngoing
        {
            get => _isMatchmakingOngoing;
            set
            {
                if (_isMatchmakingOngoing == value)
                    return;

                if (value)
                {
                    _content.AddToClassList("matchmaking-ongoing");
                    PlayOnlineButtonText = _cancelText;
                    _matchmakingStartTimestamp = Time.realtimeSinceStartup;
                }
                else
                {
                    _content.RemoveFromClassList("matchmaking-ongoing");
                    PlayOnlineButtonText = _playOnlineText;
                    _matchmakingStartTimestamp = null;
                }

                _isMatchmakingOngoing = value;
            }
        }
        private bool _isMatchmakingOngoing;

        [CreateProperty, NonSerialized, ReadOnly, ShowInInspector, FoldoutGroup("Data")]
        public LocalizedString PlayOnlineButtonText;

        [CreateProperty, DrawWithUnity, ShowInInspector, FoldoutGroup("Data")]
        public LocalizedString MatchmakingSearchText;

        private void Start()
        {
            _playerNameLabel = Root.Q<Label>("PlayerName");
            _playerNameLabel.text = Global.Game.PlayerAccount.GetPlayerName();

            _content = Root.Q<VisualElement>("Content");

            _playSoloButton = Root.Q<VisualElement>("PlaySolo");
            _playSoloButton.AddManipulator(new Clickable(StartSoloMatch));

            _playOnlineButton = Root.Q<VisualElement>("PlayOnline");
            _playOnlineButton.AddManipulator(new Clickable(StartOnlineMatch));

            PlayOnlineButtonText = _playOnlineText;
        }

        private void Update()
        {
            if (_matchmakingStartTimestamp != null 
                && MatchmakingSearchText.TryGetValue("elapsed", out var variable)
                && variable is FloatVariable elapsedVariable)
            {
                elapsedVariable.Value = Mathf.FloorToInt(Time.realtimeSinceStartup - _matchmakingStartTimestamp.Value);
                MatchmakingSearchText.RefreshString();
            }
        }

        private void StartSoloMatch()
        {
            Global.Game.TweenLibrary.DoButtonClick(_playSoloButton);
            Global.Game.StartSoloMatchAsync();
        }

        private async void StartOnlineMatch()
        {
            Global.Game.TweenLibrary.DoButtonClick(_playOnlineButton);

            if (_cancelMatchmaking != null)
            {
                _cancelMatchmaking.Cancel();
                return;
            }

            _cancelMatchmaking = new CancellationTokenSource();
            var matchStartArgs = default(IMatchStartArgs);

            try
            {
                IsMatchmakingOngoing = true;
                matchStartArgs = await Global.Game.TrySetupOnlineMatchAsync(_cancelMatchmaking.Token);
            }
            catch (OperationCanceledException _)
            {
                matchStartArgs = null;
            }

            IsMatchmakingOngoing = false;
            _cancelMatchmaking = null;

            if (matchStartArgs != null)
                _ = Global.Game.StartMatchAsync(matchStartArgs);
        }
    }
}