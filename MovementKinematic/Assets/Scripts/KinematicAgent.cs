/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using UnityEngine;
using Random = UnityEngine.Random;

// This class defines movement for kinematic agents
public class KinematicAgent : MonoBehaviour
{

    // Maximum speed for this agent
    [SerializeField] private float maxSpeed = 10;

    // The tag for this agent's target
    [SerializeField] private string targetTag = null;

    // Agent behaviours, selectable in Unity editor
    [SerializeField] private KinematicSteeringBehaviour steeringBehaviour =
        KinematicSteeringBehaviour.Seek;
    [SerializeField] private KinematicWallBumpBehaviour wallBumpBehaviour =
        KinematicWallBumpBehaviour.AppearOnOtherSide;

    // These are only valid for SeekWithArrive behaviour
    [SerializeField] private float satisfactionRadius = 0.5f;
    [SerializeField] private float timeToTarget = 1f;

    // Maximum angular velocity for wander behaviour
    [SerializeField] private float maxAngularVelocity = 10;

    // Actual functions defining agent behaviour
    private Func<GameObject, SteeringOutput> steer;
    private Action<Collider2D> bump;

    // The agent's rigid body
    private Rigidbody2D rb;

    // The game area object, provides functions to manage what happens when the
    // agent hits a wall
    private GameArea gameArea;

    // Use this for initialization
    private void Start()
    {
        // Keep reference to rigid body
        rb = GetComponent<Rigidbody2D>();

        // Get world bounds
        gameArea = new GameArea();

        // Determine the specific steering behaviour to use
        switch (steeringBehaviour)
        {
            case KinematicSteeringBehaviour.Seek:
                steer = GetSeekSteering;
                break;
            case KinematicSteeringBehaviour.Flee:
                steer = GetFleeSteering;
                break;
            case KinematicSteeringBehaviour.SeekWithArrive:
                steer = GetSeekWithArriveSteering;
                break;
            case KinematicSteeringBehaviour.Wander:
                steer = GetWanderSteering;
                break;
        }

        // Determine the specific wall bump behaviour to use
        switch (wallBumpBehaviour)
        {
            case KinematicWallBumpBehaviour.RandomPosition:
                bump = GetRandomPositionBehaviour;
                break;
            case KinematicWallBumpBehaviour.AppearOnOtherSide:
                bump = GetAppearOnOtherSideBehaviour;
                break;
        }
    }

    // This is called every physics update
    private void FixedUpdate()
    {
        // Is there any target for me?
        GameObject target = targetTag != ""
            ? GameObject.FindWithTag(targetTag)
            : null;

        // Obtain steering (velocity and angular velocity) given a target
        SteeringOutput steering = steer(target);

        // Apply steering to the current agent rigid body
        rb.velocity = steering.Linear;
        rb.angularVelocity = steering.Angular;
    }

    // This function is called by the steering behaviours in order to determine
    // a new orientation based on the current orientation and velocity
    private float GetNewOrientation(float orientation, Vector2 velocity)
    {
        // Make sure we have a velocity
        if (velocity.magnitude > 0)
        {
            // Calculate orientation using an arc tangent of the velocity
            // components
            return Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        }
        else
        {
            // Otherwise use the current orientation
            return orientation;
        }
    }

    // If we bump into wall, we call the bump delegate to decide what to do
    private void OnTriggerEnter2D(Collider2D other)
    {
        bump(other);
    }

    // /////////////////////////////////////////////////// //
    // Below are different methods for wall bump behaviour //
    // /////////////////////////////////////////////////// //

    // This behaviour makes the agent appear on the opposite side when it bumps
    // into a wall
    private void GetAppearOnOtherSideBehaviour(Collider2D collider)
    {
        if (collider.tag == "Wall")
        {
            rb.MovePosition(
                gameArea.OppositePosition(collider.name[4], rb.position));
        }
    }

    // This behaviour makes the agent appear in a random position when it bumps
    // into a wall
    private void GetRandomPositionBehaviour(Collider2D collider)
    {
        if (collider.tag == "Wall")
        {
            rb.MovePosition(gameArea.RandomPosition(0.9f));
        }
    }

    // //////////////////////////////////////// //
    // Below are different methods for steering //
    // //////////////////////////////////////// //

    // Seek behaviour
    private SteeringOutput GetSeekSteering(GameObject target)
    {
        // Initialize linear and angular velocity to zero
        Vector2 linear = Vector2.zero;
        float angular = 0f;

        // Do I have a target?
        if (target != null)
        {
            // Get the direction to the target
            linear = target.transform.position - transform.position;

            // The velocity is along this direction, at full speed
            linear = linear.normalized * maxSpeed;

            // Face in the direction we want to move
            rb.MoveRotation(GetNewOrientation(rb.rotation, linear));

            // Angular velocity not used here, we already changed orientation
            // in the code above
            angular = 0f;
        }

        // Output the steering
        return new SteeringOutput(linear, angular);
    }

    // Flee behaviour
    private SteeringOutput GetFleeSteering(GameObject target)
    {
        Vector2 linear;
        float angular;

        // Get the target steering seek behaviour
        SteeringOutput steering = GetSeekSteering(target);

        // Invert velocity
        linear = steering.Linear * -1;

        // Update rotation according to inverted velocity
        rb.MoveRotation(GetNewOrientation(rb.rotation, linear));

        // Angular velocity not used here, we already changed orientation
        // in the code above
        angular = 0f;

        // Output the steering
        return new SteeringOutput(linear, angular);
    }

    // Seek with arrive behaviour
    private SteeringOutput GetSeekWithArriveSteering(GameObject target)
    {
        // Initialize linear and angular velocity to zero
        Vector2 linear = Vector2.zero;
        float angular = 0f;

        // Do I have a target?
        if (target != null)
        {

            // Get the direction to the target
            linear = target.transform.position - transform.position;

            // Check if we're within radius
            if (linear.magnitude < satisfactionRadius)
            {
                // If so, set velocity to zero
                linear = Vector2.zero;
            }
            else
            {
                // We need to move to our target, we'd like to there in
                // timeToTarget seconds
                linear = linear / timeToTarget;

                // If this is too fast, clip it to the max speed
                if (linear.magnitude > maxSpeed)
                {
                    linear = linear.normalized * maxSpeed;
                }

                // Face in the direction we want to move
                rb.MoveRotation(GetNewOrientation(rb.rotation, linear));
            }

            // Angular velocity not used here, we change orientation
            // automatically
            angular = 0f;
        }

        // Output the steering
        return new SteeringOutput(linear, angular);
    }

    // Wander behaviour
    private SteeringOutput GetWanderSteering(GameObject target)
    {
        // Get vector form of the current rotation of this game object
        Vector2 orientation = new Vector2(
            Mathf.Cos(rb.rotation * Mathf.Deg2Rad),
            Mathf.Sin(rb.rotation * Mathf.Deg2Rad));

        // Get velocity from the vector form of the orientation
        Vector2 linear = maxSpeed * orientation;

        // Change our orientation randomly
        float angular = (Random.Range(0, 1f) - Random.Range(0, 1f))
            * maxAngularVelocity;

        // Output the steering
        return new SteeringOutput(linear, angular);
    }

}
