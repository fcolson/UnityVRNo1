using UnityEngine;
using TMPro;
using Oculus.Interaction;
using System.Collections.Generic;

public class TorchGuidedMovement : MonoBehaviour
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

    void Start()
    {
        grabData = GetComponent<SendGrabData>();
        circles = new List<GameObject> { redCircle, greenCircle, blueCircle };
        RandomizeCircleOrder();

        instructionText.text = "Grab the torch to start the simulation.\nYou will drop the torch according to the directions.";
        stopwatchText.text = "Time: 0.0s";  // Start with time at 0
    }

    void Update()
    {
        // Start the stopwatch when the torch is first grabbed
        if (grabData._isGrabbed && !isTimerRunning)
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
        if (passCount >= 5)
        {
            isTimerRunning = false;
            instructionText.text = "Simulation complete! Time: " + timer.ToString("F1") + "s";
        }
    }

    // Randomize the circle order
    private void RandomizeCircleOrder()
    {
        for (int i = 0; i < circles.Count; i++)
        {
            GameObject temp = circles[i];
            int randomIndex = Random.Range(i, circles.Count);
            circles[i] = circles[randomIndex];
            circles[randomIndex] = temp;
        }

        // Set the first target circle in the randomized order
        instructionText.text = "Grab the torch and drop the it on the " + circles[currentCircleIndex].name + " circle.";
    }

    // When the torch enters a trigger zone (red, green, or blue circle)
    private void OnTriggerEnter(Collider other)
    {
        if (!grabData._isGrabbed)
        {
            // Check if the touched circle matches the expected one
            if (other.gameObject == circles[currentCircleIndex])
            {
                passCount++; // Increment pass count
                currentCircleIndex++; // Move to the next target in the randomized order

                // After 5 passes, stop the simulation
                if (passCount >= numPasses)
                {
                    instructionText.text = "Simulation complete! Time: " + timer.ToString("F1") + "s";
                    isTimerRunning = false; // Stop the timer
                    return;
                }

                // Ensure the currentCircleIndex does not go out of bounds, loop back to the start
                if (currentCircleIndex >= circles.Count)
                {
                    currentCircleIndex = 0; // Loop back to the first circle
                }

                // Update instruction text with the next target circle
                instructionText.text = "Grab the torch and drop the it on the " + circles[currentCircleIndex].name + " circle.";
            }
            else
            {
                // If wrong circle, just stay in the same sequence
                instructionText.text = "Grab the torch and drop the it on the " + circles[currentCircleIndex].name + " circle.";
            }
        }
    }
}
