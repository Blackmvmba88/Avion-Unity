using UnityEngine;

namespace AvionUnity.Environment
{
    /// <summary>
    /// Manages environmental conditions including weather, time of day, and atmospheric properties.
    /// </summary>
    public class EnvironmentManager : MonoBehaviour
    {
        [Header("Time of Day")]
        [SerializeField] private Light sunLight;
        [SerializeField] private float timeOfDay = 12f;
        [SerializeField] private float dayDuration = 1200f;
        [SerializeField] private bool enableDayNightCycle = true;

        [Header("Weather")]
        [SerializeField] private WeatherType currentWeather = WeatherType.Clear;
        [SerializeField] private float cloudCoverage = 0.3f;
        [SerializeField] private float visibility = 10000f;
        [SerializeField] private float precipitation = 0f;

        [Header("Atmosphere")]
        [SerializeField] private float seaLevelTemperature = 288.15f;
        [SerializeField] private float seaLevelPressure = 101325f;
        [SerializeField] private float temperatureLapseRate = 0.0065f;

        [Header("Wind")]
        [SerializeField] private Vector3 windDirection = Vector3.forward;
        [SerializeField] private float windSpeed = 5f;
        [SerializeField] private float turbulenceIntensity = 0.1f;

        public float TimeOfDay => timeOfDay;
        public WeatherType CurrentWeather => currentWeather;
        public Vector3 WindDirection => windDirection;
        public float WindSpeed => windSpeed;
        public float Visibility => visibility;

        private void Start()
        {
            InitializeEnvironment();
        }

        private void Update()
        {
            if (enableDayNightCycle)
            {
                UpdateTimeOfDay();
            }

            UpdateSunPosition();
            UpdateAtmosphere();
        }

        /// <summary>
        /// Initializes the environment with default settings.
        /// </summary>
        private void InitializeEnvironment()
        {
            if (sunLight == null)
            {
                sunLight = FindFirstObjectByType<Light>();
            }

            SetWeather(currentWeather);
        }

        /// <summary>
        /// Updates the time of day based on real-time progression.
        /// </summary>
        private void UpdateTimeOfDay()
        {
            timeOfDay += (24f / dayDuration) * Time.deltaTime;
            if (timeOfDay >= 24f)
            {
                timeOfDay -= 24f;
            }
        }

        /// <summary>
        /// Updates the sun position based on time of day.
        /// </summary>
        private void UpdateSunPosition()
        {
            if (sunLight == null) return;

            float sunAngle = (timeOfDay / 24f) * 360f - 90f;
            sunLight.transform.rotation = Quaternion.Euler(sunAngle, 170f, 0f);

            // Adjust sun intensity based on angle
            float normalizedTime = Mathf.Abs(timeOfDay - 12f) / 12f;
            sunLight.intensity = Mathf.Lerp(1f, 0.1f, normalizedTime);
        }

        /// <summary>
        /// Updates atmospheric conditions.
        /// </summary>
        private void UpdateAtmosphere()
        {
            // Atmospheric calculations could be expanded here
        }

        /// <summary>
        /// Calculates air density at a given altitude using the International Standard Atmosphere model.
        /// </summary>
        public float GetAirDensityAtAltitude(float altitude)
        {
            float temperature = seaLevelTemperature - temperatureLapseRate * altitude;
            float pressure = seaLevelPressure * Mathf.Pow(temperature / seaLevelTemperature, 5.2561f);
            float density = pressure / (287.05f * temperature);
            return density;
        }

        /// <summary>
        /// Gets the temperature at a given altitude in Kelvin.
        /// </summary>
        public float GetTemperatureAtAltitude(float altitude)
        {
            return seaLevelTemperature - temperatureLapseRate * altitude;
        }

        /// <summary>
        /// Gets the wind vector at a given position, including turbulence.
        /// </summary>
        public Vector3 GetWindAtPosition(Vector3 position)
        {
            Vector3 baseWind = windDirection.normalized * windSpeed;

            // Add turbulence
            if (turbulenceIntensity > 0)
            {
                float turbulenceX = (Mathf.PerlinNoise(position.x * 0.01f + Time.time, position.z * 0.01f) - 0.5f) * 2f;
                float turbulenceY = (Mathf.PerlinNoise(position.y * 0.01f + Time.time, position.x * 0.01f) - 0.5f) * 2f;
                float turbulenceZ = (Mathf.PerlinNoise(position.z * 0.01f + Time.time, position.y * 0.01f) - 0.5f) * 2f;

                Vector3 turbulence = new Vector3(turbulenceX, turbulenceY, turbulenceZ) * turbulenceIntensity * windSpeed;
                baseWind += turbulence;
            }

            return baseWind;
        }

        /// <summary>
        /// Sets the current weather type.
        /// </summary>
        public void SetWeather(WeatherType weather)
        {
            currentWeather = weather;

            switch (weather)
            {
                case WeatherType.Clear:
                    cloudCoverage = 0.1f;
                    visibility = 10000f;
                    precipitation = 0f;
                    break;
                case WeatherType.PartlyCloudy:
                    cloudCoverage = 0.4f;
                    visibility = 8000f;
                    precipitation = 0f;
                    break;
                case WeatherType.Cloudy:
                    cloudCoverage = 0.8f;
                    visibility = 5000f;
                    precipitation = 0f;
                    break;
                case WeatherType.Rain:
                    cloudCoverage = 0.9f;
                    visibility = 3000f;
                    precipitation = 0.5f;
                    break;
                case WeatherType.Storm:
                    cloudCoverage = 1f;
                    visibility = 1000f;
                    precipitation = 1f;
                    turbulenceIntensity = 0.5f;
                    break;
                case WeatherType.Fog:
                    cloudCoverage = 0.3f;
                    visibility = 500f;
                    precipitation = 0f;
                    break;
            }

            Debug.Log($"Weather changed to: {weather}");
        }

        /// <summary>
        /// Sets the wind parameters.
        /// </summary>
        public void SetWind(Vector3 direction, float speed)
        {
            windDirection = direction.normalized;
            windSpeed = speed;
        }
    }

    /// <summary>
    /// Enumeration of available weather types.
    /// </summary>
    public enum WeatherType
    {
        Clear,
        PartlyCloudy,
        Cloudy,
        Rain,
        Storm,
        Fog
    }
}
