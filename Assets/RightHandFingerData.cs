using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.Input;
using System.Collections.Generic;

public class RightHandFingerData_OVR : MonoBehaviour
{
    [SerializeField] private OVRHand rightHand;  // Assign from Inspector
    [SerializeField] private OVRSkeleton rightHandSkeleton;  // Assign from Inspector

    private Dictionary<OVRSkeleton.BoneId, Transform> _boneTransforms = new Dictionary<OVRSkeleton.BoneId, Transform>();

    void Start()
    {
        if (rightHandSkeleton == null)
        {
            Debug.LogError("RightHandSkeleton not assigned!");
            return;
        }

        // Cache bone transforms for quick access
        foreach (var bone in rightHandSkeleton.Bones)
        {
            if (bone != null && bone.Transform != null)
            {
                _boneTransforms[bone.Id] = bone.Transform;
            }
        }
    }

    void Update()
    {
        if (rightHand == null || rightHandSkeleton == null || !rightHand.IsTracked)
            return;

        // Get per-finger curl (based on joint angles)
        float thumbCurl = GetFingerCurl(OVRSkeleton.BoneId.Hand_Thumb2, OVRSkeleton.BoneId.Hand_Thumb3);
        float indexCurl = GetFingerCurl(OVRSkeleton.BoneId.Hand_Index1, OVRSkeleton.BoneId.Hand_Index3);
        float middleCurl = GetFingerCurl(OVRSkeleton.BoneId.Hand_Middle1, OVRSkeleton.BoneId.Hand_Middle3);
        float ringCurl = GetFingerCurl(OVRSkeleton.BoneId.Hand_Ring1, OVRSkeleton.BoneId.Hand_Ring3);
        float pinkyCurl = GetFingerCurl(OVRSkeleton.BoneId.Hand_Pinky1, OVRSkeleton.BoneId.Hand_Pinky3);

        Debug.Log($"[Curl] Thumb:{thumbCurl:F2} | Index:{indexCurl:F2} | Middle:{middleCurl:F2} | Ring:{ringCurl:F2} | Pinky:{pinkyCurl:F2}");
    }

    // Estimate finger curl based on the angle difference between the base and tip joints
    private float GetFingerCurl(OVRSkeleton.BoneId baseJoint, OVRSkeleton.BoneId tipJoint)
    {
        if (!_boneTransforms.ContainsKey(baseJoint) || !_boneTransforms.ContainsKey(tipJoint))
            return 0f;

        Transform baseTransform = _boneTransforms[baseJoint];
        Transform tipTransform = _boneTransforms[tipJoint];

        float angle = Quaternion.Angle(baseTransform.rotation, tipTransform.rotation);
        return Mathf.Clamp(angle / 90f, 0f, 1f); // Normalize to 0-1 range
    }
}
