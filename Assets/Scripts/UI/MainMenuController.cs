// Assets/Scripts/UI/MainMenuController.cs

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; // (추가!) 씬 관리를 위해 필요

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
    [SerializeField] private CanvasGroup fade;    // 선택(없으면 null)

    // (추가!) 로딩 패널 변수들
    [Header("Loading Panel")]
    [SerializeField] private GameObject loadingBG;
    [SerializeField] private GameObject panelLoading;
    [SerializeField] private Slider sliderProgressBar;
    [SerializeField] private TMP_Text textLoading;

    [Header("Refs")]
    [SerializeField] private SaveManager saveManager;      // 씬에 같은 오브젝트 or 다른 오브젝트에 붙여도 OK
    [SerializeField] private SimpleSceneLoader sceneLoader; // (이제 사용되지 않을 수 있음)
    [SerializeField] private TMP_Text versionText;

    [Header("Config")]
    [SerializeField] private string gameplaySceneName = "ShopSimulator"; // (중요!) "InGameScene" 등 실제 씬 이름으로 변경하세요

    void Awake()
    {
        // 패널/텍스트 초기화
        if (versionText) versionText.text = Application.version;
        if (panelOptions) panelOptions.SetActive(false);
        if (panelConfirmQuit) panelConfirmQuit.SetActive(false);
        if (fade) fade.alpha = 1f;

        // (추가!) 로딩 패널 초기화
        if (loadingBG) loadingBG.SetActive(false);
        if (panelLoading) panelLoading.SetActive(false);

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
        // (수정!) 씬 로더 대신 로딩 스크린을 호출하도록 변경
        if (fade) StartCoroutine(CoStartWithFade());
        else StartLoading(gameplaySceneName);
    }

    IEnumerator CoStartWithFade()
    {
        yield return CoAlpha(fade, 1f, 0.25f);
        // (수정!) 씬 로더 대신 로딩 스크린을 호출
        StartLoading(gameplaySceneName);
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


    // --- (추가!) 로딩 스크린 관련 함수들 ---

    /// <summary>
    /// 로딩 패널과 배경을 모두 켜고 코루틴 시작
    /// </summary>
    private void StartLoading(string sceneName)
    {
        // 1. 배경(BG)과 패널(Panel)을 둘 다 활성화
        if (loadingBG != null)
            loadingBG.SetActive(true);
        if (panelLoading != null)
            panelLoading.SetActive(true);

        // 2. 코루틴을 시작
        StartCoroutine(LoadSceneAsyncCoroutine(sceneName));
    }

    /// <summary>
    /// 씬을 비동기(Async)로 불러오는 코루틴
    /// </summary>
    private IEnumerator LoadSceneAsyncCoroutine(string sceneName)
    {
        AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneName);
        asyncOp.allowSceneActivation = false;

        // 1. 씬 로딩이 90%가 될 때까지 반복
        while (asyncOp.progress < 0.9f)
        {
            float progress = Mathf.Clamp01(asyncOp.progress / 0.9f);

            if (sliderProgressBar != null)
                sliderProgressBar.value = progress;
            if (textLoading != null)
                textLoading.text = $"Loading... {progress * 100:0}%";

            yield return null; // 다음 프레임까지 대기
        }

        // --- 90% 로딩 완료 ---
        if (sliderProgressBar != null)
            sliderProgressBar.value = 1.0f;
        if (textLoading != null)
            textLoading.text = "Loading Complete!";

        // (선택 사항) N초간 대기
        yield return new WaitForSeconds(1.0f); // 3초가 길면 1초 정도로 조절

        // 씬 전환
        asyncOp.allowSceneActivation = true;
    }
}