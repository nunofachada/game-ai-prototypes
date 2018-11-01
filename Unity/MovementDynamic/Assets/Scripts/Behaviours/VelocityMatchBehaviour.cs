using UnityEngine;

public class VelocityMatchBehaviour : SteeringBehaviour
{

    public float timeToTarget = 0.1f;

    // Velocity match behaviour
    public override SteeringOutput GetSteering(GameObject target)
    {
        // Initialize linear and angular forces to zero
        Vector2 linear = Vector2.zero;
        float angular = 0f; // Not used

        // Do I have a target?
        if (target != null)
        {
            // Get the target's rigid body
            Rigidbody2D targetRb = target.GetComponent<Rigidbody2D>();

            // Get the target's velocity (if its rigid body is not null)
            Vector2 targetVelocity = 
                targetRb != null ? targetRb.velocity : Vector2.zero;

            // Acceleration tries to get to the target's velocity
            linear = (targetVelocity - rb.velocity) / timeToTarget;

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
