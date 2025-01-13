using Echo.Common;
using Echo.Match;

namespace Echo.Game
{
    public class MatchStartArgs : IMatchStartArgs
    {
        public readonly IStartMachHandler StartHandler;

        public MatchStartArgs(IStartMachHandler startHandler)
        {
            StartHandler = startHandler;
        }
    }
}