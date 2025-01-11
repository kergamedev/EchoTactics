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

        void GoToHome();
        void GoToMatch();
        Task ChangeLocaleAsync(Locale locale);
        void Quit();
    }
}