using UnityEngine;

public class ArriveBehaviour : SteeringBehaviour
{

    // Target radius
    public float targetRadius = 0.1f;
    // Slowdown radius
    public float slowdownRadius = 20f;
    // Time to target
    public float timeToTarget = 1f;

    // Arrive behaviour
    public override SteeringOutput GetSteering(GameObject target)
    {
        // Initialize linear and angular forces to zero
        Vector2 linear = Vector2.zero;
        float angular = 0f; // Not used

        // Do I have a target?
        if (target != null)
        {
            // Desired speed and velocity
            float desiredSpeed;
            Vector2 desiredVelocity;

            // Get the direction to the target
            Vector2 dir = target.transform.position - rb.transform.position;

            // Get distance to target
            float distance = dir.magnitude;

            // Are we at the target radius yet?
            if (distance < targetRadius)
            {
                // Return no steering whatsoever
                return new SteeringOutput(Vector2.zero, 0f);
            }
            // Are we within the slowdown radius?
            else if (distance < slowdownRadius)
            {
                // Adjust desired speed depending on distance to target
                desiredSpeed = agent.maxSpeed * distance / slowdownRadius;
            }
            else
            {
                // If we're outside the slowdown radius, go for max speed
                desiredSpeed = agent.maxSpeed;
            }

            // Desired velocity combines desired speed and direction to target
            desiredVelocity = dir.normalized * desiredSpeed;

            // Linear acceleration tries to get to the target velocity
            linear = (desiredVelocity - rb.velocity) / timeToTarget;

            // Check if acceleration is too fast
            if (linear.magnitude > agent.maxAccel)
            {
                linear = linear.normalized * agent.maxAccel;
            }
        }

        // Output the steering
        return new SteeringOutput(linear, angular);
    }
}
