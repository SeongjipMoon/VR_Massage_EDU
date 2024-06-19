using UnityEngine;
using System.Collections.Generic;

namespace Bhaptics.SDK2
{
    public class CustomHandTracking_R : MonoBehaviour
    {
        public OVRHand ovrHand; // OVRHand 객체를 참조
        public float pinchThreshold = 0.7f; // 핀치 강도 임계값 (0.7 이상이면 핀치로 간주)
        private bool isGrabbing = false; // 현재 Grab 동작 여부를 추적

        private Dictionary<OVRHand.HandFinger, float> nextLogTimes; // 로그 출력 간격 관리
        private float logInterval = 1.0f; // 로그 출력 간격 (초)

        void Start()
        {
            // 각 손가락의 다음 로그 출력 시간을 초기화
            nextLogTimes = new Dictionary<OVRHand.HandFinger, float>
            {
                { OVRHand.HandFinger.Thumb, 0f },
                { OVRHand.HandFinger.Index, 0f },
                { OVRHand.HandFinger.Middle, 0f },
                { OVRHand.HandFinger.Ring, 0f },
                { OVRHand.HandFinger.Pinky, 0f }
            };
        }

        void Update()
        {
            // Grab 동작을 먼저 감지
            CheckGrabGesture();

            // Grab 동작이 아닌 경우에만 핀치 동작을 감지
            if (!isGrabbing)
            {
                CheckFingerPinch(OVRHand.HandFinger.Thumb, "thumb_touch_r");
                CheckFingerPinch(OVRHand.HandFinger.Index, "index_touch_r");
                CheckFingerPinch(OVRHand.HandFinger.Middle, "middle_touch_r");
                CheckFingerPinch(OVRHand.HandFinger.Ring, "ring_touch_r");
                CheckFingerPinch(OVRHand.HandFinger.Pinky, "pinky_touch_r");
            }
        }

        private void CheckFingerPinch(OVRHand.HandFinger finger, string hapticPattern)
        {
            // 현재 Grab 상태에서는 핀치 동작을 무시
            if (isGrabbing) return;

            // 해당 손가락이 핀치 상태인지 확인
            if (ovrHand.GetFingerIsPinching(finger))
            {
                // 로그 출력 간격에 따라 진동을 트리거하고 로그 출력
                if (Time.time >= nextLogTimes[finger])
                {
                    //Debug.Log($"{finger} finger is pinching. Triggering haptic feedback with pattern: {hapticPattern}");

                    // 진동을 트리거
                    TriggerHapticFeedback(hapticPattern);

                    // 다음 로그 출력 시간 갱신
                    nextLogTimes[finger] = Time.time + logInterval;
                }
            }
        }

        private void CheckGrabGesture()
        {
            // 모든 손가락이 특정 강도 이상으로 구부러졌는지 확인
            bool thumbPinching = ovrHand.GetFingerPinchStrength(OVRHand.HandFinger.Thumb) > pinchThreshold;
            bool indexPinching = ovrHand.GetFingerPinchStrength(OVRHand.HandFinger.Index) > pinchThreshold;
            bool middlePinching = ovrHand.GetFingerPinchStrength(OVRHand.HandFinger.Middle) > pinchThreshold;
            bool ringPinching = ovrHand.GetFingerPinchStrength(OVRHand.HandFinger.Ring) > pinchThreshold;
            bool pinkyPinching = ovrHand.GetFingerPinchStrength(OVRHand.HandFinger.Pinky) > pinchThreshold;

            // 모든 손가락이 구부러졌다면 Grab 동작으로 간주
            bool isCurrentGrabbing = thumbPinching && indexPinching && middlePinching && ringPinching && pinkyPinching;

            if (isCurrentGrabbing && !isGrabbing)
            {
                // Grab 시작
                //Debug.Log("Grab gesture detected. Triggering haptic feedback for grabbing.");
                TriggerHapticFeedback("grab_r");
                isGrabbing = true;
            }
            else if (!isCurrentGrabbing && isGrabbing)
            {
                // Grab 끝남
                isGrabbing = false;
            }
        }

        // 물체와 충돌하여 진동을 발생시키는 메서드
        private void TriggerHapticFeedback(string pattern)
        {
            // Bhaptics 패턴을 사용하여 진동 트리거
            BhapticsLibrary.Play(pattern);
            Debug.Log($"Haptic feedback triggered with pattern: {pattern}");
        }

        // 버튼에서 직접 호출할 수 있는 메서드
        public void TriggerHapticFeedback()
        {
            // 기본 진동 패턴을 실행합니다.
            string hapticPattern = "index_touch_r";
            Debug.Log($"Triggering haptic feedback: Pattern = {hapticPattern}");
            BhapticsLibrary.Play(hapticPattern);
        }
    }
}
