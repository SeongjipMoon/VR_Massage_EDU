using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    private int currentSceneIndex; // 현재 Scene의 인덱스를 저장할 변수

    void Start()
    {
        // 현재 Scene의 인덱스를 저장합니다.
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    public void LoadNextScene()
    {
        // 다음 Scene의 인덱스를 가져옵니다.
        int nextSceneIndex = currentSceneIndex + 1;

        // 다음 Scene으로 이동합니다.
        SceneManager.LoadScene(nextSceneIndex);
    }

    public void LoadPreviousScene()
    {
        // 이전 Scene의 인덱스를 가져옵니다.
        int previousSceneIndex = currentSceneIndex - 1;

        // 이전 Scene의 인덱스로 되돌아갑니다.
        SceneManager.LoadScene(previousSceneIndex);
    }
}