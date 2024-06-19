using UnityEngine;
using System.Collections.Generic;

namespace Bhaptics.SDK2
{
    public class CustomHandTracking : MonoBehaviour
    {
        public OVRHand leftHand;  // 왼손에 대한 참조
        public OVRHand rightHand; // 오른손에 대한 참조
        
        private Dictionary<OVRHand.HandFinger, Transform> leftFingerTips;  // 왼손 손가락 끝 Transform 저장
        private Dictionary<OVRHand.HandFinger, Transform> rightFingerTips; // 오른손 손가락 끝 Transform 저장

        void Start()
        {
            // 왼손과 오른손의 손가락 끝 Transform을 초기화
            InitializeFingerTips(leftHand, ref leftFingerTips, "OVRCameraRig/TrackingSpace/LeftHandAnchor/OVRCustomHandPrefab_L/OculusHand_L/b_l_wrist/", "b_l_");
            InitializeFingerTips(rightHand, ref rightFingerTips, "OVRCameraRig/TrackingSpace/RightHandAnchor/OVRCustomHandPrefab_R/OculusHand_R/b_r_wrist/", "b_r_");

            
            // 각 손가락 끝에 Collider 추가
            AddFingerTipColliders(leftFingerTips, "_l");
            AddFingerTipColliders(rightFingerTips, "_r");
        }

        void InitializeFingerTips(OVRHand hand, ref Dictionary<OVRHand.HandFinger, Transform> fingerTips, string handBasePath, string b_hand)
        {
            // 손가락 끝 Transform을 초기화
            fingerTips = new Dictionary<OVRHand.HandFinger, Transform>
            {
                // 각 손가락 끝에 대한 경로를 정확히 설정
                { OVRHand.HandFinger.Thumb, hand.transform.Find(handBasePath + b_hand + "thumb0/" + b_hand + "thumb1/" + b_hand + "thumb2/" + b_hand + "thumb3/" + b_hand + "thumb_null") },
                { OVRHand.HandFinger.Index, hand.transform.Find(handBasePath + b_hand +"index1/" + b_hand + "index2/" + b_hand + "index3/" + b_hand + "index_null") },
                { OVRHand.HandFinger.Middle, hand.transform.Find(handBasePath + b_hand + "middle1/" + b_hand + "middle2/" + b_hand + "middle3/" + b_hand + "middle_null") },
                { OVRHand.HandFinger.Ring, hand.transform.Find(handBasePath + b_hand + "ring1/" + b_hand + "ring2/" + b_hand + "ring3/" + b_hand + "ring_null") },
                { OVRHand.HandFinger.Pinky, hand.transform.Find(handBasePath + b_hand + "pinky0/" + b_hand + "pinky1/" + b_hand + "pinky2/" + b_hand + "pinky3/" + b_hand + "pinky_null") }
            };

            // 각 손가락 끝이 존재하는지 확인하고, 존재하지 않으면 로그로 경고를 출력
            foreach (var fingerTip in fingerTips)
            {
                if (fingerTip.Value == null)
                {
                    Debug.LogWarning($"Finger tip for {fingerTip.Key} not found.");
                }
            }
        }

        void AddFingerTipColliders(Dictionary<OVRHand.HandFinger, Transform> fingerTips, string handSuffix)
        {
            foreach (var fingerTip in fingerTips)
            {
                if (fingerTip.Value != null)
                {
                    // Sphere Collider 추가 및 Trigger로 설정
                    SphereCollider collider = fingerTip.Value.gameObject.AddComponent<SphereCollider>();
                    collider.radius = 0.02f; // 필요한 경우 크기를 조정할 수 있습니다.
                    collider.isTrigger = true;

                    // 충돌 이벤트 감지 스크립트 추가
                    FingerTipCollisionHandler collisionHandler = fingerTip.Value.gameObject.AddComponent<FingerTipCollisionHandler>();
                    collisionHandler.handTracking = this; // CustomHandTracking 스크립트 할당
                    collisionHandler.finger = fingerTip.Key; // 손가락 정보를 전달
                    collisionHandler.handName = handSuffix; // 손 정보를 추가
                }
                else
                {
                    Debug.LogWarning($"Finger tip for {fingerTip.Key} not found.");
                }
            }
        }

        // 충돌 이벤트를 처리하는 메서드
        public void OnFingerTipCollision(OVRHand.HandFinger finger, string handName)
        {
            Debug.Log($"{handName} {finger} tip collided with an object!");

            // 손가락에 따라 진동 패턴을 다르게 적용할 수 있습니다.
            TriggerHapticFeedback(finger, handName);
        }

        // 버튼에서 직접 호출할 수 있는 메서드
        public void TriggerHapticFeedback()
        {
            // 기본 진동 패턴을 실행합니다.
            string hapticPattern = "index_touch_l";
            Debug.Log($"Triggering haptic feedback: Pattern = {hapticPattern}");
            BhapticsLibrary.Play(hapticPattern);
        }

        // 손가락별 진동 패턴을 실행하는 메서드
        public void TriggerHapticFeedback(OVRHand.HandFinger finger, string handSuffix)
        {
            string hapticPattern = "index_touch_r"; // 기본 진동 패턴
            switch (finger)
            {
                case OVRHand.HandFinger.Thumb:
                    hapticPattern = $"thumb_touch{handSuffix}";
                    break;
                case OVRHand.HandFinger.Index:
                    hapticPattern = $"index_touch{handSuffix}";
                    break;
                case OVRHand.HandFinger.Middle:
                    hapticPattern = $"middle_touch{handSuffix}";
                    break;
                case OVRHand.HandFinger.Ring:
                    hapticPattern = $"ring_touch{handSuffix}";
                    break;
                case OVRHand.HandFinger.Pinky:
                    hapticPattern = $"pinky_touch{handSuffix}";
                    break;
            }

            Debug.Log($"Triggering haptic feedback for {handSuffix} {finger}: Pattern = {hapticPattern}");
            BhapticsLibrary.Play(hapticPattern);
        }
    }

    public class FingerTipCollisionHandler : MonoBehaviour
    {
        public CustomHandTracking handTracking; // CustomHandTracking 스크립트를 참조
        public OVRHand.HandFinger finger; // 충돌을 감지할 손가락 정보
        public string handName; // 왼손인지 오른손인지 구분

        void OnTriggerEnter(Collider other)
        {
            // 태그 확인을 제거하고 모든 물체와의 충돌을 처리
            Debug.Log($"{handName} {finger} finger tip collided with {other.gameObject.name}");

            // CustomHandTracking 스크립트에 충돌 정보를 전달
            handTracking.OnFingerTipCollision(finger, handName);
        }
    }
}
