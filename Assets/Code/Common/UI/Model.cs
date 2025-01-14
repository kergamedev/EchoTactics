using UnityEngine;
using UnityEngine.UIElements;

namespace Echo.Common
{
    public abstract class Model : MonoBehaviour
    {
        private UIDocument _view;
        
        public UIDocument View => _view;

        protected VisualElement Root => _view.rootVisualElement;

        protected virtual void Awake()
        {
            var view = GetComponent<UIDocument>();
            Initialize(view);
        }

        protected void Initialize(UIDocument view)
        {
            _view = view;
            _view.rootVisualElement.dataSource = this;
        }
    }
}