using UnityEngine;

public class ObstacleAvoidanceBehaviour : SeekBehaviour
{
    // Minimum distance to a wall, should be greater than the radius of the
    // agent
    public float avoidDist = 2f;

    // Lookahead distance for raycast
    public float lookaheadDist = 4;

    // Where the casted ray ends, for gizmo drawing purposes
    private Vector2 endRay;

    // Obstacle layer mask, for ray casting purposes
    private int obstLayerMask;

    // Initial setup
    protected override void Start()
    {
        base.Start();
        obstLayerMask = LayerMask.GetMask("Obstacle");
    }

    // Obstacle avoidance behaviour
    public override SteeringOutput GetSteering(GameObject target)
    {
        // Initialize linear and angular forces to zero
        SteeringOutput sout = new SteeringOutput(Vector2.zero, 0);

        // Determine the collision ray direction
        Vector2 rayDir = rb.velocity.normalized;

        // Find the collision
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position, rayDir, lookaheadDist, obstLayerMask);

        // Keep the end of the casted ray, for gizmo drawing purposes
        endRay = (Vector2)transform.position + rayDir * lookaheadDist;

        // Do we have a collision?
        if (hit)
        {
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

    // Draw gizmos, basically draw the casted ray
    public void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, endRay);
    }

}
