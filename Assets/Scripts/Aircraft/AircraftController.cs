using UnityEngine;
using AvionUnity.Physics;
using AvionUnity.Controls;

namespace AvionUnity.Aircraft
{
    /// <summary>
    /// Main controller for aircraft behavior, handling input translation to physical responses.
    /// </summary>
    public class AircraftController : MonoBehaviour
    {
        [Header("Control Surfaces")]
        [SerializeField] private float pitchSensitivity = 100f;
        [SerializeField] private float rollSensitivity = 100f;
        [SerializeField] private float yawSensitivity = 50f;

        [Header("Engine")]
        [SerializeField] private float maxThrust = 50000f;
        [SerializeField] private float throttleResponse = 0.5f;

        [Header("Control Limits")]
        [SerializeField] private float maxPitchAngle = 45f;
        [SerializeField] private float maxRollAngle = 60f;

        private FlightPhysics flightPhysics;
        private InputManager inputManager;

        private float currentThrottle;
        private float pitchInput;
        private float rollInput;
        private float yawInput;

        public float CurrentThrottle => currentThrottle;
        public float Airspeed => flightPhysics != null ? flightPhysics.Airspeed : 0f;

        private void Awake()
        {
            flightPhysics = GetComponent<FlightPhysics>();
            if (flightPhysics == null)
            {
                flightPhysics = gameObject.AddComponent<FlightPhysics>();
            }

            inputManager = FindFirstObjectByType<InputManager>();
        }

        private void Update()
        {
            ReadInput();
        }

        private void FixedUpdate()
        {
            ApplyControls();
        }

        /// <summary>
        /// Reads input from the InputManager or direct input system.
        /// </summary>
        private void ReadInput()
        {
            if (inputManager != null)
            {
                pitchInput = inputManager.PitchInput;
                rollInput = inputManager.RollInput;
                yawInput = inputManager.YawInput;
                currentThrottle = Mathf.Lerp(currentThrottle, inputManager.ThrottleInput, throttleResponse * Time.deltaTime);
            }
            else
            {
                // Fallback to direct input
                pitchInput = Input.GetAxis("Vertical");
                rollInput = Input.GetAxis("Horizontal");
                yawInput = Input.GetAxis("Yaw");
                
                if (Input.GetKey(KeyCode.LeftShift))
                    currentThrottle = Mathf.Min(currentThrottle + throttleResponse * Time.deltaTime, 1f);
                if (Input.GetKey(KeyCode.LeftControl))
                    currentThrottle = Mathf.Max(currentThrottle - throttleResponse * Time.deltaTime, 0f);
            }
        }

        /// <summary>
        /// Applies control inputs to the flight physics system.
        /// </summary>
        private void ApplyControls()
        {
            // Apply thrust
            float thrust = currentThrottle * maxThrust;
            flightPhysics.ApplyThrust(thrust);

            // Apply control surface torques
            Vector3 torque = new Vector3(
                pitchInput * pitchSensitivity,
                yawInput * yawSensitivity,
                -rollInput * rollSensitivity
            );

            // Scale torque based on airspeed for more realistic control authority
            float speedFactor = Mathf.Clamp01(flightPhysics.Airspeed / 50f);
            torque *= speedFactor;

            flightPhysics.ApplyTorque(torque);
        }

        /// <summary>
        /// Sets the throttle directly (0-1 range).
        /// </summary>
        public void SetThrottle(float throttle)
        {
            currentThrottle = Mathf.Clamp01(throttle);
        }

        /// <summary>
        /// Toggles landing gear (placeholder for future implementation).
        /// </summary>
        public void ToggleLandingGear()
        {
            Debug.Log("Landing gear toggled");
        }

        /// <summary>
        /// Toggles flaps (placeholder for future implementation).
        /// </summary>
        public void ToggleFlaps()
        {
            Debug.Log("Flaps toggled");
        }
    }
}
