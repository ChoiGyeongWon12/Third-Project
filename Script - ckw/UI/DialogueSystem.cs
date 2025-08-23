using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

// 대화 데이터를 저장하는 직렬화 가능한 클래스
[System.Serializable]
public class DialogueData
{
    public string characterName; // 캐릭터 이름
    public string[] sentences; // 대화 문장들 배열
}

// 대화 시스템을 관리하는 메인 클래스
public class DialogueSystem : MonoBehaviour
{
    [Header("UI Elements")] // 인스펙터에서 UI 요소 섹션 헤더 표시
    public GameObject dialoguePanel; // 대화창 패널 오브젝트 참조
    public TextMeshProUGUI nameText; // 캐릭터 이름을 표시할 텍스트 컴포넌트
    public TextMeshProUGUI dialogueText; // 대화 내용을 표시할 텍스트 컴포넌트
    public Button nextButton; // 다음 문장으로 넘어가는 버튼
    public Button closeButton; // 대화창을 닫는 버튼

    [Header("Dialogue Settings")] // 인스펙터에서 대화 설정 섹션 헤더 표시
    public float typingSpeed = 0.05f; // 타이핑 효과의 속도 (문자 하나당 대기 시간)
    public DialogueData[] dialogues; // 대화 데이터들을 저장하는 배열

    private Queue<string> sentences = new Queue<string>(); // 현재 대화의 문장들을 순서대로 저장하는 큐
    private bool isTyping = false; // 현재 타이핑 효과가 진행 중인지 확인하는 플래그
    private bool isDialogueActive = false; // 현재 대화가 활성화되어 있는지 확인하는 플래그
    private Coroutine typingCoroutine; // 타이핑 효과를 처리하는 코루틴 참조
    private string currentSentence = ""; // 현재 타이핑 중인 문장을 저장

    // 게임 시작 시 초기 설정을 수행하는 메서드
    void Start()
    {
        // 초기 설정
        if (dialoguePanel != null) // 대화창 패널이 할당되어 있으면
            dialoguePanel.SetActive(false); // 대화창을 비활성화 상태로 설정

        // 버튼 이벤트 연결
        if (nextButton != null) // 다음 버튼이 할당되어 있으면
            nextButton.onClick.AddListener(DisplayNextSentence); // 다음 문장 표시 메서드를 클릭 이벤트에 등록

        if (closeButton != null) // 닫기 버튼이 할당되어 있으면
            closeButton.onClick.AddListener(CloseDialogue); // 대화창 닫기 메서드를 클릭 이벤트에 등록
    }

    // 대화를 시작하는 메서드
    public void StartDialogue(int dialogueIndex = 0)
    {
        if (dialogueIndex >= dialogues.Length || dialogues[dialogueIndex] == null) // 유효하지 않은 인덱스이거나 해당 대화 데이터가 null이면
        {
            Debug.LogError("유효하지 않은 대화 인덱스입니다!"); // 에러 메시지 출력
            return; // 메서드 종료
        }

        isDialogueActive = true; // 대화 활성화 플래그를 true로 설정

        // 대화창 활성화
        if (dialoguePanel != null) // 대화창 패널이 할당되어 있으면
            dialoguePanel.SetActive(true); // 대화창을 활성화

        // 대화 데이터 설정
        DialogueData dialogue = dialogues[dialogueIndex]; // 지정된 인덱스의 대화 데이터 가져오기

        // 캐릭터 이름 설정
        if (nameText != null) // 이름 텍스트가 할당되어 있으면
            nameText.text = dialogue.characterName; // 대화 데이터의 캐릭터 이름으로 설정

        // 문장 큐 초기화
        sentences.Clear(); // 기존 문장 큐를 비우기

        // 문장들을 큐에 추가
        foreach (string sentence in dialogue.sentences) // 대화 데이터의 모든 문장에 대해 반복
        {
            sentences.Enqueue(sentence); // 각 문장을 큐에 추가
        }

        // 첫 번째 문장 표시
        DisplayNextSentence(); // 첫 번째 문장 표시 메서드 호출
    }

