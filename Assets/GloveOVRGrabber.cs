using UnityEngine;

public class GloveOVRGrabber : OVRGrabber
{
    public FingerDriver fingerDriver;

    public float grabThreshold = 0.5f;
    public float releaseThreshold = 0.4f;

    private bool isGrabbing = false;

    public override void Update()
    {
        if (m_grabbedObj != null)
        {
            Debug.Log($"[GloveOVRGrabber] Currently holding: {m_grabbedObj.name}");
        }


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



        float thumbCurl = fingerDriver != null ? fingerDriver.ThumbCurl : 0f;
        float ringCurl = fingerDriver != null ? fingerDriver.RingCurl : 0f;

        float gloveGrabValue = Mathf.Min(thumbCurl, ringCurl);
        Debug.Log($"[GloveOVRGrabber] Thumb: {thumbCurl:F2}, Ring: {ringCurl:F2}");
        Debug.Log($"[GloveOVRGrabber] Grip Strength: {gloveGrabValue:F2}");

        bool shouldGrab = gloveGrabValue > grabThreshold;
        bool shouldRelease = gloveGrabValue < releaseThreshold;


        if (!isGrabbing && shouldGrab && m_grabCandidates.Count > 0)
        {
            GrabBegin();
            isGrabbing = true;
            Debug.Log("[GloveOVRGrabber] Grab started");

            if (m_grabbedObj != null)
            {
                var visualSwap = m_grabbedObj.GetComponent<VisualGrabSwap>();
                if (visualSwap != null) visualSwap.OnGrab();
            }
        }
        else if (isGrabbing && shouldRelease)
        {
            Debug.Log("[GloveOVRGrabber] Releasing...");

            var releasedObj = m_grabbedObj;  //  cache it before clearing
            GrabEnd();
            isGrabbing = false;

            if (releasedObj != null)
            {
                var visualSwap = releasedObj.GetComponent<VisualGrabSwap>();
                if (visualSwap != null) visualSwap.OnRelease();
            }

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
