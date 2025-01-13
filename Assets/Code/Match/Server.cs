using Echo.Common;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Multiplayer;
using UnityEngine;

#if DEDICATED_SERVER

using Unity.Services.Multiplay;

#endif

namespace Echo.Match
{
    public class Server
    {
        private readonly Match _match;

        #if DEDICATED_SERVER

        private MultiplayEventCallbacks _callbacks;
        private IServerQueryHandler _queryHandler;
        private bool _isAllocated;

        #endif

        public NetworkManager NetworkManager => _match.NetworkManager;

        public Server(Match match)
        {
            _match = match;

            NetworkManager.ConnectionApprovalCallback += ApproveClientConnection;
            NetworkManager.OnClientConnectedCallback += OnClientConnection;
            NetworkManager.OnClientDisconnectCallback += OnClientDisconnection;

            #if !UNITY_EDITOR

            Camera.main.enabled = false;         

            #endif
        }

        #pragma warning disable CS1998

        public async Task InitializeAsync(bool autoStart = true)

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

            #endif
        }

        #pragma warning restore CS1998

        public void Update()
        {
            #if DEDICATED_SERVER

            if (_queryHandler != null)
            {
                if (NetworkManager.IsServer)
                    _queryHandler.CurrentPlayers = (ushort)NetworkManager.ConnectedClients.Count;
                _queryHandler.UpdateServerCheck();
            }

            #endif
        }

        private void ApproveClientConnection(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            response.Approved = true;
            response.CreatePlayerObject = true;

            if (NetworkManager.ConnectedClients.Count >= 2)
            {
                response.Approved = false;
                response.Reason = "Max player count reached";
            }
        }

        private void OnClientConnection(ulong clientId)
        {
            Debug.Log($"[SERVER] 'Client={clientId}' has joined the match");

            if (NetworkManager.ConnectedClients.Count >= 2)
            {
                #if DEDICATED_SERVER
        
                MultiplayService.Instance.UnreadyServerAsync();
                
                #endif

                NetworkManager.Spawn(_match.Prefabs.GameState);
            }
        }

        private void OnClientDisconnection(ulong clientId)
        {
            Debug.Log($"[SERVER] 'Client={clientId}' has left the match");

            if (NetworkManager.ConnectedClients.Count == 0)
            {
                #if DEDICATED_SERVER
        
                MultiplayService.Instance.ReadyServerForPlayersAsync();
                
                #endif

                _match.Shutdown();
            }
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

            var transport = NetworkManager.GetComponent<UnityTransport>();
            transport.SetConnectionData("0.0.0.0", config.Port, "0.0.0.0");

            _isAllocated = true;
            
            NetworkManager.StartServer();
            MultiplayService.Instance.ReadyServerForPlayersAsync();
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
            NetworkManager.OnClientConnectedCallback -= OnClientConnection;
            NetworkManager = null;

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