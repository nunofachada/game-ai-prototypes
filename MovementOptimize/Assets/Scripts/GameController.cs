using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private GameObject dynamicAgent;
    private GameObject targetController;
    private Optimizer optimizer;

    private void Awake()
    {
        dynamicAgent = GameObject.Find("DynamicAgent");
        targetController = GameObject.Find("TargetController");
        optimizer = GameObject.Find("Optimizer")?.GetComponent<Optimizer>();
        Stop();
    }

    private void Start()
    {
        if (!(optimizer?.Optimize ?? false))
        {
            Play();
        }
    }

    public void Play()
    {
        dynamicAgent.transform.position = Vector3.zero;
        dynamicAgent.SetActive(true);
        targetController.SetActive(true);
    }

    public void Stop()
    {
        dynamicAgent.SetActive(false);
        targetController.SetActive(false);
    }
}
