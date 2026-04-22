using System.IO;
using UnityEngine;

public static class SaveManager
{
    private const string SaveFileName = "save.json";
    private const int CurrentSaveVersion = 2;

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

        if (Data.saveVersion < CurrentSaveVersion)
        {
            string username = string.IsNullOrEmpty(Data.username) ? "Player" : Data.username;
            Data = new GameSaveData();
            Data.username = username;
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
        Data.saveVersion = CurrentSaveVersion;

        if (string.IsNullOrEmpty(Data.username))
        {
            Data.username = "Player";
        }

        if (string.IsNullOrEmpty(Data.selectedPocketPalId) || Data.selectedPocketPalId == "sprout")
        {
            Data.selectedPocketPalId = "panda";
        }

        if (Data.selectedPocketPalName == "Sprout")
        {
            Data.selectedPocketPalName = "";
        }

        if (string.IsNullOrEmpty(Data.selectedHabitatId))
        {
            Data.selectedHabitatId = "sunny_garden";
        }

        if (Data.ownedPocketPalIds == null)
        {
            Data.ownedPocketPalIds = new System.Collections.Generic.List<string>();
        }

        if (!Data.ownedPocketPalIds.Contains("panda"))
        {
            Data.ownedPocketPalIds.Add("panda");
        }

        if (Data.ownedAccessoryIds == null)
        {
            Data.ownedAccessoryIds = new System.Collections.Generic.List<string>();
        }

        EnsureOwnedAccessory("sunglasses");
        EnsureOwnedAccessory("tshirt");

        if (Data.equippedAccessoryIds == null)
        {
            Data.equippedAccessoryIds = new System.Collections.Generic.List<string>();
        }

        if (!string.IsNullOrEmpty(Data.equippedAccessoryId) && !Data.equippedAccessoryIds.Contains(Data.equippedAccessoryId))
        {
            Data.equippedAccessoryIds.Add(Data.equippedAccessoryId);
        }

        if (Data.ownedHabitatIds == null)
        {
            Data.ownedHabitatIds = new System.Collections.Generic.List<string>();
        }

        if (!Data.ownedHabitatIds.Contains("sunny_garden"))
        {
            Data.ownedHabitatIds.Add("sunny_garden");
        }

        if (!Data.ownedHabitatIds.Contains("moonlit_pond"))
        {
            Data.ownedHabitatIds.Add("moonlit_pond");
        }

        if (Data.completedTaskIds == null)
        {
            Data.completedTaskIds = new System.Collections.Generic.List<string>();
        }

        if (Data.redeemedRewardIds == null)
        {
            Data.redeemedRewardIds = new System.Collections.Generic.List<string>();
        }

        if (Data.claimedLevelRewardIds == null)
        {
            Data.claimedLevelRewardIds = new System.Collections.Generic.List<string>();
        }

        if (Data.pocketPalLevel < 1)
        {
            Data.pocketPalLevel = Mathf.Max(1, Data.friendshipLevel);
        }

        if (Data.friendshipLevel < Data.pocketPalLevel)
        {
            Data.friendshipLevel = Data.pocketPalLevel;
        }

        if (Data.habitatLevel < 1)
        {
            Data.habitatLevel = 1;
        }

        if (Data.pocketPalXp < 0)
        {
            Data.pocketPalXp = 0;
        }

        if (Data.habitatXp < 0)
        {
            Data.habitatXp = 0;
        }
    }

    private static void EnsureOwnedAccessory(string accessoryId)
    {
        if (!Data.ownedAccessoryIds.Contains(accessoryId))
        {
            Data.ownedAccessoryIds.Add(accessoryId);
        }
    }
}
