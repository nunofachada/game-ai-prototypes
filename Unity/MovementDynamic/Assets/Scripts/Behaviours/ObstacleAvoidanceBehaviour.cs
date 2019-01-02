using UnityEngine;

public class ObstacleAvoidanceBehaviour : SeekBehaviour
{
    // Minimum distance to a wall, should be greater than the radius of the
    // agent
    public float avoidDist = 2f;

    // Lookahead distance for raycast
    public float lookaheadDist = 4;

    private Vector2 endRay;

    // Obstacle avoidance behaviour
    public override SteeringOutput GetSteering(GameObject target)
    {
        // Initialize linear and angular forces to zero
        SteeringOutput sout = new SteeringOutput(Vector2.zero, 0);

        // Determine the collision ray direction
        Vector2 rayDir = rb.velocity.normalized;

        // Find the collision
        RaycastHit2D hit = 
            Physics2D.Raycast(
                transform.position, rayDir, lookaheadDist, LayerMask.GetMask("Obstacle"));
        endRay = (Vector2)transform.position + rayDir * lookaheadDist;
        // Do we have a collision?
        if (hit)
        {

            Debug.Log($"Ray hit {hit.collider.gameObject.name} at {hit.fraction}");
            // Instantiate a temporary target for Seek
            target = CreateTarget(
                ((Vector2)transform.position) + hit.normal * avoidDist, 0f);

            // Delegate to seek
            sout = base.GetSteering(target);

            // Destroy temporary target
            Destroy(target);
        }

        // Output the steering
        return sout;
    }

    public void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, endRay);
    }

}
