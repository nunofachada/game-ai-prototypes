using UnityEngine;

public class LookWhereYoureGoingBehaviour : AlignBehaviour
{

    // Look Where You're Going behaviour
    public override SteeringOutput GetSteering(GameObject target)
    {
        // Initialize linear and angular forces to zero
        SteeringOutput sout = new SteeringOutput(Vector2.zero, 0);

        // Am I moving?
        if (rb.velocity.magnitude > 0)
        {
            // Determine the orientation of our temporary target, which should
            // be in the direction we're going
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x)
                * Mathf.Rad2Deg;

            // Create our temporary target
            GameObject tempTarget = CreateTarget(Vector3.zero, angle);

            // Use align superclass to determine the torque to return based
            // on the temporary target orientation (which is the direction
            // we're going)
            sout = base.GetSteering(tempTarget);

            // Destroy the temporary copy of our target
            Destroy(tempTarget);
        }

        // Output the steering
        return sout;
    }

}
