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
