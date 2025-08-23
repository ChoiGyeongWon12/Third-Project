using System;
using UnityEngine;

public enum UIType      // UI 타입 열거형
{
    None,       // 타입 없음
    MainMenu,   // 메인 메뉴
    Option,     // 옵션
    Inventory,  // 인벤토리
    Pause,      // 일시정지
    GameOver,   // 게임 오버
    Settings,   // 설정
    Shop,        // 상점
    StageDetail
}

public enum UILayer     // UI 레이어 열거형 (우선순위)
{
    BackGround = 0, // 배경 (가장 뒤)
    Normal = 100,   // 일반 UI
    Popup = 200,    // 팝업 UI
    System = 300,   // 시스템 UI (옵션, 일시정지 등)
    Alert = 400     // 경고, 알림 등 (가장 앞)
}

/// 모든 UI의 기본 클래스
public abstract class Base_UI : MonoBehaviour
{
    [Header("Base UI Settings")]
    public UIType uiType = UIType.None;     // 이 UI의 타입
    public UILayer uiLayer = UILayer.Normal; // 이 UI의 레이어 (우선순위)
    public bool closeOnEscape = true;       // ESC 키로 닫기 가능 여부
    public bool pauseGameWhenOpen = false;  // 열릴 때 게임 일시정지 여부

    protected Canvas canvas;                // Canvas 컴포넌트 참조
    protected CanvasGroup canvasGroup;      // CanvasGroup 컴포넌트 참조
    protected bool isInitialized = false;  // 초기화 완료 여부

    // UI 상태 변화 이벤트
    public Action<Base_UI> OnUIOpened;  // UI가 열릴 때 호출되는 이벤트
    public Action<Base_UI> OnUIClosed;  // UI가 닫힐 때 호출되는 이벤트

    /// 초기화되지 않은 경우 Initialize 호출
    protected virtual void Start()
    {
        if (!isInitialized)  // 아직 초기화되지 않았다면
        {
            Initialize(); // 초기화 수행
        }
    }

    #region 초기화 관련
    // UI 초기화 - 상속받은 클래스에서 오버라이드하여 구현
    protected virtual void Initialize()
    {
        //InitializeCanvas();     // Canvas 초기화 (주석 처리됨)
        //InitializeCanvasGroup(); // CanvasGroup 초기화 (주석 처리됨)
        //isInitialized = true;   // 초기화 완료 표시
        //SetUIActive(true);      // UI 활성화
    }

    /// Canvas 컴포넌트 초기화
    // private void InitializeCanvas()
    // {
    //     canvas = GetComponent<Canvas>(); // Canvas 컴포넌트 가져오기
    //     if (canvas == null) // Canvas가 없다면
    //     {
    //         canvas = gameObject.AddComponent<Canvas>(); // Canvas 컴포넌트 추가
    //     }
    //     canvas.sortingOrder = (int)uiLayer; // 레이어에 따른 정렬 순서 설정
    // }

    /// CanvasGroup 컴포넌트 초기화
    // private void InitializeCanvasGroup()
    // {
    //     canvasGroup = GetComponent<CanvasGroup>(); // CanvasGroup 컴포넌트 가져오기
    //     if (canvasGroup == null) // CanvasGroup이 없다면
    //     {
    //         canvasGroup = gameObject.AddComponent<CanvasGroup>(); // CanvasGroup 컴포넌트 추가
    //     }
    // }
    #endregion

    // UI 열기
    public virtual void OpenUI()
    {
        if (!isInitialized) Initialize(); // 초기화되지 않았다면 초기화 수행

        SetUIActive(true);          // UI 활성화
        OnOpen();                   // 열기 시 호출되는 가상 함수 실행
        OnUIOpened?.Invoke(this);   // UI 열림 이벤트 호출

        if (UI_Manager.Instance != null) // UI_Manager가 존재한다면
            UI_Manager.Instance.RegisterOpenedUI(this); // 열린 UI로 등록
    }

    // UI 닫기
    public virtual void CloseUI()
    {
        OnClose();                  // 닫기 시 호출되는 가상 함수 실행
        SetUIActive(false);         // UI 비활성화
        OnUIClosed?.Invoke(this);   // UI 닫힘 이벤트 호출

        if (UI_Manager.Instance != null) // UI_Manager가 존재한다면
            UI_Manager.Instance.UnregisterClosedUI(this); // 열린 UI 목록에서 제거
    }

    // UI 활성화/비활성화 설정
    protected virtual void SetUIActive(bool active)
    {
        gameObject.SetActive(active); // GameObject 활성화/비활성화


    }

    // UI가 열릴 때 호출되는 가상 함수 - 상속받은 클래스에서 오버라이드
    protected virtual void OnOpen() { }

    // UI가 닫힐 때 호출되는 가상 함수 - 상속받은 클래스에서 오버라이드
    protected virtual void OnClose() { }

    // UI가 활성화되어 있는지 확인
    public bool IsActive() { return gameObject.activeInHierarchy; } // GameObject가 활성화되어 있고 계층구조에서 활성화되어 있는지 확인

    // ESC 키 입력을 처리할 수 있는지 확인
    public virtual bool CanHandleEscape()
    {
        return closeOnEscape && IsActive(); // ESC로 닫기가 가능하고 현재 활성화되어 있는지 확인
    }
}