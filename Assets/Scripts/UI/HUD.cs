using UnityEngine;
using UnityEngine.UI;
using AvionUnity.Aircraft;
using AvionUnity.Environment;

namespace AvionUnity.UI
{
    /// <summary>
    /// Heads-Up Display manager for flight information visualization.
    /// </summary>
    public class HUD : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private AircraftController aircraft;
        [SerializeField] private EnvironmentManager environment;

        [Header("Speed Indicator")]
        [SerializeField] private Text speedText;
        [SerializeField] private Text machText;

        [Header("Altitude Indicator")]
        [SerializeField] private Text altitudeText;
        [SerializeField] private Text verticalSpeedText;

        [Header("Attitude Indicator")]
        [SerializeField] private RectTransform attitudeIndicator;
        [SerializeField] private RectTransform horizonLine;

        [Header("Heading Indicator")]
        [SerializeField] private Text headingText;
        [SerializeField] private RectTransform compassRose;

        [Header("Engine Indicators")]
        [SerializeField] private Text throttleText;
        [SerializeField] private Slider throttleSlider;

        [Header("Additional Info")]
        [SerializeField] private Text gForceText;
        [SerializeField] private Text weatherText;
        [SerializeField] private Text timeText;

        [Header("Warning Indicators")]
        [SerializeField] private GameObject stallWarning;
        [SerializeField] private GameObject overspeedWarning;
        [SerializeField] private GameObject altitudeWarning;

        [Header("Settings")]
        [SerializeField] private bool useMetricUnits = true;
        [SerializeField] private float stallSpeed = 50f;
        [SerializeField] private float maxSpeed = 350f;
        [SerializeField] private float minSafeAltitude = 150f;

        private Transform aircraftTransform;
        private Vector3 previousVelocity;
        private float gForce;

        private void Start()
        {
            InitializeHUD();
        }

        private void Update()
        {
            if (aircraft == null) return;

            UpdateSpeedIndicator();
            UpdateAltitudeIndicator();
            UpdateAttitudeIndicator();
            UpdateHeadingIndicator();
            UpdateEngineIndicators();
            UpdateWarnings();
            UpdateAdditionalInfo();
        }

        /// <summary>
        /// Initializes the HUD by finding required references.
        /// </summary>
        private void InitializeHUD()
        {
            if (aircraft == null)
            {
                aircraft = FindFirstObjectByType<AircraftController>();
            }

            if (environment == null)
            {
                environment = FindFirstObjectByType<EnvironmentManager>();
            }

            if (aircraft != null)
            {
                aircraftTransform = aircraft.transform;
            }

            // Initialize warning states
            if (stallWarning != null) stallWarning.SetActive(false);
            if (overspeedWarning != null) overspeedWarning.SetActive(false);
            if (altitudeWarning != null) altitudeWarning.SetActive(false);
        }

        /// <summary>
        /// Updates speed display.
        /// </summary>
        private void UpdateSpeedIndicator()
        {
            float speed = aircraft.Airspeed;

            if (speedText != null)
            {
                if (useMetricUnits)
                {
                    speedText.text = $"{speed * 3.6f:F0} km/h";
                }
                else
                {
                    speedText.text = $"{speed * 1.944f:F0} kts";
                }
            }

            // Calculate Mach number (approximate at sea level)
            float machNumber = speed / 343f;
            if (machText != null)
            {
                machText.text = $"M {machNumber:F2}";
            }
        }

