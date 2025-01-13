using Echo.Common;
using Unity.Netcode;
using UnityEngine;

namespace Echo.Match
{
    public class Player : NetworkBehaviour
    {
        [SerializeField]
        private Character _characterPrefab;

        public Character Character { get; set; }

        private Match Match => (Match)Global.Match;

        public override void OnNetworkSpawn()
        {
            if (IsServer && !IsHost)
                return;

            var info = IsOwner && GetComponent<Bot>() == null ? Match.PlayerInfo : Match.EnemyInfo;

            Character = Instantiate(_characterPrefab);
            Character.Initialize(info);
        }
    }
}