/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */
using UnityEngine;

public class DisruptiveBehaviour : SteeringBehaviour
{
    [SerializeField]
    [Tooltip("Interval between switching disruption forces")]
    private float switchInterval = 0.25f;

    [Header("Angular acceleration disruption")]

    [SerializeField]
    [Tooltip("L value of logistic function: max magnitude")]
    private float angAcellL = 90f;

    [SerializeField]
    [Tooltip("X0 value of logistic function: half-speed for disruption")]
    private float angAcellX0 = 140;

    [SerializeField]
    [Range(-1, 1)]
    [Tooltip("k value of logistic function: disruption steepness")]
    private float angAcellK = 0.04f;

    [Header("Linear acceleration disruption")]

    [SerializeField]
    [Tooltip("L value of logistic function: max magnitude")]
    private float linAcellL = 90f;

    [SerializeField]
    [Tooltip("X0 value of logistic function: half-speed for disruption")]
    private float linAcellX0 = 140;

    [SerializeField]
    [Range(-1, 1)]
    [Tooltip("k value of logistic function: disruption steepness")]
    private float linAcellK = 0.04f;

    // Last disruption
    private SteeringOutput lastDisruption;

    // Time last disruption was calculated
    private float lastDisruptionTime;

    private void Start()
    {
        lastDisruption = new SteeringOutput(Vector2.zero, 0);
        lastDisruptionTime = 0f;
    }

    // Disruptive behaviour
    public override SteeringOutput GetSteering(GameObject target)
    {
        // Is it time to create a new disruption?
        if (Time.time > lastDisruptionTime + switchInterval)
        {
            Vector2 linear = Random.onUnitSphere *
                Logistic(Velocity.magnitude, linAcellL, linAcellX0, linAcellK);
            float angular =
                Logistic(AngularVelocity, angAcellL, angAcellX0, angAcellK);
            lastDisruption = new SteeringOutput(linear, angular);
            lastDisruptionTime = Time.time;
        }

        // Output the disruption
        return lastDisruption;
    }

    /// <summary>
    /// Logistic function.
    /// </summary>
    /// <param name="x">Input variable x</param>
    /// <param name="L">The curve's maximum value</param>
    /// <param name="x0">The x-value of the sigmoid's midpoint</param>
    /// <param name="k">The steepness of the curve</param>
    /// <returns>The y output variable</returns>
    public static float Logistic(float x, float L, float x0, float k)
    {
        return L / (1 + Mathf.Exp(-k * (x - x0)));
    }

    // Draw a line showing the linear disruption caused by this behaviour.
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position,
            transform.position + (Vector3)lastDisruption.Linear);
    }
}
