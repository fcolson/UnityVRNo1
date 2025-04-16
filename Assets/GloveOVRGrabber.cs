using UnityEngine;

public class GloveOVRGrabber : OVRGrabber
{
    public FingerDriver fingerDriver;

    [Header("Grab Thresholds")]
    public float grabThreshold = 0.5f;
    public float releaseThreshold = 0.4f;

    public bool IsGrabbing { get; private set; }


    protected override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        float thumbCurl = fingerDriver != null ? fingerDriver.ThumbCurl : 0f;
        float ringCurl = fingerDriver != null ? fingerDriver.RingCurl : 0f;
        float gloveGrabValue = Mathf.Max(thumbCurl, ringCurl);

        bool shouldGrab = gloveGrabValue > grabThreshold;
        bool shouldRelease = gloveGrabValue < releaseThreshold;

        // Debug.Log($"[GloveOVRGrabber] ThumbCurl: {thumbCurl:F2}, RingCurl: {ringCurl:F2}, GrabValue: {gloveGrabValue:F2}, ShouldGrab: {shouldGrab}, ShouldRelease: {shouldRelease}");


        if (!IsGrabbing && shouldGrab && m_grabCandidates.Count > 0)
        {
            GrabBegin();
            IsGrabbing = true;

            if (m_grabbedObj != null)
            {
                var visualSwap = m_grabbedObj.GetComponent<VisualGrabSwap>();
                if (visualSwap != null) visualSwap.OnGrab();
            }

            if (fingerDriver != null)
                fingerDriver.SendSerial("grab_start");
        }
        else if (IsGrabbing && shouldRelease)
        {
            var releasedObj = m_grabbedObj;
            GrabEnd();
            IsGrabbing = false;

            if (releasedObj != null)
            {
                var visualSwap = releasedObj.GetComponent<VisualGrabSwap>();
                if (visualSwap != null) visualSwap.OnRelease();
            }

            if (fingerDriver != null)
                fingerDriver.SendSerial("grab_end");
        }

        // base.Update();
    }

}
