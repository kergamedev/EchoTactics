using UnityEditor;
using UnityEngine;

namespace SubAssetsToolbox.Editor
{
    [InitializeOnLoad]
    public static class ContextualActions
    {
        static ContextualActions()
        {
            EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemGUI;
        }

        private static void OnProjectWindowItemGUI(string guid, Rect selectionRect)
        {
            Event current = Event.current;
            Object obj;
            switch (current.type)
            {
                case EventType.MouseDown:
                    ClearLastSelection();
                    break;
                
                case EventType.KeyUp when current.keyCode == KeyCode.Delete ||
                                          current.keyCode == KeyCode.Backspace && current.command:
                    
                    obj = Selection.activeObject;
                    if (obj == null) return;
                    
                    if(AssetDatabase.IsSubAsset(obj))
                    {
                        // It's a sub-asset
                        bool decision = EditorUtility.DisplayDialog("Delete Sub-Asset",
                            $"Are you sure you want to delete {obj.name}?", "Delete",
                            "Cancel");

                        if (decision)
                        {
                            string parentObjectPath = AssetDatabase.GetAssetPath(obj);
                            SubAssetsToolbox.RemoveSubAsset(parentObjectPath, obj);
                            
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();
                        }
                    }
                    ClearLastSelection();
                    current.Use();
                    break;
                
                case EventType.ContextClick when selectionRect.Contains(current.mousePosition):
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    if (AssetDatabase.IsValidFolder(path))
                    {
                        ClearLastSelection();
                        return;
                    }
                    
                    obj = AssetDatabase.LoadAssetAtPath<Object>(path);
                    if (obj != null)
                    {
                        AssetRelocator.LastSelection = obj;
                    }
                    break;
                }
            }
        }

        private static void ClearLastSelection()
        {
            AssetRelocator.LastSelection = null;
        }
    }
}