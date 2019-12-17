using SRandom = System.Random;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using UnityEngine;
using LibGameAI.Optimizers;

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
    private StaticTargetController stcComp;
    private DynamicAgent dynAgComp;
    private Optimizer optimizer;
    private BlockingCollection<ISolution> solutionsQueue;
    private BlockingCollection<float> evaluationsQueue;
    private HillClimber hc;
    private Thread optimizationThread;
    private float currentGameStartTime;

    // Awake is called before the first frame update
    private void Awake()
    {
        dynamicAgent = GameObject.Find("DynamicAgent");
        dynAgComp = dynamicAgent.GetComponent<DynamicAgent>();
        targetController = GameObject.Find("TargetController");
        stcComp = targetController.GetComponent<StaticTargetController>();

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

    private void Optimize()
    {
        SRandom sysRand = new SRandom();
        Result r = hc.Optimize(
            10000,
            100,
            () => new Solution(
                (float)(sysRand.NextDouble() * 100),
                (float)(sysRand.NextDouble() * 100),
                (float)(sysRand.NextDouble() * 100),
                (float)(sysRand.NextDouble() * 100)),
            1);
        Debug.Log(r);
    }

    private void Start()
    {
        if (optimize)
        {
            Solution sol;
            solutionsQueue = new BlockingCollection<ISolution>();
            evaluationsQueue = new BlockingCollection<float>();
            hc = new HillClimber(
                FindNeighbor,
                s => { solutionsQueue.Add(s); return evaluationsQueue.Take(); },
                (a, b) => a > b,
                (a, b) => a > b);
            optimizationThread = new Thread(Optimize);
            optimizationThread.Start();
            sol = (Solution)solutionsQueue.Take();
            dynAgComp.MaxAccel = sol.MaxAccel;
            dynAgComp.MaxSpeed = sol.MaxSpeed;
            dynAgComp.MaxAngularAccel = sol.MaxAngularAccel;
            dynAgComp.MaxRotation = sol.MaxRotation;
        }
        Play();
        currentGameStartTime = Time.time;
    }

    private static ISolution FindNeighbor(ISolution solution)
    {
        Solution s = (Solution)solution;
        float dMaxAccel = Random.Range(-1f, 1f);
        float dMaxSpeed = Random.Range(-1f, 1f);
        float dMaxAngularAccel = Random.Range(-1f, 1f);
        float dMaxRotation = Random.Range(-1f, 1f);
        return new Solution(
            Mathf.Max(0, s.MaxAccel + dMaxAccel),
            Mathf.Max(0, s.MaxSpeed + dMaxSpeed),
            Mathf.Max(0, s.MaxAngularAccel + dMaxAngularAccel),
            Mathf.Max(0, s.MaxRotation + dMaxRotation));
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
            if (!optimizationThread.IsAlive)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }
            if (Time.time > currentGameStartTime + gameTime)
            {
                Solution s;
                Stop();
                evaluationsQueue.Add(stcComp.Points);
                s = (Solution)solutionsQueue.Take();
                dynAgComp.MaxAccel = s.MaxAccel;
                dynAgComp.MaxSpeed = s.MaxSpeed;
                dynAgComp.MaxAngularAccel = s.MaxAngularAccel;
                dynAgComp.MaxRotation = s.MaxRotation;
                Play();
            }
        }
    }
}
