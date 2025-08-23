using UnityEngine;
using UnityEngine.UI;

public class UI_StageSelect : Base_UI
{
    [System.Serializable]
    public class StageInfo
    {
        public Button stageButton; // 스테이지 선택 버튼
        public int stageNumber; // 스테이지 번호
        public GameObject stagePanel; // 스테이지 상세 정보 패널
        public bool isUnlocked; // 스테이지 잠금 해제 여부
        public Color lockedColor = Color.gray; // 잠긴 상태일 때 색상 (기본값: 회색)
        public Color unlockedColor = Color.white; // 해제된 상태일 때 색상 (기본값: 흰색)
    }


    [Header("Stage Selection")] // 인스펙터에서 헤더 표시
    public StageInfo[] stages; // 모든 스테이지 정보를 담는 배열

    protected override void Initialize() // Base_UI의 Initialize 메서드 오버라이드
    {
        base.Initialize(); // 부모 클래스의 Initialize 먼저 호출
        SetupStageButtons(); // 스테이지 버튼들 설정
        UpdateStageButtons(); // 스테이지 버튼들 상태 업데이트
    }

    private void SetupStageButtons() // 스테이지 버튼들의 클릭 이벤트 설정
    {
        if (stages == null) return; // stages 배열이 null인 경우 리턴

        for (int i = 0; i < stages.Length; i++) // 모든 스테이지를 순회
        {
            if (stages[i].stageButton == null) continue; // 버튼이 null인 경우 건너뛰기

            int stageIndex = i; // 클로저 문제 해결 (람다식에서 변수 캡처 문제 방지)

            // 기존 리스너들 제거 (중복 등록 방지)
            stages[i].stageButton.onClick.RemoveAllListeners();
            // 새 리스너 등록
            stages[i].stageButton.onClick.AddListener(() => OnStageButtonClick(stageIndex)); // 버튼 클릭 이벤트 등록

            Debug.Log($"Stage {i} button listener added"); // 디버그 로그 추가
        }
    }

    private void OnStageButtonClick(int stageIndex) // 스테이지 버튼 클릭 시 호출되는 메서드
    {
        Debug.Log($"Stage {stageIndex} button clicked!"); // 클릭 확인용 디버그 로그

        if (stageIndex < 0 || stageIndex >= stages.Length) return; // 인덱스 범위 확인

        if (stages[stageIndex].isUnlocked) // 해당 스테이지가 잠금 해제되어 있는지 확인
        {
            Debug.Log($"Opening Stage {stageIndex + 1} panel"); // 패널 열기 디버그 로그
            OpenStagePanel(stageIndex); // 잠금 해제된 경우 스테이지 패널 열기
        }
        else // 잠긴 스테이지인 경우
        {
            // 잠긴 스테이지 클릭 시 피드백 (사운드, 애니메이션 등)
            Debug.Log($"Stage {stageIndex + 1} is locked!"); // 콘솔에 잠금 메시지 출력
        }
    }

    private void OpenStagePanel(int stageIndex) // 특정 스테이지 패널을 여는 메서드
    {
        // 모든 패널 닫기
        foreach (var stage in stages) // stages 배열의 모든 스테이지 정보를 순회
        {
            if (stage.stagePanel != null) // 현재 스테이지의 패널이 존재하는지 확인
            {
                UI_StageDetail stageDetailUI = stage.stagePanel.GetComponent<UI_StageDetail>(); // 패널에서 UI_StageDetail 컴포넌트를 가져옴
                if (stageDetailUI != null && stageDetailUI.IsActive()) // UI_StageDetail 컴포넌트가 존재하고 현재 활성화되어 있는지 확인
                    stageDetailUI.CloseUI(); // Base_UI.CloseUI() 메서드를 호출하여 UI를 올바르게 닫음 (스택에서도 제거됨)
            }

            // 선택된 패널 열기
            if (stages[stageIndex].stagePanel != null) // 선택된 스테이지의 패널이 존재하는지 확인
            {
                UI_StageDetail stageDetailUI = stages[stageIndex].stagePanel.GetComponent<UI_StageDetail>(); // 선택된 패널에서 UI_StageDetail 컴포넌트를 가져옴
                if (stageDetailUI != null) // UI_StageDetail 컴포넌트가 존재하는지 확인
                    stageDetailUI.OpenUI(); // Base_UI.OpenUI() 메서드를 호출하여 UI를 올바르게 열음 (스택에도 등록됨)
            }
        }
    }
    private void UpdateStageButtons() // 스테이지 버튼들의 상태를 업데이트하는 메서드
    {
        for (int i = 0; i < stages.Length; i++) // 모든 스테이지를 순회
        {
            // 스테이지 잠금 상태 확인 (GameManager나 SaveData에서)
            stages[i].isUnlocked = CheckStageUnlocked(i); // 각 스테이지의 잠금 해제 상태 확인

            // 버튼 색상 및 상호작용 설정 (올바른 방법)
            Button button = stages[i].stageButton; // 버튼 참조
            if (button != null) // 버튼이 존재하는지 확인
            {
                // Button의 targetGraphic을 통해 올바른 Image 가져오기
                Image targetImage = button.targetGraphic as Image;
                if (targetImage != null) // targetGraphic이 Image인지 확인
                {
                    Color newColor = stages[i].isUnlocked ? stages[i].unlockedColor : stages[i].lockedColor;
                    targetImage.color = newColor; // 색상 설정
                    Debug.Log($"Stage {i} color set to: {newColor}, Alpha: {newColor.a}"); // 디버그 로그
                }

            }

            stages[i].stageButton.interactable = stages[i].isUnlocked; // 버튼의 상호작용 가능 여부 설정
        }
    }

    private bool CheckStageUnlocked(int stageIndex)
    {
        // 첫 번째 스테이지는 항상 해금
        if (stageIndex == 0) return true;
        if (stageIndex == 1) return true;
        if (stageIndex == 2) return true;

        // StageCounter.Instance가 null인지 확인
        if (StageCounter.Instance == null)
        {
            Debug.LogWarning("StageCounter.Instance가 null입니다. StageCounter가 씬에 있는지 확인하세요.");
            return false; // 또는 기본값 반환
        }

        return StageCounter.Instance.IsStageCleared(stageIndex - 1);
    }
}