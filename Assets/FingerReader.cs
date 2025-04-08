using System.IO.Ports;
using UnityEngine;

public class FingerReader : MonoBehaviour
{
    public string portName = "COM5";
    public int baudRate = 115200;

    private SerialPort serialPort;

    // Direct transform references
    public Transform index1, middle1, ring1, pinky1, thumb1;

    private float serialStartupTimer = 0.5f; // wait 0.5s after starting


    void Start()
    {
        serialPort = new SerialPort(portName, baudRate);
        try { serialPort.Open(); Debug.Log(" Serial opened"); }
        catch { Debug.LogWarning(" Serial failed"); }
    }

    void Update()
    {
        // Wait for serial to settle
        if (serialStartupTimer > 0f)
        {
            serialStartupTimer -= Time.deltaTime;
            return;
        }

        if (serialPort == null || !serialPort.IsOpen) return;

        try
        {
            string line = serialPort.ReadExisting();
            if (!string.IsNullOrWhiteSpace(line))
            {
                Debug.Log("Box " + line);
                ApplyFingerRotations(line);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Serial read error: " + e.Message);
        }
    }


    void ApplyFingerRotations(string line)
    {
        float Parse(string label)
        {
            int i = line.IndexOf(label);
            if (i < 0) return 0f;
            int start = i + 1;
            int end = start;
            while (end < line.Length && char.IsDigit(line[end])) end++;
            if (end == start) return 0f;
            return Mathf.InverseLerp(0, 4095, int.Parse(line.Substring(start, end - start)));
        }

        float i = Parse("B");
        float m = Parse("C");
        float r = Parse("D");
        float p = Parse("E");
        float t = Parse("A");

        Debug.Log($"Parsed: t={t} i={i} m={m} r={r} p={p}");


        // Rotate each proximal bone
        if (index1) index1.localRotation = Quaternion.Euler(i * 70f, 0, 0);
        Debug.Log("Rotating index1 to: " + index1.localRotation.eulerAngles);

        if (middle1) middle1.localRotation = Quaternion.Euler(m * 70f, 0, 0);
        if (ring1) ring1.localRotation = Quaternion.Euler(r * 70f, 0, 0);
        if (pinky1) pinky1.localRotation = Quaternion.Euler(p * 70f, 0, 0);
        if (thumb1) thumb1.localRotation = Quaternion.Euler(t * 50f, 0, 0);
        Debug.Log("Thumb rot: " + thumb1.localRotation.eulerAngles);

    }

    void OnApplicationQuit()
    {
        if (serialPort != null && serialPort.IsOpen)
            serialPort.Close();
    }
}
