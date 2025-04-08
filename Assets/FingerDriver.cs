using System.IO.Ports;
using UnityEngine;

public class FingerReader : MonoBehaviour
{
    public string portName = "COM5";
    public int baudRate = 115200;

    private SerialPort serialPort;

    public OVRCustomSkeleton skeleton;

    public float curlMultiplier = 1.0f; // Adjust this in the Inspector

    // Finger joints
    private Transform index1, index2, index3;
    private Transform middle1, middle2, middle3;
    private Transform ring1, ring2, ring3;
    private Transform pinky1, pinky2, pinky3;
    private Transform thumb1, thumb2, thumb3;

    void Start()
    {
        serialPort = new SerialPort(portName, baudRate);
        try { serialPort.Open(); Debug.Log("Serial opened"); }
        catch { Debug.LogWarning("Serial failed"); }

        AssignBones();
    }

    void AssignBones()
    {
        // Index
        index1 = skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Index1].Transform;
        index2 = skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Index2].Transform;
        index3 = skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Index3].Transform;

        // Middle
        middle1 = skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Middle1].Transform;
        middle2 = skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Middle2].Transform;
        middle3 = skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Middle3].Transform;

        // Ring
        ring1 = skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Ring1].Transform;
        ring2 = skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Ring2].Transform;
        ring3 = skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Ring3].Transform;

        // Pinky
        pinky1 = skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Pinky1].Transform;
        pinky2 = skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Pinky2].Transform;
        pinky3 = skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Pinky3].Transform;

        // Thumb (starts at Thumb1)
        thumb1 = skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Thumb1].Transform;
        thumb2 = skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Thumb2].Transform;
        thumb3 = skeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Thumb3].Transform;
    }

    void Update()
    {
        if (serialPort == null || !serialPort.IsOpen) return;

        try
        {
            string line = serialPort.ReadLine();
            if (!string.IsNullOrWhiteSpace(line))
            {
                ApplyFingerRotations(line);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("Serial read failed: " + ex.Message);
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

        float t = Parse("A"); // Thumb
        float i = Parse("B");
        float m = Parse("C");
        float r = Parse("D");
        float p = Parse("E");


        // Index
        index1.localRotation = Quaternion.Euler(0, 0, -i* 60f * curlMultiplier);
        index2.localRotation = Quaternion.Euler(0, 0, -i* 70f * curlMultiplier);
        index3.localRotation = Quaternion.Euler(0, 0, -i* 45f * curlMultiplier);

        // Middle
        middle1.localRotation = Quaternion.Euler(0, 0, -m* 60f * curlMultiplier);
        middle2.localRotation = Quaternion.Euler(0, 0, -m* 70f * curlMultiplier);
        middle3.localRotation = Quaternion.Euler(0, 0, -m* 45f * curlMultiplier);

        // Ring
        ring1.localRotation = Quaternion.Euler(0, 0, -r* 60f * curlMultiplier);
        ring2.localRotation = Quaternion.Euler(0, 0, -r* 70f * curlMultiplier);
        ring3.localRotation = Quaternion.Euler(0, 0, -r* 45f * curlMultiplier);

        // Pinky
        pinky1.localRotation = Quaternion.Euler(0, 0, p* 60f * curlMultiplier);
        pinky2.localRotation = Quaternion.Euler(0, 0, p* 70f * curlMultiplier);
        pinky3.localRotation = Quaternion.Euler(0, 0, p* 45f * curlMultiplier);

        // Thumb
        thumb1.localRotation = Quaternion.Euler(0, 0, -t* 40f *2* curlMultiplier);
        thumb2.localRotation = Quaternion.Euler(0, 0, -t* 40f *2* curlMultiplier);
        thumb3.localRotation = Quaternion.Euler(0, 0, -t* 40f *2* curlMultiplier);


    }

void OnApplicationQuit()
    {
        if (serialPort != null && serialPort.IsOpen)
            serialPort.Close();
    }
}
