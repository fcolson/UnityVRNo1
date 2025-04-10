using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class TorchGuidedMovement : MonoBehaviour
{
    public TMP_Text instructionText;
    public GameObject torch;
    public GameObject redCircle;
    public GameObject greenCircle;
    public GameObject blueCircle;
    public TMP_Text stopwatchText;

    private GloveOVRGrabber gloveGrabber; // NEW

    private List<GameObject> circles;
    private int currentCircleIndex = 0;
    private float timer = 0f;
    private bool isTimerRunning = false;
    private int passCount = 0;

    [SerializeField] private int numPasses = 7;

    private bool simulationComplete = false;


    void Start()
    {
        // Automatically find the glove in the scene — or assign manually if needed
        gloveGrabber = FindObjectOfType<GloveOVRGrabber>();
        if (gloveGrabber == null)
        {
            Debug.LogError("[TorchGuidedMovement] GloveOVRGrabber not found!");
        }

        circles = new List<GameObject> { redCircle, greenCircle, blueCircle };
        RandomizeCircleOrder();

        instructionText.text = "Grab the sphere to start the simulation.\nYou will drop it according to the directions.";
        stopwatchText.text = "Time: 0.0s";
    }

    void Update()
    {
        if (!simulationComplete && gloveGrabber != null && gloveGrabber.IsGrabbing && !isTimerRunning)
        {
            isTimerRunning = true;
        }


        if (isTimerRunning)
        {
            timer += Time.deltaTime;
            stopwatchText.text = "Time: " + timer.ToString("F1") + "s";
        }

        if (passCount >= numPasses)
        {
            isTimerRunning = false;
            instructionText.text = "Simulation complete! Time: " + timer.ToString("F1") + "s";
            simulationComplete = true;

        }
    }

    private void RandomizeCircleOrder()
    {
        for (int i = 0; i < circles.Count; i++)
        {
            GameObject temp = circles[i];
            int randomIndex = Random.Range(i, circles.Count);
            circles[i] = circles[randomIndex];
            circles[randomIndex] = temp;
        }

        instructionText.text = "Grab the sphere and drop it on the " + circles[currentCircleIndex].name + " circle.";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (gloveGrabber != null && !gloveGrabber.IsGrabbing)
        {
            if (other.gameObject == circles[currentCircleIndex])
            {
                passCount++;
                currentCircleIndex++;

                if (passCount >= numPasses)
                {
                    instructionText.text = "Simulation complete! Time: " + timer.ToString("F1") + "s";
                    isTimerRunning = false;
                    simulationComplete = true;

                    return;
                }

                if (currentCircleIndex >= circles.Count)
                {
                    currentCircleIndex = 0;
                }

                instructionText.text = "Grab the sphere and drop it on the " + circles[currentCircleIndex].name + " circle.";
            }
            else
            {
                instructionText.text = "Grab the sphere and drop it on the " + circles[currentCircleIndex].name + " circle.";
            }
        }
    }
}
