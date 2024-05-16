using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CanvasController : MonoBehaviour
{
    public List<Canvas> canvases;
    public List<AudioClip> voiceClips; // 각 캔버스에 대한 음성 메시지
    private Queue<Canvas> canvasQueue = new Queue<Canvas>();
    private Queue<AudioClip> voiceClipQueue = new Queue<AudioClip>();

    void Start()
    {
        // 모든 캔버스를 숨김
        foreach (Canvas canvas in canvases)
        {
            canvas.enabled = false;
        }


        // 각 캔버스와 해당 음성 메시지를 Queue에 추가
        for (int i = 0; i < canvases.Count; i++)
        {
            canvasQueue.Enqueue(canvases[i]);
            voiceClipQueue.Enqueue(voiceClips[i]);
        }

        // 첫 번째 캔버스 보여주기
        StartCoroutine(ShowNextCanvas());
    }

    IEnumerator ShowNextCanvas()
    {
        // Queue가 비어있지 않을 때까지 반복
        while (canvasQueue.Count > 0)
        {
            // Queue에서 다음 캔버스,음성 메시지 가져오기
            Canvas nextCanvas = canvasQueue.Dequeue();
            AudioClip voiceClip = voiceClipQueue.Dequeue();            

            // 캔버스 보여주기
            nextCanvas.enabled = true;    

            // 음성 메시지 재생
            AudioSource.PlayClipAtPoint(voiceClip, Camera.main.transform.position);

            // 대기
            yield return new WaitForSeconds(voiceClip.length);

            // 캔버스 숨기기
            nextCanvas.enabled = false;
        }
    }
}