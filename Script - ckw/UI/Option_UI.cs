using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class Option_UI : Base_UI  // Base_UI 클래스를 상속받는 Option_UI 클래스 선언
{
    [SerializeField] GameObject panel;
    [Header("Option UI Settings")]         // 인스펙터에서 "Option UI Settings" 헤더 표시
    public Button closeButton;             // 옵션 창을 닫는 버튼
    public Button exitButton;              // 게임을 종료하는 버튼
    public GameObject quitGameMessage;     // 게임 종료 확인 메시지 UI 오브젝트

    [Header("Audio Settings")]             // 인스펙터에서 "Audio Settings" 헤더 표시
    public Slider masterVolumeSlider;      // 마스터 볼륨 조절 슬라이더
    public Slider bgmVolumeSlider;         // BGM 볼륨 조절 슬라이더
    public Slider sfxVolumeSlider;         // SFX 볼륨 조절 슬라이더

    // Option UI 초기화
    protected override void Initialize()   // Base_UI의 Initialize 메서드를 오버라이드하여 초기화 수행
    {
        base.Initialize();                 // 부모 클래스의 Initialize 메서드 호출

        // 버튼 이벤트 등록
        if (closeButton != null)           // 닫기 버튼이 존재하는 경우
            closeButton.onClick.AddListener(CloseUI);  // 버튼 클릭 시 CloseUI 메서드 호출하도록 리스너 등록
        // if (exitButton != null)         // 종료 버튼 관련 코드 (주석 처리됨)
        //     exitButton.onClick.AddListener(() => UI_Manager.Instance.QuitGame());

        // 슬라이더 이벤트 등록
        SetupAudioSliders();               // 오디오 슬라이더들의 이벤트를 설정하는 메서드 호출

        // 저장된 설정값 로드
        LoadAudioSettings();               // PlayerPrefs에서 저장된 오디오 설정을 불러오는 메서드 호출
    }

    // 오디오 슬라이더 설정 및 이벤트 등록
    private void SetupAudioSliders()      // 각 오디오 슬라이더의 값 변경 이벤트를 등록하는 메서드
    {
        // 슬라이더 이벤트 등록
        if (masterVolumeSlider != null)    // 마스터 볼륨 슬라이더가 존재하는 경우
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);  // 값 변경 시 OnMasterVolumeChanged 메서드 호출
        if (bgmVolumeSlider != null)       // BGM 볼륨 슬라이더가 존재하는 경우
            bgmVolumeSlider.onValueChanged.AddListener(OnBGMVolumeChanged);        // 값 변경 시 OnBGMVolumeChanged 메서드 호출
        if (sfxVolumeSlider != null)       // SFX 볼륨 슬라이더가 존재하는 경우
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);        // 값 변경 시 OnSFXVolumeChanged 메서드 호출
    }

    // 마스터 볼륨 변경 처리
    private void OnMasterVolumeChanged(float value)  // 마스터 볼륨 슬라이더 값이 변경될 때 호출되는 메서드
    {
        if (SoundManager.instance != null)           // SoundManager 인스턴스가 존재하는 경우
            SoundManager.instance.SetMasterVolume(value);  // SoundManager의 마스터 볼륨을 변경된 값으로 설정

    }

    private void OnBGMVolumeChanged(float value)    // BGM 볼륨 슬라이더 값이 변경될 때 호출되는 메서드
    {
        if (SoundManager.instance != null)          // SoundManager 인스턴스가 존재하는 경우
            SoundManager.instance.SetBGMVolume(value);  // SoundManager의 BGM 볼륨을 변경된 값으로 설정
    }

    private void OnSFXVolumeChanged(float value)    // SFX 볼륨 슬라이더 값이 변경될 때 호출되는 메서드
    {
        if (SoundManager.instance != null)          // SoundManager 인스턴스가 존재하는 경우
            SoundManager.instance.SetSFXVolume(value);  // SoundManager의 SFX 볼륨을 변경된 값으로 설정
    }

    // 저장된 오디오 설정 로드
    private void LoadAudioSettings()        // PlayerPrefs에서 저장된 오디오 설정을 불러와서 UI에 반영하는 메서드
    {
        // 저장된 값 로드 (기본값 0.5)
        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 0.5f);  // 마스터 볼륨 값 로드 (기본값: 0.5)
        float bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 0.5f);        // BGM 볼륨 값 로드 (기본값: 0.5)
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.5f);        // SFX 볼륨 값 로드 (기본값: 0.5)

        // 슬라이더에 값 설정 (이벤트 호출 안함)
        if (masterVolumeSlider != null)     // 마스터 볼륨 슬라이더가 존재하는 경우
            masterVolumeSlider.SetValueWithoutNotify(masterVolume);  // 이벤트를 발생시키지 않고 슬라이더 값 설정
        if (bgmVolumeSlider != null)        // BGM 볼륨 슬라이더가 존재하는 경우
            bgmVolumeSlider.SetValueWithoutNotify(bgmVolume);        // 이벤트를 발생시키지 않고 슬라이더 값 설정
        if (sfxVolumeSlider != null)        // SFX 볼륨 슬라이더가 존재하는 경우
            sfxVolumeSlider.SetValueWithoutNotify(sfxVolume);        // 이벤트를 발생시키지 않고 슬라이더 값 설정

        // 실제 볼륨 적용
        OnMasterVolumeChanged(masterVolume);  // 로드한 마스터 볼륨값을 SoundManager에 적용
        OnBGMVolumeChanged(bgmVolume);        // 로드한 BGM 볼륨값을 SoundManager에 적용
        OnSFXVolumeChanged(sfxVolume);        // 로드한 SFX 볼륨값을 SoundManager에 적용
    }

    // UI가 열릴 때 호출
    protected override void OnOpen()        // Base_UI의 OnOpen 메서드를 오버라이드하여 UI가 열릴 때의 동작 정의
    {
        // 게임 일시정지 (필요한 경우)
        if (pauseGameWhenOpen)              // pauseGameWhenOpen 변수가 true인 경우 (Base_UI에서 상속받은 변수)
            Time.timeScale = 0f;            // 게임 시간을 0으로 설정하여 일시정지

        // 현재 설정값으로 슬라이더 갱신
        LoadAudioSettings();               // 저장된 오디오 설정을 다시 로드하여 UI 갱신
    }

    // UI가 닫힐 때 호출
    protected override void OnClose()       // Base_UI의 OnClose 메서드를 오버라이드하여 UI가 닫힐 때의 동작 정의
    {
        // 게임 재개 (일시정지했던 경우)
        if (pauseGameWhenOpen)              // pauseGameWhenOpen 변수가 true인 경우
            Time.timeScale = 1f;            // 게임 시간을 1로 설정하여 정상 속도로 재개

        // 설정 저장
        PlayerPrefs.Save();                 // 변경된 모든 PlayerPrefs 값을 디스크에 저장

        if (quitGameMessage != null)        // 게임 종료 메시지 UI가 존재하는 경우
            quitGameMessage.SetActive(false);  // 게임 종료 메시지 UI를 비활성화
    }

    // 설정 초기화 버튼 (추가 기능)
    public void ResetToDefault()           // 모든 오디오 설정을 기본값으로 초기화하는 메서드
    {
        if (masterVolumeSlider != null)    // 마스터 볼륨 슬라이더가 존재하는 경우
            masterVolumeSlider.value = 0.5f;  // 슬라이더 값을 기본값인 0.5로 설정 (이벤트 발생함)
        if (bgmVolumeSlider != null)       // BGM 볼륨 슬라이더가 존재하는 경우
            bgmVolumeSlider.value = 0.5f;     // 슬라이더 값을 기본값인 0.5로 설정 (이벤트 발생함)
        if (sfxVolumeSlider != null)       // SFX 볼륨 슬라이더가 존재하는 경우
            sfxVolumeSlider.value = 0.5f;     // 슬라이더 값을 기본값인 0.5로 설정 (이벤트 발생함)
    }

    // Update에서 키 입력 처리 (선택적 - UI_Manager에서 처리하는 경우 불필요)
    // void Update()                       // 매 프레임마다 호출되는 메서드 (주석 처리됨)
    // {
    //     // Option UI가 열려있지 않을 때 ESC로 열기
    //     if (Input.GetKeyDown(KeyCode.Escape) && !IsActive())  // ESC 키가 눌리고 현재 UI가 비활성화 상태인 경우
    //     {
    //         // 다른 UI가 열려있지 않을 때만 옵션 열기
    //         if (UI_Manager.Instance != null && UI_Manager.Instance.GetTopUI() == null)  // UI_Manager가 존재하고 최상위 UI가 없는 경우
    //         {
    //             OpenUI();               // 옵션 UI 열기
    //         }
    //     }
    // }

    public void QuitGameMessage()          // 게임 종료 확인 메시지를 표시하는 메서드
    {
        quitGameMessage.SetActive(true);   // 게임 종료 확인 메시지 UI를 활성화
    }

    void OnEnable()
    {
        panel.SetActive(true);
    }

    void OnDisable()
    {
        panel.SetActive(false);
    }

    public void QuitCancel()
    {
        quitGameMessage.SetActive(false);
    }


    // 게임 종료 (에디터에서는 플레이 모드 종료, 빌드에서는 애플리케이션 종료)
    public void QuitGame()                 // 실제로 게임을 종료하는 메서드
    {
#if UNITY_EDITOR                          // Unity 에디터에서 실행 중인 경우
        EditorApplication.ExitPlaymode();  // 플레이 모드를 종료
#else                                     // 빌드된 게임에서 실행 중인 경우
    Application.Quit();                   // 애플리케이션을 완전히 종료
#endif
    }
}