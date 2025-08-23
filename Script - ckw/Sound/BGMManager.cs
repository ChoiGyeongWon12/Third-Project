using UnityEngine;
using UnityEngine.SceneManagement;


public enum BGMType
{
    Scene_Title,
    Scene_Lobby,
    TEST_title1,
    TEST_lobby1,
    TEST,
    TEST_ice
}


public class BGMManager : MonoBehaviour
{
    [SerializeField] AudioClip[] bgmClips;

    public string currentScene;
    SoundManager soundManager;
    private AudioClip currentBGM;


    void Start()
    {
        soundManager = FindAnyObjectByType<SoundManager>();
        currentScene = SceneManager.GetActiveScene().name;
        PlaySceneBGM(currentScene);
    }

    void Update()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName != currentScene)
        {
            currentScene = sceneName;
            PlaySceneBGM(sceneName);
        }
    }



    void PlayBGM(AudioClip _clip)
    {
        if (soundManager?.bgmSource == null || _clip == null) return;

        if (currentBGM == _clip && soundManager.bgmSource.isPlaying)
        {
            Debug.Log($"동일한 BGM이 재생 : {_clip.name}. 계속 재생");
            return;
        }

        soundManager.bgmSource.clip = _clip;
        soundManager.bgmSource.Play();
        currentBGM = _clip;
    }

    public void StopBGM()
    {
        soundManager.bgmSource.Stop();
        soundManager.bgmSource.clip = null;
    }


    public void PlaySceneBGM(string sceneName)      // 씬에 맞게 브금 재생
    {
        if (System.Enum.TryParse<BGMType>(sceneName, out var bgmType))
        {
            int index = (int)bgmType;
            if (index >= 0 && index < bgmClips.Length)
                PlayBGM(bgmClips[index]);
        }
        else
        {
            StopBGM();
        }
    }
}
