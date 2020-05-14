/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System;
using UnityEngine;
using URandom = UnityEngine.Random;
using LibGameAI.FSMs;

// The script that controls an agent using an FSM
public class AIBehaviour : MonoBehaviour
{
    // Minimum distance to small enemy
    [SerializeField]
    private float minDistanceToSmallEnemy = 9f;

    // Minimum distance to big enemy
    [SerializeField]
    private float minDistanceToBigEnemy = 11f;

    // Maximum speed
    [SerializeField]
    private float maxSpeed = 5f;

    // References to enemies
    private GameObject smallEnemy, bigEnemy;

    // Reference to the state machine
    private StateMachine stateMachine;

    // Get references to enemies
    private void Awake()
    {
        smallEnemy = GameObject.Find("SmallEnemy");
        bigEnemy = GameObject.Find("BigEnemy");
    }

    // Create the FSM
    private void Start()
    {
        // Create the states
        State onGuardState = new State("On Guard",
            () => Debug.Log("Enter On Guard state"),
            null,
            () => Debug.Log("Leave On Guard state"));

        State fightState = new State("Fight",
            () => Debug.Log("Enter Fight state"),
            ChaseSmallEnemy,
            () => Debug.Log("Leave Fight state"));

        State runAwayState = new State("Runaway",
            () => Debug.Log("Enter Runaway state"),
            RunAway,
            () => Debug.Log("Leaving Runaway state"));

        // Add the transitions
        onGuardState.AddTransition(
            new Transition(
                () =>
                    (smallEnemy.transform.position - transform.position).magnitude
                    < minDistanceToSmallEnemy,
                () => Debug.Log("I just saw a small enemy!"),
                fightState));
        onGuardState.AddTransition(
            new Transition(
                () =>
                    (bigEnemy.transform.position - transform.position).magnitude
                    < minDistanceToBigEnemy,
                () => Debug.Log("I just saw a big enemy!"),
                runAwayState));
        fightState.AddTransition(
            new Transition(
                () => URandom.value < 0.001f ||
                    (bigEnemy.transform.position - transform.position).magnitude
                        < minDistanceToBigEnemy,
                () => Debug.Log("Losing a fight!"),
                runAwayState));
        runAwayState.AddTransition(
            new Transition(
                () => (smallEnemy.transform.position - transform.position).magnitude
                        > minDistanceToSmallEnemy
                    &&
                    (bigEnemy.transform.position - transform.position).magnitude
                        > minDistanceToBigEnemy,
                () => Debug.Log("I barely escaped!"),
                onGuardState));

        // Create the state machine
        stateMachine = new StateMachine(onGuardState);
    }

    // Request actions to the FSM and perform them
    private void Update()
    {
        Action actions = stateMachine.Update();
        actions?.Invoke();
    }

    // Chase the small enemy
    private void ChaseSmallEnemy()
    {
        // Get normalized direction towards small enemy
        Vector3 direction =
            (smallEnemy.transform.position - transform.position).normalized;

        // Move towards small enemy
        transform.Translate(direction * maxSpeed * Time.deltaTime, Space.World);
    }

    // Runaway from the closest enemy
    private void RunAway()
    {
        // Get vector to small enemy
        Vector3 toSmall = transform.position - smallEnemy.transform.position;
        // Get vector to big enemy
        Vector3 toBig = transform.position - bigEnemy.transform.position;

        // Get vector to closest enemy
        Vector3 toClosest =
            toSmall.magnitude < toBig.magnitude ? toSmall : toBig;

        // Move in the oposite direction
        transform.Translate(toClosest.normalized * maxSpeed * Time.deltaTime);
    }
}
