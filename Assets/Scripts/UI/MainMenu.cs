using UnityEngine;
using UnityEngine.SceneManagement; // 씬 로딩에 필요
using UnityEngine.UI; // 버튼, 패널 등에 필요

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject panelOptions;

    [SerializeField] private GameObject panelConfirmQuit;

    [SerializeField] private Button continueButton;


    void Start()
    {
        if (panelOptions != null) panelOptions.SetActive(false);
        if (panelConfirmQuit != null) panelConfirmQuit.SetActive(false);

        if (continueButton != null)
        {
            if (SaveManager.Instance != null && SaveManager.Instance.HasSaveData())
            {
                continueButton.interactable = true;
            }
            else
            {
                continueButton.interactable = false;
            }
        }
    }

    // --- 아래 함수들을 버튼의 OnClick() 이벤트에 연결 ---
    public void OnClickNewGame()
    {

        if (SceneLoader.Instance != null)
        {
            // (나중에 인게임 씬 이름이 바뀔 수 있으니 여기서 관리)
            SceneLoader.Instance.LoadSceneAsync("Main");
        }
        else
        {
            Debug.LogError("SceneLoader가 없습니다!");
        }
    }

    public void OnClickContinue()
    {
        if (SceneLoader.Instance != null && SaveManager.Instance != null)
        {
            GameData data = SaveManager.Instance.LoadGame();
            if (data != null)
            {
                // 씬 로더가 로드할 씬 이름을 가져옴
                SceneLoader.Instance.LoadSceneAsync(data.lastSavedSceneName);
                // 씬 로더에 '로드할 데이터'를 임시 저장
                SceneLoader.Instance.SetDataToLoad(data);
            }
            else
            {
                Debug.LogWarning("이어하기 버튼이 눌렸으나 로드할 데이터가 없습니다.");
                continueButton.interactable = false; 
            }
        }
        else
        {
            Debug.LogError("SceneLoader 또는 SaveManager가 없습니다!");
        }
    }

    public void OnClickOptions()
    {
        if (panelOptions != null)
        {
            panelOptions.SetActive(true);
        }
    }

    public void OnClickOpenQuitConfirm()
    {
        if (panelConfirmQuit != null)
        {
            panelConfirmQuit.SetActive(true);
        }
    }


    public void OnClickDoQuit()
    {
        Debug.Log("게임 종료!");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void OnClickCloseQuitConfirm()
    {
        if (panelConfirmQuit != null)
        {
            panelConfirmQuit.SetActive(false);
        }
    }
}