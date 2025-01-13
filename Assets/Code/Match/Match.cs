using Echo.Common;
using Sirenix.OdinInspector;
using System;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace Echo.Match
{
    public class Match : MonoBehaviour, IMatch
    {
        #region Nested types

        [Serializable]
        public class PrefabLibrary
        {
            [SerializeField]
            private GameState _gameState;

            [SerializeField]
            private Bot _bot;

            public GameState GameState => _gameState;
            public Bot Bot => _bot;
        }

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

        [SerializeField, HideLabel, InlineProperty, FoldoutGroup("Prefabs")]
        private PrefabLibrary _prefabs;

        [SerializeField, LabelText("Player"), FoldoutGroup("Team Infos")]
        private CharacterInfo _playerInfo;

        [SerializeField, LabelText("Enemy"), FoldoutGroup("Team Infos")]
        private CharacterInfo _enemyInfo;

        private Server _server;

        public PrefabLibrary Prefabs => _prefabs;
        public CharacterInfo PlayerInfo => _playerInfo;
        public CharacterInfo EnemyInfo => _enemyInfo;
        public NetworkManager NetworkManager => _networkManager;
        public GameState GameState { get; private set; }

        public async Task InitializeAsync(IStartMachHandler startHandler)
        {
            Global.Match = this;

            if (startHandler == null)
                throw new Exception($"[MATCH] No '{nameof(IStartMachHandler)}' was provided");

            await startHandler.ExecuteAsync(this);
        }

        public async Task SetupServerAsync(Server server)
        {
            _server = server;
            await _server.InitializeAsync();
        }

        public void ReadyUp(GameState gameState)
        {
            GameState = gameState;
        }

        private void Update()
        {
            if (_server != null)
                _server.Update();
        }

        public void Shutdown()
        {
            if (GameState == null)
                return;

            if (NetworkManager.IsListening && NetworkManager.SpawnManager != null)
            {
                GameState.NetworkObject.Despawn();
                //...
            }
        }

        private void OnDisable()
        {
            _server?.Dispose();
            _server = null;

            Global.Match = null;
        }
    }
}