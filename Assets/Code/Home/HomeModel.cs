using Echo.Common;
using Sirenix.OdinInspector;
using System;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace Echo.Home
{
    public class HomeModel : Model
    {
        private Label _playerName;
        private VisualElement _fightButton;

        [CreateProperty, NonSerialized, ReadOnly, ShowInInspector, FoldoutGroup("Data"), HideInEditorMode]
        public bool CanMatchmake;

        private void Start()
        {
            _playerName = Root.Q<Label>("PlayerName");
            _playerName.text = Global.Game.PlayerAccount.GetPlayerName();

            _fightButton = Root.Q<VisualElement>("Fight");
            _fightButton.AddManipulator(new Clickable(GoToMatch));
        }

        private void Update()
        {
            var canMatchmake = !Global.Game.IsMatchmakingOngoing;
            if (canMatchmake != CanMatchmake)
                CanMatchmake = canMatchmake;
        }

        private void GoToMatch()
        {
            Global.Game.TweenLibrary.DoButtonClick(_fightButton);
            Global.Home.GoToMatchAsync();
        }
    }
}