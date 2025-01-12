using Echo.Common;
using UnityEngine;
using UnityEngine.UIElements;

namespace Echo.Home
{
    public class HomeModel : Model
    {
        private Label _playerName;
        private VisualElement _fightButton;

        private void Start()
        {
            _playerName = Root.Q<Label>("PlayerName");
            _playerName.text = Global.Game.PlayerAccount.GetPlayerName();

            _fightButton = Root.Q<VisualElement>("Fight");
            _fightButton.AddManipulator(new Clickable(GoToMatch));
        }

        private void GoToMatch()
        {
            Global.Game.TweenLibrary.DoButtonClick(_fightButton);
            Global.Home.GoToMatchAsync();
        }
    }
}