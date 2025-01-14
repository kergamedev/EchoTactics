#if UNITY_EDITOR

using System;
using System.Linq;
using System.Threading.Tasks;
using Unity.Multiplayer.Playmode;
using Unity.Services.Authentication;
using UnityEditor;
using UnityEngine;

namespace Echo.Game.Editor
{
    public static class EditorUtilities
    {
        public const string SIGN_IN_METHOD_PREF_KEY = "SignInMethod";
        public const string UNITY_ACCOUNT_PASSWORD_PREF_KEY = "UnityAcount.Password";
        public const string ACCOUNT_PLAYER_NAME_KEY = "Acount.PlayerName";

        private const string PASSWORD_CHARS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!:-_%$*^¨";

        public static bool IsMainEditorPlayer()
        {
            return !Application.dataPath.Contains("/VP/");
        }

        public static EditorSignInMethod GetSignInMethod()
        {
            if (!IsMainEditorPlayer())
                return EditorSignInMethod.UnityAccount;

            if (!EditorPrefs.HasKey(SIGN_IN_METHOD_PREF_KEY))
                return EditorSignInMethod.Anonymous;

            return (EditorSignInMethod)EditorPrefs.GetInt(SIGN_IN_METHOD_PREF_KEY);
        }

        public static async Task SignWithUnityAccount(bool withLogs = true)
        {
            var username = GetUnityAccountUsername();
            var password = GetUnityAccountPassword();

            try
            {
                if (withLogs)
                    Debug.Log($"[SIGN-IN] Trying to sign-in 'Username={username} and 'Password={password}'");

                await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
            }
            catch
            {
                if (withLogs)
                    Debug.LogWarning($"[SIGN-IN] Failed to sign-in with 'Username={username} and 'Password={password}'. Trying sign-up");

                try
                {
                    await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
                }
                catch (Exception exception)
                {
                    if (withLogs)
                        Debug.LogError($"[SIGN-IN] Failed to authenticate in editor with 'Username={username} and 'Password={password}'");

                    throw exception;
                }
            }
        }

        public static string GetUnityAccountUsername()
        {
            var username = CloudProjectSettings.userName;
            if (username.Contains('@'))
                username = username.Split('@')[0];

            if (!IsMainEditorPlayer())
                username += $"-Dup";

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
            var random = new System.Random(DateTime.Now.ToString().GetHashCode());
            var password = string.Empty;
            for (var i = 0; i < 8; i++)
                password += PASSWORD_CHARS[random.Next(PASSWORD_CHARS.Length)];

            return password;
        }

        public static string GetAccoutPlayerName()
        {
            var playerName = string.Empty;

            if (!EditorPrefs.HasKey(ACCOUNT_PLAYER_NAME_KEY))
            {
                playerName = PlayerAccount.GeneratePlayerName();
                EditorPrefs.SetString(ACCOUNT_PLAYER_NAME_KEY, playerName);
            }
            else playerName = EditorPrefs.GetString(ACCOUNT_PLAYER_NAME_KEY);

            if (!IsMainEditorPlayer())
                playerName += $".Dup";

            return playerName;
        }
    }
}

#endif