    // 다음 문장을 표시하는 메서드
    public void DisplayNextSentence()
    {
        // 현재 타이핑 중이면 타이핑을 즉시 완료
        if (isTyping) // 현재 타이핑 효과가 진행 중이면
        {
            CompleteTyping(); // 타이핑 완료 메서드 호출
            return; // 메서드 종료
        }

        // 더 이상 문장이 없으면 대화 종료
        if (sentences.Count == 0) // 큐에 남은 문장이 없으면
        {
            EndDialogue(); // 대화 종료 메서드 호출
            return; // 메서드 종료
        }

        // 다음 문장 가져오기
        string sentence = sentences.Dequeue(); // 큐에서 다음 문장을 꺼내기
        currentSentence = sentence; // 현재 문장 저장

        // 타이핑 효과로 문장 표시
        if (typingCoroutine != null) // 이전 타이핑 코루틴이 실행 중이면
        {
            StopCoroutine(typingCoroutine); // 이전 코루틴 중지
        }
        typingCoroutine = StartCoroutine(TypeSentence(sentence)); // 새로운 타이핑 코루틴 시작
    }

    // 타이핑 효과를 처리하는 코루틴
    private IEnumerator TypeSentence(string sentence)
    {
        isTyping = true; // 타이핑 진행 중 플래그를 true로 설정
        dialogueText.text = ""; // 대화 텍스트를 빈 문자열로 초기화

        foreach (char letter in sentence.ToCharArray()) // 문장의 각 문자에 대해 반복
        {
            dialogueText.text += letter; // 현재 텍스트에 문자 하나씩 추가
            yield return new WaitForSeconds(typingSpeed); // 설정된 타이핑 속도만큼 대기
        }

        isTyping = false; // 타이핑 완료 후 플래그를 false로 설정
    }

    // 타이핑을 즉시 완료하는 메서드
    private void CompleteTyping()
    {
        if (typingCoroutine != null) // 타이핑 코루틴이 실행 중이면
        {
            StopCoroutine(typingCoroutine); // 코루틴 중지
            typingCoroutine = null; // 코루틴 참조를 null로 설정
        }

        isTyping = false; // 타이핑 플래그를 false로 설정
        dialogueText.text = currentSentence; // 현재 문장을 완전히 표시
    }

    // 대화창을 닫는 메서드 (진행 중인 대화 초기화)
    public void CloseDialogue()
    {
        // 타이핑 코루틴 정리
        if (typingCoroutine != null) // 타이핑 코루틴이 실행 중이면
        {
            StopCoroutine(typingCoroutine); // 코루틴 중지
            typingCoroutine = null; // 코루틴 참조를 null로 설정
        }

        // 대화 상태 초기화
        isDialogueActive = false; // 대화 활성화 플래그를 false로 설정
        isTyping = false; // 타이핑 플래그를 false로 설정
        sentences.Clear(); // 문장 큐 초기화
        currentSentence = ""; // 현재 문장 초기화

        // 대화창 비활성화
        if (dialoguePanel != null) // 대화창 패널이 할당되어 있으면
            dialoguePanel.SetActive(false); // 대화창을 비활성화

        // 텍스트 초기화
        if (dialogueText != null)
            dialogueText.text = "";
        if (nameText != null)
            nameText.text = "";

        Debug.Log("대화창이 닫혔습니다. 진행 중인 대화가 초기화되었습니다."); // 디버그 메시지 출력
    }

    // 대화를 종료하는 메서드 (자연스러운 종료)
    public void EndDialogue()
    {
        isDialogueActive = false; // 대화 활성화 플래그를 false로 설정

        // 대화창 비활성화
        if (dialoguePanel != null) // 대화창 패널이 할당되어 있으면
            dialoguePanel.SetActive(false); // 대화창을 비활성화

        // 타이핑 코루틴 정리
        if (typingCoroutine != null) // 타이핑 코루틴이 실행 중이면
        {
            StopCoroutine(typingCoroutine); // 코루틴 중지
            typingCoroutine = null; // 코루틴 참조를 null로 설정
        }

        Debug.Log("대화가 종료되었습니다."); // 대화 종료 디버그 메시지 출력
    }

    // 현재 대화가 활성화되어 있는지 확인하는 메서드
    public bool IsDialogueActive()
    {
        return isDialogueActive; // 대화 활성화 플래그 반환
    }
}