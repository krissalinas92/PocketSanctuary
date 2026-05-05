using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CustomizationSceneManager : MonoBehaviour
{
    private const string CustomizationSceneName = "CustomizationScene";
    private const string TasksRewardsSceneName = "TasksRewardsScene";
    private const string ShopSceneName = "ShopScene";
    private const string SettingsSceneName = "SettingsScene";
    private const string PocketPalSceneName = "PocketPalScene";
    private const string EyewearRoundGlassesAccessoryId = "eyewear_round_glasses";
    private const string EyewearBlackSunglassesAccessoryId = "eyewear_black_sunglasses";
    private const string EyewearPixelShadesAccessoryId = "eyewear_pixel_shades";
    private const string EyewearHeartGlassesAccessoryId = "eyewear_heart_glasses";
    private const string EyewearBlueRoundGlassesAccessoryId = "eyewear_blue_round_glasses";
    private const string EyewearStarGlassesAccessoryId = "eyewear_star_glasses";
    private const string HeadwearBlueBeanieAccessoryId = "headwear_blue_beanie";
    private const string HeadwearChefHatAccessoryId = "headwear_chef_hat";
    private const string HeadwearDinoHoodAccessoryId = "headwear_dino_hood";
    private const string HeadwearCrownAccessoryId = "headwear_crown";
    private const string HeadwearStrawHatAccessoryId = "headwear_straw_hat";
    private const string HeadwearRedCapAccessoryId = "headwear_red_cap";
    private const string HeadwearAviatorHatAccessoryId = "headwear_aviator_hat";
    private const string ShirtGreenBambooAccessoryId = "shirt_green_bamboo";
    private const string ShirtBluePawAccessoryId = "shirt_blue_paw";
    private const string ShirtYellowPandaAccessoryId = "shirt_yellow_panda";
    private const string ShirtBlueHawaiianAccessoryId = "shirt_blue_hawaiian";
    private const string ShirtBlackHeartAccessoryId = "shirt_black_heart";
    private const string ShirtRedHoodieAccessoryId = "shirt_red_hoodie";
    private const string PropBluePawFlagAccessoryId = "prop_blue_paw_flag";
    private const string PropRainbowFlagAccessoryId = "prop_rainbow_flag";
    private const string PropRedHeartFlagAccessoryId = "prop_red_heart_flag";
    private const string PropCheckeredFlagAccessoryId = "prop_checkered_flag";
    private const string PropGreenLeafFlagAccessoryId = "prop_green_leaf_flag";
    private const string PropPirateFlagAccessoryId = "prop_pirate_flag";
    private const string PropPandaFlagAccessoryId = "prop_panda_flag";
    private const string FeedTaskId = "feed_panda";
    private const string SleepTaskId = "sleep_full_cycle";
    private const string CheckInTaskId = "check_in";
    private const string AccessorizeTaskId = "accessorize_panda";
    private const string PetPalTaskId = "pet_pal_10x";
    private const string UnlockWhiteTigerTaskId = "unlock_white_tiger";
    private const string UnlockOtterTaskId = "unlock_otter";
    private const int PetPalTaskTargetCount = 10;
    private const int PetPalTaskCoinReward = 20;
    private const int PetPalTaskAccountXpReward = 26;
    private const int UnlockWhiteTigerTaskCoinReward = 350;
    private const int UnlockWhiteTigerTaskAccountXpReward = 100;
    private const int UnlockOtterTaskCoinReward = 700;
    private const int UnlockOtterTaskAccountXpReward = 200;
    private const string CleanHabitatTask7Id = "clean_habitat_7x";
    private const string CleanHabitatTask15Id = "clean_habitat_15x";
    private const int CleanHabitatTask7TargetCount = 7;
    private const int CleanHabitatTask15TargetCount = 15;
    private const int CleanHabitatTask7CoinReward = 15;
    private const int CleanHabitatTask7AccountXpReward = 14;
    private const int CleanHabitatTask15CoinReward = CleanHabitatTask7CoinReward * 2;
    private const int CleanHabitatTask15AccountXpReward = CleanHabitatTask7AccountXpReward * 2;
    private const int WhiteTigerUnlockLevel = 10;
    private const int OtterUnlockLevel = 15;
    private const float SleepSessionDurationSeconds = 480f;
    private const string CozyHouseHabitatId = "cozy_house";
    private const int CozyHouseHabitatCost = 500000;
    private const int CozyHouseRequiredAccountLevel = 30;
    private const int CozyHouseRequiredPocketPalLevel = 60;
    private const string StarterRewardId = "starter_coin_grant";
    private const string CareRewardId = "care_progress_bonus";
    private const string PettingRewardId = "petting_progress_bonus";
    private const string AccessoryPurchaseLockSetupFlag = "qa_accessory_lock_setup_v1";
    private const string AccessoryLockIconText = "🔒";
    private const string ShopAssetsSheetResourcePath = "Sprites/Shop/shopassets";
    private const string HouseHabitatPreviewResourcePath = "Sprites/Habitats/house";
    private const string PandaPocketPalIconResourcePath = "Sprites/PocketPalIcons/pal_icon_panda";
    private const string WhiteTigerPocketPalIconResourcePath = "Sprites/PocketPalIcons/pal_icon_white_tiger";
    private const string OtterPocketPalIconResourcePath = "Sprites/PocketPalIcons/pal_icon_otter";
    private const float CompactSelectorIconSize = 86f;
    private const float CompactShopIconSize = 112f;

    [SerializeField] private float scrollBottomPadding = 160f;

    private TMP_Text pocketPalBodyText;
    private TMP_Text pocketPalStatsText;
    private GameObject accessoryPurchaseDialogRoot;
    private GameObject habitatPurchaseDialogRoot;
    private GameObject coinPurchaseDialogRoot;
    private TMP_Text coinPurchaseDialogStatusText;
    private bool coinPurchaseProcessing;
    private int pocketPalFeedCount;
    private bool compactDrawerMode;
    private static Sprite lockBadgeSprite;
    private static readonly Dictionary<string, Sprite> previewSpriteCache = new Dictionary<string, Sprite>();
    private static readonly Dictionary<string, Sprite> coinPackSpriteCache = new Dictionary<string, Sprite>();
    private static readonly Dictionary<string, Sprite> pocketPalIconSpriteCache = new Dictionary<string, Sprite>();
    private static readonly Dictionary<string, Sprite> habitatPreviewSpriteCache = new Dictionary<string, Sprite>();
    private static readonly string[] AccessoryLockSetupIds = new string[]
    {
        EyewearBlackSunglassesAccessoryId,
        HeadwearBlueBeanieAccessoryId,
        ShirtBluePawAccessoryId,
        PropBluePawFlagAccessoryId
    };

    private class PocketPalDefinition
    {
        public string id;
        public string name;
        public string description;
        public int requiredAccountLevel;

        public PocketPalDefinition(string id, string name, string description, int requiredAccountLevel = 1)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.requiredAccountLevel = Mathf.Max(1, requiredAccountLevel);
        }
    }

    private class HabitatDefinition
    {
        public string id;
        public string name;
        public string description;
        public int coinCost;
        public int requiredAccountLevel;
        public int requiredPocketPalLevel;

        public HabitatDefinition(
            string id,
            string name,
            string description,
            int coinCost = 0,
            int requiredAccountLevel = 1,
            int requiredPocketPalLevel = 1)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.coinCost = Mathf.Max(0, coinCost);
            this.requiredAccountLevel = Mathf.Max(1, requiredAccountLevel);
            this.requiredPocketPalLevel = Mathf.Max(1, requiredPocketPalLevel);
        }
    }

    private class AccessoryDefinition
    {
        public string id;
        public string category;
        public string name;
        public string description;
        public int price;

        public AccessoryDefinition(string id, string category, string name, string description, int price)
        {
            this.id = id;
            this.category = category;
            this.name = name;
            this.description = description;
            this.price = price;
        }
    }

    private class CoinPackDefinition
    {
        public string id;
        public string name;
        public string priceLabel;
        public int coinReward;
        public string placeholderShortLabel;
        public Color placeholderColor;

        public CoinPackDefinition(string id, string name, string priceLabel, int coinReward, string placeholderShortLabel, Color placeholderColor)
        {
            this.id = id;
            this.name = name;
            this.priceLabel = priceLabel;
            this.coinReward = coinReward;
            this.placeholderShortLabel = placeholderShortLabel;
            this.placeholderColor = placeholderColor;
        }
    }

    private static readonly AccessoryDefinition[] AccessoryCatalog = new AccessoryDefinition[]
    {
        new AccessoryDefinition(EyewearRoundGlassesAccessoryId, "Eyewear", "Round glasses", "Classic round frames from your sprite pack.", 70),
        new AccessoryDefinition(EyewearBlackSunglassesAccessoryId, "Eyewear", "Black sunglasses", "Cool black shades.", 95),
        new AccessoryDefinition(EyewearPixelShadesAccessoryId, "Eyewear", "Pixel shades", "Retro blocky shades.", 125),
        new AccessoryDefinition(EyewearHeartGlassesAccessoryId, "Eyewear", "Heart glasses", "Bright heart-shaped frames.", 135),
        new AccessoryDefinition(EyewearBlueRoundGlassesAccessoryId, "Eyewear", "Blue goggles", "Blue circular goggles.", 120),
        new AccessoryDefinition(EyewearStarGlassesAccessoryId, "Eyewear", "Star glasses", "Bold star-shaped glasses.", 145),
        new AccessoryDefinition(HeadwearBlueBeanieAccessoryId, "Headwear", "Blue beanie", "A cozy blue knit cap.", 110),
        new AccessoryDefinition(HeadwearChefHatAccessoryId, "Headwear", "Chef hat", "Tiny chef vibes.", 140),
        new AccessoryDefinition(HeadwearDinoHoodAccessoryId, "Headwear", "Dino hood", "A playful dinosaur hood.", 170),
        new AccessoryDefinition(HeadwearCrownAccessoryId, "Headwear", "Crown", "Royal crown from your sheet.", 220),
        new AccessoryDefinition(HeadwearStrawHatAccessoryId, "Headwear", "Straw hat", "Sun hat with a leaf accent.", 130),
        new AccessoryDefinition(HeadwearRedCapAccessoryId, "Headwear", "Red cap", "A tilted red cap.", 100),
        new AccessoryDefinition(HeadwearAviatorHatAccessoryId, "Headwear", "Aviator cap", "Pilot cap with goggles.", 180),
        new AccessoryDefinition(ShirtGreenBambooAccessoryId, "Shirts", "Green fish shirt", "Green shirt with fish icon.", 150),
        new AccessoryDefinition(ShirtBluePawAccessoryId, "Shirts", "Blue paw shirt", "Blue shirt with paw icon.", 150),
        new AccessoryDefinition(ShirtYellowPandaAccessoryId, "Shirts", "Yellow mascot hoodie", "Yellow hoodie with mascot patch.", 180),
        new AccessoryDefinition(ShirtBlueHawaiianAccessoryId, "Shirts", "Blue floral shirt", "Blue floral island shirt.", 180),
        new AccessoryDefinition(ShirtBlackHeartAccessoryId, "Shirts", "Black heart hoodie", "Black hoodie with red heart.", 180),
        new AccessoryDefinition(ShirtRedHoodieAccessoryId, "Shirts", "Red hoodie", "Classic red hoodie.", 170),
        new AccessoryDefinition(PropBluePawFlagAccessoryId, "Handheld", "Blue paw flag", "Carry a blue paw flag.", 140),
        new AccessoryDefinition(PropRainbowFlagAccessoryId, "Handheld", "Rainbow flag", "Carry a rainbow flag.", 150),
        new AccessoryDefinition(PropRedHeartFlagAccessoryId, "Handheld", "Heart flag", "Carry a red heart flag.", 140),
        new AccessoryDefinition(PropCheckeredFlagAccessoryId, "Handheld", "Checkered flag", "Carry a checkered racing flag.", 140),
        new AccessoryDefinition(PropGreenLeafFlagAccessoryId, "Handheld", "Green fish flag", "Carry a green fish flag.", 140),
        new AccessoryDefinition(PropPirateFlagAccessoryId, "Handheld", "Pirate flag", "Carry a pirate flag.", 155),
        new AccessoryDefinition(PropPandaFlagAccessoryId, "Handheld", "Mascot flag", "Carry a mascot flag for your current pal.", 145)
    };

    private static readonly CoinPackDefinition[] CoinPackCatalog = new CoinPackDefinition[]
    {
        new CoinPackDefinition("coin_pack_small", "Starter Pouch", "$4.99", 500, "SP", new Color(0.98f, 0.88f, 0.46f, 1f)),
        new CoinPackDefinition("coin_pack_mid", "Garden Jar", "$16.99", 2200, "GJ", new Color(0.65f, 0.9f, 0.6f, 1f)),
        new CoinPackDefinition("coin_pack_large", "Sanctuary Chest", "$34.99", 5000, "SC", new Color(0.66f, 0.82f, 0.98f, 1f))
    };

    private static readonly Dictionary<string, string> AccessorySpriteResourceNames = new Dictionary<string, string>
    {
        { EyewearRoundGlassesAccessoryId, "eyewear_round_glasses" },
        { EyewearBlackSunglassesAccessoryId, "eyewear_black_sunglasses" },
        { EyewearPixelShadesAccessoryId, "eyewear_pixel_shades" },
        { EyewearHeartGlassesAccessoryId, "eyewear_heart_glasses" },
        { EyewearBlueRoundGlassesAccessoryId, "eyewear_blue_round_glasses" },
        { EyewearStarGlassesAccessoryId, "eyewear_star_glasses" },
        { HeadwearBlueBeanieAccessoryId, "headwear_blue_beanie" },
        { HeadwearChefHatAccessoryId, "headwear_chef_hat" },
        { HeadwearDinoHoodAccessoryId, "headwear_dino_hood" },
        { HeadwearCrownAccessoryId, "headwear_crown" },
        { HeadwearStrawHatAccessoryId, "headwear_straw_hat" },
        { HeadwearRedCapAccessoryId, "headwear_red_cap" },
        { HeadwearAviatorHatAccessoryId, "headwear_aviator_hat" },
        { ShirtGreenBambooAccessoryId, "shirt_green_bamboo" },
        { ShirtBluePawAccessoryId, "shirt_blue_paw" },
        { ShirtYellowPandaAccessoryId, "shirt_yellow_panda" },
        { ShirtBlueHawaiianAccessoryId, "shirt_blue_hawaiian" },
        { ShirtBlackHeartAccessoryId, "shirt_black_heart" },
        { ShirtRedHoodieAccessoryId, "shirt_red_hoodie" },
        { PropBluePawFlagAccessoryId, "prop_blue_paw_flag" },
        { PropRainbowFlagAccessoryId, "prop_rainbow_flag" },
        { PropRedHeartFlagAccessoryId, "prop_red_heart_flag" },
        { PropCheckeredFlagAccessoryId, "prop_checkered_flag" },
        { PropGreenLeafFlagAccessoryId, "prop_green_leaf_flag" },
        { PropPirateFlagAccessoryId, "prop_pirate_flag" },
        { PropPandaFlagAccessoryId, "prop_panda_flag" }
    };

    private static readonly HabitatDefinition[] HabitatCatalog = new HabitatDefinition[]
    {
        new HabitatDefinition("sunny_garden", "Sunny Playpen", "Zoo habitat with trees, a swing, and a small playpen."),
        new HabitatDefinition("bamboo_grove", "Bamboo Grove", "Reward habitat with extra bamboo shade."),
        new HabitatDefinition(
            CozyHouseHabitatId,
            "Cozy House",
            "Final unlockable habitat with a woodland house in your sanctuary.",
            CozyHouseHabitatCost,
            CozyHouseRequiredAccountLevel,
            CozyHouseRequiredPocketPalLevel)
    };

    private static readonly PocketPalDefinition[] PocketPalCatalog = new PocketPalDefinition[]
    {
        new PocketPalDefinition("panda", "Panda", "A cozy bamboo-loving starter pal."),
        new PocketPalDefinition("white_tiger", "White Tiger", "A graceful striped cat with a calm stare.", WhiteTigerUnlockLevel),
        new PocketPalDefinition("otter", "Otter", "A playful river buddy that loves toys and naps.", OtterUnlockLevel)
    };

    public void SetCompactDrawerMode(bool isCompact)
    {
        compactDrawerMode = isCompact;
    }

    private void Start()
    {
        SaveManager.EnsureLoaded();
        ApplyAccessoryLockSetupForTesting();
        ConfigureScrollView();
    }

    private void ApplyAccessoryLockSetupForTesting()
    {
        if (SaveManager.Data == null)
        {
            return;
        }

        if (SaveManager.Data.completedTaskIds == null)
        {
            SaveManager.Data.completedTaskIds = new List<string>();
        }

        if (SaveManager.Data.completedTaskIds.Contains(AccessoryPurchaseLockSetupFlag))
        {
            return;
        }

        bool didChangeOwnership = false;
        for (int index = 0; index < AccessoryLockSetupIds.Length; index++)
        {
            string accessoryId = AccessoryLockSetupIds[index];
            if (SaveManager.Data.ownedAccessoryIds != null && SaveManager.Data.ownedAccessoryIds.Remove(accessoryId))
            {
                didChangeOwnership = true;
            }

            if (SaveManager.Data.equippedAccessoryIds != null && SaveManager.Data.equippedAccessoryIds.Remove(accessoryId))
            {
                didChangeOwnership = true;
            }
        }

        SaveManager.Data.equippedAccessoryId = SaveManager.Data.equippedAccessoryIds != null && SaveManager.Data.equippedAccessoryIds.Count > 0
            ? SaveManager.Data.equippedAccessoryIds[0]
            : string.Empty;

        if (didChangeOwnership)
        {
            RecalculatePocketPalLevelFromEquippedAccessories();
        }

        SaveManager.Data.completedTaskIds.Add(AccessoryPurchaseLockSetupFlag);
        SaveManager.Save();
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

    public void BuildCustomizationContent(RectTransform mainContent)
    {
        ClearChildren(mainContent);

        if (compactDrawerMode)
        {
            BuildCompactAccessoryPickerContent(mainContent);
            return;
        }

        CreateTitle(mainContent, "Customization");

        CreateHabitatSection(mainContent);

        CreatePocketPalSection(mainContent);

        string[] accessoryCategories = new string[] { "Eyewear", "Headwear", "Shirts", "Handheld" };
        float accessoriesSectionHeight = GetAccessoriesSectionHeight(accessoryCategories);

        GameObject accessoriesSection = CreatePanel("AccessoriesSection", mainContent, new Color(1f, 1f, 1f, 0.42f), accessoriesSectionHeight);
        ConfigurePanelLayout(accessoriesSection, 28, 28, 34, 34, 24);

        TMP_Text accessoriesTitle = CreateText("AccessoriesTitle", accessoriesSection.transform, "Accessories", 34, FontStyles.Bold, TextAlignmentOptions.Center);
        accessoriesTitle.color = new Color(0.08f, 0.18f, 0.22f);

        TMP_Text accessoriesSubtitle = CreateText("AccessoriesSubtitle", accessoriesSection.transform, "Dress up your pal by category.", 24, FontStyles.Normal, TextAlignmentOptions.Center);
        accessoriesSubtitle.color = new Color(0.16f, 0.28f, 0.32f);

        for (int index = 0; index < accessoryCategories.Length; index++)
        {
            CreateAccessoryCategory(accessoriesSection.transform, accessoryCategories[index]);
        }
    }

    private void BuildCompactAccessoryPickerContent(RectTransform mainContent)
    {
        TMP_Text title = CreateText("CompactAccessoryTitle", mainContent, "Accessories", 30, FontStyles.Bold, TextAlignmentOptions.Left);
        title.color = new Color(0.08f, 0.18f, 0.22f);

        TMP_Text subtitle = CreateText("CompactAccessorySubtitle", mainContent, "Pick your pal, then tap an icon to equip.", 20, FontStyles.Normal, TextAlignmentOptions.Left);
        subtitle.color = new Color(0.2f, 0.32f, 0.36f);

        CreateCompactPocketPalSelector(mainContent);
        CreateCompactHabitatSelector(mainContent);

        string[] accessoryCategories = new string[] { "Headwear", "Eyewear", "Shirts", "Handheld" };
        for (int index = 0; index < accessoryCategories.Length; index++)
        {
            CreateCompactAccessoryCategory(mainContent, accessoryCategories[index]);
        }
    }

    private void CreateCompactPocketPalSelector(Transform parent)
    {
        GameObject section = CreatePanel("CompactPocketPalSelector", parent, new Color(0.93f, 0.97f, 0.96f, 0.82f), 254f);
        ConfigurePanelLayout(section, 16, 16, 14, 16, 10);

        TMP_Text title = CreateText("CompactPocketPalSelectorTitle", section.transform, "PocketPals", 22, FontStyles.Bold, TextAlignmentOptions.Left);
        title.color = new Color(0.08f, 0.18f, 0.22f);

        GameObject rowObject = new GameObject("CompactPocketPalSelectorRow", typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(LayoutElement));
        rowObject.transform.SetParent(section.transform, false);

        HorizontalLayoutGroup rowLayout = rowObject.GetComponent<HorizontalLayoutGroup>();
        rowLayout.spacing = 10f;
        rowLayout.childAlignment = TextAnchor.MiddleCenter;
        rowLayout.childControlWidth = true;
        rowLayout.childControlHeight = true;
        rowLayout.childForceExpandWidth = true;
        rowLayout.childForceExpandHeight = false;

        LayoutElement rowElement = rowObject.GetComponent<LayoutElement>();
        rowElement.preferredHeight = 156f;
        rowElement.minHeight = 156f;

        for (int index = 0; index < PocketPalCatalog.Length; index++)
        {
            CreateCompactPocketPalSelectorButton(rowObject.transform, PocketPalCatalog[index]);
        }
    }

    private void CreateCompactPocketPalSelectorButton(Transform parent, PocketPalDefinition pocketPal)
    {
        bool isOwned = IsPocketPalOwned(pocketPal.id);
        bool canUnlockNow = CanUnlockPocketPalNow(pocketPal);
        bool isSelected = isOwned && SaveManager.Data.selectedPocketPalId == pocketPal.id;
        Color buttonColor = isSelected
            ? new Color(0.72f, 0.9f, 0.7f, 1f)
            : isOwned
                ? new Color(0.95f, 0.97f, 1f, 1f)
                : new Color(0.86f, 0.88f, 0.9f, 0.92f);

        GameObject buttonObject = new GameObject(pocketPal.id + "CompactPalButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(LayoutElement));
        buttonObject.transform.SetParent(parent, false);

        Image buttonImage = buttonObject.GetComponent<Image>();
        buttonImage.color = buttonColor;
        buttonImage.raycastTarget = true;

        LayoutElement buttonElement = buttonObject.GetComponent<LayoutElement>();
        buttonElement.preferredHeight = 146f;
        buttonElement.minHeight = 146f;
        buttonElement.preferredWidth = 0f;
        buttonElement.flexibleWidth = 1f;

        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = buttonImage;
        button.onClick.AddListener(delegate { SelectPocketPal(pocketPal.id); });

        VerticalLayoutGroup buttonLayout = buttonObject.AddComponent<VerticalLayoutGroup>();
        buttonLayout.padding = new RectOffset(8, 8, 12, 10);
        buttonLayout.spacing = 2f;
        buttonLayout.childAlignment = TextAnchor.MiddleCenter;
        buttonLayout.childControlWidth = true;
        buttonLayout.childControlHeight = true;
        buttonLayout.childForceExpandWidth = true;
        buttonLayout.childForceExpandHeight = false;

        GameObject iconObject = new GameObject(pocketPal.id + "CompactPalIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(LayoutElement));
        iconObject.transform.SetParent(buttonObject.transform, false);
        Image iconImage = iconObject.GetComponent<Image>();
        iconImage.sprite = GetPocketPalSelectorSprite(pocketPal.id);
        iconImage.preserveAspect = true;
        iconImage.color = iconImage.sprite != null ? Color.white : new Color(1f, 1f, 1f, 0.2f);
        iconImage.raycastTarget = false;

        LayoutElement iconLayout = iconObject.GetComponent<LayoutElement>();
        iconLayout.preferredWidth = CompactSelectorIconSize;
        iconLayout.preferredHeight = CompactSelectorIconSize;
        iconLayout.minWidth = CompactSelectorIconSize;
        iconLayout.minHeight = CompactSelectorIconSize;

        TMP_Text nameLabel = CreateText(pocketPal.id + "CompactPalName", buttonObject.transform, pocketPal.name, 18, FontStyles.Bold, TextAlignmentOptions.Center);
        nameLabel.color = isOwned ? new Color(0.08f, 0.18f, 0.22f) : new Color(0.24f, 0.29f, 0.33f);

        string statusText = isOwned ? (isSelected ? "Selected" : "Owned") : canUnlockNow ? "Tap to unlock" : "Unlocks at Lv " + pocketPal.requiredAccountLevel;
        TMP_Text statusLabel = CreateText(pocketPal.id + "CompactPalStatus", buttonObject.transform, statusText, 14, FontStyles.Normal, TextAlignmentOptions.Center);
        statusLabel.color = isOwned ? new Color(0.1f, 0.22f, 0.26f) : new Color(0.3f, 0.34f, 0.38f);

        if (!isOwned)
        {
            GameObject lockObject = new GameObject(pocketPal.id + "CompactPalLock", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(LayoutElement));
            lockObject.transform.SetParent(buttonObject.transform, false);

            RectTransform lockRect = lockObject.GetComponent<RectTransform>();
            lockRect.anchorMin = new Vector2(1f, 1f);
            lockRect.anchorMax = new Vector2(1f, 1f);
            lockRect.pivot = new Vector2(1f, 1f);
            lockRect.sizeDelta = new Vector2(28f, 28f);
            lockRect.anchoredPosition = new Vector2(-8f, -8f);

            Image lockImage = lockObject.GetComponent<Image>();
            lockImage.sprite = GetLockBadgeSprite();
            lockImage.preserveAspect = true;
            lockImage.color = canUnlockNow ? new Color(1f, 0.95f, 0.28f, 1f) : Color.white;
            lockImage.raycastTarget = false;

            LayoutElement lockLayout = lockObject.GetComponent<LayoutElement>();
            lockLayout.ignoreLayout = true;
        }
    }

    private void CreateCompactHabitatSelector(Transform parent)
    {
        const float compactHomesCategoryHeight = 226f;
        const float compactHomeCardHeight = 122f;

        HabitatDefinition homeHabitat = GetHabitatDefinition(CozyHouseHabitatId);
        if (homeHabitat == null)
        {
            return;
        }

        float panelHeight = compactHomesCategoryHeight;
        GameObject section = CreatePanel("CompactHabitatSelector", parent, new Color(0.93f, 0.97f, 0.96f, 0.82f), panelHeight);
        ConfigurePanelLayout(section, 16, 16, 14, 16, 10);

        TMP_Text title = CreateText("CompactHabitatSelectorTitle", section.transform, "Homes", 22, FontStyles.Bold, TextAlignmentOptions.Left);
        title.color = new Color(0.08f, 0.18f, 0.22f);

        CreateCompactHabitatCard(section.transform, homeHabitat, compactHomeCardHeight);
    }

    private void CreateCompactHabitatCard(Transform parent, HabitatDefinition habitat, float cardHeight)
    {
        bool isOwned = IsHabitatOwned(habitat.id);
        bool isSelected = SaveManager.Data.selectedHabitatId == habitat.id;
        bool canUnlockNow = CanUnlockHabitatNow(habitat);
        bool canAttemptUnlock = IsHabitatUnlockableByPurchase(habitat) && !isOwned;

        Color cardColor = isSelected
            ? new Color(0.72f, 0.9f, 0.7f, 0.96f)
            : isOwned
                ? new Color(0.96f, 0.98f, 1f, 0.96f)
                : new Color(0.86f, 0.88f, 0.9f, 0.94f);

        GameObject card = CreatePanel(habitat.id + "CompactHabitatCard", parent, cardColor, cardHeight);
        HorizontalLayoutGroup cardLayout = card.AddComponent<HorizontalLayoutGroup>();
        cardLayout.padding = new RectOffset(14, 14, 14, 12);
        cardLayout.spacing = 12f;
        cardLayout.childAlignment = TextAnchor.MiddleLeft;
        cardLayout.childControlWidth = false;
        cardLayout.childControlHeight = true;
        cardLayout.childForceExpandWidth = false;
        cardLayout.childForceExpandHeight = false;

        if (isOwned || canAttemptUnlock)
        {
            Button button = card.AddComponent<Button>();
            Image image = card.GetComponent<Image>();
            image.raycastTarget = true;
            button.targetGraphic = image;
            if (isOwned)
            {
                button.onClick.AddListener(delegate { SelectHabitat(habitat.id); });
            }
            else
            {
                button.onClick.AddListener(delegate { ShowHabitatPurchaseDialog(habitat); });
            }
        }

        GameObject previewObject = new GameObject(habitat.id + "CompactHabitatPreview", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(LayoutElement));
        previewObject.transform.SetParent(card.transform, false);
        Image previewImage = previewObject.GetComponent<Image>();
        previewImage.sprite = GetHabitatPreviewSprite(habitat.id);
        previewImage.preserveAspect = true;
        previewImage.raycastTarget = false;
        previewImage.color = previewImage.sprite != null ? (isOwned ? Color.white : new Color(1f, 1f, 1f, 0.44f)) : new Color(1f, 1f, 1f, 0.22f);
        LayoutElement previewLayout = previewObject.GetComponent<LayoutElement>();
        previewLayout.preferredWidth = 72f;
        previewLayout.minWidth = 72f;
        previewLayout.preferredHeight = 72f;
        previewLayout.minHeight = 72f;

        GameObject textColumn = new GameObject(habitat.id + "CompactHabitatTextColumn", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(LayoutElement));
        textColumn.transform.SetParent(card.transform, false);
        VerticalLayoutGroup textLayout = textColumn.GetComponent<VerticalLayoutGroup>();
        textLayout.padding = new RectOffset(0, 0, 2, 0);
        textLayout.spacing = 3f;
        textLayout.childAlignment = TextAnchor.UpperLeft;
        textLayout.childControlWidth = true;
        textLayout.childControlHeight = true;
        textLayout.childForceExpandWidth = true;
        textLayout.childForceExpandHeight = false;
        LayoutElement textColumnLayout = textColumn.GetComponent<LayoutElement>();
        textColumnLayout.flexibleWidth = 1f;
        textColumnLayout.preferredWidth = 0f;

        TMP_Text homeName = CreateText(habitat.id + "CompactHabitatName", textColumn.transform, habitat.name, 20, FontStyles.Bold, TextAlignmentOptions.Left);
        homeName.color = new Color(0.08f, 0.18f, 0.22f);

        string statusText = GetHabitatStatusText(habitat, isOwned, isSelected);
        TMP_Text statusLabel = CreateText(habitat.id + "CompactHabitatLabel", textColumn.transform, statusText, 17, FontStyles.Normal, TextAlignmentOptions.Left);
        statusLabel.color = new Color(0.08f, 0.18f, 0.22f);

        if (!isOwned)
        {
            GameObject badgeObject = new GameObject(habitat.id + "CompactHabitatLock", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(LayoutElement));
            badgeObject.transform.SetParent(card.transform, false);
            RectTransform badgeRect = badgeObject.GetComponent<RectTransform>();
            badgeRect.anchorMin = new Vector2(1f, 0.5f);
            badgeRect.anchorMax = new Vector2(1f, 0.5f);
            badgeRect.pivot = new Vector2(1f, 0.5f);
            badgeRect.sizeDelta = new Vector2(30f, 30f);
            badgeRect.anchoredPosition = new Vector2(-4f, -2f);

            Image badgeImage = badgeObject.GetComponent<Image>();
            badgeImage.sprite = GetLockBadgeSprite();
            badgeImage.preserveAspect = true;
            badgeImage.color = canUnlockNow ? new Color(1f, 0.95f, 0.28f, 1f) : new Color(1f, 1f, 1f, 0.92f);
            badgeImage.raycastTarget = false;

            LayoutElement badgeLayout = badgeObject.GetComponent<LayoutElement>();
            badgeLayout.ignoreLayout = true;
        }
    }

    private Sprite GetHabitatPreviewSprite(string habitatId)
    {
        if (string.IsNullOrEmpty(habitatId))
        {
            return null;
        }

        if (habitatPreviewSpriteCache.ContainsKey(habitatId))
        {
            return habitatPreviewSpriteCache[habitatId];
        }

        string resourcePath = habitatId == CozyHouseHabitatId ? HouseHabitatPreviewResourcePath : string.Empty;
        Sprite sprite = TryCreateSpriteFromResource(resourcePath);
        habitatPreviewSpriteCache[habitatId] = sprite;
        return sprite;
    }

    private Sprite GetPocketPalSelectorSprite(string pocketPalId)
    {
        if (string.IsNullOrEmpty(pocketPalId))
        {
            return null;
        }

        if (pocketPalIconSpriteCache.ContainsKey(pocketPalId))
        {
            return pocketPalIconSpriteCache[pocketPalId];
        }

        Sprite sprite = TryCreateSpriteFromResource(GetPocketPalIconResourcePath(pocketPalId));
        if (sprite == null && pocketPalId != "panda")
        {
            sprite = TryCreateSpriteFromResource(PandaPocketPalIconResourcePath);
        }

        pocketPalIconSpriteCache[pocketPalId] = sprite;
        return sprite;
    }

    private string GetPocketPalIconResourcePath(string pocketPalId)
    {
        if (pocketPalId == "white_tiger")
        {
            return WhiteTigerPocketPalIconResourcePath;
        }

        if (pocketPalId == "otter")
        {
            return OtterPocketPalIconResourcePath;
        }

        return PandaPocketPalIconResourcePath;
    }

    private void CreateCompactAccessoryCategory(Transform parent, string categoryTitle)
    {
        AccessoryDefinition[] categoryItems = GetAccessoriesForCategory(categoryTitle);
        int rows = Mathf.CeilToInt(categoryItems.Length / 2f);
        float panelHeight = 86f + rows * 130f;
        GameObject categoryPanel = CreatePanel(categoryTitle.Replace(" ", "") + "CompactCategory", parent, new Color(0.93f, 0.97f, 0.96f, 0.82f), panelHeight);
        ConfigurePanelLayout(categoryPanel, 16, 16, 14, 16, 10);

        TMP_Text categoryLabel = CreateText(categoryTitle.Replace(" ", "") + "CompactCategoryLabel", categoryPanel.transform, categoryTitle, 22, FontStyles.Bold, TextAlignmentOptions.Left);
        categoryLabel.color = new Color(0.08f, 0.18f, 0.22f);

        GameObject gridObject = new GameObject(categoryTitle.Replace(" ", "") + "CompactGrid", typeof(RectTransform), typeof(GridLayoutGroup), typeof(ContentSizeFitter));
        gridObject.transform.SetParent(categoryPanel.transform, false);

        RectTransform gridRect = gridObject.GetComponent<RectTransform>();
        gridRect.anchorMin = new Vector2(0f, 1f);
        gridRect.anchorMax = new Vector2(1f, 1f);
        gridRect.pivot = new Vector2(0.5f, 1f);
        gridRect.anchoredPosition = Vector2.zero;
        gridRect.sizeDelta = new Vector2(0f, 0f);

        GridLayoutGroup gridLayout = gridObject.GetComponent<GridLayoutGroup>();
        gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
        gridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;
        gridLayout.childAlignment = TextAnchor.UpperCenter;
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = 2;
        gridLayout.cellSize = new Vector2(118f, 118f);
        gridLayout.spacing = new Vector2(10f, 10f);

        ContentSizeFitter sizeFitter = gridObject.GetComponent<ContentSizeFitter>();
        sizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        for (int index = 0; index < categoryItems.Length; index++)
        {
            CreateCompactAccessoryIconButton(gridObject.transform, categoryItems[index]);
        }
    }

    private void CreateCompactAccessoryIconButton(Transform parent, AccessoryDefinition accessory)
    {
        bool isOwned = IsAccessoryOwned(accessory.id);
        bool isEquipped = IsAccessoryEquipped(accessory.id);
        Color buttonColor = isEquipped
            ? new Color(0.72f, 0.9f, 0.7f, 1f)
            : isOwned
                ? new Color(0.97f, 0.98f, 1f, 1f)
                : new Color(0.86f, 0.88f, 0.9f, 0.95f);

        GameObject buttonObject = new GameObject(accessory.id + "CompactButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(parent, false);

        Image backgroundImage = buttonObject.GetComponent<Image>();
        backgroundImage.color = buttonColor;
        backgroundImage.raycastTarget = true;

        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = backgroundImage;
        button.interactable = true;
        button.onClick.AddListener(delegate { HandleAccessoryIconClicked(accessory.id); });

        GameObject previewObject = new GameObject(accessory.id + "CompactPreview", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        previewObject.transform.SetParent(buttonObject.transform, false);

        RectTransform previewRect = previewObject.GetComponent<RectTransform>();
        previewRect.anchorMin = new Vector2(0.5f, 0.5f);
        previewRect.anchorMax = new Vector2(0.5f, 0.5f);
        previewRect.pivot = new Vector2(0.5f, 0.5f);
        previewRect.sizeDelta = new Vector2(98f, 98f);
        previewRect.anchoredPosition = new Vector2(0f, -2f);

        Image previewImage = previewObject.GetComponent<Image>();
        previewImage.sprite = GetAccessoryPreviewSprite(accessory.id);
        previewImage.preserveAspect = true;
        previewImage.raycastTarget = false;
        previewImage.color = isOwned ? Color.white : new Color(1f, 1f, 1f, 0.32f);

        if (!isOwned)
        {
            CreateLockedAccessoryOverlay(buttonObject.transform);
        }

        GameObject statusObject = new GameObject(accessory.id + "CompactStatus", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        statusObject.transform.SetParent(buttonObject.transform, false);

        RectTransform statusRect = statusObject.GetComponent<RectTransform>();
        statusRect.anchorMin = new Vector2(1f, 1f);
        statusRect.anchorMax = new Vector2(1f, 1f);
        statusRect.pivot = new Vector2(1f, 1f);
        statusRect.sizeDelta = new Vector2(28f, 28f);
        statusRect.anchoredPosition = new Vector2(-4f, -4f);

        Image statusBackground = statusObject.GetComponent<Image>();
        statusBackground.color = isEquipped ? new Color(0.18f, 0.66f, 0.32f, 0.95f) : isOwned ? new Color(0.18f, 0.36f, 0.74f, 0.88f) : new Color(0.36f, 0.36f, 0.36f, 0.75f);
        statusBackground.raycastTarget = false;

        if (!isOwned)
        {
            GameObject statusIconObject = new GameObject(accessory.id + "CompactStatusLock", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            statusIconObject.transform.SetParent(statusObject.transform, false);
            RectTransform statusIconRect = statusIconObject.GetComponent<RectTransform>();
            statusIconRect.anchorMin = Vector2.zero;
            statusIconRect.anchorMax = Vector2.one;
            statusIconRect.offsetMin = new Vector2(4f, 4f);
            statusIconRect.offsetMax = new Vector2(-4f, -4f);

            Image statusIcon = statusIconObject.GetComponent<Image>();
            statusIcon.sprite = GetLockBadgeSprite();
            statusIcon.preserveAspect = true;
            statusIcon.color = Color.white;
            statusIcon.raycastTarget = false;
            return;
        }

        GameObject statusLabelObject = new GameObject(accessory.id + "CompactStatusLabel", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        statusLabelObject.transform.SetParent(statusObject.transform, false);
        RectTransform statusLabelRect = statusLabelObject.GetComponent<RectTransform>();
        statusLabelRect.anchorMin = Vector2.zero;
        statusLabelRect.anchorMax = Vector2.one;
        statusLabelRect.offsetMin = Vector2.zero;
        statusLabelRect.offsetMax = Vector2.zero;

        TextMeshProUGUI statusLabel = statusLabelObject.GetComponent<TextMeshProUGUI>();
        statusLabel.text = isEquipped ? "E" : "O";
        statusLabel.alignment = TextAlignmentOptions.Center;
        statusLabel.fontSize = 18f;
        statusLabel.fontStyle = FontStyles.Bold;
        statusLabel.color = Color.white;
        statusLabel.raycastTarget = false;
    }

    private void HandleAccessoryIconClicked(string accessoryId)
    {
        if (IsAccessoryOwned(accessoryId))
        {
            ToggleAccessory(accessoryId);
            return;
        }

        ShowAccessoryPurchaseDialog(accessoryId);
    }

    private void CreateLockedAccessoryOverlay(Transform parent)
    {
        GameObject dimObject = new GameObject("LockedDim", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(LayoutElement));
        dimObject.transform.SetParent(parent, false);

        RectTransform dimRect = dimObject.GetComponent<RectTransform>();
        dimRect.anchorMin = Vector2.zero;
        dimRect.anchorMax = Vector2.one;
        dimRect.offsetMin = Vector2.zero;
        dimRect.offsetMax = Vector2.zero;

        Image dimImage = dimObject.GetComponent<Image>();
        dimImage.color = new Color(0.08f, 0.1f, 0.12f, 0.25f);
        dimImage.raycastTarget = false;

        LayoutElement dimLayout = dimObject.GetComponent<LayoutElement>();
        dimLayout.ignoreLayout = true;

        GameObject iconObject = new GameObject("LockedIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(LayoutElement));
        iconObject.transform.SetParent(parent, false);

        RectTransform iconRect = iconObject.GetComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0.5f, 0.5f);
        iconRect.anchorMax = new Vector2(0.5f, 0.5f);
        iconRect.pivot = new Vector2(0.5f, 0.5f);
        iconRect.sizeDelta = new Vector2(36f, 36f);
        iconRect.anchoredPosition = new Vector2(0f, 0f);

        Image iconImage = iconObject.GetComponent<Image>();
        iconImage.sprite = GetLockBadgeSprite();
        iconImage.preserveAspect = true;
        iconImage.color = new Color(1f, 1f, 1f, 0.98f);
        iconImage.raycastTarget = false;

        LayoutElement iconLayout = iconObject.GetComponent<LayoutElement>();
        iconLayout.ignoreLayout = true;
    }

    private static Sprite GetLockBadgeSprite()
    {
        if (lockBadgeSprite != null)
        {
            return lockBadgeSprite;
        }

        lockBadgeSprite = CreateLockBadgeSprite();
        return lockBadgeSprite;
    }

    private static Sprite CreateLockBadgeSprite()
    {
        const int size = 64;
        Texture2D texture = CreateTransparentTexture(size, size);

        Color shackleColor = new Color(0.25f, 0.2f, 0.16f, 1f);
        Color bodyColor = new Color(1f, 0.86f, 0.2f, 1f);
        Color bodyHighlight = new Color(1f, 0.92f, 0.36f, 1f);
        Color keyholeColor = new Color(0.23f, 0.18f, 0.12f, 1f);

        DrawEllipse(texture, 32, 44, 15, 13, shackleColor);
        DrawEllipse(texture, 32, 44, 11, 9, Color.clear);

        DrawRoundedRect(texture, 14, 12, 36, 30, 7, bodyColor);
        DrawRoundedRect(texture, 18, 16, 28, 22, 5, bodyHighlight);

        DrawEllipse(texture, 32, 28, 4, 5, keyholeColor);
        DrawRoundedRect(texture, 30, 20, 4, 8, 1, keyholeColor);

        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.Apply();

        return Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), 100f);
    }

    private float GetAccessoriesSectionHeight(string[] accessoryCategories)
    {
        // Includes section padding/title/subtitle plus each category panel and layout spacing.
        float totalHeight = 300f;
        for (int index = 0; index < accessoryCategories.Length; index++)
        {
            AccessoryDefinition[] categoryItems = GetAccessoriesForCategory(accessoryCategories[index]);
            float rowHeight = accessoryCategories[index] == "Shirts" || accessoryCategories[index] == "Handheld" ? 236f : 192f;
            float categoryHeight = 120f + categoryItems.Length * rowHeight;
            totalHeight += categoryHeight;
        }

        // Category-to-category spacing and a small bottom cushion.
        totalHeight += (accessoryCategories.Length * 24f) + 80f;
        return totalHeight;
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

    private void CreateHabitatSection(Transform parent)
    {
        float dynamicHeight = Mathf.Max(540f, 190f + HabitatCatalog.Length * 142f);
        GameObject section = CreatePanel("HabitatBackgroundsSection", parent, new Color(1f, 1f, 1f, 0.42f), dynamicHeight);
        ConfigurePanelLayout(section, 28, 28, 34, 34, 20);

        TMP_Text titleText = CreateText("HabitatBackgroundsTitle", section.transform, "Habitat Backgrounds", 34, FontStyles.Bold, TextAlignmentOptions.Center);
        titleText.color = new Color(0.08f, 0.18f, 0.22f);

        TMP_Text subtitleText = CreateText("HabitatBackgroundsSubtitle", section.transform, "Choose your daytime habitat. Night mode rotates automatically.", 24, FontStyles.Normal, TextAlignmentOptions.Center);
        subtitleText.color = new Color(0.16f, 0.28f, 0.32f);

        for (int index = 0; index < HabitatCatalog.Length; index++)
        {
            CreateHabitatCard(HabitatCatalog[index], section.transform);
        }
    }

    private void CreatePocketPalSection(Transform parent)
    {
        float dynamicHeight = Mathf.Max(520f, 190f + PocketPalCatalog.Length * 142f);
        GameObject section = CreatePanel("PocketPalsSection", parent, new Color(1f, 1f, 1f, 0.42f), dynamicHeight);
        ConfigurePanelLayout(section, 28, 28, 34, 34, 20);

        TMP_Text titleText = CreateText("PocketPalsTitle", section.transform, "PocketPals", 34, FontStyles.Bold, TextAlignmentOptions.Center);
        titleText.color = new Color(0.08f, 0.18f, 0.22f);

        TMP_Text subtitleText = CreateText("PocketPalsSubtitle", section.transform, "Pick the animal living in your sanctuary.", 24, FontStyles.Normal, TextAlignmentOptions.Center);
        subtitleText.color = new Color(0.16f, 0.28f, 0.32f);

        for (int index = 0; index < PocketPalCatalog.Length; index++)
        {
            CreatePocketPalCard(PocketPalCatalog[index], section.transform);
        }
    }

    private void CreatePocketPalCard(PocketPalDefinition pocketPal, Transform parent)
    {
        bool isOwned = IsPocketPalOwned(pocketPal.id);
        bool canUnlockNow = CanUnlockPocketPalNow(pocketPal);
        bool isSelected = isOwned && SaveManager.Data.selectedPocketPalId == pocketPal.id;
        Color cardColor = isSelected
            ? new Color(0.78f, 0.9f, 0.76f, 0.96f)
            : isOwned
                ? new Color(0.94f, 0.97f, 0.98f, 0.95f)
                : new Color(0.88f, 0.9f, 0.92f, 0.78f);
        GameObject card = CreatePanel(pocketPal.id + "PocketPalCard", parent, cardColor, 126f);

        Button button = card.AddComponent<Button>();
        Image image = card.GetComponent<Image>();
        image.raycastTarget = true;
        button.interactable = true;
        button.targetGraphic = image;
        button.onClick.AddListener(delegate { SelectPocketPal(pocketPal.id); });

        ConfigurePanelLayout(card, 24, 24, 20, 20, 8);

        string statusText = isOwned ? (isSelected ? "Selected" : "Tap to use") : canUnlockNow ? "Tap to unlock" : "Unlocks at Lv " + pocketPal.requiredAccountLevel;
        TMP_Text label = CreateText(pocketPal.id + "PocketPalLabel", card.transform, pocketPal.name + "\n" + pocketPal.description + "\n" + statusText, 25, FontStyles.Normal, TextAlignmentOptions.Left);
        label.color = isOwned ? new Color(0.08f, 0.18f, 0.22f) : new Color(0.22f, 0.28f, 0.3f);
    }

    private void CreateHabitatCard(HabitatDefinition habitat, Transform parent)
    {
        bool isOwned = IsHabitatOwned(habitat.id);
        bool isSelected = SaveManager.Data.selectedHabitatId == habitat.id;
        bool canAttemptUnlock = IsHabitatUnlockableByPurchase(habitat) && !isOwned;
        Color cardColor = isSelected ? new Color(0.78f, 0.9f, 0.76f, 0.96f) : isOwned ? new Color(0.94f, 0.97f, 0.98f, 0.95f) : new Color(0.88f, 0.9f, 0.92f, 0.78f);
        GameObject card = CreatePanel(habitat.id + "HabitatCard", parent, cardColor, 126f);

        if (isOwned || canAttemptUnlock)
        {
            Button button = card.AddComponent<Button>();
            Image image = card.GetComponent<Image>();
            image.raycastTarget = true;
            button.interactable = true;
            button.targetGraphic = image;
            if (isOwned)
            {
                button.onClick.AddListener(delegate { SelectHabitat(habitat.id); });
            }
            else
            {
                button.onClick.AddListener(delegate { ShowHabitatPurchaseDialog(habitat); });
            }
        }

        ConfigurePanelLayout(card, 24, 24, 20, 20, 8);

        string status = GetHabitatStatusText(habitat, isOwned, isSelected);
        TMP_Text label = CreateText(habitat.id + "HabitatLabel", card.transform, habitat.name + "\n" + habitat.description + "\n" + status, 25, FontStyles.Normal, TextAlignmentOptions.Left);
        label.color = isOwned ? new Color(0.08f, 0.18f, 0.22f) : new Color(0.22f, 0.28f, 0.3f);
    }

    private string GetHabitatStatusText(HabitatDefinition habitat, bool isOwned, bool isSelected)
    {
        if (isSelected)
        {
            return "Selected";
        }

        if (isOwned)
        {
            return "Tap to use";
        }

        if (!IsHabitatUnlockableByPurchase(habitat))
        {
            return "Locked";
        }

        if (!MeetsHabitatUnlockLevelRequirements(habitat))
        {
            return "Needs User Lv " + habitat.requiredAccountLevel + " + Pal Lv " + habitat.requiredPocketPalLevel;
        }

        if (!CanAffordHabitatUnlock(habitat))
        {
            return "Needs " + habitat.coinCost + " coins";
        }

        return "Tap to unlock - " + habitat.coinCost + " coins";
    }

    private bool IsHabitatUnlockableByPurchase(HabitatDefinition habitat)
    {
        return habitat != null && habitat.coinCost > 0;
    }

    private bool MeetsHabitatUnlockLevelRequirements(HabitatDefinition habitat)
    {
        if (habitat == null)
        {
            return false;
        }

        return SaveManager.GetAccountLevel() >= habitat.requiredAccountLevel &&
               SaveManager.GetSelectedPocketPalLevel() >= habitat.requiredPocketPalLevel;
    }

    private bool CanAffordHabitatUnlock(HabitatDefinition habitat)
    {
        return habitat != null && SaveManager.Data != null && SaveManager.Data.coins >= habitat.coinCost;
    }

    private bool CanUnlockHabitatNow(HabitatDefinition habitat)
    {
        return IsHabitatUnlockableByPurchase(habitat) &&
               MeetsHabitatUnlockLevelRequirements(habitat) &&
               CanAffordHabitatUnlock(habitat);
    }

    private HabitatDefinition GetHabitatDefinition(string habitatId)
    {
        if (string.IsNullOrEmpty(habitatId))
        {
            return null;
        }

        for (int index = 0; index < HabitatCatalog.Length; index++)
        {
            if (HabitatCatalog[index].id == habitatId)
            {
                return HabitatCatalog[index];
            }
        }

        return null;
    }

    private void CreateAccessoryCategory(Transform parent, string categoryTitle)
    {
        AccessoryDefinition[] categoryItems = GetAccessoriesForCategory(categoryTitle);
        float rowHeight = categoryTitle == "Shirts" || categoryTitle == "Handheld" ? 236f : 192f;
        GameObject categoryPanel = CreatePanel(categoryTitle.Replace("-", "").Replace(" ", "") + "Category", parent, new Color(0.93f, 0.97f, 0.96f, 0.72f), 120f + categoryItems.Length * rowHeight);
        ConfigurePanelLayout(categoryPanel, 22, 22, 20, 20, 14);

        TMP_Text categoryLabel = CreateText(categoryTitle.Replace(" ", "") + "CategoryLabel", categoryPanel.transform, categoryTitle, 28, FontStyles.Bold, TextAlignmentOptions.Left);
        categoryLabel.color = new Color(0.08f, 0.18f, 0.22f);

        for (int index = 0; index < categoryItems.Length; index++)
        {
            AccessoryDefinition accessory = categoryItems[index];
            string status = GetAccessoryStatusText(accessory.id);
            string cardText = accessory.name + "\n" + accessory.description + "\n" + status;
            CreateAccessoryCard(accessory.id + "Card", categoryPanel.transform, cardText, accessory.id);
        }
    }

    private void CreateAccessoryCard(string objectName, Transform parent, string text, string accessoryId)
    {
        bool isOwned = IsAccessoryOwned(accessoryId);
        bool isEquipped = IsAccessoryEquipped(accessoryId);
        Color cardColor = isEquipped
            ? new Color(0.78f, 0.9f, 0.76f, 0.96f)
            : isOwned
                ? new Color(0.94f, 0.97f, 0.98f, 0.95f)
                : new Color(0.86f, 0.88f, 0.9f, 0.93f);
        string category = GetAccessoryCategory(accessoryId);
        float cardHeight = category == "Shirts" || category == "Handheld" ? 212f : 168f;
        GameObject card = CreatePanel(objectName, parent, cardColor, cardHeight);

        Button button = card.AddComponent<Button>();
        Image image = card.GetComponent<Image>();
        image.raycastTarget = true;
        button.targetGraphic = image;
        button.onClick.AddListener(delegate { HandleAccessoryIconClicked(accessoryId); });

        ConfigurePanelLayout(card, 24, 24, 20, 20, 8);

        CreateAccessoryPreview(card.transform, accessoryId, isOwned);

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

        AccessoryDefinition definition = GetAccessoryDefinition(accessoryId);
        if (definition == null)
        {
            return "Locked";
        }

        return "Locked - " + definition.price + " coins";
    }

    private bool IsHabitatOwned(string habitatId)
    {
        return SaveManager.Data.ownedHabitatIds != null && SaveManager.Data.ownedHabitatIds.Contains(habitatId);
    }

    private bool IsPocketPalOwned(string pocketPalId)
    {
        return SaveManager.Data.ownedPocketPalIds != null && SaveManager.Data.ownedPocketPalIds.Contains(pocketPalId);
    }

    private PocketPalDefinition GetPocketPalDefinition(string pocketPalId)
    {
        for (int index = 0; index < PocketPalCatalog.Length; index++)
        {
            if (PocketPalCatalog[index].id == pocketPalId)
            {
                return PocketPalCatalog[index];
            }
        }

        return null;
    }

    private bool CanUnlockPocketPalNow(PocketPalDefinition pocketPal)
    {
        if (pocketPal == null)
        {
            return false;
        }

        return SaveManager.GetAccountLevel() >= pocketPal.requiredAccountLevel;
    }

    private bool TryUnlockPocketPalByLevel(string pocketPalId)
    {
        if (IsPocketPalOwned(pocketPalId))
        {
            return true;
        }

        PocketPalDefinition pocketPal = GetPocketPalDefinition(pocketPalId);
        if (pocketPal == null || !CanUnlockPocketPalNow(pocketPal))
        {
            return false;
        }

        if (SaveManager.Data.ownedPocketPalIds == null)
        {
            SaveManager.Data.ownedPocketPalIds = new List<string>();
        }

        SaveManager.Data.ownedPocketPalIds.Add(pocketPalId);
        return true;
    }

    private void SelectHabitat(string habitatId)
    {
        SaveManager.EnsureLoaded();

        if (!IsHabitatOwned(habitatId))
        {
            return;
        }

        SaveManager.Data.selectedHabitatId = habitatId;
        SaveManager.Save();
        RefreshAfterSave(SceneManager.GetActiveScene().name);
    }

    private void SelectPocketPal(string pocketPalId)
    {
        SaveManager.EnsureLoaded();
        if (!TryUnlockPocketPalByLevel(pocketPalId))
        {
            return;
        }

        if (!SaveManager.SetSelectedPocketPal(pocketPalId))
        {
            return;
        }

        SaveManager.Data.selectedPocketPalName = GetPocketPalName(pocketPalId);
        SaveManager.Save();
        RefreshAfterSave(SceneManager.GetActiveScene().name);
    }

    private AccessoryDefinition[] GetAccessoriesForCategory(string category)
    {
        List<AccessoryDefinition> matches = new List<AccessoryDefinition>();
        for (int index = 0; index < AccessoryCatalog.Length; index++)
        {
            if (AccessoryCatalog[index].category == category)
            {
                matches.Add(AccessoryCatalog[index]);
            }
        }

        return matches.ToArray();
    }

    private AccessoryDefinition GetAccessoryDefinition(string accessoryId)
    {
        for (int index = 0; index < AccessoryCatalog.Length; index++)
        {
            if (AccessoryCatalog[index].id == accessoryId)
            {
                return AccessoryCatalog[index];
            }
        }

        return null;
    }

    private int GetAccessoryCatalogIndex(string accessoryId)
    {
        for (int index = 0; index < AccessoryCatalog.Length; index++)
        {
            if (AccessoryCatalog[index].id == accessoryId)
            {
                return index;
            }
        }

        return -1;
    }

    private int GetAccessoryXpBonus(string accessoryId)
    {
        int accessoryIndex = GetAccessoryCatalogIndex(accessoryId);
        if (accessoryIndex < 0)
        {
            return 40;
        }

        // Accessories deeper in the catalog grant larger progression bonuses.
        // Index 0 starts at 40 XP so equip/unequip has an immediately visible effect.
        return 40 + (accessoryIndex * 20);
    }

    private void CreateAccessoryPreview(Transform parent, string accessoryId, bool isUnlocked)
    {
        GameObject previewObject = new GameObject(accessoryId + "Preview", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(LayoutElement));
        previewObject.transform.SetParent(parent, false);

        Image previewImage = previewObject.GetComponent<Image>();
        previewImage.sprite = GetAccessoryPreviewSprite(accessoryId);
        previewImage.preserveAspect = true;
        previewImage.raycastTarget = false;
        previewImage.color = isUnlocked ? Color.white : new Color(1f, 1f, 1f, 0.38f);

        string category = GetAccessoryCategory(accessoryId);
        float previewHeight = compactDrawerMode ? 74f : 88f;
        if (category == "Shirts" || category == "Handheld")
        {
            previewHeight = compactDrawerMode ? 108f : 132f;
        }

        LayoutElement layoutElement = previewObject.GetComponent<LayoutElement>();
        layoutElement.preferredHeight = previewHeight;
        layoutElement.minHeight = layoutElement.preferredHeight;
        layoutElement.flexibleWidth = 1f;
    }

    private Sprite GetAccessoryPreviewSprite(string accessoryId)
    {
        string pocketPalId = SaveManager.Data != null ? SaveManager.Data.selectedPocketPalId : "panda";
        string cacheKey = pocketPalId + ":" + accessoryId;
        if (previewSpriteCache.ContainsKey(cacheKey))
        {
            return previewSpriteCache[cacheKey];
        }

        Sprite sprite = TryCreateSpriteFromResource(GetAccessoryResourcePath(accessoryId, pocketPalId));
        if (sprite == null && pocketPalId != "panda")
        {
            sprite = TryCreateSpriteFromResource(GetAccessoryResourcePath(accessoryId, "panda"));
        }

        if (sprite == null)
        {
            Texture2D fallback = CreateTransparentTexture(96, 96);
            fallback.Apply();
            sprite = Sprite.Create(fallback, new Rect(0f, 0f, 96f, 96f), new Vector2(0.5f, 0.5f), 100f);
        }

        previewSpriteCache[cacheKey] = sprite;
        return sprite;
    }

    private string GetAccessoryResourcePath(string accessoryId, string pocketPalId)
    {
        if (!AccessorySpriteResourceNames.ContainsKey(accessoryId))
        {
            return string.Empty;
        }

        string spriteName = AccessorySpriteResourceNames[accessoryId];
        // White tiger currently shares panda accessory sprites.
        if (pocketPalId == "white_tiger")
        {
            return "Sprites/Accessories/" + spriteName;
        }

        if (pocketPalId == "otter")
        {
            return "Sprites/Accessories/" + spriteName;
        }

        return "Sprites/Accessories/" + spriteName;
    }

    private Sprite TryCreateSpriteFromResource(string resourcePath)
    {
        if (string.IsNullOrEmpty(resourcePath))
        {
            return null;
        }

        Texture2D texture = Resources.Load<Texture2D>(resourcePath);
        if (texture == null)
        {
            return null;
        }

        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Bilinear;
        return Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
    }

    private string GetAccessoryCategory(string accessoryId)
    {
        AccessoryDefinition definition = GetAccessoryDefinition(accessoryId);
        return definition != null ? definition.category : "";
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
            string category = GetAccessoryCategory(accessoryId);
            RemoveEquippedAccessoriesInCategory(category);
            SaveManager.Data.equippedAccessoryIds.Add(accessoryId);
        }

        RecalculatePocketPalLevelFromEquippedAccessories();
        SaveManager.Data.equippedAccessoryId = SaveManager.Data.equippedAccessoryIds.Count > 0 ? SaveManager.Data.equippedAccessoryIds[0] : "";
        SaveManager.Save();
        RefreshAfterSave(CustomizationSceneName);
    }

    private void RecalculatePocketPalLevelFromEquippedAccessories()
    {
        int baselineLevel = SaveManager.GetAccountLevel();
        int computedLevel = Mathf.Clamp(baselineLevel, 1, LevelingSystem.MaxLevel);
        int computedXp = 0;

        if (SaveManager.Data.equippedAccessoryIds != null)
        {
            for (int index = 0; index < SaveManager.Data.equippedAccessoryIds.Count; index++)
            {
                string equippedAccessoryId = SaveManager.Data.equippedAccessoryIds[index];
                computedXp += Mathf.Max(0, GetAccessoryXpBonus(equippedAccessoryId));
            }
        }

        while (computedLevel < LevelingSystem.MaxLevel && computedXp >= LevelingSystem.GetXpForNextLevel(computedLevel))
        {
            computedXp -= LevelingSystem.GetXpForNextLevel(computedLevel);
            computedLevel++;
        }

        if (computedLevel >= LevelingSystem.MaxLevel)
        {
            computedLevel = LevelingSystem.MaxLevel;
            computedXp = 0;
        }

        SaveManager.Data.pocketPalLevel = computedLevel;
        SaveManager.Data.pocketPalXp = Mathf.Max(0, computedXp);
    }

    private bool IsAccessoryEquipped(string accessoryId)
    {
        return SaveManager.IsAccessoryEquippedForSelectedPocketPal(accessoryId);
    }

    private bool IsAccessoryOwned(string accessoryId)
    {
        return SaveManager.IsAccessoryOwnedForSelectedPocketPal(accessoryId);
    }

    private void RemoveEquippedAccessoriesInCategory(string category)
    {
        if (string.IsNullOrEmpty(category) || SaveManager.Data.equippedAccessoryIds == null)
        {
            return;
        }

        for (int index = SaveManager.Data.equippedAccessoryIds.Count - 1; index >= 0; index--)
        {
            string equippedAccessoryId = SaveManager.Data.equippedAccessoryIds[index];
            if (GetAccessoryCategory(equippedAccessoryId) == category)
            {
                SaveManager.Data.equippedAccessoryIds.RemoveAt(index);
            }
        }
    }

    public void BuildTasksRewardsContent(RectTransform mainContent)
    {
        ClearChildren(mainContent);
        SaveManager.EnsureDailyTaskClaimsForToday();

        if (!compactDrawerMode)
        {
            CreateTitle(mainContent, "Tasks & Rewards");
        }

        const int compactTaskCardCount = 9;
        const float compactTaskCardHeight = 208f;
        const float compactTasksPanelSectionPadding = 40f; // top + bottom after compact padding adjustments
        const float compactTasksPanelSpacing = 18f;
        float tasksPanelHeight = compactDrawerMode
            ? compactTasksPanelSectionPadding +
              (compactTaskCardCount * compactTaskCardHeight) +
              ((compactTaskCardCount - 1) * compactTasksPanelSpacing) +
              16f
            : 860f;
        GameObject tasksPanel = CreatePanel("TasksPanel", mainContent, new Color(0.86f, 0.95f, 1f, 0.84f), tasksPanelHeight);
        ConfigurePanelLayout(tasksPanel, 28, 28, 34, 34, compactDrawerMode ? 24f : 22f);

        if (!compactDrawerMode)
        {
            TMP_Text tasksTitle = CreateText("TasksTitle", tasksPanel.transform, "Tasks", 34, FontStyles.Bold, TextAlignmentOptions.Center);
            tasksTitle.color = new Color(0.08f, 0.18f, 0.22f);

            TMP_Text tasksSubtitle = CreateText("TasksSubtitle", tasksPanel.transform, "Complete care actions, then claim coins here.", 24, FontStyles.Normal, TextAlignmentOptions.Center);
            tasksSubtitle.color = new Color(0.16f, 0.28f, 0.32f);
        }

        CreateTaskCard(tasksPanel.transform, FeedTaskId, "Feed your panda", "Feed your panda from the PocketPal screen.", 10, 14, SaveManager.Data.snacksFed > 0);
        CreateTaskCard(tasksPanel.transform, CheckInTaskId, "Check in today", "Open the game and visit your sanctuary.", 5, 10, true);
        CreateTaskCard(tasksPanel.transform, AccessorizeTaskId, "Accessorize your panda", "Equip any style item from customization.", 15, 18, SaveManager.Data.equippedAccessoryIds != null && SaveManager.Data.equippedAccessoryIds.Count > 0);
        int petCount = SaveManager.GetSelectedPocketPalPetCount();
        int clampedPetCount = Mathf.Clamp(petCount, 0, PetPalTaskTargetCount);
        CreateTaskCard(
            tasksPanel.transform,
            PetPalTaskId,
            "Pet your pal",
            "Press and hold on your pal to pet it. Progress: " + clampedPetCount + " / " + PetPalTaskTargetCount + ".",
            PetPalTaskCoinReward,
            PetPalTaskAccountXpReward,
            petCount >= PetPalTaskTargetCount);
        CreateTaskCard(tasksPanel.transform, SleepTaskId, "Sleep 8 hours", "Let your pocketpal sleep through one full night cycle.", 20, 24, SaveManager.GetSelectedPocketPalCompletedSleepSessions() > 0);
        int habitatCleanCount = SaveManager.GetHabitatCleanCount();
        int clampedHabitatClean7Count = Mathf.Clamp(habitatCleanCount, 0, CleanHabitatTask7TargetCount);
        int clampedHabitatClean15Count = Mathf.Clamp(habitatCleanCount, 0, CleanHabitatTask15TargetCount);
        CreateTaskCard(
            tasksPanel.transform,
            CleanHabitatTask7Id,
            "Clean habitat x7",
            "Pull random weeds from the sanctuary. Progress: " + clampedHabitatClean7Count + " / " + CleanHabitatTask7TargetCount + ".",
            CleanHabitatTask7CoinReward,
            CleanHabitatTask7AccountXpReward,
            habitatCleanCount >= CleanHabitatTask7TargetCount);
        CreateTaskCard(
            tasksPanel.transform,
            CleanHabitatTask15Id,
            "Clean habitat x15",
            "Clear even more weeds for a bigger care bonus. Progress: " + clampedHabitatClean15Count + " / " + CleanHabitatTask15TargetCount + ".",
            CleanHabitatTask15CoinReward,
            CleanHabitatTask15AccountXpReward,
            habitatCleanCount >= CleanHabitatTask15TargetCount);
        CreateTaskCard(
            tasksPanel.transform,
            UnlockWhiteTigerTaskId,
            "Unlock White Tiger",
            "Reach level requirements and unlock White Tiger.",
            UnlockWhiteTigerTaskCoinReward,
            UnlockWhiteTigerTaskAccountXpReward,
            IsPocketPalOwned("white_tiger"));
        CreateTaskCard(
            tasksPanel.transform,
            UnlockOtterTaskId,
            "Unlock Otter",
            "Reach level requirements and unlock Otter.",
            UnlockOtterTaskCoinReward,
            UnlockOtterTaskAccountXpReward,
            IsPocketPalOwned("otter"));

        float rewardsPanelHeight = compactDrawerMode ? 860f : 680f;
        GameObject rewardsPanel = CreatePanel("RewardsPanel", mainContent, new Color(0.83f, 0.93f, 0.78f, 0.88f), rewardsPanelHeight);
        ConfigurePanelLayout(rewardsPanel, 28, 28, 30, 30, compactDrawerMode ? 22f : 18f);

        if (!compactDrawerMode)
        {
            TMP_Text rewardsTitle = CreateText("RewardsTitle", rewardsPanel.transform, "Rewards", 34, FontStyles.Bold, TextAlignmentOptions.Center);
            rewardsTitle.color = new Color(0.08f, 0.18f, 0.22f);
        }

        string rewardsProgressText = compactDrawerMode ? "Progress: " + SaveManager.Data.rewardProgress + " / 100" : "Reward progress: " + SaveManager.Data.rewardProgress + " / 100\nRedeem unlocked milestones for bonus coins.";
        TMP_Text rewardsBody = CreateText("RewardsBody", rewardsPanel.transform, rewardsProgressText, 24, FontStyles.Normal, TextAlignmentOptions.Left);
        rewardsBody.color = new Color(0.12f, 0.24f, 0.28f);
        CreateProgressBar("RewardProgressBar", rewardsPanel.transform, SaveManager.Data.rewardProgress / 100f, new Color(0.76f, 0.64f, 1f, 1f));

        CreateRewardCard(rewardsPanel.transform, StarterRewardId, "Starter coin grant", "A small boost for opening your sanctuary.", 0, 50);
        CreateRewardCard(rewardsPanel.transform, CareRewardId, "Care progress bonus", "A bonus for completing care tasks.", 25, 100);
        CreateRewardCard(rewardsPanel.transform, PettingRewardId, "Petting progress bonus", "A larger bonus for caring for your pal.", 50, 250);

        float levelPanelHeight = compactDrawerMode ? 980f : 620f;
        GameObject levelPanel = CreatePanel("LevelRewardsPanel", mainContent, new Color(0.9f, 0.86f, 1f, 0.88f), levelPanelHeight);
        ConfigurePanelLayout(levelPanel, 28, 28, 30, 30, compactDrawerMode ? 22f : 18f);

        if (!compactDrawerMode)
        {
            TMP_Text levelTitle = CreateText("LevelRewardsTitle", levelPanel.transform, "Level Rewards", 34, FontStyles.Bold, TextAlignmentOptions.Center);
            levelTitle.color = new Color(0.08f, 0.18f, 0.22f);
        }

        string levelProgressText = compactDrawerMode ? CreateCompactLevelProgressText() : CreateLevelProgressText();
        TMP_Text levelBody = CreateText("LevelRewardsBody", levelPanel.transform, levelProgressText, 24, FontStyles.Normal, TextAlignmentOptions.Left);
        if (compactDrawerMode)
        {
            levelBody.fontSize = 20f;
            levelBody.overflowMode = TextOverflowModes.Ellipsis;
            levelBody.lineSpacing = 4f;
        }
        levelBody.color = new Color(0.12f, 0.24f, 0.28f);
        int selectedPalLevel = SaveManager.GetSelectedPocketPalLevel();
        int selectedPalXp = SaveManager.GetSelectedPocketPalXp();
        float palProgress = selectedPalLevel >= LevelingSystem.MaxLevel ? 1f : selectedPalXp / (float)LevelingSystem.GetXpForNextLevel(selectedPalLevel);
        CreateProgressBar("PocketPalLevelProgressBar", levelPanel.transform, palProgress, new Color(0.52f, 0.8f, 1f, 1f));

        CreateLevelRewardCard(levelPanel.transform, "pal_level_2_treat", "PocketPal Treat Bundle", "Reach PocketPal level 2 to earn bonus coins.", 2, 1, 75, "", "");
        CreateLevelRewardCard(levelPanel.transform, "habitat_level_2_bamboo", "Bamboo Habitat Kit", "Reach habitat level 2 to unlock a bamboo habitat reward.", 1, 2, 0, "bamboo_grove", "");
        CreateLevelRewardCard(levelPanel.transform, "pal_level_3_hat", "Level 3 Hat Reward", "Reach PocketPal level 3 to unlock the red cap.", 3, 1, 0, "", HeadwearRedCapAccessoryId);
    }

    private void CreateTaskCard(Transform parent, string taskId, string title, string description, int coinReward, int accountXpReward, bool isReady)
    {
        bool isComplete = IsTaskComplete(taskId);
        Color cardColor = isComplete ? new Color(0.79f, 0.88f, 0.8f, 0.92f) : new Color(0.9f, 0.97f, 1f, 0.98f);

        if (!isComplete && !isReady)
        {
            cardColor = new Color(0.91f, 0.93f, 0.92f, 0.9f);
        }

        float cardHeight = compactDrawerMode ? 208f : 128f;
        GameObject card = CreatePanel(taskId + "TaskCard", parent, cardColor, cardHeight);

        if (!isComplete && isReady)
        {
            Button button = card.AddComponent<Button>();
            Image image = card.GetComponent<Image>();
            image.raycastTarget = true;
            button.targetGraphic = image;
            button.onClick.AddListener(delegate { ClaimTaskReward(taskId, coinReward, accountXpReward); });
        }

        ConfigurePanelLayout(card, 24, 24, compactDrawerMode ? 22 : 18, compactDrawerMode ? 22 : 18, compactDrawerMode ? 12f : 8f);

        string status = GetTaskStatusText(isComplete, isReady, coinReward, accountXpReward);
        TMP_Text label = CreateText(taskId + "TaskLabel", card.transform, title + "\n" + description + "\n" + status, 25, FontStyles.Normal, TextAlignmentOptions.Left);
        if (compactDrawerMode)
        {
            LayoutElement labelLayout = label.GetComponent<LayoutElement>();
            label.fontSize = 20f;
            label.overflowMode = TextOverflowModes.Ellipsis;
            label.lineSpacing = 4f;
            if (labelLayout != null)
            {
                labelLayout.preferredHeight = Mathf.Max(labelLayout.preferredHeight, 156f);
                labelLayout.minHeight = labelLayout.preferredHeight;
            }
        }
        label.color = new Color(0.08f, 0.18f, 0.22f);
    }

    private string GetTaskStatusText(bool isComplete, bool isReady, int coinReward, int accountXpReward)
    {
        if (isComplete)
        {
            return "Claimed";
        }

        if (!isReady)
        {
            return "Not ready";
        }

        return "+" + coinReward + " coins, +" + accountXpReward + " XP - tap to claim";
    }

    private void ClaimTaskReward(string taskId, int coinReward, int accountXpReward)
    {
        SaveManager.EnsureLoaded();
        if (!TryMarkTaskClaimed(taskId))
        {
            return;
        }

        SaveManager.Data.coins += coinReward;
        SaveManager.Data.rewardProgress += coinReward;
        LevelingSystem.GrantAccountXp(accountXpReward);
        if (!IsHabitatCleaningTask(taskId))
        {
            LevelingSystem.GrantPocketPalXp(coinReward * 2);
            LevelingSystem.GrantHabitatXp(coinReward);
        }
        SaveManager.Save();
        RefreshAfterSave(TasksRewardsSceneName);
    }

    private static bool IsHabitatCleaningTask(string taskId)
    {
        return taskId == CleanHabitatTask7Id || taskId == CleanHabitatTask15Id;
    }

    private static bool IsOneTimeTask(string taskId)
    {
        return taskId == UnlockWhiteTigerTaskId || taskId == UnlockOtterTaskId;
    }

    private bool TryMarkTaskClaimed(string taskId)
    {
        if (!IsOneTimeTask(taskId))
        {
            return SaveManager.TryMarkDailyTaskClaimed(taskId);
        }

        if (SaveManager.Data.completedTaskIds == null)
        {
            SaveManager.Data.completedTaskIds = new List<string>();
        }

        if (SaveManager.Data.completedTaskIds.Contains(taskId))
        {
            return false;
        }

        SaveManager.Data.completedTaskIds.Add(taskId);
        return true;
    }

    private bool IsTaskComplete(string taskId)
    {
        if (IsOneTimeTask(taskId))
        {
            return SaveManager.Data.completedTaskIds != null && SaveManager.Data.completedTaskIds.Contains(taskId);
        }

        return SaveManager.IsDailyTaskClaimed(taskId);
    }

    private void CreateRewardCard(Transform parent, string rewardId, string title, string description, int requiredProgress, int coinReward)
    {
        bool isRedeemed = IsRewardRedeemed(rewardId);
        bool isUnlocked = SaveManager.Data.rewardProgress >= requiredProgress;
        Color cardColor = isRedeemed ? new Color(0.79f, 0.88f, 0.8f, 0.92f) : new Color(0.9f, 0.98f, 0.88f, 0.98f);

        if (!isRedeemed && !isUnlocked)
        {
            cardColor = new Color(0.91f, 0.93f, 0.92f, 0.9f);
        }

        float cardHeight = compactDrawerMode ? 208f : 128f;
        GameObject card = CreatePanel(rewardId + "RewardCard", parent, cardColor, cardHeight);

        if (!isRedeemed && isUnlocked)
        {
            Button button = card.AddComponent<Button>();
            Image image = card.GetComponent<Image>();
            image.raycastTarget = true;
            button.targetGraphic = image;
            button.onClick.AddListener(delegate { RedeemReward(rewardId, coinReward); });
        }

        ConfigurePanelLayout(card, 24, 24, compactDrawerMode ? 22 : 18, compactDrawerMode ? 22 : 18, compactDrawerMode ? 12f : 8f);

        string status = GetRewardStatusText(isRedeemed, isUnlocked, requiredProgress, coinReward);
        TMP_Text label = CreateText(rewardId + "RewardLabel", card.transform, title + "\n" + description + "\n" + status, 25, FontStyles.Normal, TextAlignmentOptions.Left);
        if (compactDrawerMode)
        {
            LayoutElement labelLayout = label.GetComponent<LayoutElement>();
            label.fontSize = 20f;
            label.overflowMode = TextOverflowModes.Ellipsis;
            label.lineSpacing = 4f;
            if (labelLayout != null)
            {
                labelLayout.preferredHeight = Mathf.Max(labelLayout.preferredHeight, 156f);
                labelLayout.minHeight = labelLayout.preferredHeight;
            }
        }
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
        LevelingSystem.GrantHabitatXp(Mathf.Max(10, coinReward / 10));
        SaveManager.Save();
        RefreshAfterSave(TasksRewardsSceneName);
    }

    private bool IsRewardRedeemed(string rewardId)
    {
        return SaveManager.Data.redeemedRewardIds != null && SaveManager.Data.redeemedRewardIds.Contains(rewardId);
    }

    public void BuildShopContent(RectTransform mainContent)
    {
        ClearChildren(mainContent);
        SaveManager.Data.hasVisitedShop = true;
        SaveManager.Save();

        if (compactDrawerMode)
        {
            BuildCompactCoinShopContent(mainContent);
            return;
        }

        if (!compactDrawerMode)
        {
            CreateTitle(mainContent, "Shop");
        }

        CreateCategorySection(mainContent, "Buy Coins", "Coin pack placeholders for future purchases.", 520f, new string[]
        {
            "Small pouch - 100 coins",
            "Garden jar - 500 coins",
            "Sanctuary chest - 1200 coins"
        });

        CreateSection(mainContent, "Accessory Unlocks", "Locked accessories are purchased from the Customize drawer.", 360f, new string[]
        {
            "Open Customize",
            "Tap a locked accessory",
            "Confirm purchase and equip it from there"
        });
    }

    private void BuildCompactCoinShopContent(RectTransform mainContent)
    {
        GameObject section = CreatePanel("CompactCoinShopSection", mainContent, new Color(0.93f, 0.97f, 0.96f, 0.82f), 186f);
        LayoutElement sectionLayout = section.GetComponent<LayoutElement>();
        if (sectionLayout != null)
        {
            sectionLayout.flexibleHeight = 1f;
        }

        GameObject rowObject = new GameObject("CompactCoinShopRow", typeof(RectTransform), typeof(HorizontalLayoutGroup));
        rowObject.transform.SetParent(section.transform, false);
        RectTransform rowRect = rowObject.GetComponent<RectTransform>();
        rowRect.anchorMin = new Vector2(0f, 1f);
        rowRect.anchorMax = new Vector2(1f, 1f);
        rowRect.pivot = new Vector2(0.5f, 1f);
        rowRect.anchoredPosition = Vector2.zero;
        rowRect.offsetMin = new Vector2(8f, -172f);
        rowRect.offsetMax = new Vector2(-8f, -8f);

        HorizontalLayoutGroup rowLayout = rowObject.GetComponent<HorizontalLayoutGroup>();
        rowLayout.spacing = 8f;
        rowLayout.childAlignment = TextAnchor.UpperCenter;
        rowLayout.childControlWidth = true;
        rowLayout.childControlHeight = true;
        rowLayout.childForceExpandWidth = true;
        rowLayout.childForceExpandHeight = false;

        for (int index = 0; index < CoinPackCatalog.Length; index++)
        {
            CreateCompactCoinPackCard(rowObject.transform, CoinPackCatalog[index]);
        }
    }

    private void CreateCompactCoinPackCard(Transform parent, CoinPackDefinition pack)
    {
        string sanitizedPackName = pack.name.Replace(" ", "");
        GameObject card = CreatePanel(sanitizedPackName + "CompactShopCard", parent, new Color(0.95f, 0.98f, 1f, 0.96f), 156f);
        LayoutElement cardLayout = card.GetComponent<LayoutElement>();
        if (cardLayout != null)
        {
            cardLayout.preferredWidth = 0f;
            cardLayout.flexibleWidth = 1f;
            cardLayout.flexibleHeight = 1f;
        }

        Button button = card.AddComponent<Button>();
        Image cardImage = card.GetComponent<Image>();
        cardImage.raycastTarget = true;
        button.targetGraphic = cardImage;
        button.onClick.AddListener(delegate { ShowCoinPurchaseDialog(pack); });

        HorizontalLayoutGroup layout = card.AddComponent<HorizontalLayoutGroup>();
        layout.padding = new RectOffset(10, 10, 10, 10);
        layout.spacing = 8f;
        layout.childAlignment = TextAnchor.MiddleLeft;
        layout.childControlWidth = false;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        GameObject placeholder = new GameObject(sanitizedPackName + "Placeholder", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(LayoutElement));
        placeholder.transform.SetParent(card.transform, false);
        Image placeholderImage = placeholder.GetComponent<Image>();
        Sprite packSprite = GetCoinPackSprite(pack.id);
        placeholderImage.sprite = packSprite;
        placeholderImage.preserveAspect = true;
        placeholderImage.color = packSprite != null ? Color.white : pack.placeholderColor;
        placeholderImage.raycastTarget = false;
        LayoutElement placeholderLayout = placeholder.GetComponent<LayoutElement>();
        placeholderLayout.preferredWidth = CompactShopIconSize;
        placeholderLayout.minWidth = CompactShopIconSize;
        placeholderLayout.preferredHeight = CompactShopIconSize;
        placeholderLayout.minHeight = CompactShopIconSize;

        if (packSprite == null)
        {
            GameObject placeholderLabelObject = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            placeholderLabelObject.transform.SetParent(placeholder.transform, false);
            RectTransform placeholderLabelRect = placeholderLabelObject.GetComponent<RectTransform>();
            placeholderLabelRect.anchorMin = Vector2.zero;
            placeholderLabelRect.anchorMax = Vector2.one;
            placeholderLabelRect.offsetMin = Vector2.zero;
            placeholderLabelRect.offsetMax = Vector2.zero;
            TextMeshProUGUI placeholderLabel = placeholderLabelObject.GetComponent<TextMeshProUGUI>();
            placeholderLabel.text = pack.placeholderShortLabel;
            placeholderLabel.alignment = TextAlignmentOptions.Center;
            placeholderLabel.fontSize = 14f;
            placeholderLabel.fontStyle = FontStyles.Bold;
            placeholderLabel.color = new Color(0.1f, 0.18f, 0.22f);
            placeholderLabel.raycastTarget = false;
        }

        GameObject textGroup = new GameObject(sanitizedPackName + "TextGroup", typeof(RectTransform), typeof(LayoutElement));
        textGroup.transform.SetParent(card.transform, false);
        LayoutElement textGroupLayout = textGroup.GetComponent<LayoutElement>();
        textGroupLayout.preferredWidth = 120f;
        textGroupLayout.flexibleWidth = 1f;

        GameObject cardTextObject = new GameObject(sanitizedPackName + "PackText", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        cardTextObject.transform.SetParent(textGroup.transform, false);
        RectTransform cardTextRect = cardTextObject.GetComponent<RectTransform>();
        cardTextRect.anchorMin = Vector2.zero;
        cardTextRect.anchorMax = Vector2.one;
        cardTextRect.offsetMin = Vector2.zero;
        cardTextRect.offsetMax = Vector2.zero;

        TextMeshProUGUI cardText = cardTextObject.GetComponent<TextMeshProUGUI>();
        cardText.text = pack.name + "\n" + pack.priceLabel;
        cardText.alignment = TextAlignmentOptions.MidlineLeft;
        cardText.fontSize = 17f;
        cardText.fontStyle = FontStyles.Bold;
        cardText.color = new Color(0.08f, 0.18f, 0.22f);
        cardText.raycastTarget = false;
    }

    private Sprite GetCoinPackSprite(string packId)
    {
        if (string.IsNullOrEmpty(packId))
        {
            return null;
        }

        if (coinPackSpriteCache.ContainsKey(packId))
        {
            return coinPackSpriteCache[packId];
        }

        Texture2D spriteSheet = LoadShopAssetsTexture();
        if (spriteSheet == null)
        {
            coinPackSpriteCache[packId] = null;
            return null;
        }

        int packIndex = GetCoinPackIndex(packId);
        if (packIndex < 0)
        {
            coinPackSpriteCache[packId] = null;
            return null;
        }

        int segments = Mathf.Max(1, CoinPackCatalog.Length);
        int segmentWidth = Mathf.Max(1, spriteSheet.width / segments);
        int insetX = Mathf.RoundToInt(segmentWidth * 0.02f);
        int insetY = Mathf.RoundToInt(spriteSheet.height * 0.04f);
        int x = Mathf.Clamp((packIndex * segmentWidth) + insetX, 0, spriteSheet.width - 1);
        int y = Mathf.Clamp(insetY, 0, spriteSheet.height - 1);
        int width = Mathf.Clamp(segmentWidth - (insetX * 2), 1, spriteSheet.width - x);
        int height = Mathf.Clamp(spriteSheet.height - (insetY * 2), 1, spriteSheet.height - y);

        Rect spriteRect = new Rect(x, y, width, height);
        if (spriteSheet.isReadable)
        {
            try
            {
                Color[] segmentPixels = spriteSheet.GetPixels(x, y, width, height);
                int minX = width;
                int minY = height;
                int maxX = -1;
                int maxY = -1;

                for (int pixelY = 0; pixelY < height; pixelY++)
                {
                    int rowOffset = pixelY * width;
                    for (int pixelX = 0; pixelX < width; pixelX++)
                    {
                        if (segmentPixels[rowOffset + pixelX].a > 0.02f)
                        {
                            if (pixelX < minX)
                            {
                                minX = pixelX;
                            }

                            if (pixelY < minY)
                            {
                                minY = pixelY;
                            }

                            if (pixelX > maxX)
                            {
                                maxX = pixelX;
                            }

                            if (pixelY > maxY)
                            {
                                maxY = pixelY;
                            }
                        }
                    }
                }

                if (maxX >= minX && maxY >= minY)
                {
                    const int trimPadding = 2;
                    int trimmedX = Mathf.Max(x + minX - trimPadding, 0);
                    int trimmedY = Mathf.Max(y + minY - trimPadding, 0);
                    int trimmedMaxX = Mathf.Min(x + maxX + trimPadding, spriteSheet.width - 1);
                    int trimmedMaxY = Mathf.Min(y + maxY + trimPadding, spriteSheet.height - 1);
                    spriteRect = new Rect(
                        trimmedX,
                        trimmedY,
                        Mathf.Max(1, (trimmedMaxX - trimmedX) + 1),
                        Mathf.Max(1, (trimmedMaxY - trimmedY) + 1)
                    );
                }
            }
            catch (UnityException)
            {
                spriteRect = new Rect(x, y, width, height);
            }
        }

        Sprite sprite = Sprite.Create(spriteSheet, spriteRect, new Vector2(0.5f, 0.5f), 100f);
        coinPackSpriteCache[packId] = sprite;
        return sprite;
    }

    private Texture2D LoadShopAssetsTexture()
    {
        Texture2D loadedTexture = Resources.Load<Texture2D>(ShopAssetsSheetResourcePath);
        if (loadedTexture != null)
        {
            return loadedTexture;
        }

        Sprite loadedSprite = Resources.Load<Sprite>(ShopAssetsSheetResourcePath);
        if (loadedSprite != null)
        {
            return loadedSprite.texture;
        }

        return null;
    }

    private int GetCoinPackIndex(string packId)
    {
        for (int index = 0; index < CoinPackCatalog.Length; index++)
        {
            if (CoinPackCatalog[index].id == packId)
            {
                return index;
            }
        }

        return -1;
    }

    private void ShowCoinPurchaseDialog(CoinPackDefinition pack)
    {
        if (pack == null)
        {
            return;
        }

        SaveManager.EnsureLoaded();
        CloseCoinPurchaseDialog();

        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            return;
        }

        coinPurchaseDialogRoot = new GameObject("CoinPurchaseDialog", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        coinPurchaseDialogRoot.transform.SetParent(canvas.transform, false);
        coinPurchaseDialogRoot.transform.SetAsLastSibling();

        RectTransform rootRect = coinPurchaseDialogRoot.GetComponent<RectTransform>();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;

        Image rootImage = coinPurchaseDialogRoot.GetComponent<Image>();
        rootImage.color = new Color(0f, 0f, 0f, 0.42f);
        rootImage.raycastTarget = true;

        Button closeOverlayButton = coinPurchaseDialogRoot.GetComponent<Button>();
        closeOverlayButton.targetGraphic = rootImage;
        closeOverlayButton.onClick.AddListener(TryCloseCoinPurchaseDialog);

        GameObject dialogPanel = CreatePanel("CoinPurchaseDialogPanel", coinPurchaseDialogRoot.transform, new Color(0.96f, 0.97f, 1f, 0.99f), 300f);
        RectTransform dialogRect = dialogPanel.GetComponent<RectTransform>();
        dialogRect.anchorMin = new Vector2(0.5f, 0.5f);
        dialogRect.anchorMax = new Vector2(0.5f, 0.5f);
        dialogRect.pivot = new Vector2(0.5f, 0.5f);
        dialogRect.sizeDelta = new Vector2(540f, 300f);
        dialogRect.anchoredPosition = Vector2.zero;

        ConfigurePanelLayout(dialogPanel, 24, 24, 20, 20, 10f);
        CreateText("CoinPurchaseDialogTitle", dialogPanel.transform, "Confirm Purchase", 30f, FontStyles.Bold, TextAlignmentOptions.Center).color = new Color(0.08f, 0.18f, 0.22f);
        CreateText(
            "CoinPurchaseDialogBody",
            dialogPanel.transform,
            pack.name + "  " + pack.priceLabel + "\nYou'll receive +" + pack.coinReward + " coins.",
            21f,
            FontStyles.Normal,
            TextAlignmentOptions.Center).color = new Color(0.12f, 0.24f, 0.3f);

        coinPurchaseDialogStatusText = CreateText("CoinPurchaseDialogStatus", dialogPanel.transform, string.Empty, 20f, FontStyles.Bold, TextAlignmentOptions.Center);
        coinPurchaseDialogStatusText.color = new Color(0.1f, 0.48f, 0.25f, 1f);

        GameObject buttonsRow = new GameObject("CoinPurchaseDialogButtons", typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(LayoutElement));
        buttonsRow.transform.SetParent(dialogPanel.transform, false);
        LayoutElement buttonsLayout = buttonsRow.GetComponent<LayoutElement>();
        buttonsLayout.preferredHeight = 82f;

        HorizontalLayoutGroup buttonsGroup = buttonsRow.GetComponent<HorizontalLayoutGroup>();
        buttonsGroup.spacing = 14f;
        buttonsGroup.childAlignment = TextAnchor.MiddleCenter;
        buttonsGroup.childControlWidth = true;
        buttonsGroup.childControlHeight = true;
        buttonsGroup.childForceExpandWidth = true;
        buttonsGroup.childForceExpandHeight = false;

        CreateDialogButton(buttonsRow.transform, "No", new Color(0.72f, 0.78f, 0.88f, 1f), TryCloseCoinPurchaseDialog, true);
        CreateDialogButton(
            buttonsRow.transform,
            "Yes",
            new Color(0.56f, 0.86f, 0.64f, 1f),
            delegate { BeginCoinPurchase(pack); },
            true);
    }

    private void BeginCoinPurchase(CoinPackDefinition pack)
    {
        if (pack == null || coinPurchaseProcessing)
        {
            return;
        }

        coinPurchaseProcessing = true;

        if (coinPurchaseDialogRoot != null)
        {
            Button[] dialogButtons = coinPurchaseDialogRoot.GetComponentsInChildren<Button>(true);
            for (int index = 0; index < dialogButtons.Length; index++)
            {
                if (dialogButtons[index] != null)
                {
                    dialogButtons[index].interactable = false;
                }
            }
        }

        if (coinPurchaseDialogStatusText != null)
        {
            coinPurchaseDialogStatusText.color = new Color(0.16f, 0.3f, 0.36f, 1f);
            coinPurchaseDialogStatusText.text = "Processing...";
        }

        StartCoroutine(ProcessCoinPurchaseRoutine(pack));
    }

    private IEnumerator ProcessCoinPurchaseRoutine(CoinPackDefinition pack)
    {
        yield return new WaitForSeconds(1.1f);

        SaveManager.EnsureLoaded();
        SaveManager.Data.coins += pack.coinReward;
        SaveManager.Save();

        if (coinPurchaseDialogStatusText != null)
        {
            coinPurchaseDialogStatusText.color = new Color(0.1f, 0.48f, 0.25f, 1f);
            coinPurchaseDialogStatusText.text = "Confirmed! +" + pack.coinReward + " coins";
        }

        yield return new WaitForSeconds(1.0f);

        coinPurchaseProcessing = false;
        CloseCoinPurchaseDialog();
        RefreshAfterSave(ShopSceneName);
    }

    private void TryCloseCoinPurchaseDialog()
    {
        if (coinPurchaseProcessing)
        {
            return;
        }

        CloseCoinPurchaseDialog();
    }

    private void CloseCoinPurchaseDialog()
    {
        if (coinPurchaseDialogRoot != null)
        {
            Destroy(coinPurchaseDialogRoot);
        }

        coinPurchaseDialogRoot = null;
        coinPurchaseDialogStatusText = null;
        coinPurchaseProcessing = false;
    }

    private void ShowHabitatPurchaseDialog(HabitatDefinition habitat)
    {
        if (habitat == null)
        {
            return;
        }

        SaveManager.EnsureLoaded();

        bool levelReady = MeetsHabitatUnlockLevelRequirements(habitat);
        bool canAfford = CanAffordHabitatUnlock(habitat);
        bool canUnlock = levelReady && canAfford;
        string bodyText;
        if (canUnlock)
        {
            bodyText = "Unlock " + habitat.name + " for " + habitat.coinCost + " coins?";
        }
        else if (!levelReady)
        {
            bodyText = habitat.name + " needs User Lv " + habitat.requiredAccountLevel + " and Pal Lv " + habitat.requiredPocketPalLevel + ".";
        }
        else
        {
            bodyText = habitat.name + " costs " + habitat.coinCost + " coins.\nYou currently have " + SaveManager.Data.coins + " coins.";
        }

        CloseHabitatPurchaseDialog();

        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            return;
        }

        habitatPurchaseDialogRoot = new GameObject("HabitatPurchaseDialog", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        habitatPurchaseDialogRoot.transform.SetParent(canvas.transform, false);
        habitatPurchaseDialogRoot.transform.SetAsLastSibling();

        RectTransform rootRect = habitatPurchaseDialogRoot.GetComponent<RectTransform>();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;

        Image rootImage = habitatPurchaseDialogRoot.GetComponent<Image>();
        rootImage.color = new Color(0f, 0f, 0f, 0.4f);
        rootImage.raycastTarget = true;

        Button closeOverlayButton = habitatPurchaseDialogRoot.GetComponent<Button>();
        closeOverlayButton.targetGraphic = rootImage;
        closeOverlayButton.onClick.AddListener(CloseHabitatPurchaseDialog);

        GameObject dialogPanel = CreatePanel("HabitatPurchaseDialogPanel", habitatPurchaseDialogRoot.transform, new Color(0.96f, 0.97f, 1f, 0.99f), 320f);
        RectTransform dialogRect = dialogPanel.GetComponent<RectTransform>();
        dialogRect.anchorMin = new Vector2(0.5f, 0.5f);
        dialogRect.anchorMax = new Vector2(0.5f, 0.5f);
        dialogRect.pivot = new Vector2(0.5f, 0.5f);
        dialogRect.sizeDelta = new Vector2(560f, 320f);
        dialogRect.anchoredPosition = Vector2.zero;

        ConfigurePanelLayout(dialogPanel, 24, 24, 20, 20, 12f);
        CreateText("HabitatPurchaseDialogTitle", dialogPanel.transform, "Unlock Habitat", 30f, FontStyles.Bold, TextAlignmentOptions.Center).color = new Color(0.08f, 0.18f, 0.22f);
        CreateText("HabitatPurchaseDialogBody", dialogPanel.transform, bodyText, 22f, FontStyles.Normal, TextAlignmentOptions.Center).color = new Color(0.12f, 0.24f, 0.3f);

        GameObject buttonsRow = new GameObject("HabitatPurchaseDialogButtons", typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(LayoutElement));
        buttonsRow.transform.SetParent(dialogPanel.transform, false);
        LayoutElement buttonsLayout = buttonsRow.GetComponent<LayoutElement>();
        buttonsLayout.preferredHeight = 88f;

        HorizontalLayoutGroup buttonsGroup = buttonsRow.GetComponent<HorizontalLayoutGroup>();
        buttonsGroup.spacing = 14f;
        buttonsGroup.childAlignment = TextAnchor.MiddleCenter;
        buttonsGroup.childControlWidth = true;
        buttonsGroup.childControlHeight = true;
        buttonsGroup.childForceExpandWidth = true;
        buttonsGroup.childForceExpandHeight = false;

        CreateDialogButton(buttonsRow.transform, "No", new Color(0.72f, 0.78f, 0.88f, 1f), CloseHabitatPurchaseDialog, true);
        CreateDialogButton(
            buttonsRow.transform,
            canUnlock ? "Yes" : "Locked",
            canUnlock ? new Color(0.56f, 0.86f, 0.64f, 1f) : new Color(0.72f, 0.72f, 0.72f, 1f),
            delegate { TryPurchaseLockedHabitat(habitat.id); },
            canUnlock);
    }

    private void TryPurchaseLockedHabitat(string habitatId)
    {
        SaveManager.EnsureLoaded();

        HabitatDefinition habitat = GetHabitatDefinition(habitatId);
        if (habitat == null)
        {
            CloseHabitatPurchaseDialog();
            return;
        }

        if (IsHabitatOwned(habitatId))
        {
            SelectHabitat(habitatId);
            CloseHabitatPurchaseDialog();
            return;
        }

        if (!CanUnlockHabitatNow(habitat))
        {
            CloseHabitatPurchaseDialog();
            return;
        }

        SaveManager.Data.coins -= habitat.coinCost;
        if (SaveManager.Data.ownedHabitatIds == null)
        {
            SaveManager.Data.ownedHabitatIds = new List<string>();
        }

        if (!SaveManager.Data.ownedHabitatIds.Contains(habitatId))
        {
            SaveManager.Data.ownedHabitatIds.Add(habitatId);
        }

        SaveManager.Data.selectedHabitatId = habitatId;
        SaveManager.Save();
        CloseHabitatPurchaseDialog();
        RefreshAfterSave(CustomizationSceneName);
    }

    private void CloseHabitatPurchaseDialog()
    {
        if (habitatPurchaseDialogRoot == null)
        {
            return;
        }

        Destroy(habitatPurchaseDialogRoot);
        habitatPurchaseDialogRoot = null;
    }

    private void ShowAccessoryPurchaseDialog(string accessoryId)
    {
        AccessoryDefinition accessory = GetAccessoryDefinition(accessoryId);
        if (accessory == null)
        {
            return;
        }

        SaveManager.EnsureLoaded();
        bool canAfford = SaveManager.Data.coins >= accessory.price;
        string bodyText = canAfford
            ? "Buy " + accessory.name + " for " + accessory.price + " coins?"
            : accessory.name + " costs " + accessory.price + " coins.\nYou currently have " + SaveManager.Data.coins + " coins.";

        CloseAccessoryPurchaseDialog();

        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            return;
        }

        accessoryPurchaseDialogRoot = new GameObject("AccessoryPurchaseDialog", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        accessoryPurchaseDialogRoot.transform.SetParent(canvas.transform, false);
        accessoryPurchaseDialogRoot.transform.SetAsLastSibling();

        RectTransform rootRect = accessoryPurchaseDialogRoot.GetComponent<RectTransform>();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;

        Image rootImage = accessoryPurchaseDialogRoot.GetComponent<Image>();
        rootImage.color = new Color(0f, 0f, 0f, 0.4f);
        rootImage.raycastTarget = true;

        Button closeOverlayButton = accessoryPurchaseDialogRoot.GetComponent<Button>();
        closeOverlayButton.targetGraphic = rootImage;
        closeOverlayButton.onClick.AddListener(CloseAccessoryPurchaseDialog);

        GameObject dialogPanel = CreatePanel("AccessoryPurchaseDialogPanel", accessoryPurchaseDialogRoot.transform, new Color(0.96f, 0.97f, 1f, 0.99f), 300f);
        RectTransform dialogRect = dialogPanel.GetComponent<RectTransform>();
        dialogRect.anchorMin = new Vector2(0.5f, 0.5f);
        dialogRect.anchorMax = new Vector2(0.5f, 0.5f);
        dialogRect.pivot = new Vector2(0.5f, 0.5f);
        dialogRect.sizeDelta = new Vector2(520f, 300f);
        dialogRect.anchoredPosition = Vector2.zero;

        ConfigurePanelLayout(dialogPanel, 24, 24, 20, 20, 12f);
        CreateText("AccessoryPurchaseDialogTitle", dialogPanel.transform, "Purchase Accessory", 30f, FontStyles.Bold, TextAlignmentOptions.Center).color = new Color(0.08f, 0.18f, 0.22f);
        CreateText("AccessoryPurchaseDialogBody", dialogPanel.transform, bodyText, 22f, FontStyles.Normal, TextAlignmentOptions.Center).color = new Color(0.12f, 0.24f, 0.3f);

        GameObject buttonsRow = new GameObject("AccessoryPurchaseDialogButtons", typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(LayoutElement));
        buttonsRow.transform.SetParent(dialogPanel.transform, false);
        LayoutElement buttonsLayout = buttonsRow.GetComponent<LayoutElement>();
        buttonsLayout.preferredHeight = 88f;

        HorizontalLayoutGroup buttonsGroup = buttonsRow.GetComponent<HorizontalLayoutGroup>();
        buttonsGroup.spacing = 14f;
        buttonsGroup.childAlignment = TextAnchor.MiddleCenter;
        buttonsGroup.childControlWidth = true;
        buttonsGroup.childControlHeight = true;
        buttonsGroup.childForceExpandWidth = true;
        buttonsGroup.childForceExpandHeight = false;

        CreateDialogButton(buttonsRow.transform, "No", new Color(0.72f, 0.78f, 0.88f, 1f), CloseAccessoryPurchaseDialog, true);
        CreateDialogButton(
            buttonsRow.transform,
            canAfford ? "Yes" : "Need Coins",
            canAfford ? new Color(0.56f, 0.86f, 0.64f, 1f) : new Color(0.72f, 0.72f, 0.72f, 1f),
            delegate { TryPurchaseLockedAccessory(accessory.id, accessory.price); },
            canAfford);
    }

    private Button CreateDialogButton(Transform parent, string label, Color color, UnityEngine.Events.UnityAction onClick, bool interactable)
    {
        GameObject buttonObject = new GameObject(label + "DialogButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(LayoutElement));
        buttonObject.transform.SetParent(parent, false);

        Image buttonImage = buttonObject.GetComponent<Image>();
        buttonImage.color = color;
        buttonImage.raycastTarget = true;

        LayoutElement layoutElement = buttonObject.GetComponent<LayoutElement>();
        layoutElement.preferredHeight = 72f;
        layoutElement.minHeight = 72f;

        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = buttonImage;
        button.interactable = interactable;
        button.onClick.AddListener(onClick);

        GameObject labelObject = new GameObject(label + "DialogButtonLabel", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        labelObject.transform.SetParent(buttonObject.transform, false);

        RectTransform labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        TextMeshProUGUI labelText = labelObject.GetComponent<TextMeshProUGUI>();
        labelText.text = label;
        labelText.alignment = TextAlignmentOptions.Center;
        labelText.fontSize = 28f;
        labelText.fontStyle = FontStyles.Bold;
        labelText.color = new Color(0.08f, 0.16f, 0.22f, interactable ? 1f : 0.55f);
        labelText.raycastTarget = false;

        return button;
    }

    private void TryPurchaseLockedAccessory(string accessoryId, int price)
    {
        SaveManager.EnsureLoaded();
        if (IsAccessoryOwned(accessoryId))
        {
            CloseAccessoryPurchaseDialog();
            return;
        }

        if (SaveManager.Data.coins < price)
        {
            CloseAccessoryPurchaseDialog();
            return;
        }

        SaveManager.Data.coins -= price;
        if (!SaveManager.AddOwnedAccessoryToSelectedPocketPal(accessoryId))
        {
            CloseAccessoryPurchaseDialog();
            return;
        }

        SaveManager.Save();
        CloseAccessoryPurchaseDialog();
        RefreshAfterSave(CustomizationSceneName);
    }

    private void CloseAccessoryPurchaseDialog()
    {
        if (accessoryPurchaseDialogRoot == null)
        {
            return;
        }

        Destroy(accessoryPurchaseDialogRoot);
        accessoryPurchaseDialogRoot = null;
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

    public void BuildPocketPalContent(RectTransform mainContent)
    {
        ClearChildren(mainContent);
        pocketPalBodyText = null;
        pocketPalStatsText = null;

        CreateTitle(mainContent, "PocketPal");

        GameObject stage = CreatePanel("PocketPalStage", mainContent, new Color(0.83f, 0.92f, 1f, 0.95f), 620f);
        VerticalLayoutGroup stageLayout = ConfigurePanelLayout(stage, 28, 28, 36, 36, 22);
        stageLayout.childAlignment = TextAnchor.MiddleCenter;

        string palNameText = GetSelectedPocketPalDisplayName();

        TMP_Text palName = CreateText("PocketPalName", stage.transform, palNameText, 52, FontStyles.Bold, TextAlignmentOptions.Center);
        palName.color = new Color(0.08f, 0.18f, 0.22f);

        pocketPalFeedCount = SaveManager.GetSelectedPocketPalFeedCount();
        pocketPalBodyText = CreateText("PocketPalBody", stage.transform, CreatePocketPalStatusText(), 32, FontStyles.Normal, TextAlignmentOptions.Center);
        pocketPalBodyText.color = new Color(0.12f, 0.24f, 0.28f);

        GameObject actionPanel = CreatePanel("PocketPalActions", mainContent, new Color(1f, 1f, 1f, 0.42f), 330f);
        ConfigurePanelLayout(actionPanel, 26, 26, 26, 26, 20);

        TMP_Text actionLabel = CreateText("PocketPalActionLabel", actionPanel.transform, "What should " + palNameText + " do?", 30, FontStyles.Bold, TextAlignmentOptions.Center);
        actionLabel.color = new Color(0.08f, 0.18f, 0.22f);

        CreateButton("FeedButton", actionPanel.transform, "Feed", FeedPocketPal, new Color(1f, 0.9f, 0.52f, 1f));

        GameObject statsPanel = CreatePanel("PocketPalStats", mainContent, new Color(0.93f, 0.96f, 0.98f, 0.8f), 360f);
        ConfigurePanelLayout(statsPanel, 28, 28, 30, 30, 18);

        TMP_Text statsTitle = CreateText("PocketPalStatsTitle", statsPanel.transform, "PocketPal Stats", 34, FontStyles.Bold, TextAlignmentOptions.Center);
        statsTitle.color = new Color(0.08f, 0.18f, 0.22f);

        TMP_Text statsBody = CreateText("PocketPalStatsBody", statsPanel.transform, CreatePocketPalStatsSummary(), 28, FontStyles.Normal, TextAlignmentOptions.Left);
        statsBody.color = new Color(0.12f, 0.24f, 0.28f);
        pocketPalStatsText = statsBody;
    }

    public void BuildPocketPalOptionsContent(RectTransform mainContent)
    {
        ClearChildren(mainContent);
        pocketPalBodyText = null;
        pocketPalStatsText = null;
        GameObject feedButton = CreateButton("FeedButton", mainContent, "Feed", FeedPocketPal, new Color(1f, 0.9f, 0.52f, 1f));
        LayoutElement layoutElement = feedButton != null ? feedButton.GetComponent<LayoutElement>() : null;
        if (layoutElement != null)
        {
            layoutElement.preferredWidth = 120f;
            layoutElement.minWidth = 120f;
            layoutElement.preferredHeight = 52f;
            layoutElement.minHeight = 52f;
            layoutElement.flexibleWidth = 0f;
        }

        TMP_Text feedLabel = feedButton != null ? feedButton.GetComponentInChildren<TextMeshProUGUI>() : null;
        if (feedLabel != null)
        {
            feedLabel.fontSize = 22f;
            feedLabel.textWrappingMode = TextWrappingModes.NoWrap;
            feedLabel.overflowMode = TextOverflowModes.Overflow;
        }
    }

    private string CreateLevelProgressText()
    {
        int selectedPalLevel = SaveManager.GetSelectedPocketPalLevel();
        int selectedPalXp = SaveManager.GetSelectedPocketPalXp();
        int palNextXp = LevelingSystem.GetXpForNextLevel(selectedPalLevel);
        int habitatNextXp = LevelingSystem.GetXpForNextLevel(SaveManager.Data.habitatLevel);
        string palProgress = selectedPalLevel >= LevelingSystem.MaxLevel ? "Max level" : selectedPalXp + " / " + palNextXp + " XP";
        string habitatProgress = SaveManager.Data.habitatLevel >= LevelingSystem.MaxLevel ? "Max level" : SaveManager.Data.habitatXp + " / " + habitatNextXp + " XP";
        return "PocketPal level " + selectedPalLevel + ": " + palProgress + "\nHabitat level " + SaveManager.Data.habitatLevel + ": " + habitatProgress + "\nTasks, feeding, shopping, and reward claims add XP.";
    }

    private string CreateCompactLevelProgressText()
    {
        int selectedPalLevel = SaveManager.GetSelectedPocketPalLevel();
        int selectedPalXp = SaveManager.GetSelectedPocketPalXp();
        int palNextXp = LevelingSystem.GetXpForNextLevel(selectedPalLevel);
        int habitatNextXp = LevelingSystem.GetXpForNextLevel(SaveManager.Data.habitatLevel);
        string palProgress = selectedPalLevel >= LevelingSystem.MaxLevel ? "Max" : selectedPalXp + " / " + palNextXp + " XP";
        string habitatProgress = SaveManager.Data.habitatLevel >= LevelingSystem.MaxLevel ? "Max" : SaveManager.Data.habitatXp + " / " + habitatNextXp + " XP";
        return "PocketPal Lv " + selectedPalLevel + ": " + palProgress + "\nHabitat Lv " + SaveManager.Data.habitatLevel + ": " + habitatProgress;
    }

    private string GetSelectedPocketPalDisplayName()
    {
        return GetPocketPalName(SaveManager.Data.selectedPocketPalId);
    }

    private string GetPocketPalName(string pocketPalId)
    {
        for (int index = 0; index < PocketPalCatalog.Length; index++)
        {
            if (PocketPalCatalog[index].id == pocketPalId)
            {
                return PocketPalCatalog[index].name;
            }
        }

        return string.IsNullOrWhiteSpace(SaveManager.Data.selectedPocketPalName) ? "Panda" : SaveManager.Data.selectedPocketPalName;
    }

    private string GetPocketPalFavoriteFood(string pocketPalId)
    {
        if (pocketPalId == "white_tiger")
        {
            return "Chilled chicken bites";
        }

        if (pocketPalId == "otter")
        {
            return "River fish treats";
        }

        return "Berry bites";
    }

    private void CreateLevelRewardCard(Transform parent, string rewardId, string title, string description, int requiredPocketPalLevel, int requiredHabitatLevel, int coinReward, string habitatRewardId, string accessoryRewardId)
    {
        bool isRedeemed = SaveManager.Data.claimedLevelRewardIds != null && SaveManager.Data.claimedLevelRewardIds.Contains(rewardId);
        bool isUnlocked = SaveManager.GetSelectedPocketPalLevel() >= requiredPocketPalLevel && SaveManager.Data.habitatLevel >= requiredHabitatLevel;
        Color cardColor = isRedeemed ? new Color(0.79f, 0.88f, 0.8f, 0.92f) : new Color(0.91f, 0.87f, 1f, 0.98f);

        if (!isRedeemed && !isUnlocked)
        {
            cardColor = new Color(0.91f, 0.93f, 0.92f, 0.9f);
        }

        float cardHeight = compactDrawerMode ? 208f : 138f;
        GameObject card = CreatePanel(rewardId + "LevelRewardCard", parent, cardColor, cardHeight);

        if (!isRedeemed && isUnlocked)
        {
            Button button = card.AddComponent<Button>();
            Image image = card.GetComponent<Image>();
            image.raycastTarget = true;
            button.targetGraphic = image;
            button.onClick.AddListener(delegate { ClaimLevelReward(rewardId, requiredPocketPalLevel, requiredHabitatLevel, coinReward, habitatRewardId, accessoryRewardId); });
        }

        ConfigurePanelLayout(card, 24, 24, compactDrawerMode ? 22 : 18, compactDrawerMode ? 22 : 18, compactDrawerMode ? 12f : 8f);

        string status = isRedeemed
            ? "Claimed"
            : isUnlocked
                ? "Tap to claim"
                : compactDrawerMode
                    ? "Needs Pal Lv " + requiredPocketPalLevel + ", Habitat Lv " + requiredHabitatLevel
                    : "Requires PocketPal level " + requiredPocketPalLevel + " and habitat level " + requiredHabitatLevel;
        TMP_Text label = CreateText(rewardId + "LevelRewardLabel", card.transform, title + "\n" + description + "\n" + status, 24, FontStyles.Normal, TextAlignmentOptions.Left);
        if (compactDrawerMode)
        {
            LayoutElement labelLayout = label.GetComponent<LayoutElement>();
            label.fontSize = 20f;
            label.overflowMode = TextOverflowModes.Ellipsis;
            label.lineSpacing = 4f;
            if (labelLayout != null)
            {
                labelLayout.preferredHeight = Mathf.Max(labelLayout.preferredHeight, 156f);
                labelLayout.minHeight = labelLayout.preferredHeight;
            }
        }
        label.color = new Color(0.08f, 0.18f, 0.22f);
    }

    private void CreateProgressBar(string objectName, Transform parent, float progress, Color fillColor)
    {
        GameObject bar = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(LayoutElement));
        bar.transform.SetParent(parent, false);

        Image track = bar.GetComponent<Image>();
        track.color = new Color(1f, 1f, 1f, 0.74f);
        track.raycastTarget = false;

        LayoutElement layoutElement = bar.GetComponent<LayoutElement>();
        layoutElement.preferredHeight = compactDrawerMode ? 28f : 34f;
        layoutElement.minHeight = layoutElement.preferredHeight;
        layoutElement.flexibleWidth = 1f;

        GameObject fill = new GameObject("Fill", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        fill.transform.SetParent(bar.transform, false);

        RectTransform fillTransform = fill.GetComponent<RectTransform>();
        fillTransform.anchorMin = new Vector2(0f, 0f);
        fillTransform.anchorMax = new Vector2(Mathf.Clamp01(progress), 1f);
        fillTransform.offsetMin = new Vector2(5f, 5f);
        fillTransform.offsetMax = new Vector2(-5f, -5f);

        Image fillImage = fill.GetComponent<Image>();
        fillImage.color = fillColor;
        fillImage.raycastTarget = false;
    }

    private static Texture2D CreateTransparentTexture(int width, int height)
    {
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[width * height];
        for (int index = 0; index < pixels.Length; index++)
        {
            pixels[index] = Color.clear;
        }

        texture.SetPixels(pixels);
        return texture;
    }

    private static void DrawRoundedRect(Texture2D texture, int x, int y, int width, int height, int radius, Color color)
    {
        int minX = Mathf.Max(0, x);
        int maxX = Mathf.Min(texture.width - 1, x + width);
        int minY = Mathf.Max(0, y);
        int maxY = Mathf.Min(texture.height - 1, y + height);

        for (int pixelY = minY; pixelY <= maxY; pixelY++)
        {
            for (int pixelX = minX; pixelX <= maxX; pixelX++)
            {
                int cornerX = Mathf.Clamp(pixelX, x + radius, x + width - radius);
                int cornerY = Mathf.Clamp(pixelY, y + radius, y + height - radius);
                if (Vector2.Distance(new Vector2(pixelX, pixelY), new Vector2(cornerX, cornerY)) <= radius)
                {
                    texture.SetPixel(pixelX, pixelY, color);
                }
            }
        }
    }

    private static void DrawEllipse(Texture2D texture, int centerX, int centerY, int radiusX, int radiusY, Color color)
    {
        int minX = Mathf.Max(0, centerX - radiusX);
        int maxX = Mathf.Min(texture.width - 1, centerX + radiusX);
        int minY = Mathf.Max(0, centerY - radiusY);
        int maxY = Mathf.Min(texture.height - 1, centerY + radiusY);

        for (int y = minY; y <= maxY; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                float normalizedX = (x - centerX) / (float)radiusX;
                float normalizedY = (y - centerY) / (float)radiusY;
                if ((normalizedX * normalizedX) + (normalizedY * normalizedY) <= 1f)
                {
                    texture.SetPixel(x, y, color);
                }
            }
        }
    }

    private void ClaimLevelReward(string rewardId, int requiredPocketPalLevel, int requiredHabitatLevel, int coinReward, string habitatRewardId, string accessoryRewardId)
    {
        SaveManager.EnsureLoaded();

        if (!LevelingSystem.TryClaimLevelReward(rewardId, requiredPocketPalLevel, requiredHabitatLevel))
        {
            return;
        }

        SaveManager.Data.coins += coinReward;

        if (!string.IsNullOrEmpty(habitatRewardId) && SaveManager.Data.ownedHabitatIds != null && !SaveManager.Data.ownedHabitatIds.Contains(habitatRewardId))
        {
            SaveManager.Data.ownedHabitatIds.Add(habitatRewardId);
        }

        if (!string.IsNullOrEmpty(accessoryRewardId))
        {
            if (SaveManager.AddOwnedAccessoryToSelectedPocketPal(accessoryRewardId))
            {
                LevelingSystem.GrantPocketPalXp(GetAccessoryXpBonus(accessoryRewardId));
            }
        }

        SaveManager.Save();
        RefreshAfterSave(TasksRewardsSceneName);
    }

    private string CreatePocketPalStatusText()
    {
        int moodValue = SaveManager.GetSelectedPocketPalMood();
        int hungerValue = SaveManager.GetSelectedPocketPalHunger();
        pocketPalFeedCount = SaveManager.GetSelectedPocketPalFeedCount();
        return "Mood: " + GetMoodLabel(moodValue) + " (" + moodValue + "/100)\nHunger: " + GetHungerLabel(hungerValue) + " (" + hungerValue + "/100)\nSleep: " + CreateSleepMeterText() + "\nTimes fed: " + pocketPalFeedCount;
    }

    private string CreatePocketPalStatsSummary()
    {
        GameSaveData saveData = SaveManager.Data;
        string equippedAccessory = string.IsNullOrEmpty(saveData.equippedAccessoryId) ? "None" : saveData.equippedAccessoryId;
        return "Favorite food: " + GetPocketPalFavoriteFood(saveData.selectedPocketPalId) + "\nAccessory: " + equippedAccessory + "\nPocketPal level: " + SaveManager.GetSelectedPocketPalLevel() + "\nHunger: " + SaveManager.GetSelectedPocketPalHunger() + " / 100\nMood: " + SaveManager.GetSelectedPocketPalMood() + " / 100\nSleep meter: " + CreateSleepMeterText();
    }

    private string CreateSleepMeterText()
    {
        float sleepSeconds = SaveManager.GetSelectedPocketPalSleepSeconds();
        return FormatDurationMmSs(sleepSeconds) + " / " + FormatDurationMmSs(SleepSessionDurationSeconds);
    }

    private static string FormatDurationMmSs(float seconds)
    {
        int clampedSeconds = Mathf.Clamp(Mathf.FloorToInt(seconds), 0, 5999);
        int minutes = clampedSeconds / 60;
        int remainderSeconds = clampedSeconds % 60;
        return minutes.ToString("00") + ":" + remainderSeconds.ToString("00");
    }

    private static string GetMoodLabel(int mood)
    {
        if (mood >= 80)
        {
            return "Excited";
        }

        if (mood >= 60)
        {
            return "Happy";
        }

        if (mood >= 40)
        {
            return "Calm";
        }

        if (mood >= 20)
        {
            return "Grumpy";
        }

        return "Upset";
    }

    private static string GetHungerLabel(int hunger)
    {
        if (hunger >= 80)
        {
            return "Full";
        }

        if (hunger >= 60)
        {
            return "Content";
        }

        if (hunger >= 40)
        {
            return "Peckish";
        }

        if (hunger >= 20)
        {
            return "Hungry";
        }

        return "Very Hungry";
    }

    private TMP_Text CreateTitle(Transform parent, string text)
    {
        if (compactDrawerMode)
        {
            TMP_Text compactTitle = CreateText(text.Replace(" ", "") + "Title", parent, "", 1, FontStyles.Normal, TextAlignmentOptions.Center);
            LayoutElement layoutElement = compactTitle.GetComponent<LayoutElement>();
            if (layoutElement != null)
            {
                layoutElement.preferredHeight = 0f;
                layoutElement.minHeight = 0f;
                layoutElement.ignoreLayout = true;
            }

            return compactTitle;
        }

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
        image.color = compactDrawerMode ? MakeCuteSurfaceColor(color, objectName) : color;
        image.raycastTarget = false;

        float adjustedHeight = GetAdjustedPanelHeight(objectName, preferredHeight);
        LayoutElement layoutElement = panel.GetComponent<LayoutElement>();
        layoutElement.preferredHeight = adjustedHeight;
        layoutElement.minHeight = adjustedHeight;
        layoutElement.minWidth = -1f;
        layoutElement.preferredWidth = -1f;
        layoutElement.flexibleWidth = 1f;

        return panel;
    }

    private Color MakeCuteSurfaceColor(Color baseColor, string objectName)
    {
        if (objectName.Contains("Button"))
        {
            return baseColor;
        }

        if (objectName.Contains("Card"))
        {
            return Color.Lerp(baseColor, new Color(0.88f, 0.94f, 1f, baseColor.a), 0.18f);
        }

        return Color.Lerp(baseColor, new Color(0.9f, 0.86f, 1f, baseColor.a), 0.12f);
    }

    private float GetAdjustedPanelHeight(string objectName, float preferredHeight)
    {
        if (!compactDrawerMode)
        {
            return preferredHeight;
        }

        if (objectName.Contains("TaskCard") || objectName.Contains("RewardCard"))
        {
            return Mathf.Max(preferredHeight, 168f);
        }

        if (objectName.Contains("CompactShopCard"))
        {
            return preferredHeight;
        }

        if (objectName.Contains("ShopCard") || objectName.Contains("Accessory") || objectName.Contains("Option"))
        {
            return Mathf.Max(preferredHeight, 160f);
        }

        return preferredHeight;
    }

    private VerticalLayoutGroup ConfigurePanelLayout(GameObject panel, int left, int right, int top, int bottom, float spacing)
    {
        VerticalLayoutGroup layoutGroup = panel.AddComponent<VerticalLayoutGroup>();
        int adjustedLeft = compactDrawerMode ? Mathf.Min(left, 18) : left;
        int adjustedRight = compactDrawerMode ? Mathf.Min(right, 18) : right;
        int adjustedTop = compactDrawerMode ? Mathf.Min(top, 20) : top;
        int adjustedBottom = compactDrawerMode ? Mathf.Min(bottom, 20) : bottom;
        layoutGroup.padding = new RectOffset(adjustedLeft, adjustedRight, adjustedTop, adjustedBottom);
        layoutGroup.spacing = compactDrawerMode ? Mathf.Max(12f, spacing * 0.75f) : spacing;
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
        float adjustedFontSize = compactDrawerMode ? Mathf.Max(20f, fontSize * 0.86f) : fontSize;
        label.fontSize = adjustedFontSize;
        label.fontStyle = fontStyle;
        label.alignment = alignment;
        label.raycastTarget = false;
        label.textWrappingMode = TextWrappingModes.Normal;
        label.overflowMode = TextOverflowModes.Overflow;

        LayoutElement layoutElement = textObject.GetComponent<LayoutElement>();
        layoutElement.minWidth = -1f;
        layoutElement.preferredWidth = -1f;
        int lineCount = Mathf.Max(1, text.Split('\n').Length);
        float lineAwareHeight = lineCount * adjustedFontSize * 1.45f + (compactDrawerMode ? 22f : 16f);
        layoutElement.preferredHeight = Mathf.Max(compactDrawerMode ? 48f : 60f, lineAwareHeight, label.preferredHeight + (compactDrawerMode ? 22f : 16f));
        layoutElement.flexibleWidth = 1f;

        return label;
    }

    private GameObject CreateButton(string objectName, Transform parent, string label, UnityEngine.Events.UnityAction onClick, Color color)
    {
        GameObject buttonObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(LayoutElement));
        buttonObject.transform.SetParent(parent, false);

        Image image = buttonObject.GetComponent<Image>();
        image.color = Color.Lerp(color, new Color(0.82f, 0.72f, 1f, 1f), compactDrawerMode ? 0.24f : 0.12f);

        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(onClick);

        LayoutElement layoutElement = buttonObject.GetComponent<LayoutElement>();
        layoutElement.preferredHeight = compactDrawerMode ? 72f : 92f;
        layoutElement.minHeight = compactDrawerMode ? 72f : 92f;
        layoutElement.minWidth = -1f;
        layoutElement.preferredWidth = -1f;
        layoutElement.flexibleWidth = 1f;

        TMP_Text buttonLabel = CreateText(objectName + "Label", buttonObject.transform, label, 34, FontStyles.Bold, TextAlignmentOptions.Center);
        buttonLabel.color = new Color(0.08f, 0.18f, 0.22f);

        RectTransform labelTransform = buttonLabel.GetComponent<RectTransform>();
        labelTransform.anchorMin = Vector2.zero;
        labelTransform.anchorMax = Vector2.one;
        labelTransform.offsetMin = Vector2.zero;
        labelTransform.offsetMax = Vector2.zero;

        return buttonObject;
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

    private void OnDisable()
    {
        CloseAccessoryPurchaseDialog();
        CloseHabitatPurchaseDialog();
        CloseCoinPurchaseDialog();
    }

    public void GoBackToSanctuary()
    {
        SceneManager.LoadScene("SanctuaryScene", LoadSceneMode.Single);
    }

    private void GoToCustomization()
    {
        if (SceneManager.GetActiveScene().name == "SanctuaryScene")
        {
            SanctuaryUI sanctuaryUI = FindAnyObjectByType<SanctuaryUI>();
            if (sanctuaryUI != null)
            {
                sanctuaryUI.OnCustomizationClicked();
                return;
            }
        }

        SceneManager.LoadScene("CustomizationScene", LoadSceneMode.Single);
    }

    private void FeedPocketPal()
    {
        SaveManager.EnsureLoaded();

        SaveManager.FeedSelectedPocketPal();
        pocketPalFeedCount = SaveManager.GetSelectedPocketPalFeedCount();
        LevelingSystem.GrantPocketPalXp(12);
        LevelingSystem.GrantHabitatXp(4);
        SaveManager.Save();

        if (pocketPalBodyText != null)
        {
            pocketPalBodyText.text = CreatePocketPalStatusText();
        }

        if (pocketPalStatsText != null)
        {
            pocketPalStatsText.text = CreatePocketPalStatsSummary();
        }

        SanctuaryUI sanctuaryUI = FindAnyObjectByType<SanctuaryUI>();
        if (sanctuaryUI != null)
        {
            sanctuaryUI.PlayFeedReaction();
        }

        Debug.Log("PocketPal fed.");
    }

    private void RefreshAfterSave(string sceneName)
    {
        if (SceneManager.GetActiveScene().name == "SanctuaryScene")
        {
            SanctuaryUI sanctuaryUI = FindAnyObjectByType<SanctuaryUI>();
            if (sanctuaryUI != null)
            {
                sanctuaryUI.RefreshOpenPanel();
                return;
            }
        }

        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
