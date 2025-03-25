
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
using UnityEngine;
using Oculus.Interaction;

public class SendGrabData : MonoBehaviour
{
    private bool _isGrabbed = false;

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
    private SerialGrabSender _serial;
#endif

    void Start()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        _serial = new SerialGrabSender("COM3", 9600);
#endif
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!_isGrabbed)
            {
                Debug.Log("grab_start");
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
                _serial?.Send("grab_start");
#endif
                _isGrabbed = true;
            }
            else
            {
                Debug.Log("grab_end");
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
                _serial?.Send("grab_end");
#endif
                _isGrabbed = false;
            }
        }
    }

    void OnApplicationQuit()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        _serial?.Close();
#endif
    }
}
