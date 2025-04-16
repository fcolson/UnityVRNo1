using UnityEngine;

public class VisualGrabSwap : MonoBehaviour
{
    public GameObject visualObjectToHide;
    public GameObject handGrabbingPosePrefab;
    private GameObject spawnedPoseHand;

    public GameObject animatedHand;

    public Transform handAnchor; //  Assign RightHandAnchor here in Inspector

    public Vector3 poseHandPositionOffset = Vector3.zero;
    public Vector3 poseHandRotationOffset = Vector3.zero;

    public void OnGrab()
    {
        Debug.Log("[VisualGrabSwap] OnGrab triggered");

        // Hide the grabbed object
        if (visualObjectToHide)
        {
            foreach (var renderer in visualObjectToHide.GetComponentsInChildren<Renderer>())
                renderer.enabled = false;
        }

        // Hide the animated hand
        if (animatedHand)
        {
            foreach (var renderer in animatedHand.GetComponentsInChildren<Renderer>())
                renderer.enabled = false;
        }

        // Spawn pose hand under handAnchor
        if (handGrabbingPosePrefab && handAnchor)
        {
            spawnedPoseHand = Instantiate(handGrabbingPosePrefab);

            // Parent first
            spawnedPoseHand.transform.SetParent(handAnchor, false);

            // Reset scale and apply offsets
            spawnedPoseHand.transform.localScale = Vector3.one;
            spawnedPoseHand.transform.localPosition = poseHandPositionOffset;
            spawnedPoseHand.transform.localRotation = Quaternion.Euler(poseHandRotationOffset);
        }
    }

    public void OnRelease()
    {
        Debug.Log("[VisualGrabSwap] OnRelease triggered");

        if (spawnedPoseHand && visualObjectToHide)
        {
            visualObjectToHide.transform.position = spawnedPoseHand.transform.position;
            visualObjectToHide.transform.rotation = spawnedPoseHand.transform.rotation;
        }

        if (visualObjectToHide)
        {
            foreach (var renderer in visualObjectToHide.GetComponentsInChildren<Renderer>())
                renderer.enabled = true;
        }

        if (animatedHand)
        {
            foreach (var renderer in animatedHand.GetComponentsInChildren<Renderer>())
                renderer.enabled = true;
        }

        if (spawnedPoseHand)
        {
            Destroy(spawnedPoseHand);
        }
    }
}
