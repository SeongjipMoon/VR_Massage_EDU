using UnityEngine;
using UnityEngine.EventSystems;

public class SetRayTransformAtRuntime : MonoBehaviour
{
    public OVRHand leftHand;
    public OVRHand rightHand;
    public OVRInputModule ovrInputModule;

    void Start()
    {
        // 왼손과 오른손의 IndexTip을 찾아 설정
        Transform leftIndexTip = FindFingerTip(leftHand, OVRSkeleton.BoneId.Hand_IndexTip);
        Transform rightIndexTip = FindFingerTip(rightHand, OVRSkeleton.BoneId.Hand_IndexTip);

        // OVRInputModule의 Ray Transform을 설정
        if (leftIndexTip != null)
        {
            ovrInputModule.rayTransform = leftIndexTip;
            Debug.Log("Left IndexTip set as Ray Transform.");
        }
        else if (rightIndexTip != null)
        {
            ovrInputModule.rayTransform = rightIndexTip;
            Debug.Log("Right IndexTip set as Ray Transform.");
        }
        else
        {
            Debug.LogWarning("IndexTip not found for either hand.");
        }
    }

    private Transform FindFingerTip(OVRHand hand, OVRSkeleton.BoneId boneId)
    {
        if (hand == null)
        {
            Debug.LogWarning("Hand is not assigned.");
            return null;
        }

        OVRSkeleton skeleton = hand.GetComponent<OVRSkeleton>();
        if (skeleton == null)
        {
            Debug.LogWarning("OVRSkeleton component is missing.");
            return null;
        }

        foreach (var bone in skeleton.Bones)
        {
            if (bone.Id == boneId)
            {
                return bone.Transform;
            }
        }

        Debug.LogWarning($"Finger tip for bone ID {boneId} not found.");
        return null;
    }
}
