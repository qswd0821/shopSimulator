// Assets/Scripts/UI/MainMenuController.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button btnNewGame;
    [SerializeField] private Button btnContinue;
    [SerializeField] private Button btnOptions;
    [SerializeField] private Button btnExit;

    [Header("Panels")]
    [SerializeField] private GameObject panelOptions;
    [SerializeField] private GameObject panelConfirmQuit;
    [SerializeField] private CanvasGroup fade;   // 선택(없으면 null)

    [Header("Refs")]
    [SerializeField] private SaveManager saveManager;       // 씬에 같은 오브젝트 or 다른 오브젝트에 붙여도 OK
    [SerializeField] private SimpleSceneLoader sceneLoader; // 씬에 아무 오브젝트에 붙여놓고 참조
    [SerializeField] private TMP_Text versionText;

    [Header("Config")]
    [SerializeField] private string gameplaySceneName = "Game";

    void Awake()
    {
        // 패널/텍스트 초기화
        if (versionText) versionText.text = Application.version;
        if (panelOptions) panelOptions.SetActive(false);
        if (panelConfirmQuit) panelConfirmQuit.SetActive(false);
        if (fade) fade.alpha = 1f;

        // 버튼 바인딩
        btnNewGame.onClick.AddListener(OnNewGame);
        btnContinue.onClick.AddListener(OnContinue);
        btnOptions.onClick.AddListener(() => panelOptions?.SetActive(true));
        btnExit.onClick.AddListener(() => panelConfirmQuit?.SetActive(true));
    }

    void Start()
    {
        // 세이브 유무로 Continue 활성화
        bool has = saveManager && saveManager.HasSave();
        if (btnContinue) btnContinue.interactable = has;

        // 페이드 인(선택)
        if (fade) StartCoroutine(CoAlpha(fade, 0f, 0.35f));
    }

    void OnNewGame()
    {
        saveManager.NewGame();
        StartGame();
    }

    void OnContinue()
    {
        saveManager.LoadOrCreateDefault(); // 존재하면 로드, 없으면 생성
        StartGame();
    }

    void StartGame()
    {
        if (fade) StartCoroutine(CoStartWithFade());
        else sceneLoader.Load(gameplaySceneName);
    }

    IEnumerator CoStartWithFade()
    {
        yield return CoAlpha(fade, 1f, 0.25f);
        sceneLoader.Load(gameplaySceneName);
    }

    IEnumerator CoAlpha(CanvasGroup cg, float to, float dur)
    {
        float t = 0f; float from = cg.alpha;
        while (t < dur) { t += Time.unscaledDeltaTime; cg.alpha = Mathf.Lerp(from, to, t / dur); yield return null; }
        cg.alpha = to;
    }

    // 종료 확인 다이얼로그에서 호출할 연결 함수(버튼 OnClick)
    public void OnQuitYes()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    public void OnQuitNo() => panelConfirmQuit?.SetActive(false);
    public void OnOptionsClose() => panelOptions?.SetActive(false);
}
