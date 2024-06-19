using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiggingManager : MonoBehaviour
{
    public Transform leftHandIK;
    public Transform rightHandIK;
    public Transform headIK;

    public OVRHand leftHand;
    public OVRHand rightHand;
    public OVRSkeleton leftHandSkeleton;
    public OVRSkeleton rightHandSkeleton;
    public Transform hmd; // 메인카메라 위치 잡는 곳

    public Vector3[] leftOffset; // 0 : Position, 1 : Rotation
    public Vector3[] rightOffset;
    public Vector3[] headOffset;

    public float smoothValue = 0.1f;
    public float modelHeight = 1.67f;

    private Transform[] bananaManLeftFingers;  // 수정된 부분: 배열 필드 정의
    private Transform[] bananaManRightFingers; // 수정된 부분: 배열 필드 정의

    void Start()
    {
        // 손가락 트랜스폼 설정
        SetFingerTransforms();
        
        // 손 트랜스폼 초기화 디버깅
        if (leftHandSkeleton != null)
        {
            Debug.Log($"Left hand skeleton bones count in Start: {leftHandSkeleton.Bones.Count}");
        }
        else
        {
            Debug.LogWarning("Left hand skeleton is not assigned.");
        }

        if (rightHandSkeleton != null)
        {
            Debug.Log($"Right hand skeleton bones count in Start: {rightHandSkeleton.Bones.Count}");
        }
        else
        {
            Debug.LogWarning("Right hand skeleton is not assigned.");
        }

        // 손 IK 트랜스폼 초기화 디버깅
        if (leftHandIK != null)
        {
            Debug.Log($"Left hand IK is assigned.");
        }
        else
        {
            Debug.LogWarning("Left hand IK is not assigned.");
        }

        if (rightHandIK != null)
        {
            Debug.Log($"Right hand IK is assigned.");
        }
        else
        {
            Debug.LogWarning("Right hand IK is not assigned.");
        }
        
    }

    void LateUpdate()
    {
        // 손가락 트래킹 업데이트
        if (leftHand != null && leftHandSkeleton != null && leftHandIK != null)
        {
            //Debug.Log("Updating left hand tracking...");
            UpdateHandTracking(leftHand, leftHandSkeleton, leftHandIK, leftOffset);
            Debug.Log($"Left Hand IK Position: {leftHandIK.position}, Rotation: {leftHandIK.rotation}");
            //UpdateFingerTracking(leftHandSkeleton, bananaManLeftFingers); // 왼손 손가락 업데이트
        }
        else
        {
            Debug.LogWarning("Left hand tracking components are missing.");
        }

        if (rightHand != null && rightHandSkeleton != null && rightHandIK != null)
        {
            //Debug.Log("Updating right hand tracking...");
            UpdateHandTracking(rightHand, rightHandSkeleton, rightHandIK, rightOffset);
            Debug.Log($"Right Hand IK Position: {rightHandIK.position}, Rotation: {rightHandIK.rotation}");
            //UpdateFingerTracking(rightHandSkeleton, bananaManRightFingers); // 오른손 손가락 업데이트
        }
        else
        {
            Debug.LogWarning("Right hand tracking components are missing.");
        }

        MappingBodyTransform(headIK, hmd);
        MappingHeadTransform(headIK, hmd);
        //AdjustCameraRigPosition();
    }

    private void SetFingerTransforms()
    {
        // 손가락 트랜스폼 경로 업데이트
        string leftHandPath = "Player/Armature/Hips/Spine 1/Spine 2/Spine 3/Left Shoulder/Left Arm/Left Forearm/Left Hand/";
        string rightHandPath = "Player/Armature/Hips/Spine 1/Spine 2/Spine 3/Right Shoulder/Right Arm/Right Forearm/Right Hand/";

        // Left Hand Fingers
        bananaManLeftFingers = new Transform[]
        {
            FindTransform(leftHandPath + "Left Hand Thumb 1"),
            FindTransform(leftHandPath + "Left Hand Thumb 1/Left Hand Thumb 2"),
            FindTransform(leftHandPath + "Left Hand Thumb 1/Left Hand Thumb 2/Left Hand Thumb 3"),

            FindTransform(leftHandPath + "Left Hand Index 1"),
            FindTransform(leftHandPath + "Left Hand Index 1/Left Hand Index 2"),
            FindTransform(leftHandPath + "Left Hand Index 1/Left Hand Index 2/Left Hand Index 3"),

            FindTransform(leftHandPath + "Left Hand Middle 1"),
            FindTransform(leftHandPath + "Left Hand Middle 1/Left Hand Middle 2"),
            FindTransform(leftHandPath + "Left Hand Middle 1/Left Hand Middle 2/Left Hand Middle 3"),

            FindTransform(leftHandPath + "Left Hand Ring 1"),
            FindTransform(leftHandPath + "Left Hand Ring 1/Left Hand Ring 2"),
            FindTransform(leftHandPath + "Left Hand Ring 1/Left Hand Ring 2/Left Hand Ring 3"),

            FindTransform(leftHandPath + "Left Hand Pinky 1"),
            FindTransform(leftHandPath + "Left Hand Pinky 1/Left Hand Pinky 2"),
            FindTransform(leftHandPath + "Left Hand Pinky 1/Left Hand Pinky 2/Left Hand Pinky 3"),
        };

        // Right Hand Fingers
        bananaManRightFingers = new Transform[]
        {
            FindTransform(rightHandPath + "Right Hand Thumb 1"),
            FindTransform(rightHandPath + "Right Hand Thumb 1/Right Hand Thumb 2"),
            FindTransform(rightHandPath + "Right Hand Thumb 1/Right Hand Thumb 2/Right Hand Thumb 3"),

            FindTransform(rightHandPath + "Right Hand Index 1"),
            FindTransform(rightHandPath + "Right Hand Index 1/Right Hand Index 2"),
            FindTransform(rightHandPath + "Right Hand Index 1/Right Hand Index 2/Right Hand Index 3"),

            FindTransform(rightHandPath + "Right Hand Middle 1"),
            FindTransform(rightHandPath + "Right Hand Middle 1/Right Hand Middle 2"),
            FindTransform(rightHandPath + "Right Hand Middle 1/Right Hand Middle 2/Right Hand Middle 3"),

            FindTransform(rightHandPath + "Right Hand Ring 1"),
            FindTransform(rightHandPath + "Right Hand Ring 1/Right Hand Ring 2"),
            FindTransform(rightHandPath + "Right Hand Ring 1/Right Hand Ring 2/Right Hand Ring 3"),

            FindTransform(rightHandPath + "Right Hand Pinky 1"),
            FindTransform(rightHandPath + "Right Hand Pinky 1/Right Hand Pinky 2"),
            FindTransform(rightHandPath + "Right Hand Pinky 1/Right Hand Pinky 2/Right Hand Pinky 3"),
        };

        // Debugging: Check if all transforms are assigned correctly
        foreach (var transform in bananaManLeftFingers)
        {
            if (transform == null)
            {
                Debug.LogError("One of the left hand finger transforms is not assigned correctly.");
            }
        }
        foreach (var transform in bananaManRightFingers)
        {
            if (transform == null)
            {
                Debug.LogError("One of the right hand finger transforms is not assigned correctly.");
            }
        }
    }

    private Transform FindTransform(string path)
    {
        Transform transform = GameObject.Find(path)?.transform;
        if (transform == null)
        {
            Debug.LogError($"Transform not found at path: {path}");
        }
        return transform;
    }
    
    private void UpdateHandTracking(OVRHand hand, OVRSkeleton handSkeleton, Transform handIK, Vector3[] offsets)
    {
        if (hand == null || handSkeleton == null || handIK == null)
        {
            Debug.LogWarning("Hand tracking component is missing");
            return;
        }

        // VR 손의 위치와 회전을 handIK에 매핑
        handIK.position = hand.transform.position + offsets[0];
        handIK.rotation = hand.transform.rotation * Quaternion.Euler(offsets[1]);

        // Debugging: Check the position and rotation of the handIK transform
        //Debug.Log($"Hand IK Position: {handIK.position}, Rotation: {handIK.rotation}");
    }
    /*
    private void UpdateFingerTracking(OVRSkeleton handSkeleton, Transform[] bananaManFingers)
    {
        if (handSkeleton == null || bananaManFingers == null)
        {
            Debug.LogWarning("BananaMan hand tracking component is missing");
            return;
        }

        var bones = handSkeleton.Bones;

        // BananaMan의 손의 뼈 구조를 VR 손의 뼈 구조에 맞게 매핑
        for (int i = 0; i < bones.Count && i < bananaManFingers.Length; i++)
        {
            Transform boneTransform = bones[i].Transform;
            Transform bananaFingerTransform = bananaManFingers[i];

            if (bananaFingerTransform != null)
            {
                bananaFingerTransform.position = boneTransform.position;
                bananaFingerTransform.rotation = boneTransform.rotation;

                // Debugging: Check the position and rotation of each bone transform
                Debug.Log($"BananaMan Finger {i}: Position - {bananaFingerTransform.position}, Rotation - {bananaFingerTransform.rotation}");
            }
        }
    }
    */

    private void MappingBodyTransform(Transform ik, Transform hmd)
    {
        this.transform.position = new Vector3(hmd.position.x, hmd.position.y - modelHeight, hmd.position.z);
        float yaw = hmd.eulerAngles.y; // y축 기준으로 좌우 Unity에서 Quarternion과 eularAngle
        var targetRotation = new Vector3(this.transform.eulerAngles.x, yaw, this.transform.eulerAngles.z);
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.Euler(targetRotation), smoothValue);
    }

    private void MappingHeadTransform(Transform ik, Transform hmd)
    {
        if (hmd == null || headIK == null)
        {
        Debug.LogWarning("HMD or headIK transform is missing");
        return;
        }

        ik.position = hmd.TransformPoint(headOffset[0]);
        ik.rotation = hmd.rotation * Quaternion.Euler(headOffset[1]);
    }
}

