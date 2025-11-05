using UnityEngine;
using System; // C# 이벤트(Action)를 사용하기 위해 필요

// 게임의 핵심 상태 (예시)
public enum GameState { Playing, Paused }

public class UIManager : MonoBehaviour
{
    // 1. 싱글톤(Singleton) 패턴: 씬 어디에서나 UIManager.Instance로 쉽게 접근 가능
    public static UIManager Instance { get; private set; }

    [Header("핵심 데이터")]
    public int Money; // 현재 소지금
    public int CurrentDay; // 현재 날짜 (Day 1, Day 2...)
    public float GameTime; // 현재 시간 (예: 9.0f = 09:00, 18.5f = 18:30)
    public GameState CurrentState;

    // 2. '방송국 (API)': 다른 시스템(UI 등)이 구독할 이벤트
    // 돈이 변경되면 int(새 금액) 값을 방송
    public event Action<int> OnMoneyChanged;
    // 시간이 변경되면 float(새 시간) 값을 방송
    public event Action<float> OnTimeChanged;

    void Awake()
    {
        // 싱글톤 설정
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    void Start()
    {
        // 게임 시작 시 초기값 설정
        CurrentDay = 1;
        GameTime = 9.0f; // 오전 9시
        ChangeMoney(50000); // 초기 자금 5만원 (ChangeMoney를 써야 방송이 됨)
        CurrentState = GameState.Playing;
    }

    void Update()
    {
        // 게임 플레이 중일 때만 시간 흐르기 (예시)
        if (CurrentState == GameState.Playing)
        {
            GameTime += Time.deltaTime * 0.1f; // (속도 조절 필요!)
            OnTimeChanged?.Invoke(GameTime); // 시간 변경 '방송'
        }
    }

    // 3. '핵심 API 함수': 3번 팀원(편의점 시스템)이 이 함수를 호출할 겁니다.

    /// <summary>
    /// 돈을 변경하고 UI에 즉시 방송합니다.
    /// (양수: 벌기, 음수: 쓰기)
    /// </summary>
    public void ChangeMoney(int amount)
    {
        Money += amount;

        // OnMoneyChanged 이벤트에 '구독'한 모든 대상(UI)에게
        // 변경된 Money 값을 알림 (방송)
        OnMoneyChanged?.Invoke(Money);
    }
}