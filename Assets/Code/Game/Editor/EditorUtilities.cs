#if UNITY_EDITOR

using System;
using UnityEditor;

namespace Echo.Game.Editor
{
    public static class EditorUtilities
    {
        public const string SIGN_IN_METHOD_PREF_KEY = "SignInMethod";
        public const string UNITY_ACCOUNT_PASSWORD_PREF_KEY = "UnityAcount.Password";
        public const string ACCOUNT_PLAYER_NAME_KEY = "Acount.PlayerName";

        private const string PASSWORD_CHARS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!:-_%$*^¨";

        public static EditorSignInMethod GetSignInMethod()
        {
            if (!EditorPrefs.HasKey(SIGN_IN_METHOD_PREF_KEY))
                return EditorSignInMethod.Anonymous;

            return (EditorSignInMethod)EditorPrefs.GetInt(SIGN_IN_METHOD_PREF_KEY);
        }

        public static string GetUnityAccountUsername()
        {
            var username = CloudProjectSettings.userName;
            if (username.Contains('@'))
                username = username.Split('@')[0];

            return username;
        }

        public static string GetUnityAccountPassword()
        {
            if (!EditorPrefs.HasKey(UNITY_ACCOUNT_PASSWORD_PREF_KEY))
            {
                var password = GeneratePassword();
                EditorPrefs.SetString(UNITY_ACCOUNT_PASSWORD_PREF_KEY, password);
                return password;
            }

            return EditorPrefs.GetString(UNITY_ACCOUNT_PASSWORD_PREF_KEY);
        }

        public static string GeneratePassword()
        {
            var random = new Random(DateTime.Now.ToString().GetHashCode());
            var password = string.Empty;
            for (var i = 0; i < 8; i++)
                password += PASSWORD_CHARS[random.Next(PASSWORD_CHARS.Length)];

            return password;
        }

        public static string GetAccoutPlayerName()
        {
            if (!EditorPrefs.HasKey(ACCOUNT_PLAYER_NAME_KEY))
            {
                var playerName = PlayerAccount.GeneratePlayerName();
                EditorPrefs.SetString(ACCOUNT_PLAYER_NAME_KEY, playerName);
                return playerName;
            }

            return EditorPrefs.GetString(ACCOUNT_PLAYER_NAME_KEY);
        }
    }
}

#endif