using UnityEngine;

public class AdjustCameraRig : MonoBehaviour
{
    public GameObject xrOrigin;  // XR Origin (XR Rig) 게임 오브젝트
    public string mainCameraPath = "Camera Offset/Main Camera";  // XR Origin 내의 Main Camera 경로

    void Start()
    {
        if (xrOrigin == null)
        {
            Debug.LogError("XR Origin not assigned. Please assign it manually.");
            return;
        }

        Transform mainCameraTransform = xrOrigin.transform.Find(mainCameraPath);

        if (mainCameraTransform != null)
        {
            // Main Camera의 위치 가져오기
            Vector3 mainCameraPosition = mainCameraTransform.position;
            Quaternion mainCameraRotation = mainCameraTransform.rotation;

            // OVRCameraRig의 위치 설정
            Transform cameraRigTransform = GetComponent<Transform>();
            cameraRigTransform.position = mainCameraPosition;
            cameraRigTransform.rotation = mainCameraRotation;
        }
        else
        {
            Debug.LogError("Main Camera not found in XR Origin. Please ensure the correct path is set.");
        }
    }
}
