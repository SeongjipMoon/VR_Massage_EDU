using System.Collections;
using UnityEngine;

namespace Bhaptics.SDK2
{
    public class HapticSource : MonoBehaviour
    {
        public string EventId;
        public bool playOnAwake = false;
        public bool loop = false;
        public float loopDelaySeconds = 0f;

        private Coroutine currentCoroutine, loopCoroutine;
        private bool isLooping = false;

        private void OnEnable()
        {
            if (playOnAwake)
            {
                if (loop)
                {
                    PlayLoop();
                }
                else
                {
                    PlayHapticEvent();
                }
            }
        }

        private void OnDisable()
        {
            Stop();
        }

        public void Play()
        {
            PlayHapticEvent();
        }

        public void PlayLoop()
        {
            if (string.IsNullOrEmpty(EventId))
            {
                Debug.LogError("[bHaptics] EventId is null or empty.");
                return;
            }

            isLooping = true;
            loopCoroutine = StartCoroutine(PlayLoopCoroutine());
        }

        public void PlayDelayed(float delaySeconds = 0)
        {
            if (string.IsNullOrEmpty(EventId))
            {
                Debug.LogError("[bHaptics] EventId is null or empty.");
                return;
            }

            currentCoroutine = StartCoroutine(PlayCoroutine(delaySeconds));
        }

        public void Stop()
        {
            if (loopCoroutine != null)
            {
                isLooping = false;
                StopCoroutine(loopCoroutine);
                loopCoroutine = null;
            }

            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
                currentCoroutine = null;
            }

            if (string.IsNullOrEmpty(EventId))
            {
                return;
            }

            BhapticsLibrary.StopByEventId(EventId);
        }

        public bool IsPlaying()
        {
            if (string.IsNullOrEmpty(EventId))
            {
                return false;
            }

            return BhapticsLibrary.IsPlayingByEventId(EventId);
        }

        private IEnumerator PlayCoroutine(float delaySeconds)
        {
            yield return new WaitForSeconds(delaySeconds);
            PlayHapticEvent();
            yield return null;
        }

        private void PlayHapticEvent()
        {
            if (string.IsNullOrEmpty(EventId))
            {
                Debug.LogError("[bHaptics] EventId is null or empty.");
                return;
            }

            BhapticsLibrary.Play(EventId);
        }

        private IEnumerator PlayLoopCoroutine()
        {
            // 클립의 지속 시간을 직접 설정하는 방법
            // 만약 클립의 실제 지속 시간을 알고 있다면, 그 값을 사용하십시오.
            float clipDuration = 3.0f; // 예를 들어 3초로 설정
            //float clipDuration = BhapticsLibrary.GetClipDurationMillis(EventId) / 1000f; // Convert milliseconds to seconds
            WaitForSeconds duration = new WaitForSeconds(clipDuration * 0.95f);
            while (isLooping)
            {
                yield return new WaitForSeconds(loopDelaySeconds);
                PlayHapticEvent();
                yield return duration;
            }
        }
    }
}
