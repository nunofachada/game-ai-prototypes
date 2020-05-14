/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using UnityEngine;

public class PlaneWalls : MonoBehaviour
{
    // References to the player and AI agents
    private GameObject[] agents;

    // Plane limits
    private Vector4 planeLimits;

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Get references to the player and AI agents
        agents = new GameObject[2];
        agents[0] = GameObject.Find("PlayerAgent");
        agents[1] = GameObject.Find("AIAgent");

        // Determine plane limits
        planeLimits = new Vector4(
            transform.position.x + transform.localScale.x * 5,
            transform.position.x - transform.localScale.x * 5,
            transform.position.z + transform.localScale.z * 5,
            transform.position.z - transform.localScale.z * 5);
    }

    // Check if agents are within plane limits
    private void Update()
    {
        // Cycle through all agents
        foreach (GameObject agent in agents)
        {
            // Get current agent position
            Vector3 agPos = agent.transform.position;

            // Check if current agent is within plane limits
            if (agPos.x > planeLimits.x)
                agent.transform.position = new Vector3(
                    planeLimits.x,
                    agent.transform.position.y,
                    agent.transform.position.z);
            if (agPos.x < planeLimits.y)
                agent.transform.position = new Vector3(
                    planeLimits.y,
                    agent.transform.position.y,
                    agent.transform.position.z);
            if (agPos.z > planeLimits.z)
                agent.transform.position = new Vector3(
                    agent.transform.position.x,
                    agent.transform.position.y,
                    planeLimits.z);
            if (agPos.z < planeLimits.w)
                agent.transform.position = new Vector3(
                    agent.transform.position.x,
                    agent.transform.position.y,
                    planeLimits.w);
        }
    }
}
