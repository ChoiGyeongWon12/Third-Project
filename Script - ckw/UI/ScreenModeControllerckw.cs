using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class ScreenModeControllerckw : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown screenModeDropdown;       // 드롭다운 연결

    void Start()
    {   // 두 함수 다 dropdown 전용 함수인듯
        screenModeDropdown.ClearOptions();      // 드롭다운 초기화
        screenModeDropdown.AddOptions(new List<string>       // 드롭다운 옵션 리스트 (새로운 문자열 추가)
        {
            "창 모드",
            "전체화면",
            "창 전체화면"
        });

        SetCurrentDropdownValue();      // 현재 화면 모드에 맞게 드롭다운 값 설정
        screenModeDropdown.onValueChanged.AddListener(OnScreenModeChanged);     // 드롭다운 값 변경 이벤트 연결
    }

    void SetCurrentDropdownValue()      // 현재화면을 확인해서 동기화 (드롭다운에 지금 화면 모드를 출력(표시함))
    {
        switch (Screen.fullScreenMode)      // 현재 화면 모드를 확인해 그 값에 맞게 옵션 리스트 변경 ( 현재 창모드라면 창모드 표시)
        {
            case FullScreenMode.Windowed:       // 창모드
                screenModeDropdown.value = 0;
                break;
            case FullScreenMode.ExclusiveFullScreen:        // 전체화면
                screenModeDropdown.value = 1;
                break;
            case FullScreenMode.FullScreenWindow:       // 창 전체화면
                screenModeDropdown.value = 2;
                break;
        }
    }

    public void OnScreenModeChanged(int index)      // 다른 옵션을 클릭할 때 이벤트가 발생해 값을 전달받아 화면 모드 변경
    {
        switch (index)
        {
            case 0: // 창 모드
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
            case 1: // 전체화면
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case 2: // 창 전체화면
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
        }
    }
}