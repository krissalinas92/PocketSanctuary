using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public static class SaveManager
{
    private const string LegacySaveFileName = "save.json";
    private const string SaveProfilesDirectoryName = "SaveProfiles";
    private const string ActiveProfilePrefsKey = "active_profile_id";
    private const string DefaultProfileId = "user";
    private const int CurrentSaveVersion = 2;
    // Temporary QA mode for fit/visual validation across all content.
    private static readonly bool ForceUnlockAllContentForTesting = false;
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
        "bamboo_grove",
        "cozy_house"
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
    private const int DefaultPocketPalHunger = 55;
    private const int DefaultPocketPalMood = 65;
    private const float DefaultPocketPalSleepSeconds = 480f;
    private const int FeedHungerGain = 18;
    private const int FeedMoodGain = 10;
    private const string WhiteTigerUnlockRewardId = "unlock_white_tiger_level_reward";
    private const string OtterUnlockRewardId = "unlock_otter_level_reward";
    private const int WhiteTigerUnlockCoinReward = 500;
    private const int WhiteTigerUnlockAccountXpReward = 80;
    private const int OtterUnlockCoinReward = 1000;
    private const int OtterUnlockAccountXpReward = 160;
    private static string activeProfileId = "";

    public static GameSaveData Data { get; private set; }

    private static string SaveProfilesDirectoryPath
    {
        get { return Path.Combine(Application.persistentDataPath, SaveProfilesDirectoryName); }
    }

    private static string LegacySavePath
    {
        get { return Path.Combine(Application.persistentDataPath, LegacySaveFileName); }
    }

    private static string SavePath
    {
        get { return Path.Combine(SaveProfilesDirectoryPath, activeProfileId + ".json"); }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        EnsureActiveProfileInitialized();
        EnsureSaveDirectoryExists();
        TryMigrateLegacySaveIfNeeded();
        Load();
    }

    public static void Load()
    {
        EnsureActiveProfileInitialized();
        EnsureSaveDirectoryExists();

        if (!File.Exists(SavePath))
        {
            Data = new GameSaveData();
            Data.username = activeProfileId;
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
        PersistSelectedPocketPalProgress();
        EnsureSaveDirectoryExists();

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

    public static string GetActiveProfileId()
    {
        EnsureActiveProfileInitialized();
        return activeProfileId;
    }

    public static void SwitchToProfile(string profileName)
    {
        EnsureActiveProfileInitialized();

        string requestedProfileName = string.IsNullOrWhiteSpace(profileName) ? DefaultProfileId : profileName.Trim();
        string requestedProfileId = NormalizeProfileId(requestedProfileName);
        if (string.IsNullOrEmpty(requestedProfileId))
        {
            requestedProfileId = DefaultProfileId;
            requestedProfileName = DefaultProfileId;
        }

        if (Data != null && activeProfileId == requestedProfileId && Data.username == requestedProfileName)
        {
            return;
        }

        if (Data != null)
        {
            Save();
        }

        activeProfileId = requestedProfileId;
        PlayerPrefs.SetString(ActiveProfilePrefsKey, activeProfileId);
        PlayerPrefs.Save();

        Load();
        Data.username = requestedProfileName;
        Save();
    }

    public static bool SetSelectedPocketPal(string pocketPalId)
    {
        EnsureLoaded();

        if (string.IsNullOrEmpty(pocketPalId))
        {
            return false;
        }

        if (Data.ownedPocketPalIds == null || !Data.ownedPocketPalIds.Contains(pocketPalId))
        {
            return false;
        }

        if (Data.selectedPocketPalId == pocketPalId)
        {
            return true;
        }

        PersistSelectedPocketPalProgress();
        Data.selectedPocketPalId = pocketPalId;
        LoadSelectedPocketPalProgressIntoLegacyFields();
        Save();
        return true;
    }

    public static int GetSelectedPocketPalLevel()
    {
        EnsureLoaded();
        return Mathf.Max(GetAccountLevel(), Data.pocketPalLevel);
    }

    public static int GetSelectedPocketPalXp()
    {
        EnsureLoaded();
        return Mathf.Max(0, Data.pocketPalXp);
    }

    public static int GetAccountLevel()
    {
        EnsureLoaded();
        return Mathf.Clamp(Data.accountLevel, 1, LevelingSystem.MaxLevel);
    }

    public static int GetAccountXp()
    {
        EnsureLoaded();
        return Mathf.Max(0, Data.accountXp);
    }

    public static int GetHabitatCleanCount()
    {
        EnsureLoaded();
        return Mathf.Max(0, Data.habitatCleanCount);
    }

    public static bool IsBackgroundOnlyDebugModeEnabled()
    {
        EnsureLoaded();
        return Data != null && Data.debugBackgroundOnlyMode;
    }

    public static void SetBackgroundOnlyDebugMode(bool enabled)
    {
        EnsureLoaded();
        if (Data == null)
        {
            return;
        }

        Data.debugBackgroundOnlyMode = enabled;
    }

    public static int IncrementHabitatCleanCount(int amount = 1)
    {
        EnsureLoaded();

        if (amount <= 0)
        {
            return Mathf.Max(0, Data.habitatCleanCount);
        }

        Data.habitatCleanCount = Mathf.Max(0, Data.habitatCleanCount) + amount;
        return Data.habitatCleanCount;
    }

    public static void EnsureDailyTaskClaimsForToday()
    {
        EnsureLoaded();

        if (Data.claimedDailyTaskIds == null)
        {
            Data.claimedDailyTaskIds = new List<string>();
        }

        string todayKey = GetTodayLocalDateKey();
        if (Data.dailyTaskClaimDateKey == todayKey)
        {
            return;
        }

        Data.dailyTaskClaimDateKey = todayKey;
        Data.claimedDailyTaskIds.Clear();
        Save();
    }

    public static bool IsDailyTaskClaimed(string taskId)
    {
        EnsureDailyTaskClaimsForToday();
        return !string.IsNullOrEmpty(taskId) &&
               Data.claimedDailyTaskIds != null &&
               Data.claimedDailyTaskIds.Contains(taskId);
    }

    public static bool TryMarkDailyTaskClaimed(string taskId)
    {
        EnsureDailyTaskClaimsForToday();
        if (string.IsNullOrEmpty(taskId))
        {
            return false;
        }

        if (Data.claimedDailyTaskIds.Contains(taskId))
        {
            return false;
        }

        Data.claimedDailyTaskIds.Add(taskId);
        return true;
    }

    public static void EnforceAccountBaselineForPocketPals()
    {
        EnsureLoaded();

        int accountBaselineLevel = GetAccountLevel();
        if (Data.pocketPalProgress != null)
        {
            for (int index = 0; index < Data.pocketPalProgress.Count; index++)
            {
                PocketPalProgressData progress = Data.pocketPalProgress[index];
                if (progress == null)
                {
                    continue;
                }

                if (progress.level < accountBaselineLevel)
                {
                    progress.level = accountBaselineLevel;
                    progress.xp = 0;
                }
            }
        }

        if (Data.pocketPalLevel < accountBaselineLevel)
        {
            Data.pocketPalLevel = accountBaselineLevel;
            Data.pocketPalXp = 0;
        }
    }

    public static int GetSelectedPocketPalHunger()
    {
        EnsureLoaded();
        PocketPalProgressData progress = GetSelectedPocketPalProgress();
        EnsurePocketPalNeedsInitialized(progress);
        return progress.hunger;
    }

    public static int GetSelectedPocketPalMood()
    {
        EnsureLoaded();
        PocketPalProgressData progress = GetSelectedPocketPalProgress();
        EnsurePocketPalNeedsInitialized(progress);
        return progress.mood;
    }

    public static int GetSelectedPocketPalFeedCount()
    {
        EnsureLoaded();
        PocketPalProgressData progress = GetSelectedPocketPalProgress();
        EnsurePocketPalNeedsInitialized(progress);
        return Mathf.Max(0, progress.feedCount);
    }

    public static int GetSelectedPocketPalPetCount()
    {
        EnsureLoaded();
        PocketPalProgressData progress = GetSelectedPocketPalProgress();
        EnsurePocketPalNeedsInitialized(progress);
        return Mathf.Max(0, progress.petCount);
    }

    public static bool IncrementSelectedPocketPalPetCount(int amount = 1)
    {
        EnsureLoaded();

        if (amount <= 0)
        {
            return false;
        }

        PocketPalProgressData progress = GetSelectedPocketPalProgress();
        EnsurePocketPalNeedsInitialized(progress);
        progress.petCount = Mathf.Max(0, progress.petCount) + amount;
        return true;
    }

    public static void FeedSelectedPocketPal()
    {
        EnsureLoaded();
        PocketPalProgressData progress = GetSelectedPocketPalProgress();
        EnsurePocketPalNeedsInitialized(progress);

        progress.feedCount = Mathf.Max(0, progress.feedCount) + 1;
        progress.hunger = Mathf.Clamp(progress.hunger + FeedHungerGain, 0, 100);
        progress.mood = Mathf.Clamp(progress.mood + FeedMoodGain, 0, 100);
        Data.snacksFed = Mathf.Max(0, Data.snacksFed) + 1;
    }

    public static bool ModifySelectedPocketPalNeeds(int hungerDelta, int moodDelta)
    {
        EnsureLoaded();
        PocketPalProgressData progress = GetSelectedPocketPalProgress();
        EnsurePocketPalNeedsInitialized(progress);

        int previousHunger = progress.hunger;
        int previousMood = progress.mood;

        if (hungerDelta != 0)
        {
            progress.hunger = Mathf.Clamp(progress.hunger + hungerDelta, 0, 100);
        }

        if (moodDelta != 0)
        {
            progress.mood = Mathf.Clamp(progress.mood + moodDelta, 0, 100);
        }

        return progress.hunger != previousHunger || progress.mood != previousMood;
    }

    public static float GetSelectedPocketPalSleepSeconds()
    {
        EnsureLoaded();
        PocketPalProgressData progress = GetSelectedPocketPalProgress();
        EnsurePocketPalNeedsInitialized(progress);
        return Mathf.Max(0f, progress.sleepSeconds);
    }

    public static int GetSelectedPocketPalCompletedSleepSessions()
    {
        EnsureLoaded();
        PocketPalProgressData progress = GetSelectedPocketPalProgress();
        EnsurePocketPalNeedsInitialized(progress);
        return Mathf.Max(0, progress.completedSleepSessions);
    }

    public static float GetSelectedPocketPalSleepProgress01(float sessionDurationSeconds)
    {
        float safeDuration = Mathf.Max(1f, sessionDurationSeconds);
        return Mathf.Clamp01(GetSelectedPocketPalSleepSeconds() / safeDuration);
    }

    public static int AddSelectedPocketPalSleepTime(float deltaSeconds, float sessionDurationSeconds)
    {
        EnsureLoaded();

        if (deltaSeconds <= 0f)
        {
            return 0;
        }

        float safeDuration = Mathf.Max(1f, sessionDurationSeconds);
        PocketPalProgressData progress = GetSelectedPocketPalProgress();
        EnsurePocketPalNeedsInitialized(progress);
        progress.sleepSeconds = Mathf.Max(0f, progress.sleepSeconds + deltaSeconds);

        int completedSessions = 0;
        while (progress.sleepSeconds >= safeDuration)
        {
            progress.sleepSeconds -= safeDuration;
            progress.completedSleepSessions = Mathf.Max(0, progress.completedSleepSessions) + 1;
            completedSessions++;
        }

        return completedSessions;
    }

    public static bool ReduceSelectedPocketPalSleepTime(float deltaSeconds)
    {
        EnsureLoaded();

        if (deltaSeconds <= 0f)
        {
            return false;
        }

        PocketPalProgressData progress = GetSelectedPocketPalProgress();
        EnsurePocketPalNeedsInitialized(progress);

        float previousSleepSeconds = progress.sleepSeconds;
        progress.sleepSeconds = Mathf.Max(0f, progress.sleepSeconds - deltaSeconds);
        return !Mathf.Approximately(previousSleepSeconds, progress.sleepSeconds);
    }

    public static bool IsAccessoryOwnedForSelectedPocketPal(string accessoryId)
    {
        EnsureLoaded();
        return Data.ownedAccessoryIds != null && Data.ownedAccessoryIds.Contains(accessoryId);
    }

    public static bool IsAccessoryEquippedForSelectedPocketPal(string accessoryId)
    {
        EnsureLoaded();

        if (Data.equippedAccessoryId == accessoryId)
        {
            return true;
        }

        return Data.equippedAccessoryIds != null && Data.equippedAccessoryIds.Contains(accessoryId);
    }

    public static bool AddOwnedAccessoryToSelectedPocketPal(string accessoryId)
    {
        EnsureLoaded();

        if (string.IsNullOrEmpty(accessoryId))
        {
            return false;
        }

        if (Data.ownedAccessoryIds == null)
        {
            Data.ownedAccessoryIds = new List<string>();
        }

        if (Data.ownedAccessoryIds.Contains(accessoryId))
        {
            return false;
        }

        Data.ownedAccessoryIds.Add(accessoryId);
        return true;
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

        if (Data.selectedHabitatId == "moonlit_pond")
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

        if (!Data.ownedPocketPalIds.Contains(Data.selectedPocketPalId))
        {
            Data.selectedPocketPalId = "panda";
        }

        if (Data.accountLevel < 1)
        {
            Data.accountLevel = Mathf.Max(1, Data.friendshipLevel);
        }

        Data.accountLevel = Mathf.Clamp(Data.accountLevel, 1, LevelingSystem.MaxLevel);

        if (Data.accountXp < 0)
        {
            Data.accountXp = 0;
        }

        if (Data.accountLevel >= LevelingSystem.MaxLevel)
        {
            Data.accountXp = 0;
        }

        TryUnlockPocketPalByAccountLevel("white_tiger", 10, WhiteTigerUnlockRewardId, WhiteTigerUnlockCoinReward, WhiteTigerUnlockAccountXpReward);
        TryUnlockPocketPalByAccountLevel("otter", 15, OtterUnlockRewardId, OtterUnlockCoinReward, OtterUnlockAccountXpReward);

        HashSet<string> validAccessoryIds = new HashSet<string>(ValidAccessoryIds);
        EnsurePocketPalProgressInitialized(validAccessoryIds);
        LoadSelectedPocketPalProgressIntoLegacyFields();

        if (Data.ownedAccessoryIds == null)
        {
            Data.ownedAccessoryIds = new List<string>();
        }

        Data.ownedAccessoryIds.RemoveAll(delegate (string accessoryId) { return !validAccessoryIds.Contains(accessoryId); });

        if (Data.equippedAccessoryIds == null)
        {
            Data.equippedAccessoryIds = new List<string>();
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
            Data.ownedHabitatIds = new List<string>();
        }

        if (!Data.ownedHabitatIds.Contains("sunny_garden"))
        {
            Data.ownedHabitatIds.Add("sunny_garden");
        }

        // Testing convenience: keep the house unlocked for the default "user" profile only.
        if (activeProfileId == DefaultProfileId && !Data.ownedHabitatIds.Contains("cozy_house"))
        {
            Data.ownedHabitatIds.Add("cozy_house");
        }

        if (!Data.ownedHabitatIds.Contains(Data.selectedHabitatId))
        {
            Data.selectedHabitatId = "sunny_garden";
        }

        if (Data.completedTaskIds == null)
        {
            Data.completedTaskIds = new List<string>();
        }

        if (Data.claimedDailyTaskIds == null)
        {
            Data.claimedDailyTaskIds = new List<string>();
        }

        if (string.IsNullOrEmpty(Data.dailyTaskClaimDateKey))
        {
            Data.dailyTaskClaimDateKey = GetTodayLocalDateKey();
        }

        if (Data.redeemedRewardIds == null)
        {
            Data.redeemedRewardIds = new List<string>();
        }

        if (Data.claimedLevelRewardIds == null)
        {
            Data.claimedLevelRewardIds = new List<string>();
        }

        if (Data.pocketPalLevel < 1)
        {
            Data.pocketPalLevel = Data.accountLevel;
        }

        Data.pocketPalLevel = Mathf.Max(Data.accountLevel, Data.pocketPalLevel);
        Data.friendshipLevel = Data.accountLevel;
        EnforceAccountBaselineForPocketPals();

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

        if (Data.habitatCleanCount < 0)
        {
            Data.habitatCleanCount = 0;
        }

        if (ForceUnlockAllContentForTesting)
        {
            UnlockAllContentForTesting(validAccessoryIds);
        }

        PersistSelectedPocketPalProgress();
    }

    private static string GetTodayLocalDateKey()
    {
        return DateTime.Now.ToString("yyyy-MM-dd");
    }

    private static void EnsurePocketPalProgressInitialized(HashSet<string> validAccessoryIds)
    {
        if (Data.pocketPalProgress == null)
        {
            Data.pocketPalProgress = new List<PocketPalProgressData>();
        }

        bool isLegacyMigration = Data.pocketPalProgress.Count == 0;
        string legacySelectedPocketPalId = string.IsNullOrEmpty(Data.selectedPocketPalId) ? "panda" : Data.selectedPocketPalId;

        for (int index = 0; index < AllPocketPalIds.Length; index++)
        {
            string pocketPalId = AllPocketPalIds[index];
            PocketPalProgressData progress = GetOrCreatePocketPalProgress(pocketPalId);
            EnsurePocketPalProgressLists(progress);

            if (isLegacyMigration && pocketPalId == legacySelectedPocketPalId)
            {
                MigrateLegacySelectedPalState(progress);
            }

            SanitizePocketPalProgress(progress, validAccessoryIds);
        }

        if (string.IsNullOrEmpty(Data.selectedPocketPalId))
        {
            Data.selectedPocketPalId = "panda";
        }
    }

    private static void EnsurePocketPalProgressLists(PocketPalProgressData progress)
    {
        if (progress.ownedAccessoryIds == null)
        {
            progress.ownedAccessoryIds = new List<string>();
        }

        if (progress.equippedAccessoryIds == null)
        {
            progress.equippedAccessoryIds = new List<string>();
        }

        EnsurePocketPalNeedsInitialized(progress);
    }

    private static void EnsurePocketPalNeedsInitialized(PocketPalProgressData progress)
    {
        if (progress == null)
        {
            return;
        }

        if (progress.feedCount < 0)
        {
            progress.feedCount = 0;
        }

        if (progress.petCount < 0)
        {
            progress.petCount = 0;
        }

        if (progress.sleepSeconds < 0f)
        {
            progress.sleepSeconds = 0f;
        }

        if (progress.completedSleepSessions < 0)
        {
            progress.completedSleepSessions = 0;
        }

        bool missingNeedsData = progress.feedCount == 0 && progress.hunger <= 0 && progress.mood <= 0;
        if (missingNeedsData)
        {
            progress.hunger = DefaultPocketPalHunger;
            progress.mood = DefaultPocketPalMood;
            if (progress.completedSleepSessions == 0 && progress.sleepSeconds <= 0f)
            {
                progress.sleepSeconds = DefaultPocketPalSleepSeconds;
            }
        }

        progress.hunger = Mathf.Clamp(progress.hunger, 0, 100);
        progress.mood = Mathf.Clamp(progress.mood, 0, 100);
    }

    private static void TryUnlockPocketPalByAccountLevel(string pocketPalId, int requiredLevel, string rewardId, int coinReward, int accountXpReward)
    {
        if (string.IsNullOrEmpty(pocketPalId) || Data.accountLevel < requiredLevel)
        {
            return;
        }

        if (Data.ownedPocketPalIds == null)
        {
            Data.ownedPocketPalIds = new List<string>();
        }

        if (Data.claimedLevelRewardIds == null)
        {
            Data.claimedLevelRewardIds = new List<string>();
        }

        bool newlyUnlocked = false;
        if (!Data.ownedPocketPalIds.Contains(pocketPalId))
        {
            Data.ownedPocketPalIds.Add(pocketPalId);
            newlyUnlocked = true;
        }

        if (!newlyUnlocked || Data.claimedLevelRewardIds.Contains(rewardId))
        {
            return;
        }

        Data.claimedLevelRewardIds.Add(rewardId);
        Data.coins = Mathf.Max(0, Data.coins) + Mathf.Max(0, coinReward);
        LevelingSystem.GrantAccountXp(Mathf.Max(0, accountXpReward));
        Data.rewardProgress = Mathf.Max(0, Data.rewardProgress) + Mathf.Max(0, coinReward / 5);
    }

    private static void MigrateLegacySelectedPalState(PocketPalProgressData progress)
    {
        progress.level = Mathf.Max(GetAccountLevel(), Data.pocketPalLevel);
        progress.xp = Mathf.Max(0, Data.pocketPalXp);

        if (Data.ownedAccessoryIds != null)
        {
            for (int index = 0; index < Data.ownedAccessoryIds.Count; index++)
            {
                string accessoryId = Data.ownedAccessoryIds[index];
                if (!progress.ownedAccessoryIds.Contains(accessoryId))
                {
                    progress.ownedAccessoryIds.Add(accessoryId);
                }
            }
        }

        if (Data.equippedAccessoryIds != null)
        {
            for (int index = 0; index < Data.equippedAccessoryIds.Count; index++)
            {
                string accessoryId = Data.equippedAccessoryIds[index];
                if (!progress.equippedAccessoryIds.Contains(accessoryId))
                {
                    progress.equippedAccessoryIds.Add(accessoryId);
                }
            }
        }

        progress.equippedAccessoryId = Data.equippedAccessoryId;
    }

    private static void SanitizePocketPalProgress(PocketPalProgressData progress, HashSet<string> validAccessoryIds)
    {
        int baselineLevel = Mathf.Clamp(Data.accountLevel, 1, LevelingSystem.MaxLevel);
        progress.level = Mathf.Max(baselineLevel, progress.level);
        progress.xp = Mathf.Max(0, progress.xp);
        EnsurePocketPalNeedsInitialized(progress);

        progress.ownedAccessoryIds.RemoveAll(delegate (string accessoryId) { return !validAccessoryIds.Contains(accessoryId); });
        progress.equippedAccessoryIds.RemoveAll(delegate (string accessoryId) { return !validAccessoryIds.Contains(accessoryId); });

        for (int index = progress.equippedAccessoryIds.Count - 1; index >= 0; index--)
        {
            string equippedAccessoryId = progress.equippedAccessoryIds[index];
            if (!progress.ownedAccessoryIds.Contains(equippedAccessoryId))
            {
                progress.equippedAccessoryIds.RemoveAt(index);
            }
        }

        if (!string.IsNullOrEmpty(progress.equippedAccessoryId) && !progress.ownedAccessoryIds.Contains(progress.equippedAccessoryId))
        {
            progress.equippedAccessoryId = "";
        }

        if (!string.IsNullOrEmpty(progress.equippedAccessoryId) && !progress.equippedAccessoryIds.Contains(progress.equippedAccessoryId))
        {
            progress.equippedAccessoryIds.Insert(0, progress.equippedAccessoryId);
        }

        if (string.IsNullOrEmpty(progress.equippedAccessoryId))
        {
            if (progress.equippedAccessoryIds.Count > 0)
            {
                progress.equippedAccessoryId = progress.equippedAccessoryIds[0];
            }
        }

        if (progress.level >= LevelingSystem.MaxLevel)
        {
            progress.level = LevelingSystem.MaxLevel;
            progress.xp = 0;
        }
    }

    private static PocketPalProgressData GetOrCreatePocketPalProgress(string pocketPalId)
    {
        if (Data.pocketPalProgress == null)
        {
            Data.pocketPalProgress = new List<PocketPalProgressData>();
        }

        for (int index = 0; index < Data.pocketPalProgress.Count; index++)
        {
            PocketPalProgressData entry = Data.pocketPalProgress[index];
            if (entry != null && entry.pocketPalId == pocketPalId)
            {
                return entry;
            }
        }

        PocketPalProgressData created = new PocketPalProgressData();
        created.pocketPalId = pocketPalId;
        Data.pocketPalProgress.Add(created);
        return created;
    }

    private static PocketPalProgressData GetSelectedPocketPalProgress()
    {
        string selectedPocketPalId = string.IsNullOrEmpty(Data.selectedPocketPalId) ? "panda" : Data.selectedPocketPalId;
        PocketPalProgressData progress = GetOrCreatePocketPalProgress(selectedPocketPalId);
        EnsurePocketPalProgressLists(progress);
        return progress;
    }

    private static void LoadSelectedPocketPalProgressIntoLegacyFields()
    {
        string selectedPocketPalId = string.IsNullOrEmpty(Data.selectedPocketPalId) ? "panda" : Data.selectedPocketPalId;
        PocketPalProgressData progress = GetOrCreatePocketPalProgress(selectedPocketPalId);
        EnsurePocketPalProgressLists(progress);

        Data.pocketPalLevel = Mathf.Max(GetAccountLevel(), progress.level);
        Data.pocketPalXp = Mathf.Max(0, progress.xp);
        Data.friendshipLevel = GetAccountLevel();

        Data.ownedAccessoryIds = new List<string>(progress.ownedAccessoryIds);
        Data.equippedAccessoryIds = new List<string>(progress.equippedAccessoryIds);
        Data.equippedAccessoryId = progress.equippedAccessoryId;

        if (string.IsNullOrEmpty(Data.equippedAccessoryId) && Data.equippedAccessoryIds.Count > 0)
        {
            Data.equippedAccessoryId = Data.equippedAccessoryIds[0];
        }
    }

    private static void PersistSelectedPocketPalProgress()
    {
        if (Data == null || string.IsNullOrEmpty(Data.selectedPocketPalId))
        {
            return;
        }

        PocketPalProgressData progress = GetOrCreatePocketPalProgress(Data.selectedPocketPalId);
        EnsurePocketPalProgressLists(progress);

        progress.level = Mathf.Max(1, Data.pocketPalLevel);
        progress.xp = Mathf.Max(0, Data.pocketPalXp);

        progress.ownedAccessoryIds.Clear();
        if (Data.ownedAccessoryIds != null)
        {
            for (int index = 0; index < Data.ownedAccessoryIds.Count; index++)
            {
                string accessoryId = Data.ownedAccessoryIds[index];
                if (!string.IsNullOrEmpty(accessoryId) && !progress.ownedAccessoryIds.Contains(accessoryId))
                {
                    progress.ownedAccessoryIds.Add(accessoryId);
                }
            }
        }

        progress.equippedAccessoryIds.Clear();
        if (Data.equippedAccessoryIds != null)
        {
            for (int index = 0; index < Data.equippedAccessoryIds.Count; index++)
            {
                string accessoryId = Data.equippedAccessoryIds[index];
                if (!string.IsNullOrEmpty(accessoryId) && !progress.equippedAccessoryIds.Contains(accessoryId))
                {
                    progress.equippedAccessoryIds.Add(accessoryId);
                }
            }
        }

        progress.equippedAccessoryId = Data.equippedAccessoryId;
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

    private static void EnsureActiveProfileInitialized()
    {
        if (!string.IsNullOrEmpty(activeProfileId))
        {
            return;
        }

        activeProfileId = NormalizeProfileId(PlayerPrefs.GetString(ActiveProfilePrefsKey, DefaultProfileId));
        if (string.IsNullOrEmpty(activeProfileId))
        {
            activeProfileId = DefaultProfileId;
        }
    }

    private static void EnsureSaveDirectoryExists()
    {
        if (!Directory.Exists(SaveProfilesDirectoryPath))
        {
            Directory.CreateDirectory(SaveProfilesDirectoryPath);
        }
    }

    private static bool HasAnyProfileSaves()
    {
        if (!Directory.Exists(SaveProfilesDirectoryPath))
        {
            return false;
        }

        return Directory.GetFiles(SaveProfilesDirectoryPath, "*.json").Length > 0;
    }

    private static void TryMigrateLegacySaveIfNeeded()
    {
        if (!File.Exists(LegacySavePath) || HasAnyProfileSaves())
        {
            return;
        }

        try
        {
            string legacyJson = File.ReadAllText(LegacySavePath);
            if (string.IsNullOrEmpty(legacyJson))
            {
                return;
            }

            GameSaveData legacyData = JsonUtility.FromJson<GameSaveData>(legacyJson);
            if (legacyData != null && !string.IsNullOrWhiteSpace(legacyData.username))
            {
                activeProfileId = NormalizeProfileId(legacyData.username);
                if (string.IsNullOrEmpty(activeProfileId))
                {
                    activeProfileId = DefaultProfileId;
                }

                PlayerPrefs.SetString(ActiveProfilePrefsKey, activeProfileId);
                PlayerPrefs.Save();
            }

            File.WriteAllText(SavePath, legacyJson);
        }
        catch (Exception exception)
        {
            Debug.LogWarning("Failed to migrate legacy save to profile-based save files. " + exception.Message);
        }
    }

    private static string NormalizeProfileId(string profileName)
    {
        if (string.IsNullOrWhiteSpace(profileName))
        {
            return DefaultProfileId;
        }

        string trimmed = profileName.Trim().ToLowerInvariant();
        char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
        for (int index = 0; index < invalidFileNameChars.Length; index++)
        {
            trimmed = trimmed.Replace(invalidFileNameChars[index], '_');
        }

        trimmed = trimmed.Replace(' ', '_');
        return string.IsNullOrEmpty(trimmed) ? DefaultProfileId : trimmed;
    }
}
