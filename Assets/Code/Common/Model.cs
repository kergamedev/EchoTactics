using UnityEngine;
using UnityEngine.UIElements;

namespace Echo.Common
{
    [RequireComponent(typeof(UIDocument))]
    public abstract class Model : MonoBehaviour
    {
        private UIDocument _view;
        
        public UIDocument View => _view;

        protected VisualElement Root => _view.rootVisualElement;

        protected virtual void Awake()
        {
            _view = GetComponent<UIDocument>();
            _view.rootVisualElement.dataSource = this;
        }
    }
}