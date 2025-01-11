using System.Linq;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UIElements;

namespace Echo.Editor
{
    [Overlay(editorWindowType = typeof(SceneView), displayName = "Localization"), Icon("Assets/Editor/Icons/ProfilerUIDetails.EditorIcon.png")]
    public class LocalizationOverlay : Overlay
    {
        private IMGUIContainer _container;

        public override VisualElement CreatePanelContent()
        {
            _container = new IMGUIContainer(OnImGUI);
            return _container;
        }

        private void OnImGUI()
        {
            var validity = CheckValidity(out var validityInfo);
            if (validity != MessageType.None)
            {
                EditorGUILayout.HelpBox(validityInfo, validity);
                return;
            }

            var options = LocalizationSettings.AvailableLocales.Locales.Select(locale => locale.Identifier.ToString()).ToArray();
            var previouslySelectedOptionIndex = LocalizationSettings.AvailableLocales.Locales.IndexOf(LocalizationSettings.SelectedLocale);
            var newlySelectedOptionIndex = EditorGUILayout.Popup(previouslySelectedOptionIndex, options);

            if (newlySelectedOptionIndex != previouslySelectedOptionIndex)
                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[newlySelectedOptionIndex];
        }

        private MessageType CheckValidity(out string message)
        {
            if (!EditorApplication.isPlaying)
            {
                message = "Can only change locale while in play";
                return MessageType.Info;
            }

            if (!LocalizationSettings.InitializationOperation.IsDone)
            {
                message = "Settings haven't been initialized yet";
                return MessageType.Warning;
            }

            message = null;
            return MessageType.None;
        }
    }
}
