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

    public bool Optimize => optimize;

    // Start is called before the first frame update
    private void Awake()
    {
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

    private void Update()
    {
        if (optimize)
        {
            if (Time.time > gameTime)
            {
                // save any game data here
#if UNITY_EDITOR
                // Application.Quit() does not work in the editor so
                // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif

            }
            else Debug.Log($"Time is {Time.time}");
        }
    }


}
