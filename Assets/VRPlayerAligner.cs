using UnityEngine;
using System.Collections;

public class VRPlayerAligner : MonoBehaviour
{
    public Transform playerRig;     // OVRCameraRig
    public Transform head;          // CenterEyeAnchor
    public Transform targetPoint;   // Where the head should end up
    public float extraHeight = 0.15f;

    IEnumerator Start()
    {
        Debug.Log("[VRPlayerAligner] Waiting for head tracking to stabilize...");
        yield return new WaitForSeconds(1.0f); // allow tracking to stabilize

        Debug.Log("[VRPlayerAligner] Head tracking stabilized.");

        // STEP 1: Apply Y-axis rotation FIRST
        float yawDelta = targetPoint.eulerAngles.y - head.eulerAngles.y;
        playerRig.Rotate(Vector3.up, yawDelta);

        // STEP 2: Now compute and apply position
        Vector3 headOffset = head.position - playerRig.position;
        Vector3 targetHeadPosition = targetPoint.position + Vector3.up * extraHeight;
        playerRig.position = targetHeadPosition - headOffset;

        Debug.Log("[VRPlayerAligner] FINAL ALIGNMENT COMPLETE");
        Debug.Log("Target Head Pos: " + targetHeadPosition);
        Debug.Log("Final Head Pos: " + head.position);
    }
}
