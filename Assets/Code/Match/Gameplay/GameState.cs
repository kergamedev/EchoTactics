using Echo.Common;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace Echo.Match
{
    public delegate void OnGameEnd(Player winner, Player loser);

    public class GameState : NetworkBehaviour
    {
        public event OnGameEnd OnGameEnd;

        private Match Match => (Match)Global.Match;

        public override void OnNetworkSpawn()
        {
            Match.ReadyUp(this);
        }

        [Rpc(SendTo.Server)]
        public void DeclareWinnerRpc(ulong clientId)
        {
            // Do leaderboard, ranking updates and the likes...
            OnEndRpc(clientId);
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void OnEndRpc(ulong winnerId)
        {
            var players = gameObject.scene.GetComponentsInRoots<Player>();
            var winner = players.FirstOrDefault(player => player.OwnerClientId == winnerId);
            var loser = players.FirstOrDefault(player => player != winner);

            OnGameEnd?.Invoke(winner, loser);
        }
    }
}