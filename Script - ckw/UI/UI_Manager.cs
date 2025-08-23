using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager Instance; // 싱글톤 인스턴스

    [Header("UI Manager Settings")]
    [SerializeField] bool handleEscapeInput = true; // ESC 키 입력 처리 여부
    [SerializeField] List<string> disabledScenes = new List<string>(); // 이 씬들에서는 UI_Manager 비활성화

    // UI 관리용 자료구조
    private Dictionary<UIType, Base_UI> registeredUIs = new Dictionary<UIType, Base_UI>(); // 등록된 UI들을 타입별로 저장
    private Stack<Base_UI> uiStack = new Stack<Base_UI>(); // 열린 UI들의 스택 (최상단 UI 추적용)
    private List<Base_UI> openedUIs = new List<Base_UI>(); // 현재 열린 UI들의 리스트

    // 싱글톤 패턴 적용 및 초기화
    void Awake()
    {
        // 싱글톤 처리
        if (Instance != null) // 이미 인스턴스가 존재하면
            Destroy(gameObject); // 현재 오브젝트 파괴
        else
        {
            Instance = this; // 인스턴스 설정
            DontDestroyOnLoad(gameObject); // 씬 전환 시에도 유지
        }

        SceneManager.sceneLoaded += OnSceneLoaded;      // 씬 변경 이벤트 등록

        CheckCurrentScene();        // 현재 씬 체크
        RegisterAllUIs(); // 씬의 모든 Base_UI를 받고 있는 UI 등록
    }

    // ESC 키 입력 처리
    void Update()
    {
        if (handleEscapeInput && Input.GetKeyDown(KeyCode.Escape)) // ESC 키 입력 처리가 활성화되어 있고 ESC 키가 눌렸을 때
            HandleEscapeInput(); // ESC 입력 처리 함수 호출
    }


    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 씬이 로드될 때 호출
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckCurrentScene();

        //if (gameObject.activeInHierarchy)
        //RegisterAllUIs();
    }



    // 현재 씬이 비활성화 대상인지 체크
    private void CheckCurrentScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (disabledScenes.Contains(currentScene))
        {
            // 비활성화 대상 씬이면 오브젝트 비활성화
            gameObject.SetActive(false);
            Debug.Log($"UI_Manager disabled in scene: {currentScene}");
        }
        else
        {
            // 활성화 대상 씬이면 오브젝트 활성화
            if (!gameObject.activeInHierarchy)
            {
                gameObject.SetActive(true);
                Debug.Log($"UI_Manager enabled in scene: {currentScene}");
            }
        }
    }
    //씬의 모든 Base_UI 컴포넌트를 찾아서 등록
    private void RegisterAllUIs()
    {
        Base_UI[] allUIs = FindObjectsByType<Base_UI>(FindObjectsInactive.Include, FindObjectsSortMode.None); // 비활성화된 오브젝트도 포함해서 모든 Base_UI 찾기
        foreach (Base_UI ui in allUIs) // 찾은 모든 UI에 대해
            RegisterUI(ui); // 각 UI를 등록
    }

    //UI를 딕셔너리에 등록
    public void RegisterUI(Base_UI ui)
    {
        if (ui.uiType != UIType.None && !registeredUIs.ContainsKey(ui.uiType)) // UI 타입이 None이 아니고 아직 등록되지 않은 경우
            registeredUIs[ui.uiType] = ui; // 딕셔너리에 UI 등록
    }

    // 지정된 타입의 UI 열기
    public void OpenUI(UIType uIType)
    {
        if (registeredUIs.TryGetValue(uIType, out Base_UI ui)) // 해당 타입의 UI가 등록되어 있는지 확인
            ui.OpenUI(); // UI 열기
        else
            Debug.Log("랄랄라 ui안열렸다"); // UI를 찾을 수 없을 때 디버그 메시지
    }

    // 열린 모든 UI 닫기
    // public void CloseAllUI()
    // {
    //     for (int i = openedUIs.Count - 1; i >= 0; i--) // 열린 UI 리스트를 역순으로 순회
    //         openedUIs[i].CloseUI(); // 각 UI 닫기
    // }

    // 특정 레이어의 UI들만 닫기
    public void CloseUIsByLayer(UILayer layer)
    {
        for (int i = openedUIs.Count - 1; i >= 0; i--) // 열린 UI 리스트를 역순으로 순회
            if (openedUIs[i].uiLayer == layer) // UI의 레이어가 지정된 레이어와 같으면
                openedUIs[i].CloseUI(); // 해당 UI 닫기
    }

    // 스택 최상단의 UI 반환 (가장 최근에 열린 UI)
    public Base_UI GetTopUI()
    {
        if (uiStack.Count > 0) // 스택에 UI가 있으면
            return uiStack.Peek(); // 최상단 UI 반환

        return null; // 스택이 비어있으면 null 반환
    }

    // 지정된 타입의 UI가 열려있는지 확인
    // public bool IsUIOpen(UIType uIType)
    // {
    //     //if (registeredUIs.TryGetValue(uIType, out Base_UI ui)) // 해당 타입의 UI가 등록되어 있는지 확인
    //         return ui.IsActive(); // UI의 활성화 상태 반환

    //     return false; // 등록되지 않은 UI는 false 반환
    // }

    // UI가 열릴 때 열린 UI 목록과 스택에 등록
    public void RegisterOpenedUI(Base_UI ui)
    {
        if (!openedUIs.Contains(ui)) // 이미 열린 UI 목록에 포함되어 있지 않으면
        {
            openedUIs.Add(ui); // 열린 UI 목록에 추가
            uiStack.Push(ui); // UI 스택에 푸시

            // UI 레이어에 따라 정렬 (레이어 우선순위별로)
            openedUIs.Sort((a, b) => ((int)a.uiLayer).CompareTo((int)b.uiLayer)); // 레이어 값으로 오름차순 정렬
        }
    }

    // UI가 닫힐 때 열린 UI 목록과 스택에서 제거
    public void UnregisterClosedUI(Base_UI ui)
    {
        openedUIs.Remove(ui); // 열린 UI 목록에서 제거

        // 스택에서 해당 UI 제거 (스택 재구성)
        var tempStack = new Stack<Base_UI>(); // 임시 스택 생성
        while (uiStack.Count > 0) // 기존 스택의 모든 요소를 처리할 때까지
        {
            Base_UI stackUI = uiStack.Pop(); // 스택에서 UI 팝
            if (stackUI != ui) // 제거하려는 UI가 아니고 활성화된 UI라면
                tempStack.Push(stackUI); // 임시 스택에 푸시
        }

        while (tempStack.Count > 0) // 임시 스택의 모든 요소를 다시 원래 스택으로
            uiStack.Push(tempStack.Pop()); // 원래 스택에 푸시
    }



    // ESC 키 입력 처리 - 최상단 UI 닫기 또는 옵션 UI 열기
    private void HandleEscapeInput()
    {
        Base_UI topUI = GetTopUI(); // 현재 UI 스택의 최상단(가장 최근에 열린) UI를 가져옴
        Debug.Log($"ESC pressed. TopUI: {(topUI != null ? topUI.name : "null")}"); // ESC 키가 눌렸을 때 최상단 UI 정보를 로그로 출력

        if (topUI != null && topUI.CanHandleEscape()) // 최상단 UI가 존재하고 ESC 키로 닫을 수 있는 UI인지 확인
        {
            Debug.Log($"Closing UI: {topUI.name}"); // 닫을 UI의 이름을 로그로 출력
            topUI.CloseUI(); // 해당 UI를 닫음
        }
        else if (topUI == null) // 열린 UI가 없는 경우
        {
            OpenUI(UIType.Option); // 옵션 UI를 열음
        }
    }
}