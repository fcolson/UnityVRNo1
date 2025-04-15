using UnityEngine;
using TMPro;
using Oculus.Interaction;
using System.Collections.Generic;

public class OldTorchGuidedMovement : MonoBehaviour
{
    public TMP_Text instructionText;  // Text object to display instructions
    public GameObject torch;         // The torch object
    public GameObject redCircle;     // Red target circle
    public GameObject greenCircle;   // Green target circle
    public GameObject blueCircle;    // Blue target circle
    public TMP_Text stopwatchText;   // Text to display stopwatch timer

    private SendGrabData grabData;
    private List<GameObject> circles; // List to store the target circles in randomized order
    private int currentCircleIndex = 0; // Tracks the current target circle
    private float timer = 0f; // Stopwatch timer
    private bool isTimerRunning = false; // Check if timer is running
    private int passCount = 0; // Tracks the number of passes

    [SerializeField] private int numPasses = 7;

    private bool simulationComplete = false;

    void Start()
    {
        grabData = GetComponent<SendGrabData>();
        circles = new List<GameObject> { redCircle, greenCircle, blueCircle };
        SetNextTargetCircle();


        instructionText.text = "Grab the torch to start the simulation.\nYou will drop the torch according to the directions.";
        stopwatchText.text = "Time: 0.0s";  // Start with time at 0
    }

    void Update()
    {
        // Start the stopwatch when the torch is first grabbed
        if (!simulationComplete && grabData._isGrabbed && !isTimerRunning)
        {
            isTimerRunning = true;
        }

        // Update the stopwatch timer if it's running
        if (isTimerRunning)
        {
            timer += Time.deltaTime;
            stopwatchText.text = "Time: " + timer.ToString("F1") + "s";
        }

        // If 5 passes are completed, stop the timer
        if (passCount >= numPasses)
        {
            isTimerRunning = false;
            instructionText.text = "Simulation complete! Time: " + timer.ToString("F1") + "s";
            simulationComplete = true;
        }
    }

    // Randomize the circle order
    private GameObject lastCircle = null; // To track the previous target

    private void SetNextTargetCircle()
    {
        GameObject nextCircle;

        // Keep picking a new circle until it's different from the last one
        do
        {
            nextCircle = circles[Random.Range(0, circles.Count)];
        } while (nextCircle == lastCircle);

        lastCircle = nextCircle;
        instructionText.text = "Grab the torch and drop the it on the " + nextCircle.name + " circle.";
        currentCircleIndex = circles.IndexOf(nextCircle); // update index for comparison in OnTriggerEnter
    }


    // When the torch enters a trigger zone (red, green, or blue circle)
    private void OnTriggerEnter(Collider other)
    {
        if (!grabData._isGrabbed)
        {
            // Check if the touched circle matches the expected one
            if (other.gameObject == circles[currentCircleIndex])
            {
                passCount++;

                if (passCount >= numPasses)
                {
                    instructionText.text = "Simulation complete! Time: " + timer.ToString("F1") + "s";
                    isTimerRunning = false;
                    simulationComplete = true;
                    return;
                }

                // Pick a new target that's not the same as last
                SetNextTargetCircle();

            }
            else
            {
                // If wrong circle, just stay in the same sequence
                instructionText.text = "Grab the torch and drop the it on the " + circles[currentCircleIndex].name + " circle.";
            }
        }
    }
}