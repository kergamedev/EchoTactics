using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SubAssetsToolbox.Editor
{
    static class DragAndDropAddon
    {
        [InitializeOnLoadMethod]
        public static void AddDragProjectHandler() => DragAndDrop.AddDropHandler(ProjectWindowHandler);

        static DragAndDropVisualMode ProjectWindowHandler(int id, string destinationPath, bool released)
        {
            if (DragAndDrop.paths.Length == 0 || DragAndDrop.objectReferences.Length == 0)
            {
                // Was not dragging from another window
                return DragAndDropVisualMode.None;
            }
            
            if (released)
            {
                foreach (string path in DragAndDrop.paths)
                {
                    if (AssetDatabase.IsValidFolder(path)) return DragAndDropVisualMode.None; // Verify no folder is included in the selection
                    if (string.Equals(path, destinationPath)) return DragAndDropVisualMode.None; // Verify it's not dragging onto itself
                }

                if (!AssetDatabase.IsValidFolder(destinationPath))
                {
                    // Adding sub-asset(s)
                    // (drag destination is an asset)

                    // TODO: disable certain types of assets?
                    // if (!destinationPath.EndsWith(".asset") || !draggedObjectPath.EndsWith(".asset"))
                    // {
                    //     Debug.LogWarning("Can't add a sub-asset to this type of asset.");
                    //     return DragAndDropVisualMode.Link;
                    // }

                    string objectName = Path.GetFileNameWithoutExtension(destinationPath);

                    bool isOne = DragAndDrop.objectReferences.Length == 1;

                    int choice = EditorUtility.DisplayDialogComplex(
                        isOne ? "Add as Sub-Asset" : "Add as Sub-Assets",
                        isOne
                            ? $"Are you sure you want to add {DragAndDrop.objectReferences[0].name} as a sub-asset to {objectName}?"
                            : $"Are you sure you want to add {DragAndDrop.objectReferences[0].name}, {DragAndDrop.objectReferences[1].name}, (...) as sub-assets to {objectName}?",
                        isOne ? "Add" : "Add all",
                        "Cancel", 
                        isOne ? "Add and Keep Original" : "Add and Keep Originals");

                    foreach (Object draggedObject in DragAndDrop.objectReferences)
                    {
                        string draggedObjectPath = AssetDatabase.GetAssetPath(draggedObject);

                        if (choice != 1) // Add confirmed
                        {
                            // TODO: preserve selection if the object selected was the one being destroyed
                            SubAssetsToolbox.AddSubAsset(destinationPath, draggedObject);

                            if (choice == 0) // Also delete previous object
                            {
                                if (AssetDatabase.IsMainAsset(draggedObject))
                                    AssetDatabase.DeleteAsset(draggedObjectPath);
                                else
                                {
                                    // AssetAwareObject source: remove dragged to sub-assets array
                                    SubAssetsToolbox.RemoveSubAsset(draggedObjectPath, draggedObject);
                                }
                            }
                        }
                    }

                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    return DragAndDropVisualMode.Link;
                }
                else
                {
                    // Removing sub-asset(s)
                    // (drag destination is a folder or an empty space)

                    int rejected = 0;
                    
                    foreach (Object draggedObject in DragAndDrop.objectReferences)
                    {
                        string draggedObjectPath = AssetDatabase.GetAssetPath(draggedObject);
                        
                        if (AssetDatabase.IsSubAsset(draggedObject))
                        {
                            // Was dragging a sub-asset: un-parent
                            
                            // Check if its parent is in selection too
                            Object mainAssetAtPath = AssetDatabase.LoadMainAssetAtPath(draggedObjectPath);
                            if(DragAndDrop.objectReferences.Contains(mainAssetAtPath)) continue;

                            // Verify if there's an object with the same name
                            string desiredName = Path.Combine(destinationPath, $"{draggedObject.name}.asset");
                            if (AssetDatabase.LoadAssetAtPath<Object>(desiredName) != null)
                            {
                                Debug.LogWarning($"Impossible to extract {draggedObject.name} from its main asset. " +
                                                 "An asset with the same name already exists in the same location.");

                                rejected++;
                                continue;
                            }

                            Object newObject = Object.Instantiate(draggedObject);
                            newObject.name = draggedObject.name;
                            AssetDatabase.CreateAsset(newObject, desiredName);

                            SubAssetsToolbox.RemoveSubAsset(draggedObjectPath, draggedObject);
                        }
                    }

                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    
                    if(rejected != 0) return DragAndDropVisualMode.Rejected;
                    if (Path.GetDirectoryName(DragAndDrop.paths[0]) != destinationPath) return DragAndDropVisualMode.None; // Allows relocation of main assets
                    
                    return DragAndDropVisualMode.Move; // Avoid confirmation popup when drag/dropping in the same folder
                }
            }
            else
            {
                // While dragging

                if (AssetDatabase.IsValidFolder(destinationPath))
                {
                    // Over a folder
                    return DragAndDropVisualMode.Generic;
                }
                else
                {
                    // Over a file
                    return DragAndDropVisualMode.Link;
                }
            }
        }
    }
}