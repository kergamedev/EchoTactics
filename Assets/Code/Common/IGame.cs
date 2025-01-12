using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Localization;

namespace Echo.Common
{
    public interface IGame
    {
        TweenLibrary TweenLibrary { get; }
        IPlayerAccount PlayerAccount { get; }
        ISaveSystem SaveSystem { get; }

        Task GoToHomeAsync(bool withTransition = true);
        Task GoToMatchAsync();
        Task ChangeLocaleAsync(Locale locale);
        void Quit();
    }
}