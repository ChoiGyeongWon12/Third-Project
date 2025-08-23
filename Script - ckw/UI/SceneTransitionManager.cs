using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{
    // 싱글톤 패턴을 위한 정적 인스턴스 변수
    public static SceneTransitionManager Instance;

    [Header("Fade Settings")]
    public Image fadeImage; // 페이드 효과에 사용할 이미지
    public float fadeInDuration = 1.0f; // 페이드 인 지속 시간
    public float fadeOutDuration = 1.0f; // 페이드 아웃 지속 시간

    // 현재 씬 전환 중인지 확인하는 플래그
    private bool isTransitioning = false;

    void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeFadeImage();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 씬이 로드될 때마다 자동으로 페이드 인 실행
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        // 이벤트 구독 해제 (메모리 누수 방지)
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 페이드 이미지를 초기화하는 메서드
    void InitializeFadeImage()
    {
        if (fadeImage != null)
        {
            // 초기에는 투명하게 설정
            SetFadeAlpha(0f);
        }
        else
        {
            Debug.LogWarning("AutoFadeSceneManager: Fade Image가 할당되지 않았습니다!");
        }
    }

    // 씬이 로드될 때마다 자동으로 호출되는 메서드
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬 전환이 완료된 후에만 페이드 인 실행
        if (isTransitioning)
        {
            StartCoroutine(FadeIn());
        }
    }

    /// 자동 페이드가 적용되는 씬 로드 메서드
    /// 이 메서드를 사용하면 자동으로 페이드 아웃 → 씬 로드 → 페이드 인이 실행됩니다
    public static void LoadScene(string sceneName)
    {
        if (Instance != null)
        {
            Instance.LoadSceneWithFade(sceneName);
        }
        else
        {
            // AutoFadeSceneManager가 없으면 일반 씬 로드
            SceneManager.LoadScene(sceneName);
        }
    }



    /// 페이드 없이 즉시 씬을 로드하는 메서드
    public static void LoadSceneImmediately(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }


    // 페이드 효과와 함께 씬을 전환하는 내부 메서드
    public void LoadSceneWithFade(string sceneName)
    {
        if (!isTransitioning && fadeImage != null)
        {
            StartCoroutine(LoadSceneCoroutine(sceneName));
        }
    }


    // 씬 이름을 사용한 씬 전환 코루틴
    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        isTransitioning = true;

        // Fade Out (화면을 점점 어둡게 만들기)
        yield return StartCoroutine(FadeOut());

        // 씬을 비동기적으로 로드
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // 씬 로딩이 완료될 때까지 기다리기
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // 씬 로드 완료 후 잠시 대기
        yield return new WaitForSeconds(0.1f);

        // isTransitioning은 OnSceneLoaded에서 페이드 인 완료 후 false로 설정됨
    }



    // 페이드 아웃 효과
    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeOutDuration);
            SetFadeAlpha(alpha);
            yield return null;
        }

        SetFadeAlpha(1f);
    }

    // 페이드 인 효과
    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeInDuration);
            SetFadeAlpha(alpha);
            yield return null;
        }

        SetFadeAlpha(0f);
        isTransitioning = false; // 페이드 인 완료 후 전환 플래그 해제
    }

    // 페이드 이미지의 알파값 설정
    private void SetFadeAlpha(float alpha)
    {
        if (fadeImage != null)
        {
            Color color = fadeImage.color;
            color.a = alpha;
            fadeImage.color = color;
        }
    }





    // 현재 씬 전환 중인지 확인
    public bool IsTransitioning()
    {
        return isTransitioning;
    }

    // 즉시 페이드 아웃 설정
    public void SetFadeOut()
    {
        SetFadeAlpha(1f);
    }

    // 즉시 페이드 인 설정
    public void SetFadeIn()
    {
        SetFadeAlpha(0f);
    }
}