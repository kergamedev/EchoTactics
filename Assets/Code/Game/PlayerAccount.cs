using Echo.Common;
using System;
using Unity.Services.Authentication;
using UnityEngine;

using Random = System.Random;

namespace Echo.Game
{
    public class PlayerAccount : IPlayerAccount, IDisposable
    {
        private static readonly string[] PLAYER_NAME_ADJECTIVES = new string[]
        {
            "Cute",
            "Happy",
            "Angry",
            "Hungry",
            "Sad",
            "Hyper",
            "Dope",
            "Pro",
            "Cool",
            "Lazy",
            "Great",
            "Wise",
            "Young",
            "Old"
        };
        private static readonly string[] PLAYER_ANIMAL_NAMES = new string[]
        {
            "Hippo",
            "Frog",
            "Cat",
            "Dog",
            "Bat",
            "Fish",
            "Whale",
            "Monkey",
            "Lion",
            "Zebra",
            "Crab",
            "Hamster"
        };

        public static string GeneratePlayerName()
        {
            var random = new Random(DateTime.Now.ToString().GetHashCode());

            var firstAdjective = PLAYER_NAME_ADJECTIVES[random.Next(PLAYER_ANIMAL_NAMES.Length)];
            var secondAdjective = PLAYER_NAME_ADJECTIVES[random.Next(PLAYER_ANIMAL_NAMES.Length)];
            var animalName = PLAYER_ANIMAL_NAMES[random.Next(PLAYER_ANIMAL_NAMES.Length)];
            return $"{firstAdjective}{secondAdjective}{animalName}";
        }

        public PlayerAccount()
        {
            AuthenticationService.Instance.SignedIn += OnPlayerSignedIn;
            AuthenticationService.Instance.SignedOut += OnPlayerSignedOut;
        }

        private void OnPlayerSignedIn()
        {
            Debug.Log($"[PLAYER-ACCOUNT] 'Player={GetPlayerName()}' has signed in");
        }

        public string GetPlayerId()
        {
            return AuthenticationService.Instance.PlayerId;
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
            Debug.Log($"[PLAYER-ACCOUNT] Player has signed out");
        }

        public void Dispose()
        {
            AuthenticationService.Instance.SignedIn -= OnPlayerSignedIn;
            AuthenticationService.Instance.SignedOut -= OnPlayerSignedOut;
        }
    }
}