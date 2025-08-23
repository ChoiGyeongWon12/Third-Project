using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class UI_Lobby : Base_UI
{
    [Header("Lobby UI Buttons")] // 인스펙터에서 버튼 섹션 헤더 표시
    public Button optionButton; // 옵션 창을 열기 위한 버튼
    public Button returnTitleSceneButton; // 타이틀 씬으로 돌아가기 위한 버튼
    public Button dialogueButton; // 대화를 시작할 버튼 추가


    [SerializeField] UI_StageSelect stageSelectUI; // 스테이지 선택 UI 참조 (인스펙터에서 할당)
    [SerializeField] GameObject guideUI; // 가이드 UI 오브젝트 참조 (인스펙터에서 할당)

    [Header("Dialogue System")] // 인스펙터에서 대화 시스템 섹션 헤더 표시
    [SerializeField] DialogueSystem dialogueSystem; // 대화 시스템 스크립트 참조 (인스펙터에서 할당)
    [SerializeField] int dialogueIndex = 0; // 실행할 대화의 인덱스 번호 (기본값 0)

    // UI 초기화 메서드 (Base_UI의 Initialize를 오버라이드)
    protected override void Initialize()
    {
        base.Initialize(); // 부모 클래스의 Initialize 먼저 실행

        // 버튼 이벤트 등록 시작
        if (optionButton != null) // 옵션 버튼이 할당되어 있으면
            optionButton.onClick.AddListener(ToggleOptionUI); // 옵션 UI 토글 메서드를 클릭 이벤트에 등록
        if (returnTitleSceneButton != null) // 타이틀 돌아가기 버튼이 할당되어 있으면
            returnTitleSceneButton.onClick.AddListener(ReturnTitleScene); // 타이틀 씬 전환 메서드를 클릭 이벤트에 등록
        if (dialogueButton != null) // 대화 버튼이 할당되어 있으면
            dialogueButton.onClick.AddListener(StartDialogue); // 대화 시작 메서드를 클릭 이벤트에 등록

        // 대화 시스템 자동 찾기 (만약 할당되지 않았다면)
        if (dialogueSystem == null) // 대화 시스템이 할당되지 않았으면
        {
            dialogueSystem = FindAnyObjectByType<DialogueSystem>(); // 씬에서 DialogueSystem 컴포넌트를 찾아서 할당
            if (dialogueSystem == null) // 그래도 찾지 못했으면
            {
                Debug.LogWarning("DialogueSystem을 찾을 수 없습니다!"); // 경고 메시지 출력
            }
        }
    }

    // 옵션 UI를 열거나 닫는 메서드
    private void ToggleOptionUI()
    {
        var optionUI = FindAnyObjectByType<Option_UI>(FindObjectsInactive.Include); // 비활성화된 오브젝트까지 포함해서 Option_UI 찾기

        if (optionUI.IsActive()) // UI 매니저에서 옵션 UI가 열려있는지 확인
            optionUI.CloseUI(); // 열려있으면 닫기
        else
            optionUI.OpenUI(); // 닫혀있으면 열기
    }

    // 타이틀 씬으로 돌아가는 메서드
    private void ReturnTitleScene()
    {
        SceneTransitionManager.LoadScene("TEST_title1"); // 씬 전환 매니저를 통해 타이틀 씬으로 전환
    }

    // 대화를 시작하는 메서드
    private void StartDialogue()
    {
        if (dialogueSystem != null) // 대화 시스템이 할당되어 있으면
        {
            // 이미 대화 중이면 무시
            if (dialogueSystem.IsDialogueActive()) // 현재 대화가 진행 중인지 확인
            {
                Debug.Log("이미 대화가 진행 중입니다."); // 디버그 메시지 출력
                return; // 메서드 종료
            }

            // 대화 시작
            dialogueSystem.StartDialogue(dialogueIndex); // 설정된 인덱스의 대화 시작

            // 대화 중에는 버튼 비활성화 (선택사항)
            if (dialogueButton != null) // 대화 버튼이 할당되어 있으면
            {
                dialogueButton.interactable = false; // 버튼 비활성화 (클릭 불가능하게 만듦)

                // 대화 종료 후 버튼 다시 활성화를 위한 코루틴 시작
                StartCoroutine(WaitForDialogueEnd()); // 대화 종료 대기 코루틴 실행
            }
        }
        else
        {
            Debug.LogError("DialogueSystem이 설정되지 않았습니다!"); // 에러 메시지 출력
        }
    }

    // 대화 종료를 기다리는 코루틴
    private IEnumerator WaitForDialogueEnd()
    {
        // 대화가 끝날 때까지 기다리기
        while (dialogueSystem != null && dialogueSystem.IsDialogueActive()) // 대화 시스템이 있고 대화가 진행 중인 동안 반복
        {
            yield return null; // 한 프레임 기다리기
        }

        // 대화 종료 후 버튼 다시 활성화
        if (dialogueButton != null) // 대화 버튼이 할당되어 있으면
        {
            dialogueButton.interactable = true; // 버튼 다시 활성화 (클릭 가능하게 만듦)
        }
    }


}