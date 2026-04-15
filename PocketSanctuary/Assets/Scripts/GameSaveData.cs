using System;
using System.Collections.Generic;

[Serializable]
public class GameSaveData
{
    public int coins = 250;
    public string username = "Player";

    public string selectedPocketPalId = "sprout";
    public string selectedPocketPalName = "Sprout";
    public string selectedHabitatId = "sunny_garden";
    public string equippedAccessoryId = "";

    public int friendshipLevel = 1;
    public int snacksFed = 0;
    public int rewardProgress = 0;

    public bool soundEnabled = true;
    public bool notificationsEnabled = true;

    public List<string> ownedPocketPalIds = new List<string> { "sprout" };
    public List<string> ownedAccessoryIds = new List<string>();
    public List<string> ownedHabitatIds = new List<string> { "sunny_garden" };
    public List<string> completedTaskIds = new List<string>();
}
