using System;

namespace Echo.Common
{
    public interface IPlayerAccount : IDisposable
    {
        bool IsAdmin { get; }

        string GetPlayerId();
        string GetPlayerName();
    }
}