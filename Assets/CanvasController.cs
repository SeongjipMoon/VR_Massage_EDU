using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;
using System.Collections.Generic;

public class CanvasController : MonoBehaviour
{
    public List<Canvas> canvases;
    public List<AudioClip> voiceClips; // 각 캔버스에 대한 음성 메시지
    public List<int> autoAdvanceCanvasIndices; // 자동 전환이 필요한 Canvas 인덱스
    public List<VideoPlayer> videoPlayers; // 각 캔버스에 대한 VideoPlayer 리스트
    public List<GameObject> babyObjects; // 각 Canvas에 대응하는 baby 오브젝트 리스트

    private Queue<Canvas> canvasQueue = new Queue<Canvas>();
    private Queue<AudioClip> voiceClipQueue = new Queue<AudioClip>();
    private Queue<VideoPlayer> videoPlayerQueue = new Queue<VideoPlayer>(); // VideoPlayer 큐 추가

    private Canvas currentCanvas;
    private AudioClip currentVoiceClip;
    private VideoPlayer currentVideoPlayer; // 현재 VideoPlayer
    private AudioSource audioSource;
    private Coroutine currentCoroutine;
    
    void Start()
    {
        // AudioSource 컴포넌트를 추가합니다.
        audioSource = gameObject.AddComponent<AudioSource>();

        // 모든 캔버스를 숨김
        foreach (Canvas canvas in canvases)
        {
            canvas.enabled = false;
        }

        // 각 캔버스와 해당 음성 메시지를 Queue에 추가
        for (int i = 0; i < canvases.Count; i++)
        {
            canvasQueue.Enqueue(canvases[i]);
            videoPlayerQueue.Enqueue(videoPlayers[i]);
            voiceClipQueue.Enqueue(voiceClips[i]);
        }

        // 첫 번째 캔버스 보여주기
        ShowNextCanvas();
    }

    public void OnNextButtonClick()
    {
        // 현재 재생 중인 음성 메시지와 비디오 멈춤
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        if (currentVideoPlayer != null && currentVideoPlayer.isPlaying)
        {
            currentVideoPlayer.Stop();
        }

        // 현재 진행 중인 코루틴 멈춤
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }


        // 현재 캔버스를 숨기고, 다음 캔버스를 표시
        if (currentCanvas != null)
        {
            currentCanvas.enabled = false;
        }

        ShowNextCanvas();
    }

    public void OnReplayButtonClick()
    {
        // 현재 재생 중인 음성 메시지 멈춤
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        
        if (currentVideoPlayer != null && currentVideoPlayer.isPlaying)
        {
            currentVideoPlayer.Stop();
        }
        
        // 현재 진행 중인 코루틴 멈춤
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        // 현재 캔버스를 다시 재생
        if (currentCanvas != null)
        {
            currentCanvas.enabled = true;

            if (currentVoiceClip != null)
            {
                audioSource.clip = currentVoiceClip;
                audioSource.Play();
                currentCoroutine = StartCoroutine(PlayVoiceAndWait(currentVoiceClip));
            }

            if (currentVideoPlayer != null)
            {
                currentVideoPlayer.playOnAwake = true;
                currentVideoPlayer.Play();
            }
        }
    }

    private void ShowNextCanvas()
    {
        if (canvasQueue.Count > 0)
        {
            // Queue에서 다음 캔버스와 음성 메시지 가져오기
            currentCanvas = canvasQueue.Dequeue();
            currentVideoPlayer = videoPlayerQueue.Dequeue();
            // Queue에서 음성 메시지 가져오기 (있을 경우에만)
            AudioClip voiceClip = voiceClipQueue.Count > 0 ? voiceClipQueue.Dequeue() : null;

            // 현재 음성 클립을 업데이트
            currentVoiceClip = voiceClip;
            
            // 캔버스 보여주기
            currentCanvas.enabled = true;

            // Video Player 재생
            if (currentVideoPlayer != null)
            {
                currentVideoPlayer.playOnAwake = true;
                currentVideoPlayer.Play();
            }

            if (voiceClip != null)
            {
                // 음성 메시지가 있을 경우 재생 및 코루틴 시작
                audioSource.clip = voiceClip;
                audioSource.Play();
                currentCoroutine = StartCoroutine(PlayVoiceAndWait(voiceClip));
            }
                else
            {
                // 음성 메시지가 없을 경우 코루틴 없이 캔버스만 표시
                currentCoroutine = StartCoroutine(ShowCanvasWithoutVoice());
            }

            // 모든 baby 오브젝트를 비활성화
            foreach (GameObject baby in babyObjects)
            {
                if (baby != null)
                {
                    baby.SetActive(false);
                }
            }

            // 현재 Canvas에 대응하는 baby 오브젝트를 활성화
            int currentIndex = canvases.IndexOf(currentCanvas);
            if (currentIndex >= 0 && currentIndex < babyObjects.Count && babyObjects[currentIndex] != null)
            {
                babyObjects[currentIndex].SetActive(true);
            }
        }
    }

    private IEnumerator ShowCanvasWithoutVoice()
    {
        // 일정 시간 동안 대기 (필요 시 변경 가능)
        yield return new WaitForSeconds(60f);

        // 현재 캔버스를 숨기기
        if (currentCanvas != null)
        {
            currentCanvas.enabled = false;
        }

        // 다음 캔버스로 넘어가기
         ShowNextCanvas();
    }

    private IEnumerator PlayVoiceAndWait(AudioClip voiceClip)
    {
        // 음성 메시지 재생
        audioSource.clip = voiceClip;
        audioSource.Play();

        // 음성 메시지가 끝날 때까지 대기
        yield return new WaitForSeconds(voiceClip.length);

        // 현재 캔버스를 숨기기
        if (currentCanvas != null)
        {
            currentCanvas.enabled = false;
        }

        // 자동 전환이 필요한 Canvas가 아닌 경우 현재 캔버스를 계속 표시
        int currentIndex = canvases.IndexOf(currentCanvas);
        if (autoAdvanceCanvasIndices.Contains(currentIndex))
        {
            ShowNextCanvas();
        }
        else
        {
            // 특정 캔버스는 계속 표시
            currentCanvas.enabled = true;
        }
    }
}