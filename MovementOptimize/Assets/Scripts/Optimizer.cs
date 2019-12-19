/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 *
 * Author: Nuno Fachada
 * */

using SRandom = System.Random;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using UnityEngine;
using LibGameAI.Optimizers;

// This script controls the game and the optimization process
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

    // References to game objects
    private GameObject dynamicAgent;
    private GameObject targetController;

    // References to components in other game objects
    private DynamicAgent dynAgComp;
    private StaticTargetController stcComp;

    // Blocking queues for exchanging messages between the main Unity thread
    // and the optimization thread
    private BlockingCollection<ISolution> solutionsQueue;
    private BlockingCollection<float> evaluationsQueue;

    // Reference to the hill climber algorithm class
    private HillClimber hc;

    // Reference to the optimization thread
    private Thread optimizationThread;

    // The start time for the current run
    private float currentRunStartTime;

    // A C# native random number generator, to be used by the optimization
    // thread (which can't use the Unity PRNG)
    private SRandom threadRnd;

    // Is a run running?
    private bool running;

    // Number of current run
    private int run;

    // Current solution
    private ISolution sol;

    // Awake is called before the first frame update
    private void Awake()
    {
        // Get references to the game objects and their components
        dynamicAgent = GameObject.Find("DynamicAgent");
        dynAgComp = dynamicAgent.GetComponent<DynamicAgent>();
        targetController = GameObject.Find("TargetController");
        stcComp = targetController.GetComponent<StaticTargetController>();

        // Deactivate the game's game objects, so the game doesn't start
        // running right away
        Stop();

        // Is this an optimization play or a single game play?
        if (optimize)
        {
            // Change the time scale as specified in the editor
            Time.timeScale = timeScale;

            // Should we show the game will optimizing?
            if (!showGame)
            {
                // If not, disable the camera
                GameObject goCam = GameObject.Find("Main Camera");
                goCam.SetActive(false);
            }

            // Instantiate a new C# native random number generator, to be used
            // by the optimization thread
            threadRnd = new SRandom();
        }
    }

    // Start is called on the frame when a script is enabled just before any
    // of the Update methods are called the first time
    private void Start()
    {
        // Is this an optimization play?
        if (optimize)
        {
            // Set run number to zero
            run = 0;

            // Initialize the thread communication queues
            solutionsQueue = new BlockingCollection<ISolution>();
            evaluationsQueue = new BlockingCollection<float>();

            // Initialize the hill climber algorithm
            hc = new HillClimber(
                // Method to find neighbor solutions
                FindNeighbor,
                // Method for evaluating solutions, which is basically saying
                // add current solution for evaluation and return the resulting
                // evaluation
                s => { solutionsQueue.Add(s); return evaluationsQueue.Take(); },
                // Method for comparing solutions (which is best)
                (a, b) => a > b,
                // Method for selecting the solution for the next iteration
                (a, b) =>
                {
                    // If neighbor is best, always select it
                    if (a > b) return true;
                    // Otherwise we can still select it with a given probability
                    float p = 1 / (1 + Mathf.Exp((b - a) / (0.25f * b)));
                    return threadRnd.NextDouble() < p;
                });

            // Create and start optimization thread, which will run the
            // Optimize() method
            optimizationThread = new Thread(Optimize);
            optimizationThread.Start();

            // Get first solution given by the hill climber
            sol = (Solution)solutionsQueue.Take();

            // Set the game parameters to the given solution
            SetSolution();
        }

        // Order the game to start playing
        Play();

        // Keep track of the current run's start time
        currentRunStartTime = Time.time;
    }

    // Play a game
    // This method simply activates the game's game objects
    private void Play()
    {
        running = true;
        dynamicAgent.transform.position = Vector3.zero;
        dynamicAgent.SetActive(true);
        targetController.SetActive(true);
    }

    // Stop a game
    // This method simply deactivates the game's game objects
    private void Stop()
    {
        running = false;
        dynamicAgent.SetActive(false);
        targetController.SetActive(false);
    }

    // Update method, called once per frame
    private void Update()
    {
        // Is this an optimization play?
        if (optimize)
        {
            // If the optimization thread is finished, quit the program
            if (!optimizationThread.IsAlive)
            {
                Finish();
            }
            // Otherwise check if the game is running and should stop
            else if (running && Time.time > currentRunStartTime + gameTime)
            {
                // Stop current run
                Stop();
                // Notify of current status
                Debug.Log($"Run {run++} scored {stcComp.Points} for " +
                    $"{sol} " +
                    $"(current = {hc.CurrentEvaluation}, " +
                    $"best in run = {hc.BestEvaluationInRun}, " +
                    $"best all runs = {hc.BestEvaluation})");
                // Send evaluation to optimization thread
                evaluationsQueue.Add(stcComp.Points);
                // Try and take a new solution for the next run
                if (!solutionsQueue.TryTake(out sol, 1500))
                {
                    // If we timed out, it means the hill climber is finished,
                    // and we terminate the program
                    Finish();
                }
                else
                // We didn't time out, so set the parameters according to the
                // new solution provided by the hill climber
                {
                    SetSolution();
                }
            }
            // Otherwise check if the game is not running and should start
            else if (!running && Time.time > currentRunStartTime + gameTime)
            {
                Play();
                currentRunStartTime = Time.time;
            }
        }
        // If this is not an optimization play, just finish after the game time
        else if (Time.time > currentRunStartTime + gameTime)
        {
            Debug.Log($"Score after {gameTime}s is {stcComp.Points}");
            Finish();
        }
    }

    // Stop and quit the program
    public void Finish()
    {
        // If this was an optimization play, join with the optimization thread
        if (optimize) optimizationThread.Join();
        // Finish the game
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Set the current solution values for the next run
    private void SetSolution()
    {
        dynAgComp.MaxAccel = ((Solution)sol).MaxAccel;
        dynAgComp.MaxSpeed = ((Solution)sol).MaxSpeed;
        dynAgComp.MaxAngularAccel = ((Solution)sol).MaxAngularAccel;
        dynAgComp.MaxRotation = ((Solution)sol).MaxRotation;
    }

    // ////////////////////////////////////////////////////////// //
    // The methods below will only run in the optimization thread //
    // ////////////////////////////////////////////////////////// //

    // Returns a value derived from the random binomial distribution
    private float RandBinom() =>
        (float)(threadRnd.NextDouble() - threadRnd.NextDouble());

    // The method which the optimization thread will run
    private void Optimize()
    {
        Result r = hc.Optimize(
            maxSteps,
            120, // Criteria
            () => new Solution(
                (float)(threadRnd.NextDouble() * 250),
                (float)(threadRnd.NextDouble() * 250),
                (float)(threadRnd.NextDouble() * 900),
                (float)(threadRnd.NextDouble() * 900)),
            numRuns,
            evalsPerSolution
        );
        Debug.Log(r);
    }

    // Method which returns a neighbor of the current solution
    // It's internally called by the hill climber
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
}
