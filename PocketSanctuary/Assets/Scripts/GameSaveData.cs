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
}
