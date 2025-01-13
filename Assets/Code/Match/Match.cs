using Echo.Common;
using Sirenix.OdinInspector;
using System;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace Echo.Match
{
    public class Match : MonoBehaviour, IMatch
    {
        #region Nested types

        [Serializable]
        public class CharacterInfo
        {
            [SerializeField]
            private Material _material;

            [SerializeField]
            private Transform _spot;

            public Material Material => _material;
            public Transform Spot => _spot;
        }

        #endregion

        [SerializeField]
        private NetworkManager _networkManager;

        [SerializeField, LabelText("Player"), FoldoutGroup("Team Infos")]
        private CharacterInfo _playerInfo;

        [SerializeField, LabelText("Enemy"), FoldoutGroup("Team Infos")]
        private CharacterInfo _enemyInfo;

        private Server _server;

        public CharacterInfo PlayerInfo => _playerInfo;
        public CharacterInfo EnemyInfo => _enemyInfo;

        private async void OnEnable()
        {
            Global.Match = this;

            if (Utilities.IsServer())
            {
                _server = new Server(_networkManager);
                await _server.InitializeAsync();
            }
            else
            {
                if (Global.Game.MatchmakingAssignment != null)
                {
                    var transport = _networkManager.GetComponent<UnityTransport>();
                    var assignment = Global.Game.MatchmakingAssignment;
                    transport.SetConnectionData(assignment.Ip, (ushort)assignment.Port);
                }
                _networkManager.StartClient();
            }
        }

        private void Update()
        {
            if (_server != null)
                _server.Update();
        }

        private void OnDisable()
        {
            Global.Match = null;

            _server?.Dispose();
        }
    }
}