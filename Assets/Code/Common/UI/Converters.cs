using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace Echo.Common
{
    public class Converters
    {
        #if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        #else
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        #endif
        public static void RegisterConverters()
        {
            var invertBool = new ConverterGroup("Invert");
            invertBool.AddConverter((ref bool value) => !value);
            ConverterGroups.RegisterConverterGroup(invertBool);
        }
    }
}