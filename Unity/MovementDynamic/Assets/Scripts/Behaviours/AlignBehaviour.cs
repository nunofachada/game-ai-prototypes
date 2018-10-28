using UnityEngine;

public class AlignBehaviour : SteeringBehaviour
{

    // Target radius
    public float targetRadius = 0.1f;
    // Slowdown radius
    public float slowdownRadius = 20f;
    // Time to target
    public float timeToTarget = 1f;

    // Align behaviour
    public override SteeringOutput GetSteering(GameObject target)
    {
        // Initialize linear and angular forces to zero
        Vector2 linear = Vector2.zero;
        float angular = 0f; // Not used

        // Do I have a target?
        if (target != null)
        {
            // Desired rotation
            float orientation, orientationAbs, desiredRotation, angularAccel;

            // Get the naive rotation to the target
            orientation =
                target.transform.eulerAngles.z - agent.transform.eulerAngles.z;

            // Map the angle to the (-180, 180) interval
            while (orientation > 180) orientation -= 360;
            while (orientation < -180) orientation += 360;

            // Get the absolute angle
            orientationAbs = Mathf.Abs(orientation);

            // Are we at the target radius yet?
            if (orientationAbs < targetRadius)
            {
                // Return no steering whatsoever
                return new SteeringOutput(Vector2.zero, 0f);
             }
            // Are we within the slowdown radius?
            else if (orientation < slowdownRadius)
            {
                // Adjust desired rotation depending current orientation
                desiredRotation =
                    agent.maxRotation * orientationAbs / slowdownRadius;
            }
            else
            {
                // If we're outside the slowdown radius, go for max rotation
                desiredRotation = agent.maxRotation;
            }

            // The final desired rotation combines rotation and orientation
            desiredRotation *= orientation / orientationAbs;

            // Acceleration tries to get to the target rotation
            angular = desiredRotation - agent.Rb.angularVelocity;
            angular /= timeToTarget;

            // Check if acceleration is too great
            angularAccel = Mathf.Abs(angular);
            if (angularAccel > agent.maxAngularAccel)
            {
                angular /= angularAccel;
                angular *= agent.maxAngularAccel;
            }
        }

        // Output the steering
        return new SteeringOutput(linear, angular);
    }
}
