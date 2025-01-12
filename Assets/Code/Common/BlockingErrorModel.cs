using Sirenix.OdinInspector;
using System;
using Unity.Properties;
using UnityEngine.Localization;
using UnityEngine.UIElements;

namespace Echo.Common
{
    public class BlockingErrorModel : Model
    {
        private Button _quitButton;

        [CreateProperty, NonSerialized, ReadOnly, ShowInInspector, FoldoutGroup("Data"), HideInEditorMode]
        public LocalizedString Reason;

        [CreateProperty, NonSerialized, ReadOnly, ShowInInspector, FoldoutGroup("Data"), HideInEditorMode]
        public LocalizedString Description;

        private void Start()
        {
            base.Awake();

            _quitButton = Root.Q<Button>("Quit-Button");
            _quitButton.clicked += Quit;         
        }

        private void Quit()
        {
            Global.Game.TweenLibrary.DoButtonClick(_quitButton);
            Global.Game.Quit();
        }

        private void OnDestroy()
        {
            _quitButton.clicked -= Quit;
        }
    }
}