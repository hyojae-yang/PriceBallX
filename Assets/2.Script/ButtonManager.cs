using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    public GameObject settingsPanel;   // ���� UI �г�
    public GameObject tutorialPanel;   // Ʃ�丮�� UI �г�

    // ���� ����
    public void OnStartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void OnTitleGame()
    {
        SceneManager.LoadScene(0);
    }
    // ���� ����
    public void OnQuitGame()
    {
        Application.Quit();
    }

    // ���� ����
    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    // ���� �ݱ�
    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    // Ʃ�丮�� ����
    public void OpenTutorial()
    {
        tutorialPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    // Ʃ�丮�� �ݱ�
    public void CloseTutorial()
    {
        tutorialPanel.SetActive(false);
        Time.timeScale = 1f;
    }
    //��ư    Ŭ�� ȿ���� ���
    public void PlayButtonClickSFX()
    {
        AudioManager.Instance.PlayButtonClickSFX();
    }

}
