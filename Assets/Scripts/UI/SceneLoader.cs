using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.UI;
using TMPro;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    [Header("Loading Panel UI")]
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
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (panelLoading != null)
        {
            panelLoading.SetActive(false);
        }
    }

    public void LoadSceneAsync(string sceneName)
    {
        if (panelLoading == null || sliderProgressBar == null || textLoading == null)
        {
            Debug.LogError("SceneLoader에 UI 요소들이 연결되지 않았습니다! Inspector 창을 확인해주세요.");
            SceneManager.LoadScene(sceneName);
            return;
        }

        dataToLoadOnSceneStart = null;

        panelLoading.SetActive(true);

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

    private IEnumerator LoadSceneAsyncCoroutine(string sceneName)
    {
        AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneName);

        asyncOp.allowSceneActivation = false;

        float timer = 0f;
        float progress = 0f;

        while (!asyncOp.isDone)
        {
            progress = Mathf.Clamp01(asyncOp.progress / 0.9f);

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
                    panelLoading.SetActive(false);
                    yield break;
                }
            }

            yield return null;
        }
    }
}