using UnityEngine;

namespace AvionUnity.Physics
{
    /// <summary>
    /// Handles the core flight physics calculations including lift, drag, thrust, and gravity.
    /// </summary>
    public class FlightPhysics : MonoBehaviour
    {
        [Header("Aircraft Properties")]
        [SerializeField] private float mass = 1000f;
        [SerializeField] private float wingArea = 20f;
        [SerializeField] private float dragCoefficient = 0.02f;
        [SerializeField] private float liftCoefficient = 0.5f;

        [Header("Environment")]
        [SerializeField] private float airDensity = 1.225f;

        private Rigidbody rb;
        private Vector3 velocity;
        private float airspeed;

        public float Airspeed => airspeed;
        public Vector3 Velocity => velocity;
        public float Mass => mass;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
            }
            rb.mass = mass;
            rb.useGravity = true;
        }

        private void FixedUpdate()
        {
            velocity = rb.velocity;
            airspeed = velocity.magnitude;

            ApplyAerodynamicForces();
        }

        /// <summary>
        /// Calculates and applies aerodynamic forces (lift and drag) to the aircraft.
        /// </summary>
        private void ApplyAerodynamicForces()
        {
            if (airspeed < 0.1f) return;

            float dynamicPressure = 0.5f * airDensity * airspeed * airspeed;

            // Calculate lift (perpendicular to velocity)
            float liftMagnitude = liftCoefficient * dynamicPressure * wingArea;
            Vector3 liftDirection = Vector3.Cross(velocity.normalized, transform.right).normalized;
            Vector3 lift = liftDirection * liftMagnitude;

            // Calculate drag (opposite to velocity)
            float dragMagnitude = dragCoefficient * dynamicPressure * wingArea;
            Vector3 drag = -velocity.normalized * dragMagnitude;

            rb.AddForce(lift + drag, ForceMode.Force);
        }

        /// <summary>
        /// Applies thrust force in the forward direction of the aircraft.
        /// </summary>
        public void ApplyThrust(float thrustAmount)
        {
            Vector3 thrustForce = transform.forward * thrustAmount;
            rb.AddForce(thrustForce, ForceMode.Force);
        }

        /// <summary>
        /// Applies a torque for aircraft rotation.
        /// </summary>
        public void ApplyTorque(Vector3 torque)
        {
            rb.AddTorque(torque, ForceMode.Force);
        }

        /// <summary>
        /// Sets the air density for physics calculations.
        /// </summary>
        public void SetAirDensity(float density)
        {
            airDensity = density;
        }
    }
}
