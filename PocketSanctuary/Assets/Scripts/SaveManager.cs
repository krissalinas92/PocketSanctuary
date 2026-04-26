using System.IO;
using UnityEngine;

public static class SaveManager
{
    private const string SaveFileName = "save.json";
    private const int CurrentSaveVersion = 2;
    // Temporary QA mode for fit/visual validation across all content.
    private const bool ForceUnlockAllContentForTesting = true;
    private static readonly string[] AllPocketPalIds = new string[]
    {
        "panda",
        "white_tiger",
        "otter"
    };
    private static readonly string[] AllHabitatIds = new string[]
    {
        "sunny_garden",
        "moonlit_pond",
        "bamboo_grove"
    };
    private static readonly string[] ValidAccessoryIds = new string[]
    {
        "eyewear_round_glasses",
        "eyewear_black_sunglasses",
        "eyewear_pixel_shades",
        "eyewear_heart_glasses",
        "eyewear_blue_round_glasses",
        "eyewear_star_glasses",
        "headwear_blue_beanie",
        "headwear_chef_hat",
        "headwear_dino_hood",
        "headwear_crown",
        "headwear_straw_hat",
        "headwear_red_cap",
        "headwear_aviator_hat",
        "shirt_green_bamboo",
        "shirt_blue_paw",
        "shirt_yellow_panda",
        "shirt_blue_hawaiian",
        "shirt_black_heart",
        "shirt_red_hoodie",
        "prop_blue_paw_flag",
        "prop_rainbow_flag",
        "prop_red_heart_flag",
        "prop_checkered_flag",
        "prop_green_leaf_flag",
        "prop_pirate_flag",
        "prop_panda_flag"
    };
    private static readonly string[] StarterAccessoryIds = new string[]
    {
        "eyewear_round_glasses",
        "shirt_red_hoodie"
    };

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

        if (!Data.ownedPocketPalIds.Contains("white_tiger"))
        {
            Data.ownedPocketPalIds.Add("white_tiger");
        }

        if (!Data.ownedPocketPalIds.Contains("otter"))
        {
            Data.ownedPocketPalIds.Add("otter");
        }

        if (Data.ownedAccessoryIds == null)
        {
            Data.ownedAccessoryIds = new System.Collections.Generic.List<string>();
        }

        System.Collections.Generic.HashSet<string> validAccessoryIds = new System.Collections.Generic.HashSet<string>(ValidAccessoryIds);
        Data.ownedAccessoryIds.RemoveAll(delegate (string accessoryId) { return !validAccessoryIds.Contains(accessoryId); });

        for (int index = 0; index < StarterAccessoryIds.Length; index++)
        {
            EnsureOwnedAccessory(StarterAccessoryIds[index]);
        }

        if (Data.equippedAccessoryIds == null)
        {
            Data.equippedAccessoryIds = new System.Collections.Generic.List<string>();
        }

        Data.equippedAccessoryIds.RemoveAll(delegate (string accessoryId) { return !validAccessoryIds.Contains(accessoryId); });

        if (!string.IsNullOrEmpty(Data.equippedAccessoryId) && !validAccessoryIds.Contains(Data.equippedAccessoryId))
        {
            Data.equippedAccessoryId = "";
        }

        if (!string.IsNullOrEmpty(Data.equippedAccessoryId) && !Data.equippedAccessoryIds.Contains(Data.equippedAccessoryId))
        {
            Data.equippedAccessoryIds.Add(Data.equippedAccessoryId);
        }

        if (string.IsNullOrEmpty(Data.equippedAccessoryId))
        {
            Data.equippedAccessoryId = Data.equippedAccessoryIds.Count > 0 ? Data.equippedAccessoryIds[0] : "";
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

        if (ForceUnlockAllContentForTesting)
        {
            UnlockAllContentForTesting(validAccessoryIds);
        }
    }

    private static void EnsureOwnedAccessory(string accessoryId)
    {
        if (!Data.ownedAccessoryIds.Contains(accessoryId))
        {
            Data.ownedAccessoryIds.Add(accessoryId);
        }
    }

    private static void UnlockAllContentForTesting(System.Collections.Generic.HashSet<string> validAccessoryIds)
    {
        if (Data.ownedPocketPalIds == null)
        {
            Data.ownedPocketPalIds = new System.Collections.Generic.List<string>();
        }

        for (int index = 0; index < AllPocketPalIds.Length; index++)
        {
            if (!Data.ownedPocketPalIds.Contains(AllPocketPalIds[index]))
            {
                Data.ownedPocketPalIds.Add(AllPocketPalIds[index]);
            }
        }

        if (Data.ownedHabitatIds == null)
        {
            Data.ownedHabitatIds = new System.Collections.Generic.List<string>();
        }

        for (int index = 0; index < AllHabitatIds.Length; index++)
        {
            if (!Data.ownedHabitatIds.Contains(AllHabitatIds[index]))
            {
                Data.ownedHabitatIds.Add(AllHabitatIds[index]);
            }
        }

        if (Data.ownedAccessoryIds == null)
        {
            Data.ownedAccessoryIds = new System.Collections.Generic.List<string>();
        }

        for (int index = 0; index < ValidAccessoryIds.Length; index++)
        {
            string accessoryId = ValidAccessoryIds[index];
            if (validAccessoryIds.Contains(accessoryId) && !Data.ownedAccessoryIds.Contains(accessoryId))
            {
                Data.ownedAccessoryIds.Add(accessoryId);
            }
        }

        if (Data.equippedAccessoryIds == null)
        {
            Data.equippedAccessoryIds = new System.Collections.Generic.List<string>();
        }

        if (Data.equippedAccessoryIds.Count == 0)
        {
            Data.equippedAccessoryIds.Add("shirt_red_hoodie");
        }

        Data.equippedAccessoryId = Data.equippedAccessoryIds[0];

        if (Data.coins < 250000)
        {
            Data.coins = 250000;
        }

        if (Data.pocketPalLevel < LevelingSystem.MaxLevel)
        {
            Data.pocketPalLevel = LevelingSystem.MaxLevel;
        }

        if (Data.habitatLevel < LevelingSystem.MaxLevel)
        {
            Data.habitatLevel = LevelingSystem.MaxLevel;
        }

        Data.friendshipLevel = Data.pocketPalLevel;
        Data.pocketPalXp = 0;
        Data.habitatXp = 0;
        Data.rewardProgress = Mathf.Max(Data.rewardProgress, 100);
        Data.snacksFed = Mathf.Max(Data.snacksFed, 1);
        Data.hasVisitedShop = true;

        if (string.IsNullOrEmpty(Data.selectedPocketPalId) || !Data.ownedPocketPalIds.Contains(Data.selectedPocketPalId))
        {
            Data.selectedPocketPalId = "panda";
        }

        if (string.IsNullOrEmpty(Data.selectedHabitatId) || !Data.ownedHabitatIds.Contains(Data.selectedHabitatId))
        {
            Data.selectedHabitatId = "sunny_garden";
        }
    }
}
