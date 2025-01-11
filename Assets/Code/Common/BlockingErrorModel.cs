using Sirenix.OdinInspector;
using Unity.Properties;
using UnityEngine.UIElements;

namespace Echo.Common
{
    public class BlockingErrorModel : Model
    {       
        private string _reason;
        private string _description;
        private Button _quitButton;

        [CreateProperty, ReadOnly, ShowInInspector, FoldoutGroup("Data"), HideInEditorMode]
        public string Reason
        {
            get => _reason;
            set => _reason = value;
        }

        [CreateProperty, ReadOnly, ShowInInspector, FoldoutGroup("Data"), HideInEditorMode]
        public string Description
        {
            get => _description;
            set => _description = value;
        }

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