using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchMe_L : MonoBehaviour
{
    //public Light light1;

    // 손가락 끝 Transform 참조
    public Transform thumbSphere;
    public Transform indexSphere;
    public Transform middleSphere;
    public Transform ringSphere;
    public Transform pinkySphere;
    public Transform wristSphere;

    void Start()
    {
        // 초기화 코드가 필요한 경우 추가
    }

    void Update()
    {
        // 필요 시 업데이트 코드
    }

    private void OnTriggerEnter(Collider other)
    {
        // 충돌한 물체가 손가락 끝인지 확인하고, 해당 손가락 끝의 진동 패턴을 실행
        if (other.transform == thumbSphere)
        {
            TriggerHapticFeedback("thumb_touch_l");
        }
        else if (other.transform == indexSphere)
        {
            TriggerHapticFeedback("index_touch_l");
        }
        else if (other.transform == middleSphere)
        {
            TriggerHapticFeedback("middle_touch_l");
        }
        else if (other.transform == ringSphere)
        {
            TriggerHapticFeedback("ring_touch_l");
        }
        else if (other.transform == pinkySphere)
        {
            TriggerHapticFeedback("pinky_touch_l");
        }
        else if (other.transform == wristSphere)
        {
            TriggerHapticFeedback("wrist_l");
        }
        /*
        // Light On/Off 토글
        if (light1 != null)
        {
            if (light1.enabled == false)
            {
                Debug.Log("Light On!");
                light1.enabled = true;
            }
            else
            {
                Debug.Log("Light Off!");
                light1.enabled = false;
            }
        }
        */
    }

    private void TriggerHapticFeedback(string pattern)
    {
        // Bhaptics 패턴을 사용하여 진동 트리거
        Bhaptics.SDK2.BhapticsLibrary.Play(pattern);
        Debug.Log($"Haptic feedback triggered with pattern: {pattern}");
    }
}