/*
LateUpdate안에 들어있던 것

        // HMD의 위치 로그 출력
        if (hmd != null)
        {
            Debug.Log($"HMD Position: {hmd.position}, Rotation: {hmd.rotation}");
        }
        else
        {
            Debug.LogWarning("HMD transform is not assigned.");
        }
        */
        /*
        if (leftHand != null && leftHandSkeleton != null && leftHandIK != null)
        {
            UpdateHandTracking(leftHand, leftHandSkeleton, leftHandIK, leftOffset);
        }
        else
        {
            Debug.LogWarning("Left hand components are missing.");
        }

        if (rightHand != null && rightHandSkeleton != null && rightHandIK != null)
        {
            UpdateHandTracking(rightHand, rightHandSkeleton, rightHandIK, rightOffset);
        }
        else
        {
            Debug.LogWarning("Right hand components are missing.");
        }

    이건 그냥 밖에
    /*
    private void MappingBodyTransform()
    {
        if (hmd == null)
        {
            Debug.LogWarning("HMD transform is missing");
            return;
        }

        // OVRManager를 통해 트래킹된 HMD 위치 가져오기
        Vector3 hmdPosition = OVRManager.instance.transform.position;
        hmdPosition.y -= modelHeight;  // HMD 높이를 고려하여 위치 조정
        this.transform.position = Vector3.Lerp(this.transform.position, hmdPosition, smoothValue);

        float yaw = hmd.eulerAngles.y;  // HMD의 Yaw 값을 가져옴
        Vector3 targetRotation = new Vector3(this.transform.eulerAngles.x, yaw, this.transform.eulerAngles.z);
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.Euler(targetRotation), smoothValue);
    }

    private void MappingHeadTransform()
    {
        if (hmd == null || headIK == null)
        {
            Debug.LogWarning("HMD or headIK transform is missing");
            return;
        }

        // OVRManager를 통해 트래킹된 HMD 위치 사용
        Vector3 hmdPosition = OVRManager.instance.transform.position;
        headIK.position = hmdPosition + headOffset[0];
        headIK.rotation = OVRManager.instance.transform.rotation * Quaternion.Euler(headOffset[1]);
    }
    */

    /*
    private void AdjustCameraRigPosition()
    {
        // OVRCameraRig의 Y 위치를 조정하여 높이를 맞춥니다.
        if (OVRManager.instance != null)
        {
            Transform cameraRigTransform = OVRManager.instance.transform;

            // HMD의 위치를 가져옴
            Vector3 hmdPosition = hmd.position;
            
            // 원하는 Y 위치로 설정 (예: -0.5만큼 낮추기)
            float yOffset = -1.0f;
            Vector3 adjustedPosition = new Vector3(hmdPosition.x, hmdPosition.y + yOffset, hmdPosition.z);
            
            // OVRCameraRig의 위치를 조정
            cameraRigTransform.position = adjustedPosition;
            cameraRigTransform.rotation = hmd.rotation;
        }
    }
    
    UpdateHandTracking에 있던 부분
    
        var bones = handSkeleton.Bones;

        // Debugging: Check the number of bones and children
        Debug.Log($"Number of bones in handSkeleton: {bones.Count}");
        Debug.Log($"Number of children in handIK: {handIK.childCount}");

        // 손 트랜스폼 업데이트
        for (int i = 0; i < bones.Count && i < handIK.childCount; i++)
        {
            Transform boneTransform = bones[i].Transform;
            Transform fingerTransform = handIK.GetChild(i);
            
            // 손가락 IK의 위치와 회전을 업데이트합니다.
            if (fingerTransform != null)
            {
                fingerTransform.position = boneTransform.position + offsets[0];
                fingerTransform.rotation = boneTransform.rotation * Quaternion.Euler(offsets[1]);

                // Debugging: Check the position and rotation of each finger transform

                Debug.Log($"Finger {i}: Position - {fingerTransform.position}, Rotation - {fingerTransform.rotation}");
            }
            else
            {
                Debug.LogError($"Finger transform at index {i} is not assigned.");
            }
        }
*/
