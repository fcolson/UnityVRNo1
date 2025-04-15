using UnityEngine;

public class VRPlayerAligner : MonoBehaviour
{
    public Transform playerRig;     // OVRCameraRig
    public Transform head;          // CenterEyeAnchor or camera
    public Transform targetPoint;   // The desired spawn location

    public float extraHeight = 0.15f; // Extra vertical height in meters


    void Start()
    {
        Vector3 headOffset = head.position - playerRig.position;

        // Reposition rig so head ends up at target point

        Vector3 adjustedTarget = targetPoint.position + Vector3.up * extraHeight;

        playerRig.position = adjustedTarget - headOffset;

        Quaternion rotationOffset = Quaternion.Inverse(head.rotation) * targetPoint.rotation;
        playerRig.rotation = rotationOffset * playerRig.rotation;

    }
}
