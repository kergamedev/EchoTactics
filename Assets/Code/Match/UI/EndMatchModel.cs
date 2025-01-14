using DG.Tweening;
using Echo.Common;
using Sirenix.OdinInspector;
using System;
using Unity.Properties;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UIElements;

namespace Echo.Match
{
    public class EndMatchModel : Model, IOnMatchReady
    {
        [SerializeField]
        private UIDocument _viewPrefab;

        [SerializeField, DrawWithUnity, FoldoutGroup("Texts")]
        private LocalizedString _victoryText;

        [SerializeField, DrawWithUnity, FoldoutGroup("Texts")]
        private LocalizedString _defeatText;

        private VisualElement _frame;
        private VisualElement _goBackToHomeButton;

        private Match Match => (Match)Global.Match;

        [CreateProperty, NonSerialized, ReadOnly, ShowInInspector, FoldoutGroup("Data")]
        public LocalizedString Text;

        protected override void Awake()
        {
            // NO-OP
        }

        void IOnMatchReady.OnMatchReady()
        {
            Match.GameState.OnGameEnd += OnGameEnd;
        }

        private void OnGameEnd(Player winner, Player loser)
        {
            var view = Instantiate(_viewPrefab, transform);
            Initialize(view);

            _frame = Root.Q<VisualElement>("Frame");
            if (Match.NetworkManager.LocalClientId == winner.OwnerClientId)
            {
                _frame.AddToClassList("victory");
                Text = _victoryText;
            }
            else
            {
                _frame.AddToClassList("defeat");
                Text = _defeatText;
            }

            _goBackToHomeButton = Root.Q<VisualElement>("GoBackToHome");
            _goBackToHomeButton.AddManipulator(new Clickable(GoBackToHome));

            Global.Game.TweenLibrary.DoAppear(_frame);
        }

        private void GoBackToHome()
        {
            Global.Game.TweenLibrary.DoButtonClick(_goBackToHomeButton);
            Match.GoBackToHome();
        }
    }
}