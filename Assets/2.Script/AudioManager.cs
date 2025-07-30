using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    // �̱��� �ν��Ͻ�
    public static AudioManager Instance { get; private set; }
    // BGM ����� AudioSource (������� ����)
    public AudioSource bgmSource;

    // ȿ���� ����� AudioSource (PlayOneShot ����)
    public AudioSource sfxSource;

    // BGM Ŭ����
    public AudioClip titleBGM;
    public AudioClip gameBGM;

    // ȿ���� Ŭ����
    public AudioClip sfx_GameEvent;      // ��: ���� �� ��Ȯ, ���� ��
    public AudioClip sfx_StorageOpen;    // ��: ���ʹ� �̺�Ʈ ��
    public AudioClip sfx_ButtonClick;    // ��ư Ŭ�� ȿ����

    void Start()
    {
        // �̱��� �ʱ�ȭ
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �� ��ȯ �� �ı����� �ʵ���
        }
        else
        {
            Destroy(gameObject); // �ߺ� ����
        }
        // ���� �� �ε��� Ȯ��
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        // ���� ���� BGM ����
        if (sceneIndex == 0) // Ÿ��Ʋ ��
        {
            PlayBGM(titleBGM);
        }
        else if (sceneIndex == 1) // ���� ��
        {
            PlayBGM(gameBGM);
        }
    }

    /// <summary>
    /// ������� ��� (Ŭ�� ���� �� �ݺ�)
    /// </summary>
    /// <param name="clip">����� BGM Ŭ��</param>
    public void PlayBGM(AudioClip clip)
    {
        if (bgmSource == null || clip == null) return;

        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    /// <summary>
    /// ȿ���� ��� (��Ȯ �� ���� �̺�Ʈ)
    /// </summary>
    public void PlayDistroySFX()
    {
        if (sfxSource != null && sfx_GameEvent != null)
            sfxSource.PlayOneShot(sfx_GameEvent);
    }

    /// <summary>
    /// ȿ���� ��� (���ʹ� �̺�Ʈ)
    /// </summary>
    public void PlaySetSFX()
    {
        if (sfxSource != null && sfx_StorageOpen != null)
            sfxSource.PlayOneShot(sfx_StorageOpen);
    }

    /// <summary>
    /// ��ư Ŭ�� ȿ����
    /// </summary>
    public void PlayButtonClickSFX()
    {
        if (sfxSource != null && sfx_ButtonClick != null)
            sfxSource.PlayOneShot(sfx_ButtonClick);
    }
}
