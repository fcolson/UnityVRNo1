using System.IO.Ports;
using UnityEngine;

public class FingerDriver : MonoBehaviour
{

    public Transform controllerAnchor;

    public string portName = "COM5";
    public int baudRate = 115200;

    private SerialPort serialPort;

    public OVRCustomSkeleton skeleton;

    public float tMult = 2.0f; // Adjust this in the Inspector
    public float iMult = 1.0f; // Adjust this in the Inspector
    public float mMult = 2.2f; // Adjust this in the Inspector
    public float rMult = 4.0f; // Adjust this in the Inspector
    public float pMult = 4.0f; // Adjust this in the Inspector

    // Finger joints
    private Transform index1, index2, index3;
    private Transform middle1, middle2, middle3;
    private Transform ring1, ring2, ring3;
    private Transform pinky1, pinky2, pinky3;
    private Transform thumb1, thumb2, thumb3;

    public float IndexCurl { get; private set; }
    public float MiddleCurl { get; private set; }
    public float RingCurl { get; private set; }
    public float PinkyCurl { get; private set; }
    public float ThumbCurl { get; private set; }

    public Vector3 positionOffset = Vector3.zero; // tweak in Inspector
    private Vector3 initialPositionOffset;
    private Quaternion initialRotationOffset;


    void Start()
    {
        serialPort = new SerialPort(portName, baudRate);
        try { serialPort.Open(); Debug.Log("Serial opened"); }
        catch { Debug.LogWarning("Serial failed"); }

        AssignBones();

        if (controllerAnchor != null)
        {
            // How the glove is rotated relative to the controller at startup
            initialPositionOffset = Quaternion.Inverse(controllerAnchor.rotation) * (transform.position - controllerAnchor.position);
            initialRotationOffset = Quaternion.Inverse(controllerAnchor.rotation) * transform.rotation;
        }
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

    void LateUpdate()
    {
        if (controllerAnchor != null)
        {
            transform.position = controllerAnchor.position
                + controllerAnchor.rotation * (initialPositionOffset + positionOffset);
            transform.rotation = controllerAnchor.rotation * initialRotationOffset;
        }

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


        IndexCurl = i;
        MiddleCurl = m;
        RingCurl = r;
        PinkyCurl = p;
        ThumbCurl = t;


        // Index
        index1.localRotation = Quaternion.Euler(0, 0, -i* 60f * iMult);
        index2.localRotation = Quaternion.Euler(0, 0, -i* 70f * iMult);
        index3.localRotation = Quaternion.Euler(0, 0, -i* 45f * iMult);

        // Middle
        middle1.localRotation = Quaternion.Euler(0, 0, -m* 60f * mMult);
        middle2.localRotation = Quaternion.Euler(0, 0, -m* 70f * mMult);
        middle3.localRotation = Quaternion.Euler(0, 0, -m* 45f * mMult);

        // Ring
        ring1.localRotation = Quaternion.Euler(0, 0, -r* 60f * rMult);
        ring2.localRotation = Quaternion.Euler(0, 0, -r* 70f * rMult);
        ring3.localRotation = Quaternion.Euler(0, 0, -r* 45f * rMult);

        // Pinky
        pinky1.localRotation = Quaternion.Euler(0, 0, -p* 60f * pMult);
        pinky2.localRotation = Quaternion.Euler(0, 0, -p* 70f * pMult);
        pinky3.localRotation = Quaternion.Euler(0, 0, -p * 45f * pMult);

        // Thumb
        thumb1.localRotation = Quaternion.Euler(0, 0, -t* 40f *2* tMult);
        thumb2.localRotation = Quaternion.Euler(0, 0, -t* 40f *2* tMult);
        thumb3.localRotation = Quaternion.Euler(0, 0, -t* 40f *2* tMult);


    }

void OnApplicationQuit()
    {
        if (serialPort != null && serialPort.IsOpen)
            serialPort.Close();
    }
}
