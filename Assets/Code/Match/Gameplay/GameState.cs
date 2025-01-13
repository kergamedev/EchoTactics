using Echo.Common;
using Unity.Netcode;

namespace Echo.Match
{
    public class GameState : NetworkBehaviour
    {
        private Match Match => (Match)Global.Match;

        public override void OnNetworkSpawn()
        {
            Match.ReadyUp(this);
        }
    }
}