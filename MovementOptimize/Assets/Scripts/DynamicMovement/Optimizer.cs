using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Optimizer : MonoBehaviour
{
    [SerializeField] private bool optimize;
    [SerializeField] private bool showOptimizationSteps;

    // Start is called before the first frame update
    private void Awake()
    {
        if (optimize)
        {
            Time.timeScale = 100;
            if (!showOptimizationSteps)
            {
                GameObject goCam = GameObject.Find("Main Camera");
                goCam.SetActive(false);
                //Camera cam = goCam.GetComponent<Camera>();
                //cam.SetActive(false);
            }
        }
    }


}
