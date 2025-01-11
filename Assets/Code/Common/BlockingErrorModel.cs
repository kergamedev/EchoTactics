using Sirenix.OdinInspector;
using Unity.Properties;
using UnityEngine.Localization;
using UnityEngine.UIElements;

namespace Echo.Common
{
    public class BlockingErrorModel : Model
    {
        private Button _quitButton;

        [CreateProperty, ReadOnly, ShowInInspector, FoldoutGroup("Data"), HideInEditorMode]
        public LocalizedString Reason;

        [CreateProperty, ReadOnly, ShowInInspector, FoldoutGroup("Data"), HideInEditorMode]
        public LocalizedString Description;

        protected override void Awake()
        {
            base.Awake();

            _quitButton = Root.Q<Button>("Quit-Button");
            _quitButton.clicked += Quit;         
        }

        public void Quit()
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