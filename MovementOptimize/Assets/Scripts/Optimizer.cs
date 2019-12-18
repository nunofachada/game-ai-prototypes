using SRandom = System.Random;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using UnityEngine;
using LibGameAI.Optimizers;

public class Optimizer : MonoBehaviour
{
    [Tooltip("How long should the game run for each optimization step?")]
    [SerializeField] private float gameTime = 60;

    [Tooltip("Perform optimization?")]
    [SerializeField] private bool optimize = false;

    [Tooltip("The higher it is, the faster the game runs")]
    [SerializeField] private float timeScale = 200;

    [Tooltip("How many times do we evaluate each solution?")]
    [SerializeField] private int evalsPerSolution = 2;

    [Tooltip("Maximum optimization steps to perform per run?")]
    [SerializeField] private int maxSteps = 10000;

    [Tooltip("How many runs to perform?")]
    [SerializeField] private int numRuns = 4;

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

    private SRandom threadRnd;
    private bool playing;

    private int run;

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
            threadRnd = new SRandom();
        }
    }

    private void Optimize()
    {
        SRandom sysRand = new SRandom();
        Result r = hc.Optimize(
            maxSteps,
            120, // Criteria
            () => new Solution(
                (float)(sysRand.NextDouble() * 250),
                (float)(sysRand.NextDouble() * 250),
                (float)(sysRand.NextDouble() * 900),
                (float)(sysRand.NextDouble() * 900)),
            numRuns,
            evalsPerSolution
        );
        Debug.Log(r);
    }

    private void Start()
    {
        if (optimize)
        {
            Solution sol;
            run = 0;
            solutionsQueue = new BlockingCollection<ISolution>();
            evaluationsQueue = new BlockingCollection<float>();
            hc = new HillClimber(
                FindNeighbor,
                s => { solutionsQueue.Add(s); return evaluationsQueue.Take(); },
                (a, b) => a > b,
                (a, b) => {
                    if (a > b) return true;
                    float p = 1 / (1 + Mathf.Exp((b - a) / (0.25f * b)));
                    return threadRnd.NextDouble() < p; } );
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

    private float RandBinom() =>
        (float)(threadRnd.NextDouble() - threadRnd.NextDouble());

    private ISolution FindNeighbor(ISolution solution)
    {
        Solution s = (Solution)solution;
        float dMaxAccel = RandBinom() * 25;
        float dMaxSpeed = RandBinom() * 25;
        float dMaxAngularAccel = RandBinom() * 60;
        float dMaxRotation = RandBinom() * 60;
        return new Solution(
            Mathf.Max(0, s.MaxAccel + dMaxAccel),
            Mathf.Max(0, s.MaxSpeed + dMaxSpeed),
            Mathf.Max(0, s.MaxAngularAccel + dMaxAngularAccel),
            Mathf.Max(0, s.MaxRotation + dMaxRotation));
    }

    private void Play()
    {
        playing = true;
        dynamicAgent.transform.position = Vector3.zero;
        dynamicAgent.SetActive(true);
        targetController.SetActive(true);
    }

    private void Stop()
    {
        playing = false;
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
            else if (playing && Time.time > currentGameStartTime + gameTime)
            {
                Debug.Log($"Run {run++} scored {stcComp.Points} " +
                    $"(current is = {hc.CurrentEvaluation}, " +
                    $"best in run = {hc.BestEvaluationInRun}, " +
                    $"best all runs = {hc.BestEvaluation})");
                ISolution s;
                Stop();
                evaluationsQueue.Add(stcComp.Points);
                if (!solutionsQueue.TryTake(out s, 1500))
                {
                    UnityEditor.EditorApplication.isPlaying = false;
                    return;
                }
                dynAgComp.MaxAccel = ((Solution)s).MaxAccel;
                dynAgComp.MaxSpeed = ((Solution)s).MaxSpeed;
                dynAgComp.MaxAngularAccel = ((Solution)s).MaxAngularAccel;
                dynAgComp.MaxRotation = ((Solution)s).MaxRotation;
            } else if  (!playing && Time.time > currentGameStartTime + gameTime)
            {
                Play();
                currentGameStartTime = Time.time;
            }
        }
        else if (Time.time > currentGameStartTime + gameTime)
        {
            Debug.Log($"Score after {gameTime}s is {stcComp.Points}");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
