using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    public GameObject settingsPanel;   // 설정 UI 패널
    public GameObject tutorialPanel;   // 튜토리얼 UI 패널

    // 게임 시작
    public void OnStartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void OnTitleGame()
    {
        SceneManager.LoadScene(0);
    }
    // 게임 종료
    public void OnQuitGame()
    {
        Application.Quit();
    }

    // 설정 열기
    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    // 설정 닫기
    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    // 튜토리얼 열기
    public void OpenTutorial()
    {
        tutorialPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    // 튜토리얼 닫기
    public void CloseTutorial()
    {
        tutorialPanel.SetActive(false);
        Time.timeScale = 1f;
    }
    //버튼    클릭 효과음 재생
    public void PlayButtonClickSFX()
    {
        AudioManager.Instance.PlayButtonClickSFX();
    }

}
