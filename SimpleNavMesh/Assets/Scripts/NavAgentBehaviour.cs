using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavAgentBehaviour : MonoBehaviour
{
    // Current goal of navigation agent
    [SerializeField] private Transform goal = null;

    // Reference to the NavMeshAgent component
    private NavMeshAgent agent;

    // Start is called before the first frame update
    private void Start()
    {
        // Get reference to the NavMeshAgent component
        agent = GetComponent<NavMeshAgent>();
        // Set initial agent goal
        agent.SetDestination(goal.position);
    }

    // Method called when agent collides with something
    private void OnTriggerEnter(Collider other)
    {
        // Did agent collide with goal?
        if (other.name == "Goal")
            // If so, update destination (let goal reposition itself first)
            Invoke("UpdateDestination", 0.1f);
    }

    // Update destination
    private void UpdateDestination()
    {
        // Set destination to current goal position
        agent.SetDestination(goal.position);
    }

}
