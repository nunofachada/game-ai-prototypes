using UnityEngine;

public interface ISteeringBehaviour
{
    float Weight { get; }
    SteeringOutput GetSteering(GameObject target);
}

