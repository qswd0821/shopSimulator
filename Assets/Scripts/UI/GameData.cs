using UnityEngine;

/// <summary>
/// 저장/로드할 게임 데이터의 '청사진' 또는 '상자' 역할을 하는 클래스입니다.
/// MonoBehaviour를 상속하지 않습니다. (오브젝트에 붙일 필요 없음)
/// </summary>
[System.Serializable] // (필수!) 이 클래스를 JsonUtility가 읽고 쓸 수 있게 함
public class GameData
{
    // --- 저장할 실제 데이터들 ---

    public int money;
    public int currentDay;
    public float gameTime;

    // --- (필수) 씬 저장용 ---
    // 이 변수 이름이 MainMenu.cs에서 쓴 이름과 같아야 합니다.
    public string lastSavedSceneName;

    // (TODO: 나중에 팀원 데이터도 여기에 추가)
    // public Vector3 playerPosition;

    /// <summary>
    /// 기본 생성자: 데이터가 없을 때 (or 새 게임) 기본값을 설정
    /// </summary>
    public GameData()
    {
        // '새 게임'을 눌렀을 때의 기본값
        this.money = 50000;
        this.currentDay = 1;
        this.gameTime = 9.0f;

        // (이 이름은 Build Settings에 등록된 씬 이름과 같아야 합니다)
        // 이전에 "Main" 씬이라고 하셔서 "Main"으로 설정했습니다.
        this.lastSavedSceneName = "Main";
    }
}