using Echo.Game;
using Echo.Game.Editor;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

namespace Echo.Editor
{
    [Overlay(editorWindowType = typeof(SceneView), displayName = "Player Account"), Icon("Assets/Editor/Icons/InDevelopment.EditorIcon.png")]
    public class PlayerAccountOverlay : Overlay
    {
        private IMGUIContainer _container;

        public override void OnCreated()
        {
            if (!EditorPrefs.HasKey(EditorUtilities.SIGN_IN_METHOD_PREF_KEY))
                EditorPrefs.SetInt(EditorUtilities.SIGN_IN_METHOD_PREF_KEY, (int)EditorSignInMethod.Anonymous);

            if (!EditorPrefs.HasKey(EditorUtilities.UNITY_ACCOUNT_PASSWORD_PREF_KEY))
                EditorPrefs.SetString(EditorUtilities.UNITY_ACCOUNT_PASSWORD_PREF_KEY, EditorUtilities.GeneratePassword());

            if (!EditorPrefs.HasKey(EditorUtilities.UNITY_ACCOUNT_PASSWORD_PREF_KEY))
                EditorPrefs.SetString(EditorUtilities.UNITY_ACCOUNT_PASSWORD_PREF_KEY, PlayerAccount.GeneratePlayerName());

            base.OnCreated();
        }

        public override VisualElement CreatePanelContent()
        {
            _container = new IMGUIContainer(OnImGUI);
            return _container;
        }

        private void OnImGUI()
        {           
            var previousLabelWidth = EditorGUIUtility.labelWidth;
            var previousFieldWidth = EditorGUIUtility.fieldWidth;

            var halfWidth = (previousLabelWidth + previousFieldWidth) * 0.5f;
            EditorGUIUtility.labelWidth = halfWidth;
            EditorGUIUtility.fieldWidth = halfWidth;

            var previousSignInMethod = (EditorSignInMethod)EditorPrefs.GetInt(EditorUtilities.SIGN_IN_METHOD_PREF_KEY);
            var newSignInMethod = (EditorSignInMethod)EditorGUILayout.EnumPopup("Sign-In Method", previousSignInMethod);

            if (newSignInMethod != previousSignInMethod)
                EditorPrefs.SetInt(EditorUtilities.SIGN_IN_METHOD_PREF_KEY, (int)newSignInMethod);

            if (newSignInMethod == EditorSignInMethod.UnityAccount)
            {
                using (new EditorGUI.DisabledGroupScope(true))
                    EditorGUILayout.TextField("Username", EditorUtilities.GetUnityAccountUsername());

                var previousPassword = EditorPrefs.GetString(EditorUtilities.UNITY_ACCOUNT_PASSWORD_PREF_KEY);
                var newPassword = EditorGUILayout.TextField("Password", previousPassword);

                if (newPassword != previousPassword)
                    EditorPrefs.SetString(EditorUtilities.UNITY_ACCOUNT_PASSWORD_PREF_KEY, newPassword);
            }

            var previousPlayerName = EditorPrefs.GetString(EditorUtilities.ACCOUNT_PLAYER_NAME_KEY);
            var newPlayerName = EditorGUILayout.TextField("Player Name", previousPlayerName);

            if (newPlayerName != previousPlayerName)
                EditorPrefs.SetString(EditorUtilities.ACCOUNT_PLAYER_NAME_KEY, newPlayerName);

            EditorGUIUtility.labelWidth = previousLabelWidth;
            EditorGUIUtility.fieldWidth = previousFieldWidth;
        }
    }
}
