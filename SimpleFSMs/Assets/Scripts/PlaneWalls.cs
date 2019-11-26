/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Component to associate with a plane where the game takes place
// It stops agents from going out of the plane bounds, which can be resized
// in real time
public class PlaneWalls : MonoBehaviour
{
    // References to the player and AI agents
    private GameObject[] agents;

    // Get references to the player and AI agents
    private void Awake()
    {
        agents = GameObject.FindGameObjectsWithTag("Agent");
    }

    // Property that returns the limits of the plane
    public Vector4 Limits => new Vector4(
            transform.position.x + transform.localScale.x * 5,
            transform.position.x - transform.localScale.x * 5,
            transform.position.z + transform.localScale.z * 5,
            transform.position.z - transform.localScale.z * 5);

    // Check if agents are within plane limits
    private void Update()
    {
        // Cycle through all agents
        foreach (GameObject agent in agents)
        {
            // Get current agent position
            Vector3 agPos = agent.transform.position;

            // Check if current agent is within plane limits
            if (agPos.x > Limits.x)
                agent.transform.position = new Vector3(
                    Limits.x,
                    agent.transform.position.y,
                    agent.transform.position.z);
            if (agPos.x < Limits.y)
                agent.transform.position = new Vector3(
                    Limits.y,
                    agent.transform.position.y,
                    agent.transform.position.z);
            if (agPos.z > Limits.z)
                agent.transform.position = new Vector3(
                    agent.transform.position.x,
                    agent.transform.position.y,
                    Limits.z);
            if (agPos.z < Limits.w)
                agent.transform.position = new Vector3(
                    agent.transform.position.x,
                    agent.transform.position.y,
                    Limits.w);
        }
    }
}
