using UnityEngine;
using TMPro; // TextMeshPro 사용
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{
    [Header("HUD 요소")]
    public TMP_Text moneyText; // 돈 텍스트 (Inspector에서 연결)
    public TMP_Text timeText;  // 시간 텍스트 (Inspector에서 연결)

    // (추가) 일시정지 패널
    // public GameObject pausePanel; 

    // --- '시청자'가 '방송국'을 구독 신청 ---
    void OnEnable()
    {
        // UIManager가 존재할 때만 구독 신청
        if (UIManager.Instance != null)
        {
            // "UIManager님, OnMoneyChanged 방송 하시면... 제 UpdateMoneyText 좀 실행해주세요."
            UIManager.Instance.OnMoneyChanged += UpdateMoneyText;

            // "UIManager님, OnTimeChanged 방송 하시면... 제 UpdateTimeText 좀 실행해주세요."
            UIManager.Instance.OnTimeChanged += UpdateTimeText;
        }
    }

    // --- 구독 해지 (오류 방지) ---
    void OnDisable()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.OnMoneyChanged -= UpdateMoneyText;
            UIManager.Instance.OnTimeChanged -= UpdateTimeText;
        }
    }

    // --- 방송을 받으면 실행될 함수들 ---

    private void UpdateMoneyText(int newMoneyAmount)
    {
        // GameManager가 "돈 바뀌었어!"라고 방송하면 이 함수가 호출됨
        moneyText.text = $"₩ {newMoneyAmount:N0}"; // (N0: 1,000처럼 콤마 찍어줌)
    }

    private void UpdateTimeText(float newGameTime)
    {
        // newGameTime (예: 9.5f)을 시/분으로 변환
        int hours = (int)newGameTime % 24;
        int minutes = (int)((newGameTime - hours) * 60) % 60;
        timeText.text = $"{hours:00}:{minutes:00}"; // 09:30 형태
    }
}