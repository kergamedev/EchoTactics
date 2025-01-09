using Echo.Common;
using System;
using Unity.Services.Authentication;
using UnityEngine;

namespace Echo.Game
{
    internal class PlayerAccount : IPlayerAccount, IDisposable
    {
        public PlayerAccount()
        {
            AuthenticationService.Instance.SignedIn += OnPlayerSignedIn;
            AuthenticationService.Instance.SignedOut += OnPlayerSignedOut;
        }

        private void OnPlayerSignedIn()
        {
            Debug.Log($"'Player={GetPlayerName()}' has signed in");
        }

        public string GetPlayerName()
        {
            var playerName = AuthenticationService.Instance.PlayerName;
            if (playerName.IsNullOrEmpty())
                playerName = "N/A";

            return playerName;
        }

        private void OnPlayerSignedOut()
        {
            Debug.Log($"Player has signed out");
        }

        public void Dispose()
        {
            AuthenticationService.Instance.SignedIn -= OnPlayerSignedIn;
            AuthenticationService.Instance.SignedOut -= OnPlayerSignedOut;
        }
    }
}