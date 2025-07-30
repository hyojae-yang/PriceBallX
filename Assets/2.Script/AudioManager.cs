using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static AudioManager Instance { get; private set; }
    // BGM 재생용 AudioSource (배경음악 전용)
    public AudioSource bgmSource;

    // 효과음 재생용 AudioSource (PlayOneShot 전용)
    public AudioSource sfxSource;

    // BGM 클립들
    public AudioClip titleBGM;
    public AudioClip gameBGM;

    // 효과음 클립들
    public AudioClip sfx_GameEvent;      // 예: 게임 내 수확, 수집 등
    public AudioClip sfx_StorageOpen;    // 예: 에너미 이벤트 등
    public AudioClip sfx_ButtonClick;    // 버튼 클릭 효과음

    void Start()
    {
        // 싱글톤 초기화
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않도록
        }
        else
        {
            Destroy(gameObject); // 중복 방지
        }
        // 현재 씬 인덱스 확인
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        // 씬에 따라 BGM 설정
        if (sceneIndex == 0) // 타이틀 씬
        {
            PlayBGM(titleBGM);
        }
        else if (sceneIndex == 1) // 게임 씬
        {
            PlayBGM(gameBGM);
        }
    }

    /// <summary>
    /// 배경음악 재생 (클립 변경 및 반복)
    /// </summary>
    /// <param name="clip">재생할 BGM 클립</param>
    public void PlayBGM(AudioClip clip)
    {
        if (bgmSource == null || clip == null) return;

        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    /// <summary>
    /// 효과음 재생 (수확 등 게임 이벤트)
    /// </summary>
    public void PlayDistroySFX()
    {
        if (sfxSource != null && sfx_GameEvent != null)
            sfxSource.PlayOneShot(sfx_GameEvent);
    }

    /// <summary>
    /// 효과음 재생 (에너미 이벤트)
    /// </summary>
    public void PlaySetSFX()
    {
        if (sfxSource != null && sfx_StorageOpen != null)
            sfxSource.PlayOneShot(sfx_StorageOpen);
    }

    /// <summary>
    /// 버튼 클릭 효과음
    /// </summary>
    public void PlayButtonClickSFX()
    {
        if (sfxSource != null && sfx_ButtonClick != null)
            sfxSource.PlayOneShot(sfx_ButtonClick);
    }
}
