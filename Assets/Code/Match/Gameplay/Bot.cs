using Unity.Netcode;
using UnityEngine;

namespace Echo.Match
{
    public class Bot : NetworkBehaviour
    {
        [SerializeField]
        private Player _player;
    }
}