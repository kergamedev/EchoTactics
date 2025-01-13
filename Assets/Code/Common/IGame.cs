using System.Threading.Tasks;
using Unity.Services.Matchmaker.Models;
using UnityEngine;
using UnityEngine.Localization;

namespace Echo.Common
{
    public interface IGame
    {
        TweenLibrary TweenLibrary { get; }
        IPlayerAccount PlayerAccount { get; }
        ISaveSystem SaveSystem { get; }
        bool IsMatchmakingOngoing { get; }
        MultiplayAssignment MatchmakingAssignment { get; set; }

        Task GoToHomeAsync(bool withTransition = true);
        Task GoToMatchAsync(bool withTransition = true);
        Task ChangeLocaleAsync(Locale locale);
        void Quit();
    }
}