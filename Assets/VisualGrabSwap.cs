using UnityEngine;

public class VisualGrabSwap : MonoBehaviour
{
    public GameObject visualObjectToHide; // e.g. the mesh/renderer object
    public GameObject handGrabbingPosePrefab; // drag your FBX prefab here
    private GameObject spawnedPoseHand; // internal instance

    public GameObject animatedHand;       // the normal glove model

    public Transform handAnchor;

    public void OnGrab()
    {
        Debug.Log("[VisualGrabSwap] OnGrab triggered");

        // Disable mesh renderers of the object to hide
        if (visualObjectToHide)
        {
            foreach (var renderer in visualObjectToHide.GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = false;
            }
        }

        // Spawn and parent pose hand with adjusted rotation
        if (handGrabbingPosePrefab && handAnchor)
        {
            // Define the additional rotation (e.g., 90 degrees around the Y-axis)
            Quaternion additionalRotation = Quaternion.Euler(0, -90, 90);

            // Combine the handAnchor's rotation with the additional rotation
            Quaternion finalRotation = handAnchor.rotation * additionalRotation;

            // Instantiate the pose hand with the final rotation
            spawnedPoseHand = Instantiate(handGrabbingPosePrefab, handAnchor.position, finalRotation);
            spawnedPoseHand.transform.SetParent(handAnchor);
        }

        // Disable the animated hand's renderers
        if (animatedHand)
        {
            foreach (var renderer in animatedHand.GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = false;
            }
        }
    }




    public void OnRelease()
    {
        Debug.Log("[VisualGrabSwap] OnRelease triggered");

        // Move the object to the pose hand's current position/rotation
        if (spawnedPoseHand && visualObjectToHide)
        {
            visualObjectToHide.transform.position = spawnedPoseHand.transform.position;
            visualObjectToHide.transform.rotation = spawnedPoseHand.transform.rotation;
        }

        // Re-enable the original object
        if (visualObjectToHide)
        {
            foreach (var renderer in visualObjectToHide.GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = true;
            }
        }

        //  Re-enable animated hand visuals
        if (animatedHand)
        {
            foreach (var renderer in animatedHand.GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = true;
            }
        }

        // Remove pose hand
        if (spawnedPoseHand) Destroy(spawnedPoseHand);
    }





}
