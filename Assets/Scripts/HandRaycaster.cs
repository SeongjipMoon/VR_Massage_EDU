using UnityEngine;
using UnityEngine.EventSystems;

public class HandRaycaster : MonoBehaviour
{
    public OVRHand ovrHand; // OVRHand를 통해 손의 위치를 가져옵니다.
    public LayerMask uiLayer; // UI Layer 마스크 설정.

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (ovrHand != null)
        {
            // 손의 위치와 방향으로 레이캐스트 발사.
            Ray ray = new Ray(ovrHand.transform.position, ovrHand.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, uiLayer))
            {
                // UI 요소에 레이캐스트가 맞았는지 확인.
                if (hit.collider != null)
                {
                    Debug.Log($"UI Hit: {hit.collider.name}");

                    // EventSystem을 통해 UI와 상호작용 처리.
                    PointerEventData pointerData = new PointerEventData(EventSystem.current)
                    {
                        pointerId = -1,
                        position = mainCamera.WorldToScreenPoint(hit.point)
                    };

                    ExecuteEvents.Execute(hit.collider.gameObject, pointerData, ExecuteEvents.pointerClickHandler);
                }
            }
        }
    }
}
