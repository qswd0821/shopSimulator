using UnityEngine;
using System.IO; 

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private string saveFilePath; 
    private const string saveFileName = "savegame.json"; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 

            saveFilePath = Path.Combine(Application.persistentDataPath, saveFileName);
            Debug.Log($"<color=yellow>세이브 파일 경로: {saveFilePath}</color>");
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void SaveGame(GameData data)
    {
        try
        {
            string json = JsonUtility.ToJson(data, true);

            File.WriteAllText(saveFilePath, json);

            Debug.Log("게임 저장 성공!");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"게임 저장 실패: {e.Message}");
        }
    }

    public GameData LoadGame()
    {
        if (!HasSaveData())
        {
            Debug.LogWarning("로드할 세이브 파일이 없습니다.");
            return null;
        }

        try
        {
            string json = File.ReadAllText(saveFilePath);

            GameData data = JsonUtility.FromJson<GameData>(json);

            Debug.Log("게임 로드 성공!");
            return data;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"게임 로드 실패: {e.Message}");
            return null;
        }
    }

    public bool HasSaveData()
    {
        return File.Exists(saveFilePath);
    }

    public void DeleteSaveData()
    {
        if (HasSaveData())
        {
            File.Delete(saveFilePath);
            Debug.Log("세이브 파일 삭제됨.");
        }
    }
}