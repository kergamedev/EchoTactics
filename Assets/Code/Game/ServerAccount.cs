using Echo.Common;
using System;

namespace Echo.Game
{
    public class ServerAccount : IPlayerAccount
    {
        public bool IsAdmin => true;

        public string GetPlayerId()
        {
            return string.Empty;
        }

        public string GetPlayerName()
        {
            return "Server";
        }

        void IDisposable.Dispose()
        {
            // NO-OP
        }
    }
}