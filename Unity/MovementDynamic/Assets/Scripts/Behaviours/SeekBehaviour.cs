using UnityEngine;

public class SeekBehaviour : SteeringBehaviour
{

    // Seek behaviour
    public override SteeringOutput GetSteering(GameObject target)
    {
        // Initialize linear and angular forces to zero
        Vector2 linear = Vector2.zero;
        float angular = 0f;

        // Do I have a target?
        if (target != null)
        {
            // Get the direction to the target
            linear = target.transform.position - rb.transform.position;

            // Give full acceleration along this direction
            linear = linear.normalized * agent.maxAccel;

            // Seek behaviour does not set torque
            angular = 0f;
        }

        // Output the steering
        return new SteeringOutput(linear, angular);
    }

}
