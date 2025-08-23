using UnityEngine;
using UnityEngine.UI;

public class UI_StageDetail : Base_UI
{
    [Header("Stage Detail")]
    public Button closeButton; // 닫기 버튼 참조
    public Button startButton; // 시작 버튼 참조

    [Header("Stage Settings")]
    [SerializeField] private string targetSceneName = ""; // Inspector에서 각 스테이지마다 다르게 설정

    protected override void Initialize() // Base_UI의 Initialize 메서드를 오버라이드
    {
        base.Initialize(); // 부모 클래스의 Initialize 메서드 먼저 호출

        if (closeButton != null) // closeButton이 할당되어 있는지 확인
            closeButton.onClick.AddListener(CloseUI); // closeButton 클릭 시 CloseUI 메서드 호출하도록 리스너 추가

        if (startButton != null) // startButton이 할당되어 있는지 확인
            startButton.onClick.AddListener(StartStage); // startButton 클릭 시 StartStage 메서드 호출하도록 리스너 추가
    }


    private void StartStage() // 스테이지 시작 버튼 클릭 시 호출되는 메서드
    {
        if (!string.IsNullOrEmpty(targetSceneName)) // targetSceneName이 null이 아니고 빈 문자열이 아닌지 확인
        {
            SceneTransitionManager.LoadScene(targetSceneName); // 설정된 씬 이름으로 씬 전환 실행
        }
        else // targetSceneName이 설정되지 않았거나 빈 문자열인 경우
        {
            Debug.LogWarning("Target scene name is not set!"); // 콘솔에 경고 메시지 출력
        }
    }
}