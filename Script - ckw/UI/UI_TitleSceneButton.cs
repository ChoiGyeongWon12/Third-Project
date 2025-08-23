#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;


public class UI_TitleSceneButton : Base_UI
{
    [Header("UI References")]
    [SerializeField] Button startButton;
    //[SerializeField] Button 
    [SerializeField] Button optionButton;
    [SerializeField] Button quitButton;

    protected override void Initialize()
    {
        base.Initialize();

        if (startButton != null)
            startButton.onClick.AddListener(StartGame);
        if (optionButton != null)
            optionButton.onClick.AddListener(OptionUIOpen);
        if (quitButton != null)
            quitButton.onClick.AddListener(OnClickExit);

    }

    private void StartGame()
    {
        SceneTransitionManager.LoadScene("TEST_lobby1");
    }

    private void OptionUIOpen()
    {
        var option = FindAnyObjectByType<Option_UI>(FindObjectsInactive.Include);
        option.OpenUI();
    }


    private void OnClickExit()
    {
#if UNITY_EDITOR    
        EditorApplication.isPlaying = false;        // 유니티 에디터 상에서만 실행
#else       
        Application.Quit();     // 실제 빌드상태에서
#endif

    }



}
