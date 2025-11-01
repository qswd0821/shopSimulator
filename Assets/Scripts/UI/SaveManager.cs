// Assets/Scripts/Core/SaveManager.cs
using System.IO;
using UnityEngine;

[System.Serializable]
public struct SaveData
{
    public int version;
    public int day;
    public int money;
    public int reputation;
    public int timeOfDay; // 분 (0~1439)
}

public class SaveManager : MonoBehaviour
{
    public const int CURRENT_VERSION = 1;
    private static string FilePath => Path.Combine(Application.persistentDataPath, "save.json");
    public SaveData Current { get; private set; }

    public bool HasSave() => File.Exists(FilePath);

    public SaveData LoadOrCreateDefault()
    {
        if (!HasSave())
        {
            Current = NewDefault();
            Save(Current);
            return Current;
        }
        var json = File.ReadAllText(FilePath);
        var data = JsonUtility.FromJson<SaveData>(json);
        Current = MigrateIfNeeded(data);
        return Current;
    }

    public void Save(SaveData data)
    {
        data.version = CURRENT_VERSION;
        Current = data;
        var json = JsonUtility.ToJson(Current);
        File.WriteAllText(FilePath, json);
#if UNITY_EDITOR
        Debug.Log($"[SaveManager] Saved: {FilePath}\n{json}");
#endif
    }

    public void NewGame()
    {
        Save(NewDefault());
    }

    SaveData NewDefault() => new SaveData
    {
        version = CURRENT_VERSION,
        day = 1,
        money = 100000,
        reputation = 0,
        timeOfDay = 8 * 60
    };

    SaveData MigrateIfNeeded(SaveData old)
    {
        // 버전업 시 여기에 마이그레이션 로직 추가
        if (old.version == 0) { old.version = 1; }
        if (old.version != CURRENT_VERSION) Save(old);
        return old;
    }
}
