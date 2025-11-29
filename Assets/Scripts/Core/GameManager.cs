using UnityEngine;
using AvionUnity.Aircraft;
using AvionUnity.Environment;
using AvionUnity.Controls;
using AvionUnity.UI;
using AvionUnity.Network;

namespace AvionUnity.Core
{
    /// <summary>
    /// Main game manager that initializes and coordinates all game systems.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Header("System References")]
        [SerializeField] private InputManager inputManager;
        [SerializeField] private EnvironmentManager environmentManager;
        [SerializeField] private NetworkManager networkManager;
        [SerializeField] private HUD hud;

        [Header("Player Aircraft")]
        [SerializeField] private AircraftController playerAircraft;

        [Header("Game Settings")]
        [SerializeField] private bool autoInitialize = true;
        [SerializeField] private bool startPaused = false;

        private static GameManager instance;
        private bool isPaused;
        private bool isInitialized;

        public static GameManager Instance => instance;
        public InputManager InputManager => inputManager;
        public EnvironmentManager EnvironmentManager => environmentManager;
        public NetworkManager NetworkManager => networkManager;
        public AircraftController PlayerAircraft => playerAircraft;
        public bool IsPaused => isPaused;

        private void Awake()
        {
            // Singleton pattern
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);

            if (autoInitialize)
            {
                Initialize();
            }
        }

        private void Start()
        {
            if (startPaused)
            {
                Pause();
            }
        }

        /// <summary>
        /// Initializes all game systems.
        /// </summary>
        public void Initialize()
        {
            if (isInitialized) return;

            Debug.Log("Avion-Unity: Initializing game systems...");

            // Find references if not assigned
            FindSystemReferences();

            // Subscribe to input events
            if (inputManager != null)
            {
                inputManager.OnPausePressed += TogglePause;
            }

            isInitialized = true;
            Debug.Log("Avion-Unity: Game systems initialized successfully.");
        }

        /// <summary>
        /// Finds and assigns system references if not already set.
        /// </summary>
        private void FindSystemReferences()
        {
            if (inputManager == null)
                inputManager = FindFirstObjectByType<InputManager>();

            if (environmentManager == null)
                environmentManager = FindFirstObjectByType<EnvironmentManager>();

            if (networkManager == null)
                networkManager = FindFirstObjectByType<NetworkManager>();

            if (hud == null)
                hud = FindFirstObjectByType<HUD>();

            if (playerAircraft == null)
                playerAircraft = FindFirstObjectByType<AircraftController>();
        }

        /// <summary>
        /// Pauses the game.
        /// </summary>
        public void Pause()
        {
            if (isPaused) return;

            Time.timeScale = 0f;
            isPaused = true;
            Debug.Log("Game paused.");
        }

        /// <summary>
        /// Resumes the game.
        /// </summary>
        public void Resume()
        {
            if (!isPaused) return;

            Time.timeScale = 1f;
            isPaused = false;
            Debug.Log("Game resumed.");
        }

        /// <summary>
        /// Toggles the pause state.
        /// </summary>
        public void TogglePause()
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }

        /// <summary>
        /// Quits the game application.
        /// </summary>
        public void QuitGame()
        {
            Debug.Log("Quitting game...");

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        /// <summary>
        /// Spawns the player aircraft at the specified position.
        /// </summary>
        public void SpawnPlayerAircraft(Vector3 position, Quaternion rotation)
        {
            if (playerAircraft == null)
            {
                Debug.LogError("Player aircraft prefab not assigned.");
                return;
            }

            playerAircraft.transform.position = position;
            playerAircraft.transform.rotation = rotation;
            Debug.Log($"Player aircraft spawned at {position}");
        }

        /// <summary>
        /// Resets the player aircraft to the starting position.
        /// </summary>
        public void ResetPlayerAircraft()
        {
            if (playerAircraft == null) return;

            var rb = playerAircraft.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            playerAircraft.transform.position = Vector3.up * 100f;
            playerAircraft.transform.rotation = Quaternion.identity;
            playerAircraft.SetThrottle(0f);

            Debug.Log("Player aircraft reset.");
        }

        private void OnDestroy()
        {
            if (inputManager != null)
            {
                inputManager.OnPausePressed -= TogglePause;
            }
        }
    }
}
