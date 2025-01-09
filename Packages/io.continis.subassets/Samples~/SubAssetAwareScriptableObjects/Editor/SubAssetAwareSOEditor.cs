using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SubAssetsToolbox.Samples.Editor
{
    [CustomEditor(typeof(SubAssetAwareSO), true)]
    public class SubAssetAwareSOEditor : UnityEditor.Editor
    {
        public VisualTreeAsset lineTemplate;
        public VisualTreeAsset extrasTemplate;
        
        private SerializedProperty _subAssetsProp;

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement inspector = new();
            InspectorElement.FillDefaultInspector(inspector, serializedObject, this);
            
            _subAssetsProp = serializedObject.FindProperty("_subAssets");
            if (_subAssetsProp.arraySize != 0)
            {
                extrasTemplate.CloneTree(inspector);
                
                // Draw sub-assets list
                ListView subAssetsList = inspector.Q<ListView>("SubAssetsList");
                subAssetsList.makeItem += MakeItem;
                subAssetsList.bindItem += BindItem;
                subAssetsList.BindProperty(_subAssetsProp);
            }
            
            return inspector;
        }

        private VisualElement MakeItem()
        {
            return lineTemplate.Instantiate();
        }

        private void BindItem(VisualElement visualElement, int index)
        {
            Object subAsset = _subAssetsProp.GetArrayElementAtIndex(index).objectReferenceValue;
            visualElement.Q<ObjectField>("SubAssetField").value = subAsset;
        }
    }
}