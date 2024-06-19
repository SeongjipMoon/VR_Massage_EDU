using UnityEngine;

public class HandPalmUpDetection : MonoBehaviour
{
    public Transform handTransform; // 손의 Transform
    public GameObject canvasUI; // Canvas UI 오브젝트
    public float hoverThreshold = 0.7f; // 손바닥이 위를 향하는 정도의 임계값

    private bool isHovering = false;

    void Start()
    {
        if (canvasUI != null)
        {
            canvasUI.SetActive(false); // 처음에는 Canvas UI를 비활성화
        }
    }

    void Update()
    {
        DetectHandHover();
    }

    void DetectHandHover()
    {
        // 손의 up 벡터와 World down 벡터의 내적을 계산
        float dotProduct = Vector3.Dot(handTransform.up, Vector3.down);

        // 손바닥이 위를 향할 때 Hover 이벤트 발생
        if (dotProduct > hoverThreshold)
        {
            if (!isHovering)
            {
                isHovering = true;
                OnHoverStart(); // Hover 시작 처리
            }
        }
        else
        {
            if (isHovering)
            {
                isHovering = false;
                OnHoverEnd(); // Hover 종료 처리
            }
        }
    }

    private void OnHoverStart()
    {
        if (canvasUI != null)
        {
            canvasUI.SetActive(true); // Canvas UI를 활성화
        }
    }

    private void OnHoverEnd()
    {
        if (canvasUI != null)
        {
            canvasUI.SetActive(false); // Canvas UI를 비활성화
        }
    }
}
