using UnityEngine;

public abstract class SteeringBehaviour : MonoBehaviour, ISteeringBehaviour {

    public float weight = 1;

    protected DynamicAgent agent;
    protected Rigidbody2D rb;

    public float Weight { get { return weight; } }

    // Use this for initialization
    void Start()
    {
        agent = GetComponent<DynamicAgent>();
        rb = GetComponent<Rigidbody2D>();
    }

    public abstract SteeringOutput GetSteering(GameObject target);
}
