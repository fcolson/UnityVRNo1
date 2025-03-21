using UnityEngine;

public class HandCollisionOVR : MonoBehaviour
{
    [SerializeField] private GameObject _thisHandModel;
    private Transform _originalParent;

    void Start()
    {
        _originalParent = _thisHandModel.transform.parent;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer != LayerMask.NameToLayer("Default"))
            return;

        if (collision.collider.GetComponent<OVRGrabbable>())
            return;

        // Unparent the hand model so it stops following the controller
        _thisHandModel.transform.parent = null;
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.collider.gameObject.layer != LayerMask.NameToLayer("Default"))
            return;

        // Reparent the hand back to the controller
        _thisHandModel.transform.SetParent(_originalParent);
        _thisHandModel.transform.localPosition = Vector3.zero;
        _thisHandModel.transform.localRotation = Quaternion.identity;
    }
}
