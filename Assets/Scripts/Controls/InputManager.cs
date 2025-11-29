using UnityEngine;
using System;

namespace AvionUnity.Controls
{
    /// <summary>
    /// Handles all input processing for flight controls, supporting keyboard, mouse, and gamepad.
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        [Header("Input Settings")]
        [SerializeField] private float inputSensitivity = 1f;
        [SerializeField] private float inputDeadzone = 0.1f;
        [SerializeField] private bool invertPitch = false;
        [SerializeField] private bool invertYaw = false;

        [Header("Mouse Look")]
        [SerializeField] private bool enableMouseLook = true;
        [SerializeField] private float mouseSensitivity = 2f;

        [Header("Throttle Settings")]
        [SerializeField] private float throttleStep = 0.1f;
        [SerializeField] private bool useAnalogThrottle = false;

        // Input values
        private float pitchInput;
        private float rollInput;
        private float yawInput;
        private float throttleInput;

        // Public accessors
        public float PitchInput => pitchInput;
        public float RollInput => rollInput;
        public float YawInput => yawInput;
        public float ThrottleInput => throttleInput;

        // Events
        public event Action OnBrakePressed;
        public event Action OnLandingGearToggle;
        public event Action OnFlapsToggle;
        public event Action OnPausePressed;

        private void Update()
        {
            ProcessFlightControls();
            ProcessThrottle();
            ProcessSpecialInputs();
        }

        /// <summary>
        /// Processes flight control inputs (pitch, roll, yaw).
        /// </summary>
        private void ProcessFlightControls()
        {
            // Keyboard/Gamepad input
            float rawPitch = Input.GetAxis("Vertical");
            float rawRoll = Input.GetAxis("Horizontal");
            float rawYaw = 0f;

            // Yaw input (Q/E keys or gamepad triggers)
            if (Input.GetKey(KeyCode.Q)) rawYaw -= 1f;
            if (Input.GetKey(KeyCode.E)) rawYaw += 1f;

            // Apply gamepad yaw if available
            rawYaw += Input.GetAxis("Yaw");

            // Mouse look for additional pitch/roll control
            if (enableMouseLook && Input.GetMouseButton(1))
            {
                rawPitch += Input.GetAxis("Mouse Y") * mouseSensitivity * 0.1f;
                rawRoll += Input.GetAxis("Mouse X") * mouseSensitivity * 0.1f;
            }

            // Apply deadzone
            rawPitch = ApplyDeadzone(rawPitch);
            rawRoll = ApplyDeadzone(rawRoll);
            rawYaw = ApplyDeadzone(rawYaw);

            // Apply sensitivity and inversion
            pitchInput = Mathf.Clamp(rawPitch * inputSensitivity * (invertPitch ? -1f : 1f), -1f, 1f);
            rollInput = Mathf.Clamp(rawRoll * inputSensitivity, -1f, 1f);
            yawInput = Mathf.Clamp(rawYaw * inputSensitivity * (invertYaw ? -1f : 1f), -1f, 1f);
        }

        /// <summary>
        /// Processes throttle input.
        /// </summary>
        private void ProcessThrottle()
        {
            if (useAnalogThrottle)
            {
                // Use analog axis for throttle (e.g., gamepad trigger)
                float analogThrottle = (Input.GetAxis("Throttle") + 1f) / 2f;
                throttleInput = analogThrottle;
            }
            else
            {
                // Digital throttle control
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.W))
                {
                    throttleInput = Mathf.Min(throttleInput + throttleStep * Time.deltaTime, 1f);
                }
                if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.S))
                {
                    throttleInput = Mathf.Max(throttleInput - throttleStep * Time.deltaTime, 0f);
                }
            }

            // Full throttle/idle shortcuts
            if (Input.GetKeyDown(KeyCode.Equals) || Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                throttleInput = 1f;
            }
            if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                throttleInput = 0f;
            }
        }

        /// <summary>
        /// Processes special inputs like brakes, landing gear, and flaps.
        /// </summary>
        private void ProcessSpecialInputs()
        {
            // Brakes
            if (Input.GetKeyDown(KeyCode.B))
            {
                OnBrakePressed?.Invoke();
            }

            // Landing gear
            if (Input.GetKeyDown(KeyCode.G))
            {
                OnLandingGearToggle?.Invoke();
            }

            // Flaps
            if (Input.GetKeyDown(KeyCode.F))
            {
                OnFlapsToggle?.Invoke();
            }

            // Pause
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
            {
                OnPausePressed?.Invoke();
            }
        }

        /// <summary>
        /// Applies deadzone to an input value.
        /// </summary>
        private float ApplyDeadzone(float value)
        {
            if (Mathf.Abs(value) < inputDeadzone)
            {
                return 0f;
            }

            // Rescale input to maintain full range after deadzone
            float sign = Mathf.Sign(value);
            float rescaled = (Mathf.Abs(value) - inputDeadzone) / (1f - inputDeadzone);
            return sign * rescaled;
        }

        /// <summary>
        /// Sets input sensitivity.
        /// </summary>
        public void SetSensitivity(float sensitivity)
        {
            inputSensitivity = Mathf.Clamp(sensitivity, 0.1f, 3f);
        }

        /// <summary>
        /// Sets pitch inversion state.
        /// </summary>
        public void SetInvertPitch(bool invert)
        {
            invertPitch = invert;
        }

        /// <summary>
        /// Sets yaw inversion state.
        /// </summary>
        public void SetInvertYaw(bool invert)
        {
            invertYaw = invert;
        }

        /// <summary>
        /// Enables or disables mouse look.
        /// </summary>
        public void SetMouseLookEnabled(bool enabled)
        {
            enableMouseLook = enabled;
        }
    }
}
