using System.IO;
using UnityEngine;

public static class SaveManager
{
    private const string SaveFileName = "save.json";

    public static GameSaveData Data { get; private set; }

    private static string SavePath
    {
        get { return Path.Combine(Application.persistentDataPath, SaveFileName); }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        Load();
    }

    public static void Load()
    {
        if (!File.Exists(SavePath))
        {
            Data = new GameSaveData();
            Save();
            return;
        }

        try
        {
            string json = File.ReadAllText(SavePath);
            Data = JsonUtility.FromJson<GameSaveData>(json);
        }
        catch (System.Exception exception)
        {
            Debug.LogWarning("Could not load save file. A new save will be created. " + exception.Message);
        }

        if (Data == null)
        {
            Data = new GameSaveData();
        }

        ApplyDefaultsForMissingValues();
        Save();
    }

    public static void Save()
    {
        EnsureLoaded();

        string json = JsonUtility.ToJson(Data, true);
        File.WriteAllText(SavePath, json);
    }

    public static void ResetSave()
    {
        Data = new GameSaveData();
        Save();
    }

    public static void EnsureLoaded()
    {
        if (Data == null)
        {
            Load();
        }
    }

    public static string GetSavePath()
    {
        return SavePath;
    }

    private static void ApplyDefaultsForMissingValues()
    {
        if (string.IsNullOrEmpty(Data.username))
        {
            Data.username = "Player";
        }

        if (string.IsNullOrEmpty(Data.selectedPocketPalId))
        {
            Data.selectedPocketPalId = "sprout";
        }

        if (string.IsNullOrEmpty(Data.selectedPocketPalName))
        {
            Data.selectedPocketPalName = "Sprout";
        }

        if (string.IsNullOrEmpty(Data.selectedHabitatId))
        {
            Data.selectedHabitatId = "sunny_garden";
        }

        if (Data.ownedPocketPalIds == null)
        {
            Data.ownedPocketPalIds = new System.Collections.Generic.List<string>();
        }

        if (!Data.ownedPocketPalIds.Contains("sprout"))
        {
            Data.ownedPocketPalIds.Add("sprout");
        }

        if (Data.ownedAccessoryIds == null)
        {
            Data.ownedAccessoryIds = new System.Collections.Generic.List<string>();
        }

        if (Data.ownedHabitatIds == null)
        {
            Data.ownedHabitatIds = new System.Collections.Generic.List<string>();
        }

        if (!Data.ownedHabitatIds.Contains("sunny_garden"))
        {
            Data.ownedHabitatIds.Add("sunny_garden");
        }

        if (Data.completedTaskIds == null)
        {
            Data.completedTaskIds = new System.Collections.Generic.List<string>();
        }
    }
}
