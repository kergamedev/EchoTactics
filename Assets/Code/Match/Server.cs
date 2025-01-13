using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

#if DEDICATED_SERVER

using Unity.Services.Multiplay;

#endif

namespace Echo.Match
{
    public class Server : IDisposable
    {       
        private NetworkManager _networkManager;

        #if DEDICATED_SERVER

        private MultiplayEventCallbacks _callbacks;
        private IServerQueryHandler _queryHandler;
        private bool _isAllocated;

        #endif

        public Server(NetworkManager networkManager)
        {
            _networkManager = networkManager;

            #if !UNITY_EDITOR

            Camera.main.enabled = false;         
            _networkManager.OnClientConnectedCallback += OnClientConnection;

            #endif
        }

        public async Task InitializeAsync()
        {
            #if DEDICATED_SERVER

            _callbacks = new MultiplayEventCallbacks();
            _callbacks.Allocate += OnAllocation;
            _callbacks.SubscriptionStateChanged += OnSubscriptionStateChanged;
            _callbacks.Error += OnError;
            _callbacks.Deallocate += OnDeallocation;

            _queryHandler = await MultiplayService.Instance.StartServerQueryHandlerAsync(2, Guid.NewGuid().ToString(), "Default", "Default", "Default");

            var config = MultiplayService.Instance.ServerConfig;
            if (config.AllocationId != "")
                OnAllocation(new MultiplayAllocation("", config.ServerId, config.AllocationId));

            #else

            _networkManager.StartServer();

            #endif
        }

        public void Update()
        {
            #if DEDICATED_SERVER

            if (_queryHandler != null)
            {
                if (_networkManager.IsServer)
                    _queryHandler.CurrentPlayers = (ushort)_networkManager.ConnectedClients.Count;
                _queryHandler.UpdateServerCheck();
            }

            #endif
        }

        #if DEDICATED_SERVER

        private void OnAllocation(MultiplayAllocation allocation)
        {
            if (_isAllocated)
            {
                Debug.LogWarning($"[SERVER] Server has already been allocated");
                return;
            }

            var config = MultiplayService.Instance.ServerConfig;
            Debug.Log($"[SERVER] Server has been allocated. 'ServerId={config.ServerId}', 'AllocationId={config.AllocationId}', 'Port={config.Port}', 'QueryPort={config.QueryPort}', 'LogDirectory={config.ServerLogDirectory}'");

            var transport = _networkManager.GetComponent<UnityTransport>();
            transport.SetConnectionData("0.0.0.0", config.Port, "0.0.0.0");

            _isAllocated = true;
            
            _networkManager.StartServer();
            MultiplayService.Instance.ReadyServerForPlayersAsync();
        }

        private void OnClientConnection(ulong clientId)
        {
            Debug.Log($"[SERVER] A client has joined the match");

            if (_networkManager.ConnectedClients.Count >= 2)
                MultiplayService.Instance.UnreadyServerAsync();
        }

        private void OnSubscriptionStateChanged(MultiplayServerSubscriptionState state)
        {
            Debug.Log($"[SERVER] Received new subscription state: '{state}'");
        }

        private void OnError(MultiplayError error)
        {
            Debug.LogError($"[SERVER] An error ocurred with 'Reason={error.Reason}' and 'Detail={error.Detail}'");
        }

        private void OnDeallocation(MultiplayDeallocation deallocation)
        {
            Debug.Log($"[SERVER] Server has been deallocated");
            _isAllocated = false;
        }

        public void Dispose()
        {
            _networkManager.OnClientConnectedCallback -= OnClientConnection;
            _networkManager = null;

            _callbacks.Allocate -= OnAllocation;
            _callbacks.SubscriptionStateChanged -= OnSubscriptionStateChanged;
            _callbacks.Error -= OnError;
            _callbacks.Deallocate -= OnDeallocation;
            _callbacks = null;
        }
    
        #else

        public void Dispose()
        {
            // NO-OP
        }

        #endif
    }
}