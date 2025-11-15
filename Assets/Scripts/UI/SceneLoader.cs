using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 씬 로딩을 전문적으로 관리하는 싱글톤 클래스입니다.
/// 이 스크립트가 붙은 오브젝트(@SceneLoader)는
/// 씬이 바뀌어도 파괴되지 않습니다.
/// </summary>
public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    [Header("Loading Panel UI")]
    // (참고: 이 UI 오브젝트들은 @SceneLoader의 자식이어야 합니다)
    [SerializeField] private GameObject panelLoading;
    [SerializeField] private Slider sliderProgressBar;
    [SerializeField] private TMP_Text textLoading;
    [Tooltip("로딩이 너무 빨리 끝나도 최소한 이 시간(초)만큼은 로딩 화면을 보여줍니다.")]
    [SerializeField] private float minLoadTime = 3.0f;

    private GameData dataToLoadOnSceneStart = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // @SceneLoader 오브젝트를 유지
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 로딩 패널은 시작할 때 무조건 꺼둡니다.
        if (panelLoading != null)
        {
            panelLoading.SetActive(false);
        }
    }

    /// <summary>
    /// 지정된 씬을 비동기 로드합니다.
    /// </summary>
    public void LoadSceneAsync(string sceneName)
    {
        if (panelLoading == null || sliderProgressBar == null || textLoading == null)
        {
            Debug.LogError("SceneLoader에 UI 요소들이 연결되지 않았습니다! Inspector 창을 확인해주세요.");
            SceneManager.LoadScene(sceneName); // UI 없이 비상 로드
            return;
        }

        dataToLoadOnSceneStart = null;

        // 1. (단순하게) 로딩 패널을 활성화합니다.
        panelLoading.SetActive(true);

        // (복잡한 SetParent, AddComponent 코드 모두 삭제)

        // 2. 코루틴을 시작합니다.
        StartCoroutine(LoadSceneAsyncCoroutine(sceneName));
    }

    public void SetDataToLoad(GameData data)
    {
        dataToLoadOnSceneStart = data;
    }

    public GameData GetDataToLoad()
    {
        GameData data = dataToLoadOnSceneStart;
        dataToLoadOnSceneStart = null;
        return data;
    }

    /// <summary>
    /// 씬을 비동기(Async)로 불러오는 코루틴
    /// </summary>
    private IEnumerator LoadSceneAsyncCoroutine(string sceneName)
    {
        AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneName);

        asyncOp.allowSceneActivation = false;

        float timer = 0f;
        float progress = 0f;

        while (!asyncOp.isDone)
        {
            progress = Mathf.Clamp01(asyncOp.progress / 0.9f);

            // (null 체크)
            if (sliderProgressBar != null)
                sliderProgressBar.value = progress;
            if (textLoading != null)
                textLoading.text = $"Loading... {progress * 100:0}%";

            if (asyncOp.progress >= 0.9f)
            {
                if (textLoading != null)
                    textLoading.text = "Loading Complete!";

                timer += Time.unscaledDeltaTime;
                if (timer >= minLoadTime)
                {
                    asyncOp.allowSceneActivation = true;

                    yield return new WaitForSeconds(0.5f);

                    // (null 체크)
                    if (panelLoading != null)
                    {
                        // 씬 전환이 끝났으니 패널을 다시 비활성화
                        panelLoading.SetActive(false);
                    }
                    yield break;
                }
            }

            yield return null;
        }
    }
}