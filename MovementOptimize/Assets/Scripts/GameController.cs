using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private GameObject dynamicAgent;
    private GameObject targetController;

    private void Awake()
    {
        dynamicAgent = GameObject.Find("DynamicAgent");
        targetController = GameObject.Find("TargetController");
    }

    // Start is called before the first frame update
    public void Play()
    {
        Instantiate(dynamicAgent);
        Instantiate(targetController);
    }

    // Update is called once per frame
    public void Stop()
    {

    }
}