        /// <summary>
        /// Updates altitude display.
        /// </summary>
        private void UpdateAltitudeIndicator()
        {
            if (aircraftTransform == null) return;

            float altitude = aircraftTransform.position.y;
            
            if (altitudeText != null)
            {
                if (useMetricUnits)
                {
                    altitudeText.text = $"{altitude:F0} m";
                }
                else
                {
                    altitudeText.text = $"{altitude * 3.281f:F0} ft";
                }
            }

            // Calculate vertical speed
            if (verticalSpeedText != null)
            {
                Rigidbody rb = aircraft.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    float verticalSpeed = rb.velocity.y;
                    string direction = verticalSpeed >= 0 ? "↑" : "↓";
                    
                    if (useMetricUnits)
                    {
                        verticalSpeedText.text = $"{direction} {Mathf.Abs(verticalSpeed):F1} m/s";
                    }
                    else
                    {
                        verticalSpeedText.text = $"{direction} {Mathf.Abs(verticalSpeed * 196.85f):F0} fpm";
                    }
                }
            }
        }

        /// <summary>
        /// Updates the artificial horizon/attitude indicator.
        /// </summary>
        private void UpdateAttitudeIndicator()
        {
            if (aircraftTransform == null) return;

            Vector3 eulerAngles = aircraftTransform.eulerAngles;

            // Normalize angles to -180 to 180 range
            float pitch = eulerAngles.x > 180f ? eulerAngles.x - 360f : eulerAngles.x;
            float roll = eulerAngles.z > 180f ? eulerAngles.z - 360f : eulerAngles.z;

            if (attitudeIndicator != null)
            {
                attitudeIndicator.localRotation = Quaternion.Euler(0, 0, roll);
            }

            if (horizonLine != null)
            {
                horizonLine.localPosition = new Vector3(0, -pitch * 2f, 0);
            }
        }

        /// <summary>
        /// Updates heading/compass display.
        /// </summary>
        private void UpdateHeadingIndicator()
        {
            if (aircraftTransform == null) return;

            float heading = aircraftTransform.eulerAngles.y;

            if (headingText != null)
            {
                headingText.text = $"{heading:F0}°";
            }

            if (compassRose != null)
            {
                compassRose.localRotation = Quaternion.Euler(0, 0, heading);
            }
        }

        /// <summary>
        /// Updates engine/throttle indicators.
        /// </summary>
        private void UpdateEngineIndicators()
        {
            float throttle = aircraft.CurrentThrottle;

            if (throttleText != null)
            {
                throttleText.text = $"THR: {throttle * 100f:F0}%";
            }

            if (throttleSlider != null)
            {
                throttleSlider.value = throttle;
            }
        }

        /// <summary>
        /// Updates warning indicators based on flight conditions.
        /// </summary>
        private void UpdateWarnings()
        {
            float speed = aircraft.Airspeed;
            float altitude = aircraftTransform != null ? aircraftTransform.position.y : 0f;

            // Stall warning
            if (stallWarning != null)
            {
                stallWarning.SetActive(speed < stallSpeed && altitude > 10f);
            }

            // Overspeed warning
            if (overspeedWarning != null)
            {
                overspeedWarning.SetActive(speed > maxSpeed);
            }

            // Low altitude warning
            if (altitudeWarning != null)
            {
                altitudeWarning.SetActive(altitude < minSafeAltitude && altitude > 0f);
            }
        }

        /// <summary>
        /// Updates additional information displays.
        /// </summary>
        private void UpdateAdditionalInfo()
        {
            // G-Force calculation
            if (gForceText != null && aircraft != null)
            {
                Rigidbody rb = aircraft.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 acceleration = (rb.velocity - previousVelocity) / Time.deltaTime;
                    gForce = Mathf.Lerp(gForce, (acceleration.magnitude / 9.81f) + 1f, Time.deltaTime * 5f);
                    gForceText.text = $"G: {gForce:F1}";
                    previousVelocity = rb.velocity;
                }
            }

            // Weather info
            if (weatherText != null && environment != null)
            {
                weatherText.text = $"WX: {environment.CurrentWeather}";
            }

            // Time of day
            if (timeText != null && environment != null)
            {
                int hours = Mathf.FloorToInt(environment.TimeOfDay);
                int minutes = Mathf.FloorToInt((environment.TimeOfDay - hours) * 60f);
                timeText.text = $"{hours:D2}:{minutes:D2}";
            }
        }

        /// <summary>
        /// Sets the unit system for display.
        /// </summary>
        public void SetMetricUnits(bool useMetric)
        {
            useMetricUnits = useMetric;
        }

        /// <summary>
        /// Shows or hides the entire HUD.
        /// </summary>
        public void SetHUDVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }
    }
}
