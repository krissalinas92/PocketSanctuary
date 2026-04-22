using System;
using System.Collections.Generic;

[Serializable]
public class GameSaveData
{
    public int saveVersion = 2;
    public int coins = 5000;
    public string username = "Player";

    public string selectedPocketPalId = "panda";
    public string selectedPocketPalName = "";
    public string selectedHabitatId = "sunny_garden";
    public string equippedAccessoryId = "";

    public int friendshipLevel = 1;
    public int pocketPalLevel = 1;
    public int pocketPalXp = 0;
    public int habitatLevel = 1;
    public int habitatXp = 0;
    public int snacksFed = 0;
    public int rewardProgress = 0;

    public bool soundEnabled = true;
    public bool notificationsEnabled = true;
    public bool hasVisitedShop = false;

    public List<string> ownedPocketPalIds = new List<string> { "panda" };
    public List<string> ownedAccessoryIds = new List<string>();
    public List<string> equippedAccessoryIds = new List<string>();
    public List<string> ownedHabitatIds = new List<string> { "sunny_garden" };
    public List<string> completedTaskIds = new List<string>();
    public List<string> redeemedRewardIds = new List<string>();
    public List<string> claimedLevelRewardIds = new List<string>();
}

public static class LevelingSystem
{
    public const int MaxLevel = 10;

    public static void GrantPocketPalXp(int amount)
    {
        SaveManager.EnsureLoaded();
        ApplyXp(ref SaveManager.Data.pocketPalLevel, ref SaveManager.Data.pocketPalXp, amount);
        SaveManager.Data.friendshipLevel = SaveManager.Data.pocketPalLevel;
    }

    public static void GrantHabitatXp(int amount)
    {
        SaveManager.EnsureLoaded();
        ApplyXp(ref SaveManager.Data.habitatLevel, ref SaveManager.Data.habitatXp, amount);
    }

    public static int GetXpForNextLevel(int level)
    {
        return 40 + ((level - 1) * 20);
    }

    public static bool TryClaimLevelReward(string rewardId, int requiredPocketPalLevel, int requiredHabitatLevel)
    {
        SaveManager.EnsureLoaded();

        if (SaveManager.Data.claimedLevelRewardIds == null)
        {
            SaveManager.Data.claimedLevelRewardIds = new List<string>();
        }

        if (SaveManager.Data.claimedLevelRewardIds.Contains(rewardId))
        {
            return false;
        }

        if (SaveManager.Data.pocketPalLevel < requiredPocketPalLevel || SaveManager.Data.habitatLevel < requiredHabitatLevel)
        {
            return false;
        }

        SaveManager.Data.claimedLevelRewardIds.Add(rewardId);
        return true;
    }

    private static void ApplyXp(ref int level, ref int xp, int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        if (level < 1)
        {
            level = 1;
        }

        xp += amount;

        while (level < MaxLevel && xp >= GetXpForNextLevel(level))
        {
            xp -= GetXpForNextLevel(level);
            level++;
        }

        if (level >= MaxLevel)
        {
            level = MaxLevel;
            xp = 0;
        }
    }
}
