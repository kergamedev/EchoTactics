using UnityEngine;

namespace Echo.Common
{
    public interface IGame
    {
        TweenLibrary TweenLibrary { get; }
        IPlayerAccount PlayerAccount { get; }

        void GoToHome();
        void GoToMatch();
        void Quit();
    }
}