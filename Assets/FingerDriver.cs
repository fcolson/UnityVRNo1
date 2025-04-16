using System.IO.Ports;
using System.Linq;
using UnityEngine;

public class FingerDriver : MonoBehaviour
{
    public Transform controllerAnchor;
    public string portName = "COM5";
    public int baudRate = 115200;

    private SerialPort serialPort;
    public OVRCustomSkeleton skeleton;

    public float tMult = 2.0f;
    public float iMult = 1.0f;
    public float mMult = 2.2f;
    public float rMult = 4.0f;
    public float pMult = 4.0f;

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

    public Vector3 positionOffset = Vector3.zero;
    private Vector3 initialPositionOffset;
    private Quaternion initialRotationOffset;

    public bool simulateInput = true; // Toggle in Inspector

    void Start()
    {
        serialPort = new SerialPort(portName, baudRate);
        try { serialPort.Open(); Debug.Log("Serial opened"); }
        catch { Debug.LogWarning("Serial failed"); }

        StartCoroutine(WaitForBones());

        if (controllerAnchor != null)
        {
            initialPositionOffset = Quaternion.Inverse(controllerAnchor.rotation) * (transform.position - controllerAnchor.position);
            initialRotationOffset = Quaternion.Inverse(controllerAnchor.rotation) * transform.rotation;
        }
    }

    System.Collections.IEnumerator WaitForBones()
    {
        while (skeleton.Bones == null || skeleton.Bones.Count == 0)
        {
            Debug.Log("Waiting for skeleton bones...");
            yield return null;
        }

        Debug.Log("Skeleton bones ready. Assigning...");
        AssignBones();

        if (!thumb1 || !thumb2 || !thumb3)
            Debug.LogWarning("One or more thumb bones were not found.");
        else
            Debug.Log("Thumb bones assigned.");
    }

    void AssignBones()
    {
        index1 = Get("XRHand_IndexProximal");
        index2 = Get("XRHand_IndexIntermediate");
        index3 = Get("XRHand_IndexDistal");

        middle1 = Get("XRHand_MiddleProximal");
        middle2 = Get("XRHand_MiddleIntermediate");
        middle3 = Get("XRHand_MiddleDistal");

        ring1 = Get("XRHand_RingProximal");
        ring2 = Get("XRHand_RingIntermediate");
        ring3 = Get("XRHand_RingDistal");

        pinky1 = Get("XRHand_LittleProximal");
        pinky2 = Get("XRHand_LittleIntermediate");
        pinky3 = Get("XRHand_LittleDistal");

        thumb1 = Get("XRHand_ThumbMetacarpal");
        thumb2 = Get("XRHand_ThumbProximal");
        thumb3 = Get("XRHand_ThumbDistal");
    }

    Transform Get(string name)
    {
        return skeleton.Bones.FirstOrDefault(b => b.Transform.name == name)?.Transform;
    }

    void LateUpdate()
    {
        if (controllerAnchor != null)
        {
            transform.position = controllerAnchor.position + controllerAnchor.rotation * (initialPositionOffset + positionOffset);
            transform.rotation = controllerAnchor.rotation * initialRotationOffset;
        }

        if (simulateInput)
        {
            string simulatedLine = SimulateSerialFromKeys();
            if (!string.IsNullOrWhiteSpace(simulatedLine))
            {
                ApplyFingerRotations(simulatedLine);
            }
        }
        else if (serialPort != null && serialPort.IsOpen)
        {
            try
            {
                string line = serialPort.ReadLine();
                if (!string.IsNullOrWhiteSpace(line) && line.Contains("A") && line.Contains("B") && line.Contains("C"))
                {
                    ApplyFingerRotations(line);
                }
                else
                {
                    Debug.LogWarning("[FingerDriver] Ignored invalid line: " + line);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("Serial read failed: " + ex.Message);
            }
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

        float t = Parse("A");
        float i = Parse("B");
        float m = Parse("C");
        float r = Parse("D");
        float p = Parse("E");

        IndexCurl = i;
        MiddleCurl = m;
        RingCurl = r;
        PinkyCurl = p;
        ThumbCurl = t;

        index1.localRotation = Quaternion.Euler(i * 60f * iMult, 0, 0);
        index2.localRotation = Quaternion.Euler(i * 70f * iMult, 0, 0);
        index3.localRotation = Quaternion.Euler(i * 45f * iMult, 0, 0);

        middle1.localRotation = Quaternion.Euler(m * 60f * mMult, 0, 0);
        middle2.localRotation = Quaternion.Euler(m * 70f * mMult, 0, 0);
        middle3.localRotation = Quaternion.Euler(m * 45f * mMult, 0, 0);

        ring1.localRotation = Quaternion.Euler(r * 60f * rMult, 0, 0);
        ring2.localRotation = Quaternion.Euler(r * 70f * rMult, 0, 0);
        ring3.localRotation = Quaternion.Euler(r * 45f * rMult, 0, 0);

        pinky1.localRotation = Quaternion.Euler(p * 60f * pMult, 0, 0);
        pinky2.localRotation = Quaternion.Euler(p * 70f * pMult, 0, 0);
        pinky3.localRotation = Quaternion.Euler(p * 45f * pMult, 0, 0);

        // thumb1.localRotation = Quaternion.Euler(t * 50f * tMult, -t * 30f * tMult, 0);
        thumb2.localRotation = Quaternion.Euler(t * 50f * tMult, -t * 10f * tMult, 0);
        thumb3.localRotation = Quaternion.Euler(t * 40f * tMult, 0, 0);
    }

    private string SimulateSerialFromKeys()
    {
        string line = "";

        line += Input.GetKey(KeyCode.Alpha1) ? "A4095" : "A0";
        line += Input.GetKey(KeyCode.Alpha2) ? "B4095" : "B0";
        line += Input.GetKey(KeyCode.Alpha3) ? "C4095" : "C0";
        line += Input.GetKey(KeyCode.Alpha4) ? "D4095" : "D0";
        line += Input.GetKey(KeyCode.Alpha5) ? "E4095" : "E0";

        return line;
    }

    void OnApplicationQuit()
    {
        if (serialPort != null && serialPort.IsOpen)
            serialPort.Close();
    }

    public void SendSerial(string message)
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.WriteLine(message);
            Debug.Log("[FingerDriver] Sent: " + message);
        }
    }
}