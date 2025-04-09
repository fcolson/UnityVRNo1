using UnityEngine;

public class GloveOVRGrabber : OVRGrabber
{
    public FingerDriver fingerDriver;

    public float grabThreshold = 0.5f;
    public float releaseThreshold = 0.4f;

    public override void Update()
    {

        if (m_grabCandidates.Count > 0)
        {
            foreach (var candidate in m_grabCandidates)
            {
                Debug.Log($"[GloveOVRGrabber] Candidate in range: {candidate.Key.name}");
            }
        }
        else
        {
            Debug.Log("[GloveOVRGrabber] No candidates in range");
        }



        float indexCurl = fingerDriver != null ? fingerDriver.ThumbCurl : 0f;
        float middleCurl = fingerDriver != null ? fingerDriver.MiddleCurl : 0f;

        Debug.Log($"[GloveOVRGrabber] Index: {indexCurl:F2}, Middle: {middleCurl:F2}");

        float gloveGrabValue = Mathf.Max(indexCurl, middleCurl);
        float prevValue = m_prevFlex;
        m_prevFlex = gloveGrabValue;

        if ((m_prevFlex >= grabThreshold) && (prevValue < grabThreshold))
        {
            Debug.Log("[GloveOVRGrabber] Attempting to Grab...");
            GrabBegin();
            Debug.Log("grab_start");
        }
        else if ((m_prevFlex <= releaseThreshold) && (prevValue > releaseThreshold))
        {
            Debug.Log("[GloveOVRGrabber] Releasing...");
            GrabEnd();
            Debug.Log("grab_end");
        }

        /*
        if (m_grabbedObj == null && m_grabCandidates.Count > 0)
        {
            Debug.Log("[GloveOVRGrabber] Forcing grab now!");
            GrabBegin();
            Debug.Log("grab_start");
        }
        */

        base.Update(); // Keep this last to ensure standard grab logic runs
    }

}
