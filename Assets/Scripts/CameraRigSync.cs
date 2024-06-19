using UnityEngine;

public class CameraRigSync : MonoBehaviour
{
    public Transform ovrCameraRig;
    public Transform xrOrigin;

    void Update()
    {
        // OVRCameraRig의 위치와 회전을 XR Origin에 동기화
        if (ovrCameraRig != null && xrOrigin != null)
        {
            xrOrigin.position = ovrCameraRig.position;
            xrOrigin.rotation = ovrCameraRig.rotation;
        }
    }
}
