using UnityEngine;

public class SaveManager : MonoBehaviour
{
    /// <summary>
    /// 메인 메뉴가 "저장 파일이 있는지?" 물어볼 때 호출
    /// </summary>
    public bool HasSave()
    {
        // TODO: 나중에 실제 저장 파일(예: PlayerPrefs)이 있는지 확인해야 함

        // 지금은 "저장 파일 없음"으로 가정
        return false;
    }

    /// <summary>
    /// '새 게임' 버튼을 눌렀을 때 호출
    /// </summary>
    public void NewGame()
    {
        // TODO: 나중에 기존 저장 파일을 삭제하는 로직 추가

        Debug.Log("Starting...");
    }

    /// <summary>
    /// '이어하기' 버튼을 눌렀을 때 호출
    /// </summary>
    public void LoadOrCreateDefault()
    {
        // TODO: 나중에 실제 저장 파일을 불러오는 로직 추가

        Debug.Log("Load Game...");
    }
}