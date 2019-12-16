using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Optimizer : MonoBehaviour
{
    [Tooltip("Perform optimization?")]
    [SerializeField] private bool optimize = false;

    [Tooltip("The higher it is, the faster the game runs")]
    [SerializeField] private float timeScale = 200;

    [Tooltip("How long should the game run for each optimization step?")]
    [SerializeField] private float gameTime = 60;

    [Tooltip("Maximum optimization steps to perform?")]
    [SerializeField] private float maxSteps = 10000;

    [Tooltip("Show game while optimizing? (slower!)")]
    [SerializeField] private bool showGame = false;

    private GameObject dynamicAgent;
    private GameObject targetController;
    private Optimizer optimizer;

    // Awake is called before the first frame update
    private void Awake()
    {
        dynamicAgent = GameObject.Find("DynamicAgent");
        targetController = GameObject.Find("TargetController");
        optimizer = GameObject.Find("Optimizer")?.GetComponent<Optimizer>();
        Stop();

        if (optimize)
        {
            Time.timeScale = timeScale;
            if (!showGame)
            {
                GameObject goCam = GameObject.Find("Main Camera");
                goCam.SetActive(false);
            }
        }
    }

    private void Start()
    {
        if (optimize)
        {
            Play();
        }
        else
        {
            Play();
        }
    }

    private void Play()
    {
        dynamicAgent.transform.position = Vector3.zero;
        dynamicAgent.SetActive(true);
        targetController.SetActive(true);
    }

    private void Stop()
    {
        dynamicAgent.SetActive(false);
        targetController.SetActive(false);
    }

    private void Update()
    {
        if (optimize)
        {
            if (Time.time > gameTime)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }
        }
    }
}
