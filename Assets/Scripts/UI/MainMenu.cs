using System.Collections; // Coroutine을 위해 필요
using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리를 위해 필요
using UnityEngine.UI; // Slider를 위해 필요
using TMPro; // TextMeshPro를 위해 필요

public class MainMenu : MonoBehaviour
{
    // --- 1단계에서 만든 UI 요소들을 Inspector 창에서 연결 ---
    [Header("Loading Panel")]
    public GameObject panelLoading; // Panel_Loading 오브젝트
    public Slider sliderProgressBar; // Slider_ProgressBar 슬라이더
    public TMP_Text textLoading; // Text_Loading 텍스트

    // --- 기존 버튼 함수들 ---

    /// <summary>
    /// '새 게임' 버튼을 클릭했을 때 호출될 함수
    /// </summary>
    public void OnClickNewGame()
    {
        // "InGameScene"이라는 이름의 씬을 불러옵니다.
        // Build Settings의 인덱스(숫자)로도 가능합니다. (예: LoadScene(1))
        StartLoading("InGameScene");
    }

    /// <summary>
    /// '이어하기' 버튼을 클릭했을 때 호출될 함수
    /// </summary>
    public void OnClickContinue()
    {
        // TODO: 실제로는 저장된 씬 이름을 불러와야 함
        StartLoading("InGameScene");
    }

    // --- 4단계: 로딩 코루틴 (핵심) ---

    private void StartLoading(string sceneName)
    {
        // 1. 로딩 패널을 활성화
        panelLoading.SetActive(true);

        // 2. 코루틴을 시작
        StartCoroutine(LoadSceneAsyncCoroutine(sceneName));
    }

    /// <summary>
    /// 씬을 비동기(Async)로 불러오는 코루틴
    /// </summary>
    private IEnumerator LoadSceneAsyncCoroutine(string sceneName)
    {
        // yield return null; // 한 프레임 대기 (패널이 먼저 보이도록)

        // 1. 씬을 비동기로 로드 시작
        AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneName);

        // 2. allowSceneActivation = false: 로딩이 끝나도 씬이 바로 넘어가지 않게 함 (90%에서 멈춤)
        asyncOp.allowSceneActivation = false;

        float timer = 0f;

        // 3. 씬 로딩이 90%가 될 때까지 반복
        while (!asyncOp.isDone)
        {
            // asyncOp.progress 값은 0.0 ~ 0.9 까지 오름
            float progress = Mathf.Clamp01(asyncOp.progress / 0.9f);

            // 슬라이더와 텍스트 업데이트
            sliderProgressBar.value = progress;
            textLoading.text = $"로딩 중... {progress * 100:0}%";

            // 4. 로딩이 90% (progress 1.0)가 되면
            if (asyncOp.progress >= 0.9f)
            {
                // "로딩 완료! (클릭하여 계속)" 같은 텍스트로 변경 가능
                textLoading.text = "로딩 완료!";

                // 3초간 추가 대기 (선택 사항: 너무 빨리 넘어가는 것을 방지)
                timer += Time.deltaTime;
                if (timer >= 3.0f)
                {
                    // 5. 씬 활성화를 허용하여 씬을 전환
                    asyncOp.allowSceneActivation = true;
                }
            }

            yield return null; // 다음 프레임까지 대기
        }
    }
}