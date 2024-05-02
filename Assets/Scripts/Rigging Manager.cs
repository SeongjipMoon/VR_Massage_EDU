using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiggingManager : MonoBehaviour
{
    public Transform leftHandIK;
    public Transform rightHandIK;
    public Transform headIK;

    public Transform leftHandController;
    public Transform rightHandController;
    public Transform hmd; // 메인카메라 위치 잡는 곳

    public Vector3[] leftOffset; // 0 : Position, 1 : Rotation
    public Vector3[] rightOffset;
    public Vector3[] headOffset;

    public float smoothValue = 0.1f;
    public float modelHeight = 1.67f;

    void LateUpdate() 
    {
        MappingHandTransform(leftHandIK, leftHandController, true);
        MappingHandTransform(rightHandIK, rightHandController, false);
        MappingBodyTransform(headIK, hmd);
        MappingHeadTransform(headIK, hmd);
    }

    private void MappingHandTransform(Transform ik, Transform controller, bool isLeft)
    {
        // ik의 Transform = controller의 Transform

        var offset = isLeft ? leftOffset : rightOffset;

        ik.position = controller.TransformPoint(offset[0]);
        ik.rotation = controller.rotation * Quaternion.Euler(offset[1]);
    }

    private void MappingBodyTransform(Transform ik, Transform hmd)
    {
        this.transform.position = new Vector3(hmd.position.x, hmd.position.y - modelHeight, hmd.position.z);
        float yaw = hmd.eulerAngles.y; // y축 기준으로 좌우 Unity에서 Quarternion과 eularAngle
        var targetRotation = new Vector3(this.transform.eulerAngles.x, yaw, this.transform.eulerAngles.z);
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.Euler(targetRotation), smoothValue);
    }

    private void MappingHeadTransform(Transform ik, Transform hmd)
    {
        ik.position = hmd.TransformPoint(headOffset[0]);
        ik.rotation = hmd.rotation * Quaternion.Euler(headOffset[1]);
    }
}
