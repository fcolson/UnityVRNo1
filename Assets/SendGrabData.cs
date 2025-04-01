
/*
// using System.IO.Ports;
using UnityEngine;
using Oculus.Interaction;


public class SendGrabData : MonoBehaviour
{
    private Grabbable _grabbable;
    private bool _isGrabbed = false;

    private void Awake()
    {
        _grabbable = GetComponent<Grabbable>();

        if (_grabbable == null)
        {
            Debug.LogError("No Grabbable component found on this GameObject.");
            return;
        }

        _grabbable.WhenPointerEventRaised += OnPointerEvent;
    }

    private void OnPointerEvent(PointerEvent evt)
    {
        if (evt.Type == PointerEventType.Select && !_isGrabbed)
        {
            Debug.Log("grab_start");
            _isGrabbed = true;
        }
        else if (evt.Type == PointerEventType.Unselect && _isGrabbed)
        {
            Debug.Log("grab_end");
            _isGrabbed = false;
        }
    }
}
*/



/*
using System.IO.Ports;
using UnityEngine;

public class SendSerialOnGrab : MonoBehaviour
{
    public string portName = "COM3"; // Change this to your ESP32's COM port
    public int baudRate = 9600;

    private SerialPort serialPort;
    private bool grabActive = false;

    void Start()
    {
        // Open serial connection
        serialPort = new SerialPort(portName, baudRate);
        try
        {
            serialPort.Open();
            Debug.Log("Serial port opened: " + portName);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Could not open serial port: " + e.Message);
        }
    }

    void Update()
    {
        // Detect left mouse click
        if (Input.GetMouseButtonDown(0)) // left click
        {
            ToggleGrabState();
        }
    }

    void ToggleGrabState()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            if (!grabActive)
            {
                serialPort.WriteLine("grab_start");
                Debug.Log("Sent: grab_start");
            }
            else
            {
                serialPort.WriteLine("grab_end");
                Debug.Log("Sent: grab_end");
            }

            grabActive = !grabActive;
        }
        else
        {
            Debug.LogWarning("Serial port not open.");
        }
    }

    private void OnApplicationQuit()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
}
*/



using System.IO.Ports;
using UnityEngine;
using Oculus.Interaction;

public class SendGrabData : MonoBehaviour
{
    public string portName = "COM3"; // Set to your ESP32 COM port
    public int baudRate = 9600;

    private SerialPort serialPort;
    private Grabbable _grabbable;
    public bool _isGrabbed = false;

    private void Awake()
    {
        _grabbable = GetComponent<Grabbable>();

        if (_grabbable == null)
        {
            Debug.LogError("No Grabbable component found on this GameObject.");
            return;
        }

        _grabbable.WhenPointerEventRaised += OnPointerEvent;

        // Initialize serial connection
        serialPort = new SerialPort(portName, baudRate);
        try
        {
            serialPort.Open();
            Debug.Log("Serial port opened: " + portName);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Could not open serial port: " + e.Message);
        }
    }

    private void OnPointerEvent(PointerEvent evt)
    {
        if (evt.Type == PointerEventType.Select && !_isGrabbed)
        {
            Debug.Log("grab_start");
            SendSerial("grab_start");
            _isGrabbed = true;
        }
        else if (evt.Type == PointerEventType.Unselect && _isGrabbed)
        {
            Debug.Log("grab_end");
            SendSerial("grab_end");
            _isGrabbed = false;
        }
    }

    private void SendSerial(string message)
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.WriteLine(message);
            Debug.Log("Sent: " + message);
        }
        else
        {
            Debug.LogWarning("Serial port not open.");
        }
    }

    private void OnApplicationQuit()
    {
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
}
