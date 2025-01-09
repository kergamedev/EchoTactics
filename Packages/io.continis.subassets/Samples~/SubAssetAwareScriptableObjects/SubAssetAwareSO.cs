using System.Collections.Generic;
using UnityEngine;

namespace SubAssetsToolbox.Samples
{
    /// <summary>
    /// ScriptableObject class that implements the <see cref="ISubAssetAware"/> interface to interact with sub-asset workflows.
    /// Contains a List to keep references to its sub-assets. The list is displayed in the asset's Inspector using a custom editor.
    /// Use this class as a starting point, and implement your own sub-asset aware ScriptableObjects.
    /// </summary>
    [CreateAssetMenu(menuName = "SubAssets Toolbox/SubAsset Aware SO", fileName = "SubAssetAwareSO")]
    public class SubAssetAwareSO : ScriptableObject, ISubAssetAware
    {
        [SerializeField, HideInInspector] private List<Object> _subAssets;

        public void AddSubAsset(Object newSubAsset) => _subAssets.Add(newSubAsset);
        public void RemoveSubAsset(Object removedSubAsset) => _subAssets.Remove(removedSubAsset);

        public virtual Object GetSubAssetByName(string subAssetName) => _subAssets.Find(o => o.name == subAssetName);
        public virtual List<Object> GetAllSubAssetsByName(string subAssetName) => _subAssets.FindAll(o => o.name == subAssetName);
    
        public virtual Object GetSubAssetByType<T>() => _subAssets.Find(o => o.GetType() == typeof(T));
        public virtual List<Object> GetAllSubAssetsByType<T>() => _subAssets.FindAll(o => o.GetType() == typeof(T));
    }
}