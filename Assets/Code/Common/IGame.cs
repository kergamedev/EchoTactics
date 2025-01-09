using UnityEngine;

namespace Echo.Common
{
    public interface IGame
    {
        IPlayerAccount PlayerAccount { get; }

        void GoToHome();
        void GoToMatch();
    }
}