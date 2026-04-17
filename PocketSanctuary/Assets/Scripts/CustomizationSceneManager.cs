using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CustomizationSceneManager : MonoBehaviour
{
    private const string CustomizationSceneName = "CustomizationScene";
    private const string TasksRewardsSceneName = "TasksRewardsScene";
    private const string ShopSceneName = "ShopScene";
    private const string SettingsSceneName = "SettingsScene";
    private const string PocketPalSceneName = "PocketPalScene";
    private const string SunglassesAccessoryId = "sunglasses";
    private const string HatAccessoryId = "hat";
    private const string TShirtAccessoryId = "tshirt";
    private const int HatPrice = 120;
    private const int TShirtPrice = 160;
    private const string FeedTaskId = "feed_panda";
    private const string CheckInTaskId = "check_in";
    private const string AccessorizeTaskId = "accessorize_panda";
    private const string VisitShopTaskId = "visit_shop";
    private const string StarterRewardId = "starter_coin_grant";
    private const string CareRewardId = "care_progress_bonus";
    private const string SanctuaryRewardId = "sanctuary_growth_bonus";

    [SerializeField] private float scrollBottomPadding = 160f;

    private TMP_Text pocketPalBodyText;
    private int pocketPalFeedCount;

    private void Start()
    {
        SaveManager.EnsureLoaded();
        ConfigureScrollView();
    }

    private void ConfigureScrollView()
    {
        ScrollRect scrollRect = FindAnyObjectByType<ScrollRect>();
        if (scrollRect == null)
        {
            Debug.LogWarning("CustomizationSceneManager could not find a ScrollRect.");
            return;
        }

        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;
        scrollRect.inertia = true;

        RectTransform scrollTransform = scrollRect.GetComponent<RectTransform>();
        if (scrollTransform != null)
        {
            scrollTransform.anchorMin = new Vector2(0f, 0f);
            scrollTransform.anchorMax = new Vector2(1f, 1f);
            scrollTransform.anchoredPosition = new Vector2(0f, -80f);
            scrollTransform.sizeDelta = new Vector2(0f, -160f);
        }

        RectTransform viewport = scrollRect.viewport;
        if (viewport != null)
        {
            viewport.anchorMin = Vector2.zero;
            viewport.anchorMax = Vector2.one;
            viewport.anchoredPosition = new Vector2(-10f, 0f);
            viewport.sizeDelta = new Vector2(-20f, 0f);
        }

        RectTransform content = scrollRect.content;
        if (content != null)
        {
            content.anchorMin = new Vector2(0f, 1f);
            content.anchorMax = new Vector2(1f, 1f);
            content.pivot = new Vector2(0.5f, 1f);
            content.anchoredPosition = Vector2.zero;
            ConfigureVerticalLayout(content.gameObject, 20, 20, 20, 20, 20, true);
        }

        GameObject mainContentObject = GameObject.Find("MainContent");
        RectTransform mainContent = mainContentObject != null ? mainContentObject.GetComponent<RectTransform>() : null;
        if (mainContent != null)
        {
            mainContent.anchorMin = new Vector2(0f, 1f);
            mainContent.anchorMax = new Vector2(1f, 1f);
            mainContent.pivot = new Vector2(0.5f, 1f);
            mainContent.anchoredPosition = Vector2.zero;
            ConfigureVerticalLayout(mainContent.gameObject, 20, 20, 20, Mathf.RoundToInt(scrollBottomPadding), 24, false);
            BuildSceneContentIfNeeded(mainContent);
            PreserveExistingChildHeights(mainContent);
        }

        RebuildDynamicScrollHeight(scrollRect, mainContent);
    }

    private void ConfigureVerticalLayout(GameObject target, int left, int right, int top, int bottom, float spacing, bool resizeToPreferredHeight)
    {
        VerticalLayoutGroup layoutGroup = target.GetComponent<VerticalLayoutGroup>();
        if (layoutGroup == null)
        {
            layoutGroup = target.AddComponent<VerticalLayoutGroup>();
        }

        layoutGroup.enabled = true;
        layoutGroup.padding = new RectOffset(left, right, top, bottom);
        layoutGroup.spacing = spacing;
        layoutGroup.childAlignment = TextAnchor.UpperCenter;
        layoutGroup.childControlWidth = true;
        layoutGroup.childControlHeight = true;
        layoutGroup.childForceExpandWidth = true;
        layoutGroup.childForceExpandHeight = false;

        ContentSizeFitter contentSizeFitter = target.GetComponent<ContentSizeFitter>();
        if (!resizeToPreferredHeight)
        {
            if (contentSizeFitter != null)
            {
                contentSizeFitter.enabled = false;
            }

            return;
        }

        if (contentSizeFitter == null)
        {
            contentSizeFitter = target.AddComponent<ContentSizeFitter>();
        }

        contentSizeFitter.enabled = true;
        contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    private void BuildSceneContentIfNeeded(RectTransform mainContent)
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == CustomizationSceneName)
        {
            BuildCustomizationContent(mainContent);
        }
        else if (sceneName == TasksRewardsSceneName)
        {
            BuildTasksRewardsContent(mainContent);
        }
        else if (sceneName == ShopSceneName)
        {
            BuildShopContent(mainContent);
        }
        else if (sceneName == SettingsSceneName)
        {
            BuildSettingsContent(mainContent);
        }
        else if (sceneName == PocketPalSceneName)
        {
            BuildPocketPalContent(mainContent);
        }
    }

    private void BuildCustomizationContent(RectTransform mainContent)
    {
        ClearChildren(mainContent);

        CreateTitle(mainContent, "Customization");

        CreateCategorySection(mainContent, "Habitat Backgrounds", "Choose the space your panda calls home.", 420f, new string[]
        {
            "Sunny Garden\nSelected starter habitat",
            "Moonlit Pond\nLocked reward habitat",
            "Cozy Burrow\nLocked reward habitat"
        });

        CreateCategorySection(mainContent, "PocketPals", "Pick the animal living in your sanctuary.", 300f, new string[]
        {
            "Panda\nSelected starter pal"
        });

        GameObject accessoriesSection = CreatePanel("AccessoriesSection", mainContent, new Color(1f, 1f, 1f, 0.42f), 980f);
        ConfigurePanelLayout(accessoriesSection, 28, 28, 34, 34, 24);

        TMP_Text accessoriesTitle = CreateText("AccessoriesTitle", accessoriesSection.transform, "Accessories", 34, FontStyles.Bold, TextAlignmentOptions.Center);
        accessoriesTitle.color = new Color(0.08f, 0.18f, 0.22f);

        TMP_Text accessoriesSubtitle = CreateText("AccessoriesSubtitle", accessoriesSection.transform, "Dress up your panda by category.", 24, FontStyles.Normal, TextAlignmentOptions.Center);
        accessoriesSubtitle.color = new Color(0.16f, 0.28f, 0.32f);

        CreateAccessoryCategory(accessoriesSection.transform, "Sunglasses", SunglassesAccessoryId, "Round shades", "A cool pair of panda-sized sunglasses.");
        CreateAccessoryCategory(accessoriesSection.transform, "Hats", HatAccessoryId, "Green cap", "A soft cap for cozy walks.");
        CreateAccessoryCategory(accessoriesSection.transform, "T-Shirts", TShirtAccessoryId, "Blue tee", "A tiny shirt for your panda.");
    }

    private void CreateCategorySection(Transform parent, string title, string subtitle, float preferredHeight, string[] options)
    {
        float dynamicHeight = Mathf.Max(preferredHeight, 190f + options.Length * 126f);
        GameObject section = CreatePanel(title.Replace(" ", "") + "Section", parent, new Color(1f, 1f, 1f, 0.42f), dynamicHeight);
        ConfigurePanelLayout(section, 28, 28, 34, 34, 20);

        TMP_Text titleText = CreateText(title.Replace(" ", "") + "Title", section.transform, title, 34, FontStyles.Bold, TextAlignmentOptions.Center);
        titleText.color = new Color(0.08f, 0.18f, 0.22f);

        TMP_Text subtitleText = CreateText(title.Replace(" ", "") + "Subtitle", section.transform, subtitle, 24, FontStyles.Normal, TextAlignmentOptions.Center);
        subtitleText.color = new Color(0.16f, 0.28f, 0.32f);

        for (int index = 0; index < options.Length; index++)
        {
            CreateOptionCard(title.Replace(" ", "") + "Option" + index, section.transform, options[index]);
        }
    }

    private void CreateAccessoryCategory(Transform parent, string categoryTitle, string accessoryId, string itemName, string description)
    {
        GameObject categoryPanel = CreatePanel(categoryTitle.Replace("-", "").Replace(" ", "") + "Category", parent, new Color(0.93f, 0.97f, 0.96f, 0.72f), 250f);
        ConfigurePanelLayout(categoryPanel, 22, 22, 20, 20, 14);

        TMP_Text categoryLabel = CreateText(categoryTitle.Replace(" ", "") + "CategoryLabel", categoryPanel.transform, categoryTitle, 28, FontStyles.Bold, TextAlignmentOptions.Left);
        categoryLabel.color = new Color(0.08f, 0.18f, 0.22f);

        string status = GetAccessoryStatusText(accessoryId);
        string cardText = itemName + "\n" + description + "\n" + status;
        CreateAccessoryCard(accessoryId + "Card", categoryPanel.transform, cardText, accessoryId);
    }

    private void CreateAccessoryCard(string objectName, Transform parent, string text, string accessoryId)
    {
        bool isEquipped = IsAccessoryEquipped(accessoryId);
        Color cardColor = isEquipped ? new Color(0.78f, 0.9f, 0.76f, 0.96f) : new Color(0.94f, 0.97f, 0.98f, 0.95f);
        GameObject card = CreatePanel(objectName, parent, cardColor, 142f);

        if (IsAccessoryOwned(accessoryId))
        {
            Button button = card.AddComponent<Button>();
            Image image = card.GetComponent<Image>();
            image.raycastTarget = true;
            button.targetGraphic = image;
            button.onClick.AddListener(delegate { ToggleAccessory(accessoryId); });
        }

        ConfigurePanelLayout(card, 24, 24, 20, 20, 8);

        TMP_Text label = CreateText(objectName + "Label", card.transform, text, 25, FontStyles.Normal, TextAlignmentOptions.Left);
        label.color = new Color(0.08f, 0.18f, 0.22f);
    }

    private string GetAccessoryStatusText(string accessoryId)
    {
        if (IsAccessoryEquipped(accessoryId))
        {
            return "Equipped";
        }

        if (IsAccessoryOwned(accessoryId))
        {
            return "Owned";
        }

        return "Locked";
    }

    private void ToggleAccessory(string accessoryId)
    {
        SaveManager.EnsureLoaded();

        if (SaveManager.Data.equippedAccessoryIds == null)
        {
            SaveManager.Data.equippedAccessoryIds = new System.Collections.Generic.List<string>();
        }

        if (!IsAccessoryOwned(accessoryId))
        {
            return;
        }

        if (SaveManager.Data.equippedAccessoryIds.Contains(accessoryId))
        {
            SaveManager.Data.equippedAccessoryIds.Remove(accessoryId);
        }
        else
        {
            SaveManager.Data.equippedAccessoryIds.Add(accessoryId);
        }

        SaveManager.Data.equippedAccessoryId = SaveManager.Data.equippedAccessoryIds.Count > 0 ? SaveManager.Data.equippedAccessoryIds[0] : "";
        SaveManager.Save();
        SceneManager.LoadScene(CustomizationSceneName, LoadSceneMode.Single);
    }

    private bool IsAccessoryEquipped(string accessoryId)
    {
        return SaveManager.Data.equippedAccessoryIds != null && SaveManager.Data.equippedAccessoryIds.Contains(accessoryId);
    }

    private bool IsAccessoryOwned(string accessoryId)
    {
        return SaveManager.Data.ownedAccessoryIds != null && SaveManager.Data.ownedAccessoryIds.Contains(accessoryId);
    }

    private void BuildTasksRewardsContent(RectTransform mainContent)
    {
        ClearChildren(mainContent);

        CreateTitle(mainContent, "Tasks & Rewards");

        GameObject tasksPanel = CreatePanel("TasksPanel", mainContent, new Color(1f, 1f, 1f, 0.42f), 860f);
        ConfigurePanelLayout(tasksPanel, 28, 28, 34, 34, 22);

        TMP_Text tasksTitle = CreateText("TasksTitle", tasksPanel.transform, "Tasks", 34, FontStyles.Bold, TextAlignmentOptions.Center);
        tasksTitle.color = new Color(0.08f, 0.18f, 0.22f);

        TMP_Text tasksSubtitle = CreateText("TasksSubtitle", tasksPanel.transform, "Complete care actions, then claim coins here.", 24, FontStyles.Normal, TextAlignmentOptions.Center);
        tasksSubtitle.color = new Color(0.16f, 0.28f, 0.32f);

        CreateTaskCard(tasksPanel.transform, FeedTaskId, "Feed your panda", "Feed your panda from the PocketPal screen.", 10, SaveManager.Data.snacksFed > 0);
        CreateTaskCard(tasksPanel.transform, CheckInTaskId, "Check in today", "Open the game and visit your sanctuary.", 5, true);
        CreateTaskCard(tasksPanel.transform, AccessorizeTaskId, "Accessorize your panda", "Equip sunglasses, a hat, or a t-shirt.", 15, SaveManager.Data.equippedAccessoryIds != null && SaveManager.Data.equippedAccessoryIds.Count > 0);
        CreateTaskCard(tasksPanel.transform, VisitShopTaskId, "Visit the shop", "Browse the panda accessory shop.", 5, SaveManager.Data.hasVisitedShop);

        GameObject rewardsPanel = CreatePanel("RewardsPanel", mainContent, new Color(1f, 1f, 1f, 0.42f), 680f);
        ConfigurePanelLayout(rewardsPanel, 28, 28, 30, 30, 18);

        TMP_Text rewardsTitle = CreateText("RewardsTitle", rewardsPanel.transform, "Rewards", 34, FontStyles.Bold, TextAlignmentOptions.Center);
        rewardsTitle.color = new Color(0.08f, 0.18f, 0.22f);

        TMP_Text rewardsBody = CreateText("RewardsBody", rewardsPanel.transform, "Reward progress: " + SaveManager.Data.rewardProgress + " / 100\nRedeem unlocked milestones for bonus coins.", 26, FontStyles.Normal, TextAlignmentOptions.Left);
        rewardsBody.color = new Color(0.12f, 0.24f, 0.28f);

        CreateRewardCard(rewardsPanel.transform, StarterRewardId, "Starter coin grant", "A small boost for opening your sanctuary.", 0, 50);
        CreateRewardCard(rewardsPanel.transform, CareRewardId, "Care progress bonus", "A bonus for completing care tasks.", 25, 100);
        CreateRewardCard(rewardsPanel.transform, SanctuaryRewardId, "Sanctuary growth bonus", "A larger bonus for steady progress.", 50, 250);
    }

    private void CreateTaskCard(Transform parent, string taskId, string title, string description, int coinReward, bool isReady)
    {
        bool isComplete = IsTaskComplete(taskId);
        Color cardColor = isComplete ? new Color(0.72f, 0.76f, 0.76f, 0.78f) : new Color(0.94f, 0.97f, 0.98f, 0.95f);

        if (!isComplete && !isReady)
        {
            cardColor = new Color(0.9f, 0.9f, 0.9f, 0.82f);
        }

        GameObject card = CreatePanel(taskId + "TaskCard", parent, cardColor, 128f);

        if (!isComplete && isReady)
        {
            Button button = card.AddComponent<Button>();
            Image image = card.GetComponent<Image>();
            image.raycastTarget = true;
            button.targetGraphic = image;
            button.onClick.AddListener(delegate { ClaimTaskReward(taskId, coinReward); });
        }

        ConfigurePanelLayout(card, 24, 24, 18, 18, 8);

        string status = GetTaskStatusText(isComplete, isReady, coinReward);
        TMP_Text label = CreateText(taskId + "TaskLabel", card.transform, title + "\n" + description + "\n" + status, 25, FontStyles.Normal, TextAlignmentOptions.Left);
        label.color = new Color(0.08f, 0.18f, 0.22f);
    }

    private string GetTaskStatusText(bool isComplete, bool isReady, int coinReward)
    {
        if (isComplete)
        {
            return "Claimed";
        }

        if (!isReady)
        {
            return "Not ready";
        }

        return "+" + coinReward + " coins - tap to claim";
    }

    private void ClaimTaskReward(string taskId, int coinReward)
    {
        SaveManager.EnsureLoaded();

        if (SaveManager.Data.completedTaskIds == null)
        {
            SaveManager.Data.completedTaskIds = new System.Collections.Generic.List<string>();
        }

        if (SaveManager.Data.completedTaskIds.Contains(taskId))
        {
            return;
        }

        SaveManager.Data.completedTaskIds.Add(taskId);
        SaveManager.Data.coins += coinReward;
        SaveManager.Data.rewardProgress += coinReward;
        SaveManager.Save();
        SceneManager.LoadScene(TasksRewardsSceneName, LoadSceneMode.Single);
    }

    private bool IsTaskComplete(string taskId)
    {
        return SaveManager.Data.completedTaskIds != null && SaveManager.Data.completedTaskIds.Contains(taskId);
    }

    private void CreateRewardCard(Transform parent, string rewardId, string title, string description, int requiredProgress, int coinReward)
    {
        bool isRedeemed = IsRewardRedeemed(rewardId);
        bool isUnlocked = SaveManager.Data.rewardProgress >= requiredProgress;
        Color cardColor = isRedeemed ? new Color(0.72f, 0.76f, 0.76f, 0.78f) : new Color(0.94f, 0.97f, 0.98f, 0.95f);

        if (!isRedeemed && !isUnlocked)
        {
            cardColor = new Color(0.9f, 0.9f, 0.9f, 0.82f);
        }

        GameObject card = CreatePanel(rewardId + "RewardCard", parent, cardColor, 128f);

        if (!isRedeemed && isUnlocked)
        {
            Button button = card.AddComponent<Button>();
            Image image = card.GetComponent<Image>();
            image.raycastTarget = true;
            button.targetGraphic = image;
            button.onClick.AddListener(delegate { RedeemReward(rewardId, coinReward); });
        }

        ConfigurePanelLayout(card, 24, 24, 18, 18, 8);

        string status = GetRewardStatusText(isRedeemed, isUnlocked, requiredProgress, coinReward);
        TMP_Text label = CreateText(rewardId + "RewardLabel", card.transform, title + "\n" + description + "\n" + status, 25, FontStyles.Normal, TextAlignmentOptions.Left);
        label.color = new Color(0.08f, 0.18f, 0.22f);
    }

    private string GetRewardStatusText(bool isRedeemed, bool isUnlocked, int requiredProgress, int coinReward)
    {
        if (isRedeemed)
        {
            return "Redeemed";
        }

        if (!isUnlocked)
        {
            return "Unlocks at " + requiredProgress + " progress";
        }

        return "+" + coinReward + " coins - tap to redeem";
    }

    private void RedeemReward(string rewardId, int coinReward)
    {
        SaveManager.EnsureLoaded();

        if (SaveManager.Data.redeemedRewardIds == null)
        {
            SaveManager.Data.redeemedRewardIds = new System.Collections.Generic.List<string>();
        }

        if (SaveManager.Data.redeemedRewardIds.Contains(rewardId))
        {
            return;
        }

        SaveManager.Data.redeemedRewardIds.Add(rewardId);
        SaveManager.Data.coins += coinReward;
        SaveManager.Save();
        SceneManager.LoadScene(TasksRewardsSceneName, LoadSceneMode.Single);
    }

    private bool IsRewardRedeemed(string rewardId)
    {
        return SaveManager.Data.redeemedRewardIds != null && SaveManager.Data.redeemedRewardIds.Contains(rewardId);
    }

    private void BuildShopContent(RectTransform mainContent)
    {
        ClearChildren(mainContent);
        SaveManager.Data.hasVisitedShop = true;
        SaveManager.Save();

        CreateTitle(mainContent, "Shop");

        CreateCategorySection(mainContent, "Buy Coins", "Coin pack placeholders for future purchases.", 520f, new string[]
        {
            "Small pouch - 100 coins",
            "Garden jar - 500 coins",
            "Sanctuary chest - 1200 coins"
        });

        GameObject accessoryShop = CreatePanel("AccessoryShopSection", mainContent, new Color(1f, 1f, 1f, 0.42f), 760f);
        ConfigurePanelLayout(accessoryShop, 28, 28, 34, 34, 24);

        TMP_Text shopTitle = CreateText("AccessoryShopTitle", accessoryShop.transform, "Panda Accessories", 34, FontStyles.Bold, TextAlignmentOptions.Center);
        shopTitle.color = new Color(0.08f, 0.18f, 0.22f);

        TMP_Text shopSubtitle = CreateText("AccessoryShopSubtitle", accessoryShop.transform, "Buy items here, then equip them in Customization.", 24, FontStyles.Normal, TextAlignmentOptions.Center);
        shopSubtitle.color = new Color(0.16f, 0.28f, 0.32f);

        CreateShopItemCard("SunglassesShopCard", accessoryShop.transform, SunglassesAccessoryId, "Round shades", 0, "Starter item");
        CreateShopItemCard("HatShopCard", accessoryShop.transform, HatAccessoryId, "Green cap", HatPrice, "Sits right above the panda ears.");
        CreateShopItemCard("TShirtShopCard", accessoryShop.transform, TShirtAccessoryId, "Blue tee", TShirtPrice, "A tiny shirt with a sunny badge.");
    }

    private void CreateShopItemCard(string objectName, Transform parent, string accessoryId, string itemName, int price, string description)
    {
        bool isOwned = IsAccessoryOwned(accessoryId);
        bool canAfford = SaveManager.Data.coins >= price;
        Color cardColor = isOwned ? new Color(0.78f, 0.9f, 0.76f, 0.96f) : new Color(0.94f, 0.97f, 0.98f, 0.95f);

        if (!isOwned && !canAfford)
        {
            cardColor = new Color(0.9f, 0.9f, 0.9f, 0.82f);
        }

        GameObject card = CreatePanel(objectName, parent, cardColor, 150f);

        if (!isOwned && canAfford)
        {
            Button button = card.AddComponent<Button>();
            Image image = card.GetComponent<Image>();
            image.raycastTarget = true;
            button.targetGraphic = image;
            button.onClick.AddListener(delegate { BuyAccessory(accessoryId, price); });
        }

        ConfigurePanelLayout(card, 24, 24, 20, 20, 8);

        string status = GetShopItemStatus(accessoryId, price);
        string cardText = itemName + "\n" + description + "\n" + status;
        TMP_Text label = CreateText(objectName + "Label", card.transform, cardText, 25, FontStyles.Normal, TextAlignmentOptions.Left);
        label.color = new Color(0.08f, 0.18f, 0.22f);
    }

    private string GetShopItemStatus(string accessoryId, int price)
    {
        if (IsAccessoryOwned(accessoryId))
        {
            return "Owned";
        }

        if (SaveManager.Data.coins < price)
        {
            return price + " coins - need more coins";
        }

        return price + " coins - tap to buy";
    }

    private void BuyAccessory(string accessoryId, int price)
    {
        SaveManager.EnsureLoaded();

        if (IsAccessoryOwned(accessoryId) || SaveManager.Data.coins < price)
        {
            return;
        }

        SaveManager.Data.coins -= price;

        if (SaveManager.Data.ownedAccessoryIds == null)
        {
            SaveManager.Data.ownedAccessoryIds = new System.Collections.Generic.List<string>();
        }

        SaveManager.Data.ownedAccessoryIds.Add(accessoryId);
        SaveManager.Save();
        SceneManager.LoadScene(ShopSceneName, LoadSceneMode.Single);
    }

    private void BuildSettingsContent(RectTransform mainContent)
    {
        ClearChildren(mainContent);
        GameSaveData saveData = SaveManager.Data;

        CreateTitle(mainContent, "Settings");
        CreateSection(mainContent, "Account", "Signed in as " + saveData.username + ".", 560f, new string[]
        {
            "Change username",
            "Change email",
            "Change password"
        });

        string soundStatus = saveData.soundEnabled ? "on" : "off";
        string notificationStatus = saveData.notificationsEnabled ? "on" : "off";
        CreateSection(mainContent, "Preferences", "Tune how PocketSanctuary feels.", 680f, new string[]
        {
            "Sound settings - currently " + soundStatus,
            "Notifications - currently " + notificationStatus,
            "Buy more coins",
            "Accessibility options"
        });

        CreateSection(mainContent, "Session", "Account actions and sign out options.", 560f, new string[]
        {
            "Log out",
            "Delete account"
        });
    }

    private void BuildPocketPalContent(RectTransform mainContent)
    {
        ClearChildren(mainContent);

        CreateTitle(mainContent, "PocketPal");

        GameObject stage = CreatePanel("PocketPalStage", mainContent, new Color(0.83f, 0.92f, 1f, 0.95f), 620f);
        VerticalLayoutGroup stageLayout = ConfigurePanelLayout(stage, 28, 28, 36, 36, 22);
        stageLayout.childAlignment = TextAnchor.MiddleCenter;

        GameSaveData saveData = SaveManager.Data;

        TMP_Text palName = CreateText("PocketPalName", stage.transform, saveData.selectedPocketPalName, 52, FontStyles.Bold, TextAlignmentOptions.Center);
        palName.color = new Color(0.08f, 0.18f, 0.22f);

        pocketPalFeedCount = saveData.snacksFed;
        pocketPalBodyText = CreateText("PocketPalBody", stage.transform, CreatePocketPalStatusText("Curious", "Ready for a snack"), 32, FontStyles.Normal, TextAlignmentOptions.Center);
        pocketPalBodyText.color = new Color(0.12f, 0.24f, 0.28f);

        GameObject actionPanel = CreatePanel("PocketPalActions", mainContent, new Color(1f, 1f, 1f, 0.42f), 330f);
        ConfigurePanelLayout(actionPanel, 26, 26, 26, 26, 20);

        TMP_Text actionLabel = CreateText("PocketPalActionLabel", actionPanel.transform, "What should " + saveData.selectedPocketPalName + " do?", 30, FontStyles.Bold, TextAlignmentOptions.Center);
        actionLabel.color = new Color(0.08f, 0.18f, 0.22f);

        CreateButton("AccessorizeButton", actionPanel.transform, "Accessorize", GoToCustomization, new Color(0.69f, 0.88f, 0.66f, 1f));
        CreateButton("FeedButton", actionPanel.transform, "Feed", FeedPocketPal, new Color(1f, 0.9f, 0.52f, 1f));

        GameObject statsPanel = CreatePanel("PocketPalStats", mainContent, new Color(0.93f, 0.96f, 0.98f, 0.8f), 360f);
        ConfigurePanelLayout(statsPanel, 28, 28, 30, 30, 18);

        TMP_Text statsTitle = CreateText("PocketPalStatsTitle", statsPanel.transform, "PocketPal Stats", 34, FontStyles.Bold, TextAlignmentOptions.Center);
        statsTitle.color = new Color(0.08f, 0.18f, 0.22f);

        string equippedAccessory = string.IsNullOrEmpty(saveData.equippedAccessoryId) ? "None equipped" : saveData.equippedAccessoryId;
        TMP_Text statsBody = CreateText("PocketPalStatsBody", statsPanel.transform, "Favorite food: Berry bites\nAccessory: " + equippedAccessory + "\nFriendship: Level " + saveData.friendshipLevel + "\nSnacks fed: " + saveData.snacksFed + "\nNext reward: 25 coins", 28, FontStyles.Normal, TextAlignmentOptions.Left);
        statsBody.color = new Color(0.12f, 0.24f, 0.28f);
    }

    private string CreatePocketPalStatusText(string mood, string hunger)
    {
        return "Pal lives here\n\nMood: " + mood + "\nHunger: " + hunger + "\nSnacks eaten: " + pocketPalFeedCount;
    }

    private TMP_Text CreateTitle(Transform parent, string text)
    {
        TMP_Text title = CreateText(text.Replace(" ", "") + "Title", parent, text, 48, FontStyles.Bold, TextAlignmentOptions.Center);
        title.color = Color.white;
        return title;
    }

    private void CreateSection(Transform parent, string title, string subtitle, float preferredHeight, string[] options)
    {
        float dynamicHeight = Mathf.Max(preferredHeight, 210f + options.Length * 136f);
        GameObject section = CreatePanel(title.Replace(" ", "") + "Section", parent, new Color(1f, 1f, 1f, 0.42f), dynamicHeight);
        ConfigurePanelLayout(section, 28, 28, 34, 34, 24);

        TMP_Text titleText = CreateText(title.Replace(" ", "") + "Title", section.transform, title, 34, FontStyles.Bold, TextAlignmentOptions.Center);
        titleText.color = new Color(0.08f, 0.18f, 0.22f);

        TMP_Text subtitleText = CreateText(title.Replace(" ", "") + "Subtitle", section.transform, subtitle, 24, FontStyles.Normal, TextAlignmentOptions.Center);
        subtitleText.color = new Color(0.16f, 0.28f, 0.32f);

        for (int index = 0; index < options.Length; index++)
        {
            CreateOptionCard(title.Replace(" ", "") + "Option" + index, section.transform, options[index]);
        }
    }

    private void CreateOptionCard(string objectName, Transform parent, string text)
    {
        GameObject card = CreatePanel(objectName, parent, new Color(0.94f, 0.97f, 0.98f, 0.95f), 118f);
        ConfigurePanelLayout(card, 24, 24, 22, 22, 8);

        TMP_Text label = CreateText(objectName + "Label", card.transform, text, 28, FontStyles.Normal, TextAlignmentOptions.Left);
        label.color = new Color(0.08f, 0.18f, 0.22f);
    }

    private void ClearChildren(RectTransform parent)
    {
        for (int index = parent.childCount - 1; index >= 0; index--)
        {
            GameObject child = parent.GetChild(index).gameObject;
            child.SetActive(false);
            Destroy(child);
        }
    }

    private GameObject CreatePanel(string objectName, Transform parent, Color color, float preferredHeight)
    {
        GameObject panel = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(LayoutElement));
        panel.transform.SetParent(parent, false);

        Image image = panel.GetComponent<Image>();
        image.color = color;
        image.raycastTarget = false;

        LayoutElement layoutElement = panel.GetComponent<LayoutElement>();
        layoutElement.preferredHeight = preferredHeight;
        layoutElement.minHeight = preferredHeight;
        layoutElement.flexibleWidth = 1f;

        return panel;
    }

    private VerticalLayoutGroup ConfigurePanelLayout(GameObject panel, int left, int right, int top, int bottom, float spacing)
    {
        VerticalLayoutGroup layoutGroup = panel.AddComponent<VerticalLayoutGroup>();
        layoutGroup.padding = new RectOffset(left, right, top, bottom);
        layoutGroup.spacing = spacing;
        layoutGroup.childAlignment = TextAnchor.UpperCenter;
        layoutGroup.childControlWidth = true;
        layoutGroup.childControlHeight = true;
        layoutGroup.childForceExpandWidth = true;
        layoutGroup.childForceExpandHeight = false;
        return layoutGroup;
    }

    private TMP_Text CreateText(string objectName, Transform parent, string text, float fontSize, FontStyles fontStyle, TextAlignmentOptions alignment)
    {
        GameObject textObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI), typeof(LayoutElement));
        textObject.transform.SetParent(parent, false);

        TextMeshProUGUI label = textObject.GetComponent<TextMeshProUGUI>();
        label.text = text;
        label.fontSize = fontSize;
        label.fontStyle = fontStyle;
        label.alignment = alignment;
        label.raycastTarget = false;
        label.textWrappingMode = TextWrappingModes.Normal;

        LayoutElement layoutElement = textObject.GetComponent<LayoutElement>();
        layoutElement.preferredHeight = Mathf.Max(60f, label.preferredHeight + 16f);
        layoutElement.flexibleWidth = 1f;

        return label;
    }

    private void CreateButton(string objectName, Transform parent, string label, UnityEngine.Events.UnityAction onClick, Color color)
    {
        GameObject buttonObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(LayoutElement));
        buttonObject.transform.SetParent(parent, false);

        Image image = buttonObject.GetComponent<Image>();
        image.color = color;

        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(onClick);

        LayoutElement layoutElement = buttonObject.GetComponent<LayoutElement>();
        layoutElement.preferredHeight = 92f;
        layoutElement.minHeight = 92f;
        layoutElement.flexibleWidth = 1f;

        TMP_Text buttonLabel = CreateText(objectName + "Label", buttonObject.transform, label, 34, FontStyles.Bold, TextAlignmentOptions.Center);
        buttonLabel.color = new Color(0.08f, 0.18f, 0.22f);

        RectTransform labelTransform = buttonLabel.GetComponent<RectTransform>();
        labelTransform.anchorMin = Vector2.zero;
        labelTransform.anchorMax = Vector2.one;
        labelTransform.offsetMin = Vector2.zero;
        labelTransform.offsetMax = Vector2.zero;
    }

    private void PreserveExistingChildHeights(RectTransform parent)
    {
        foreach (RectTransform child in parent)
        {
            if (!child.gameObject.activeSelf)
            {
                continue;
            }

            if (LayoutUtility.GetPreferredHeight(child) > 0f)
            {
                continue;
            }

            float currentHeight = child.rect.height;
            if (currentHeight <= 0f)
            {
                continue;
            }

            LayoutElement layoutElement = child.GetComponent<LayoutElement>();
            if (layoutElement == null)
            {
                layoutElement = child.gameObject.AddComponent<LayoutElement>();
            }

            layoutElement.preferredHeight = currentHeight;
        }
    }

    private void RebuildDynamicScrollHeight(ScrollRect scrollRect, RectTransform mainContent)
    {
        Canvas.ForceUpdateCanvases();

        if (mainContent != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(mainContent);
        }

        if (scrollRect.content != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);
        }

        EnsureContentFillsViewport(scrollRect);
        scrollRect.verticalNormalizedPosition = 1f;
    }

    private void EnsureContentFillsViewport(ScrollRect scrollRect)
    {
        if (scrollRect.content == null || scrollRect.viewport == null)
        {
            return;
        }

        float viewportHeight = scrollRect.viewport.rect.height;
        float contentHeight = scrollRect.content.rect.height;
        if (contentHeight >= viewportHeight)
        {
            return;
        }

        scrollRect.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, viewportHeight + 1f);
    }

    public void GoBackToSanctuary()
    {
        SceneManager.LoadScene("SanctuaryScene", LoadSceneMode.Single);
    }

    private void GoToCustomization()
    {
        SceneManager.LoadScene("CustomizationScene", LoadSceneMode.Single);
    }

    private void FeedPocketPal()
    {
        SaveManager.EnsureLoaded();

        pocketPalFeedCount++;
        SaveManager.Data.snacksFed = pocketPalFeedCount;
        SaveManager.Save();

        if (pocketPalBodyText != null)
        {
            pocketPalBodyText.text = CreatePocketPalStatusText("Happy", "Full");
        }

        Debug.Log("PocketPal fed.");
    }
}
