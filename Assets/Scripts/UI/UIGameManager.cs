using UnityEngine;
using System;
using UnityEngine.SceneManagement; // 씬 관리를 위해 필요 (Save/Load용)

// 게임의 핵심 상태 (예시)
public enum GameState { Playing, Paused }

/// <summary>
/// UI와 핵심 게임 로직(돈, 시간)을 관리하고,
/// Save/Load를 트리거하는 싱글톤 매니저입니다.
/// </summary>
public class UIGameManager : MonoBehaviour
{
    // --- 1. 싱글톤 ---
    public static UIGameManager Instance { get; private set; }

    [Header("핵심 데이터")]
    public int Money; // 현재 소지금
    public int CurrentDay; // 현재 날짜 (Day 1, Day 2...)
    public float GameTime; // 현재 시간 (예: 9.0f = 09:00, 18.5f = 18:30)
    public GameState CurrentState;

    // 2. '방송국 (API)': 다른 시스템(UI 등)이 구독할 이벤트
    public event Action<int> OnMoneyChanged;
    public event Action<float> OnTimeChanged;

    void Awake()
    {
        // 싱글톤 설정
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("UIGameManager 인스턴스가 2개 이상 감지됨. 이 오브젝트를 파괴합니다.");
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            // (참고: UIGameManager도 DontDestroyOnLoad가 필요할 수 있습니다)
            // DontDestroyOnLoad(gameObject); 
        }
    }

    void Start()
    {
        // --- (2단계에서 필요한 '이어하기'/'새 게임' 구분 로직) ---

        // 1. SceneLoader가 '이어하기' 데이터를 가지고 있는지 확인
        GameData dataToLoad = null;
        if (SceneLoader.Instance != null)
        {
            dataToLoad = SceneLoader.Instance.GetDataToLoad();
        }

        // 2. 데이터가 있으면 (Continue)
        if (dataToLoad != null)
        {
            Debug.Log("<color=cyan>UIGameManager: '이어하기' 데이터를 적용합니다.</color>");

            // 로드한 데이터를 현재 상태에 적용
            this.CurrentDay = dataToLoad.currentDay;
            this.GameTime = dataToLoad.gameTime;
            this.Money = 0; // 0으로 초기화 후 ChangeMoney로 방송
            ChangeMoney(dataToLoad.money);

            OnTimeChanged?.Invoke(this.GameTime); // 시간 UI도 업데이트
            CurrentState = GameState.Playing;
        }
        // 3. 데이터가 없으면 (New Game)
        else
        {
            Debug.Log("<color=green>UIGameManager: '새 게임'으로 시작합니다.</color>");

            // 기존의 '새 게임' 초기값 설정
            CurrentDay = 1;
            GameTime = 9.0f; // 오전 9시
            ChangeMoney(50000); // 초기 자금 5만원
            CurrentState = GameState.Playing;
        }
        // --- ---
    }

    void Update()
    {
        // 게임 플레이 중일 때만 시간 흐르기
        if (CurrentState == GameState.Playing)
        {
            GameTime += Time.deltaTime * 0.1f; // (속도 조절 필요!)
            OnTimeChanged?.Invoke(GameTime); // 시간 변경 '방송'
        }

        // --- (1단계) Save/Load 테스트용 코드 ---

        // F5 키를 누르면 저장
        if (Input.GetKeyDown(KeyCode.F5))
        {
            TriggerSaveGame();
        }

        // F9 키를 누르면 로드 (테스트용)
        if (Input.GetKeyDown(KeyCode.F9))
        {
            TriggerLoadGame();
        }
    }

    /// <summary>
    /// 돈을 변경하고 UI에 즉시 방송합니다.
    /// </summary>
    public void ChangeMoney(int amount)
    {
        Money += amount;
        OnMoneyChanged?.Invoke(Money);
    }

    // --- 4. Save/Load 실행 함수 ---

    /// <summary>
    /// 현재 UIGameManager의 상태를 모아서 SaveManager에게 저장을 요청합니다.
    /// </summary>
    public void TriggerSaveGame()
    {
        // SaveManager가 없으면 저장 불가
        if (SaveManager.Instance == null)
        {
            Debug.LogError("SaveManager.Instance가 없습니다! Hierarchy에 @SaveManager가 있는지 확인하세요.");
            return;
        }

        Debug.Log("저장 시도 (UIGameManager)...");

        // 1. 저장할 데이터를 GameData 객체에 채워 넣습니다.
        GameData dataToSave = new GameData();

        dataToSave.money = this.Money;
        dataToSave.currentDay = this.CurrentDay;
        dataToSave.gameTime = this.GameTime;
        dataToSave.lastSavedSceneName = SceneManager.GetActiveScene().name;

        // (TODO: 플레이어 위치 등 팀원의 GameManager가 가진 데이터도 저장)
        // (예: if (Shared.GameManager != null && Shared.GameManager.Player != null) ... )

        // 2. SaveManager에게 이 데이터를 저장하라고 명령합니다.
        SaveManager.Instance.SaveGame(dataToSave);
    }

    /// <summary>
    /// SaveManager로부터 데이터를 로드하여 UIGameManager에 적용합니다.
    /// (F9 테스트 또는 이어하기 후 씬 로드 시 사용)
    /// </summary>
    public void TriggerLoadGame()
    {
        if (SaveManager.Instance == null)
        {
            Debug.LogError("SaveManager.Instance가 없습니다!");
            return;
        }

        Debug.Log("로드 시도 (UIGameManager)...");

        // 1. SaveManager에게 데이터를 로드해달라고 요청합니다.
        GameData loadedData = SaveManager.Instance.LoadGame();

        // 2. 로드 성공 여부 확인
        if (loadedData == null)
        {
            Debug.LogWarning("로드할 데이터가 없습니다.");
            return;
        }

        // 3. (중요!) 다른 씬의 데이터인지 확인
        string currentScene = SceneManager.GetActiveScene().name;
        if (loadedData.lastSavedSceneName != currentScene)
        {
            if (SceneLoader.Instance == null)
            {
                Debug.LogError("SceneLoader.Instance가 없습니다!");
                return;
            }

            Debug.Log($"다른 씬({loadedData.lastSavedSceneName})의 데이터이므로, 씬을 먼저 로드합니다.");

            // SceneLoader가 이 loadedData를 '들고' 씬을 전환하게 함
            SceneLoader.Instance.SetDataToLoad(loadedData);
            SceneLoader.Instance.LoadSceneAsync(loadedData.lastSavedSceneName);
            return;
        }

        // 4. (같은 씬) 로드한 데이터를 현재 UIGameManager 상태에 적용합니다.
        this.Money = 0; // 0으로 초기화 후 ChangeMoney
        ChangeMoney(loadedData.money);

        this.CurrentDay = loadedData.currentDay;
        this.GameTime = loadedData.gameTime;
        OnTimeChanged?.Invoke(this.GameTime); // 시간도 방송

        // (TODO: 플레이어 위치 등 다른 데이터도 적용)
        // if (Shared.GameManager != null && Shared.GameManager.Player != null) ...

        Debug.Log("UIGameManager 상태 적용 완료!");
    }
}