using UnityEngine;
using System;
using System.Collections.Generic;

namespace AvionUnity.Network
{
    /// <summary>
    /// Handles multiplayer networking functionality including connection management,
    /// state synchronization, and player management.
    /// </summary>
    public class NetworkManager : MonoBehaviour
    {
        [Header("Network Settings")]
        [SerializeField] private string serverAddress = "localhost";
        [SerializeField] private int serverPort = 7777;
        [SerializeField] private int maxConnections = 16;
        [SerializeField] private float syncRate = 20f;

        [Header("Player Settings")]
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Transform[] spawnPoints;

        [Header("Network State")]
        [SerializeField] private bool isServer;
        [SerializeField] private bool isConnected;
        [SerializeField] private string playerId;

        private Dictionary<string, NetworkPlayer> connectedPlayers;
        private float lastSyncTime;

        // Events
        public event Action OnConnected;
        public event Action OnDisconnected;
        public event Action<string> OnPlayerJoined;
        public event Action<string> OnPlayerLeft;
        public event Action<string> OnConnectionError;

        // Properties
        public bool IsServer => isServer;
        public bool IsConnected => isConnected;
        public string PlayerId => playerId;
        public int PlayerCount => connectedPlayers?.Count ?? 0;

        private void Awake()
        {
            connectedPlayers = new Dictionary<string, NetworkPlayer>();
            playerId = GeneratePlayerId();
        }

        private void Update()
        {
            if (isConnected)
            {
                ProcessNetworkUpdates();

                if (Time.time - lastSyncTime >= 1f / syncRate)
                {
                    SyncLocalPlayer();
                    lastSyncTime = Time.time;
                }
            }
        }

        /// <summary>
        /// Starts the network as a server/host.
        /// </summary>
        public void StartServer()
        {
            Debug.Log($"Starting server on port {serverPort}...");
            isServer = true;
            isConnected = true;
            
            // Server initialization would go here
            // For now, just log the action as a placeholder
            
            OnConnected?.Invoke();
            Debug.Log("Server started successfully.");
        }

        /// <summary>
        /// Connects to a remote server as a client.
        /// </summary>
        public void ConnectToServer(string address = null, int port = 0)
        {
            string targetAddress = address ?? serverAddress;
            int targetPort = port > 0 ? port : serverPort;

            Debug.Log($"Connecting to {targetAddress}:{targetPort}...");

            // Client connection would go here
            // For now, simulate connection
            
            isServer = false;
            isConnected = true;
            
            OnConnected?.Invoke();
            Debug.Log("Connected to server successfully.");
        }

        /// <summary>
        /// Disconnects from the current session.
        /// </summary>
        public void Disconnect()
        {
            if (!isConnected) return;

            Debug.Log("Disconnecting...");

            // Cleanup connected players
            foreach (var player in connectedPlayers.Values)
            {
                if (player.PlayerObject != null)
                {
                    Destroy(player.PlayerObject);
                }
            }
            connectedPlayers.Clear();

            isConnected = false;
            isServer = false;

            OnDisconnected?.Invoke();
            Debug.Log("Disconnected.");
        }

        /// <summary>
        /// Processes incoming network updates.
        /// </summary>
        private void ProcessNetworkUpdates()
        {
            // Network message processing would go here
            // This is a placeholder for actual networking implementation
        }

        /// <summary>
        /// Synchronizes the local player's state to the network.
        /// </summary>
        private void SyncLocalPlayer()
        {
            if (!isConnected) return;

            // Find local player and sync state
            // This is a placeholder for actual networking implementation
        }

        /// <summary>
        /// Handles a new player joining the session.
        /// </summary>
        private void HandlePlayerJoined(string joinedPlayerId, Vector3 spawnPosition)
        {
            if (connectedPlayers.ContainsKey(joinedPlayerId))
            {
                Debug.LogWarning($"Player {joinedPlayerId} already exists.");
                return;
            }

            NetworkPlayer newPlayer = new NetworkPlayer
            {
                PlayerId = joinedPlayerId,
                Position = spawnPosition,
                Rotation = Quaternion.identity
            };

            // Spawn player object if prefab is set
            if (playerPrefab != null)
            {
                newPlayer.PlayerObject = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
                newPlayer.PlayerObject.name = $"Player_{joinedPlayerId}";
            }

            connectedPlayers[joinedPlayerId] = newPlayer;
            OnPlayerJoined?.Invoke(joinedPlayerId);

            Debug.Log($"Player {joinedPlayerId} joined. Total players: {connectedPlayers.Count}");
        }

        /// <summary>
        /// Handles a player leaving the session.
        /// </summary>
        private void HandlePlayerLeft(string leftPlayerId)
        {
            if (!connectedPlayers.TryGetValue(leftPlayerId, out NetworkPlayer player))
            {
                return;
            }

            if (player.PlayerObject != null)
            {
                Destroy(player.PlayerObject);
            }

            connectedPlayers.Remove(leftPlayerId);
            OnPlayerLeft?.Invoke(leftPlayerId);

            Debug.Log($"Player {leftPlayerId} left. Total players: {connectedPlayers.Count}");
        }

        /// <summary>
        /// Updates a remote player's state.
        /// </summary>
        private void UpdateRemotePlayer(string remotePlayerId, Vector3 position, Quaternion rotation, Vector3 velocity)
        {
            if (!connectedPlayers.TryGetValue(remotePlayerId, out NetworkPlayer player))
            {
                return;
            }

            player.Position = position;
            player.Rotation = rotation;
            player.Velocity = velocity;

            if (player.PlayerObject != null)
            {
                // Interpolate for smooth movement
                player.PlayerObject.transform.position = Vector3.Lerp(
                    player.PlayerObject.transform.position,
                    position,
                    Time.deltaTime * 10f
                );
                player.PlayerObject.transform.rotation = Quaternion.Slerp(
                    player.PlayerObject.transform.rotation,
                    rotation,
                    Time.deltaTime * 10f
                );
            }
        }

        /// <summary>
        /// Gets a spawn point for a new player.
        /// </summary>
        public Vector3 GetSpawnPoint()
        {
            if (spawnPoints == null || spawnPoints.Length == 0)
            {
                return Vector3.zero;
            }

            int index = PlayerCount % spawnPoints.Length;
            return spawnPoints[index].position;
        }

        /// <summary>
        /// Generates a unique player ID.
        /// </summary>
        private string GeneratePlayerId()
        {
            return Guid.NewGuid().ToString().Substring(0, 8);
        }

        /// <summary>
        /// Sends a chat message to all players.
        /// </summary>
        public void SendChatMessage(string message)
        {
            if (!isConnected) return;

            Debug.Log($"[Chat] {playerId}: {message}");
            // Actual message sending would go here
        }

        /// <summary>
        /// Gets information about all connected players.
        /// </summary>
        public List<NetworkPlayer> GetConnectedPlayers()
        {
            return new List<NetworkPlayer>(connectedPlayers.Values);
        }

        /// <summary>
        /// Sets the server address and port.
        /// </summary>
        public void SetServerInfo(string address, int port)
        {
            serverAddress = address;
            serverPort = port;
        }

        private void OnDestroy()
        {
            Disconnect();
        }
    }

    /// <summary>
    /// Represents a networked player's state.
    /// </summary>
    [Serializable]
    public class NetworkPlayer
    {
        public string PlayerId;
        public string DisplayName;
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Velocity;
        public GameObject PlayerObject;
        public float LastUpdateTime;
    }
}
