using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;    // 싱글톤 패턴을 위한 정적 인스턴스 변수

    [Header("Audio Sources")]               // 인스펙터에서 "Audio Sources" 헤더 표시
    public AudioSource bgmSource;           // 배경음악(BGM) 재생을 위한 AudioSource 컴포넌트
    public AudioSource sfxSource;           // 효과음(SFX) 재생을 위한 AudioSource 컴포넌트

    [Header("Current Volume Values")]       // 인스펙터에서 "Current Volume Values" 헤더 표시
    private float currentMasterVolume;      // 현재 마스터 볼륨 값을 저장하는 변수
    private float currentBGMVolume;         // 현재 BGM 볼륨 값을 저장하는 변수
    private float currentSFXVolume;         // 현재 SFX 볼륨 값을 저장하는 변수

    // 싱글톤 패턴 및 초기 설정
    void Awake()                           // 게임 오브젝트가 생성될 때 가장 먼저 호출되는 메서드
    {
        // 싱글톤 처리
        if (instance != null && instance != this)  // 이미 인스턴스가 존재하고 현재 객체가 아닌 경우
        {
            Destroy(gameObject);           // 현재 게임 오브젝트를 파괴
            return;                        // 메서드 종료
        }
        instance = this;                   // 현재 객체를 싱글톤 인스턴스로 설정
        DontDestroyOnLoad(gameObject);     // 씬이 바뀌어도 이 오브젝트가 파괴되지 않도록 설정

        SetupAudioSources();               // AudioSource 컴포넌트들의 초기 설정 수행
        LoadVolumeSettings();              // PlayerPrefs에서 저장된 볼륨 설정을 로드
    }

    // AudioSource 기본 설정
    private void SetupAudioSources()       // AudioSource들의 기본 속성을 설정하는 메서드
    {
        if (bgmSource != null)             // BGM AudioSource가 null이 아닌 경우
        {
            bgmSource.loop = true;         // BGM은 반복 재생하도록 설정
            bgmSource.playOnAwake = false; // 게임 시작 시 자동 재생되지 않도록 설정
        }

        if (sfxSource != null)             // SFX AudioSource가 null이 아닌 경우
        {
            sfxSource.loop = false;        // SFX는 반복 재생하지 않도록 설정
            sfxSource.playOnAwake = false; // 게임 시작 시 자동 재생되지 않도록 설정
        }
    }

    // PlayerPrefs에서 볼륨 설정 로드
    private void LoadVolumeSettings()      // 저장된 볼륨 설정을 불러오는 메서드
    {
        currentMasterVolume = PlayerPrefs.GetFloat("MasterVolume", 0.5f);  // 마스터 볼륨 로드 (기본값: 0.5)
        currentBGMVolume = PlayerPrefs.GetFloat("BGMVolume", 0.5f);        // BGM 볼륨 로드 (기본값: 0.5)
        currentSFXVolume = PlayerPrefs.GetFloat("SFXVolume", 0.5f);        // SFX 볼륨 로드 (기본값: 0.5)

        ApplyVolumeSettings();             // 로드한 볼륨 설정을 실제 오디오에 적용
    }

    // 현재 볼륨 설정을 실제 오디오에 적용
    private void ApplyVolumeSettings()     // 볼륨 설정값들을 실제 오디오 컴포넌트에 적용하는 메서드
    {
        // 마스터 볼륨 적용
        AudioListener.volume = currentMasterVolume;  // 전역 오디오 리스너의 볼륨을 마스터 볼륨으로 설정

        // BGM 볼륨 적용
        if (bgmSource != null)             // BGM AudioSource가 존재하는 경우
            bgmSource.volume = currentBGMVolume * currentMasterVolume;  // BGM 볼륨에 마스터 볼륨을 곱해서 적용

        if (sfxSource != null)             // SFX AudioSource가 존재하는 경우
            sfxSource.volume = currentSFXVolume * currentMasterVolume;  // SFX 볼륨에 마스터 볼륨을 곱해서 적용

        // SFX 볼륨 적용 (개별 SFX 재생 시 적용됨)
    }

    // Update에서 볼륨 변경사항 체크 및 적용
    void Update()                          // 매 프레임마다 호출되는 메서드
    {
        CheckAndUpdateVolume();            // 볼륨 변경사항을 체크하고 업데이트
    }

    // PlayerPrefs에서 볼륨 변경사항 체크하고 업데이트
    private void CheckAndUpdateVolume()    // PlayerPrefs의 볼륨값 변경을 실시간으로 감지하고 적용하는 메서드
    {
        float newMasterVolume = PlayerPrefs.GetFloat("MasterVolume", 0.5f);  // 현재 저장된 마스터 볼륨값 가져오기
        float newBGMVolume = PlayerPrefs.GetFloat("BGMVolume", 0.5f);        // 현재 저장된 BGM 볼륨값 가져오기
        float newSFXVolume = PlayerPrefs.GetFloat("SFXVolume", 0.5f);        // 현재 저장된 SFX 볼륨값 가져오기

        // 값이 변경된 경우에만 업데이트
        if (!Mathf.Approximately(currentMasterVolume, newMasterVolume) ||    // 마스터 볼륨이 변경되었거나
            !Mathf.Approximately(currentBGMVolume, newBGMVolume) ||          // BGM 볼륨이 변경되었거나
            !Mathf.Approximately(currentSFXVolume, newSFXVolume))            // SFX 볼륨이 변경된 경우
        {
            currentMasterVolume = newMasterVolume;  // 현재 마스터 볼륨값을 새 값으로 업데이트
            currentBGMVolume = newBGMVolume;        // 현재 BGM 볼륨값을 새 값으로 업데이트
            currentSFXVolume = newSFXVolume;        // 현재 SFX 볼륨값을 새 값으로 업데이트

            ApplyVolumeSettings();          // 변경된 볼륨 설정을 실제 오디오에 적용
        }
    }

    #region BGM 관련 메서드              // BGM 관련 메서드들을 그룹화하는 영역 시작
    // BGM 재생
    public void PlayBGM(AudioClip clip)   // 지정된 오디오 클립으로 BGM을 재생하는 메서드
    {
        if (bgmSource != null && clip != null)  // BGM AudioSource와 오디오 클립이 모두 존재하는 경우
        {
            bgmSource.clip = clip;         // BGM AudioSource에 재생할 클립 설정
            bgmSource.volume = currentBGMVolume * currentMasterVolume;  // 볼륨을 BGM볼륨 × 마스터볼륨으로 설정
            bgmSource.Play();              // BGM 재생 시작
        }
    }

    // BGM 정지
    public void StopBGM()                 // BGM 재생을 완전히 정지하는 메서드
    {
        if (bgmSource != null)            // BGM AudioSource가 존재하는 경우
            bgmSource.Stop();             // BGM 재생 정지
    }

    // BGM 일시정지
    public void PauseBGM()                // BGM을 일시정지하는 메서드
    {
        if (bgmSource != null)            // BGM AudioSource가 존재하는 경우
            bgmSource.Pause();            // BGM 일시정지
    }

    // BGM 재개
    public void ResumeBGM()               // 일시정지된 BGM을 다시 재생하는 메서드
    {
        if (bgmSource != null)            // BGM AudioSource가 존재하는 경우
            bgmSource.UnPause();          // BGM 재생 재개
    }
    #endregion                           // BGM 관련 메서드 영역 끝

    #region SFX 관련 메서드              // SFX 관련 메서드들을 그룹화하는 영역 시작
    // SFX 재생 (한 번만)
    public void PlaySFX(AudioClip clip)  // 지정된 오디오 클립으로 효과음을 한 번 재생하는 메서드
    {
        if (sfxSource != null && clip != null)  // SFX AudioSource와 오디오 클립이 모두 존재하는 경우
        {
            sfxSource.clip = clip;        // SFX AudioSource에 재생할 클립 설정
            sfxSource.volume = currentSFXVolume * currentMasterVolume;  // 볼륨을 SFX볼륨 × 마스터볼륨으로 설정
            sfxSource.PlayOneShot(clip);  // 클립을 한 번만 재생 (다른 SFX와 겹쳐서 재생 가능)
        }
    }

    #endregion                           // SFX 관련 메서드 영역 끝

    #region 볼륨 조절 메서드 (외부 호출용)  // 외부에서 볼륨을 조절할 때 사용하는 메서드들을 그룹화하는 영역 시작
    // 마스터 볼륨 설정
    public void SetMasterVolume(float volume)  // 마스터 볼륨을 설정하는 메서드
    {
        currentMasterVolume = Mathf.Clamp01(volume);  // 볼륨값을 0~1 범위로 제한하여 설정
        PlayerPrefs.SetFloat("MasterVolume", currentMasterVolume);  // 변경된 마스터 볼륨을 PlayerPrefs에 저장
        ApplyVolumeSettings();            // 변경된 볼륨 설정을 실제 오디오에 적용
    }

    // BGM 볼륨 설정
    public void SetBGMVolume(float volume)     // BGM 볼륨을 설정하는 메서드
    {
        currentBGMVolume = Mathf.Clamp01(volume);    // 볼륨값을 0~1 범위로 제한하여 설정
        PlayerPrefs.SetFloat("BGMVolume", currentBGMVolume);  // 변경된 BGM 볼륨을 PlayerPrefs에 저장
        ApplyVolumeSettings();            // 변경된 볼륨 설정을 실제 오디오에 적용
    }

    // SFX 볼륨 설정
    public void SetSFXVolume(float volume)     // SFX 볼륨을 설정하는 메서드
    {
        currentSFXVolume = Mathf.Clamp01(volume);    // 볼륨값을 0~1 범위로 제한하여 설정
        PlayerPrefs.SetFloat("SFXVolume", currentSFXVolume);  // 변경된 SFX 볼륨을 PlayerPrefs에 저장
        ApplyVolumeSettings();            // 변경된 볼륨 설정을 실제 오디오에 적용
    }
    #endregion                           // 볼륨 조절 메서드 영역 끝

    #region Getter 메서드                // 현재 볼륨값들을 반환하는 메서드들을 그룹화하는 영역 시작
    // 현재 마스터 볼륨 반환
    // public float GetMasterVolume() { return currentMasterVolume; }  // 현재 마스터 볼륨값을 반환하는 메서드 (주석 처리됨)

    // // 현재 BGM 볼륨 반환
    // public float GetBGMVolume() { return currentBGMVolume; }        // 현재 BGM 볼륨값을 반환하는 메서드 (주석 처리됨)

    // // 현재 SFX 볼륨 반환
    // public float GetSFXVolume() { return currentSFXVolume; }        // 현재 SFX 볼륨값을 반환하는 메서드 (주석 처리됨)
    #endregion                           // Getter 메서드 영역 끝
}