using UnityEngine;

public class ExitGameButton : MonoBehaviour
{
    public void ExitGame()
    {
        // 에디터에서는 플레이 모드를 종료합니다.
        UnityEditor.EditorApplication.isPlaying = false;
        // 어플리케이션을 종료합니다.
        Application.Quit();
    }
}