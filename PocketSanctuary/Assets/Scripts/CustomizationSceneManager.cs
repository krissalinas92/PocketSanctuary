using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

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
    private const string MonocleAccessoryId = "monocle";
    private const string AviatorsAccessoryId = "aviators";
    private const string BeanieAccessoryId = "beanie";
    private const string HeadbandAccessoryId = "headband";
    private const string CrownAccessoryId = "crown";
    private const string LakersShirtAccessoryId = "shirt_lakers";
    private const string BullsShirtAccessoryId = "shirt_bulls";
    private const string CelticsShirtAccessoryId = "shirt_celtics";
    private const string KnicksShirtAccessoryId = "shirt_knicks";
    private const string NirvanaShirtAccessoryId = "shirt_nirvana";
    private const string MetallicaShirtAccessoryId = "shirt_metallica";
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
    private bool compactDrawerMode;
    private static readonly Dictionary<string, Sprite> previewSpriteCache = new Dictionary<string, Sprite>();

    private class HabitatDefinition
    {
        public string id;
        public string name;
        public string description;

        public HabitatDefinition(string id, string name, string description)
        {
            this.id = id;
            this.name = name;
            this.description = description;
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

    private static readonly AccessoryDefinition[] AccessoryCatalog = new AccessoryDefinition[]
    {
        new AccessoryDefinition(SunglassesAccessoryId, "Eyewear", "Round shades", "Gucci-brown round sunglasses.", 0),
        new AccessoryDefinition(MonocleAccessoryId, "Eyewear", "Gold monocle", "A fancy one-eye glass with a tiny chain.", 90),
        new AccessoryDefinition(AviatorsAccessoryId, "Eyewear", "Sky aviators", "Large blue-tinted aviators.", 130),
        new AccessoryDefinition(HatAccessoryId, "Headwear", "Green cap", "A soft cap for cozy walks.", HatPrice),
        new AccessoryDefinition(BeanieAccessoryId, "Headwear", "Lavender beanie", "A plush beanie for chilly sanctuary mornings.", 140),
        new AccessoryDefinition(HeadbandAccessoryId, "Headwear", "Sport headband", "A green-and-lavender workout band.", 100),
        new AccessoryDefinition(CrownAccessoryId, "Headwear", "Tiny crown", "A shiny royal crown for your pocket pal.", 220),
        new AccessoryDefinition(TShirtAccessoryId, "Shirts", "Lavender tank", "A sleeveless lavender tank.", TShirtPrice),
        new AccessoryDefinition(LakersShirtAccessoryId, "Shirts", "Lakers tank", "Purple and gold Los Angeles Lakers-inspired tank.", 190),
        new AccessoryDefinition(BullsShirtAccessoryId, "Shirts", "Bulls tank", "Red and black Chicago Bulls-inspired tank.", 190),
        new AccessoryDefinition(CelticsShirtAccessoryId, "Shirts", "Celtics tank", "Green and white Boston Celtics-inspired tank.", 190),
        new AccessoryDefinition(KnicksShirtAccessoryId, "Shirts", "Knicks tank", "Blue and orange New York Knicks-inspired tank.", 190),
        new AccessoryDefinition(NirvanaShirtAccessoryId, "Shirts", "Nirvana tank", "Black band-style tank with a sunny badge.", 210),
        new AccessoryDefinition(MetallicaShirtAccessoryId, "Shirts", "Metallica tank", "Charcoal band-style tank with silver trim.", 210)
    };

    private static readonly HabitatDefinition[] HabitatCatalog = new HabitatDefinition[]
    {
        new HabitatDefinition("sunny_garden", "Sunny Playpen", "Zoo habitat with trees, a swing, and a small playpen."),
        new HabitatDefinition("moonlit_pond", "Moonlit Pond", "Night habitat with a darker sky, moonlight, and a reflective pond."),
        new HabitatDefinition("bamboo_grove", "Bamboo Grove", "Reward habitat with extra bamboo shade.")
    };

    public void SetCompactDrawerMode(bool isCompact)
    {
        compactDrawerMode = isCompact;
    }

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

    public void BuildCustomizationContent(RectTransform mainContent)
    {
        ClearChildren(mainContent);

        CreateTitle(mainContent, "Customization");

        CreateHabitatSection(mainContent);

        CreateCategorySection(mainContent, "PocketPals", "Pick the animal living in your sanctuary.", 300f, new string[]
        {
            "Panda\nSelected starter pal"
        });

        GameObject accessoriesSection = CreatePanel("AccessoriesSection", mainContent, new Color(1f, 1f, 1f, 0.42f), 2600f);
        ConfigurePanelLayout(accessoriesSection, 28, 28, 34, 34, 24);

        TMP_Text accessoriesTitle = CreateText("AccessoriesTitle", accessoriesSection.transform, "Accessories", 34, FontStyles.Bold, TextAlignmentOptions.Center);
        accessoriesTitle.color = new Color(0.08f, 0.18f, 0.22f);

        TMP_Text accessoriesSubtitle = CreateText("AccessoriesSubtitle", accessoriesSection.transform, "Dress up your panda by category.", 24, FontStyles.Normal, TextAlignmentOptions.Center);
        accessoriesSubtitle.color = new Color(0.16f, 0.28f, 0.32f);

        CreateAccessoryCategory(accessoriesSection.transform, "Eyewear");
        CreateAccessoryCategory(accessoriesSection.transform, "Headwear");
        CreateAccessoryCategory(accessoriesSection.transform, "Shirts");
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

        TMP_Text subtitleText = CreateText("HabitatBackgroundsSubtitle", section.transform, "Choose the space your panda calls home.", 24, FontStyles.Normal, TextAlignmentOptions.Center);
        subtitleText.color = new Color(0.16f, 0.28f, 0.32f);

        for (int index = 0; index < HabitatCatalog.Length; index++)
        {
            CreateHabitatCard(HabitatCatalog[index], section.transform);
        }
    }

    private void CreateHabitatCard(HabitatDefinition habitat, Transform parent)
    {
        bool isOwned = IsHabitatOwned(habitat.id);
        bool isSelected = SaveManager.Data.selectedHabitatId == habitat.id;
        Color cardColor = isSelected ? new Color(0.78f, 0.9f, 0.76f, 0.96f) : isOwned ? new Color(0.94f, 0.97f, 0.98f, 0.95f) : new Color(0.88f, 0.9f, 0.92f, 0.78f);
        GameObject card = CreatePanel(habitat.id + "HabitatCard", parent, cardColor, 126f);

        if (isOwned)
        {
            Button button = card.AddComponent<Button>();
            Image image = card.GetComponent<Image>();
            image.raycastTarget = true;
            button.interactable = true;
            button.targetGraphic = image;
            button.onClick.AddListener(delegate { SelectHabitat(habitat.id); });
        }

        ConfigurePanelLayout(card, 24, 24, 20, 20, 8);

        string status = isSelected ? "Selected" : isOwned ? "Tap to use" : "Locked";
        TMP_Text label = CreateText(habitat.id + "HabitatLabel", card.transform, habitat.name + "\n" + habitat.description + "\n" + status, 25, FontStyles.Normal, TextAlignmentOptions.Left);
        label.color = isOwned ? new Color(0.08f, 0.18f, 0.22f) : new Color(0.22f, 0.28f, 0.3f);
    }

    private void CreateAccessoryCategory(Transform parent, string categoryTitle)
    {
        AccessoryDefinition[] categoryItems = GetAccessoriesForCategory(categoryTitle);
        GameObject categoryPanel = CreatePanel(categoryTitle.Replace("-", "").Replace(" ", "") + "Category", parent, new Color(0.93f, 0.97f, 0.96f, 0.72f), 120f + categoryItems.Length * 178f);
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

        CreateAccessoryPreview(card.transform, accessoryId, IsAccessoryOwned(accessoryId));

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

    private bool IsHabitatOwned(string habitatId)
    {
        return SaveManager.Data.ownedHabitatIds != null && SaveManager.Data.ownedHabitatIds.Contains(habitatId);
    }

    private void SelectHabitat(string habitatId)
    {
        if (!IsHabitatOwned(habitatId))
        {
            return;
        }

        SaveManager.Data.selectedHabitatId = habitatId;
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

    private void CreateAccessoryPreview(Transform parent, string accessoryId, bool isUnlocked)
    {
        GameObject previewObject = new GameObject(accessoryId + "Preview", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(LayoutElement));
        previewObject.transform.SetParent(parent, false);

        Image previewImage = previewObject.GetComponent<Image>();
        previewImage.sprite = GetAccessoryPreviewSprite(accessoryId);
        previewImage.preserveAspect = true;
        previewImage.raycastTarget = false;
        previewImage.color = isUnlocked ? Color.white : new Color(1f, 1f, 1f, 0.38f);

        LayoutElement layoutElement = previewObject.GetComponent<LayoutElement>();
        layoutElement.preferredHeight = compactDrawerMode ? 74f : 88f;
        layoutElement.minHeight = layoutElement.preferredHeight;
        layoutElement.flexibleWidth = 1f;
    }

    private Sprite GetAccessoryPreviewSprite(string accessoryId)
    {
        if (previewSpriteCache.ContainsKey(accessoryId))
        {
            return previewSpriteCache[accessoryId];
        }

        Texture2D texture = CreateAccessoryPreviewTexture(accessoryId);
        Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, 96f, 96f), new Vector2(0.5f, 0.5f), 100f);
        previewSpriteCache[accessoryId] = sprite;
        return sprite;
    }

    private Texture2D CreateAccessoryPreviewTexture(string accessoryId)
    {
        Texture2D texture = CreateTransparentTexture(96, 96);
        Color lavender = new Color(0.57f, 0.48f, 0.93f, 1f);
        Color grass = new Color(0.35f, 0.62f, 0.34f, 1f);
        Color sky = new Color(0.62f, 0.84f, 0.96f, 1f);
        Color gold = new Color(0.96f, 0.72f, 0.16f, 1f);
        Color brown = new Color(0.42f, 0.24f, 0.1f, 1f);

        if (GetAccessoryCategory(accessoryId) == "Eyewear")
        {
            if (accessoryId == MonocleAccessoryId)
            {
                DrawEllipse(texture, 48, 50, 18, 18, gold);
                DrawEllipse(texture, 48, 50, 12, 12, sky);
                DrawRoundedRect(texture, 64, 45, 18, 4, 2, gold);
            }
            else
            {
                Color frame = accessoryId == AviatorsAccessoryId ? new Color(0.18f, 0.28f, 0.36f, 1f) : brown;
                Color lens = accessoryId == AviatorsAccessoryId ? sky : new Color(0.22f, 0.12f, 0.05f, 0.92f);
                DrawEllipse(texture, 34, 50, 22, 18, frame);
                DrawEllipse(texture, 62, 50, 22, 18, frame);
                DrawEllipse(texture, 34, 50, 16, 13, lens);
                DrawEllipse(texture, 62, 50, 16, 13, lens);
                DrawRoundedRect(texture, 46, 47, 8, 4, 2, frame);
            }
        }
        else if (GetAccessoryCategory(accessoryId) == "Headwear")
        {
            Color color = accessoryId == BeanieAccessoryId ? lavender : accessoryId == CrownAccessoryId ? gold : grass;
            DrawRoundedRect(texture, 24, 38, 48, 24, 10, color);
            DrawEllipse(texture, 48, 64, 34, 8, color);
            if (accessoryId == CrownAccessoryId)
            {
                DrawEllipse(texture, 30, 67, 8, 12, color);
                DrawEllipse(texture, 48, 70, 8, 16, color);
                DrawEllipse(texture, 66, 67, 8, 12, color);
            }
        }
        else
        {
            Color shirtColor = GetShirtColor(accessoryId);
            DrawRoundedRect(texture, 21, 20, 54, 62, 13, shirtColor);
            DrawEllipse(texture, 48, 54, 7, 7, gold);
        }

        texture.Apply();
        return texture;
    }

    private string GetAccessoryCategory(string accessoryId)
    {
        AccessoryDefinition definition = GetAccessoryDefinition(accessoryId);
        return definition != null ? definition.category : "";
    }

    private Color GetShirtColor(string accessoryId)
    {
        if (accessoryId == LakersShirtAccessoryId) return new Color(0.36f, 0.19f, 0.63f, 1f);
        if (accessoryId == BullsShirtAccessoryId) return new Color(0.72f, 0.05f, 0.08f, 1f);
        if (accessoryId == CelticsShirtAccessoryId) return new Color(0.02f, 0.48f, 0.23f, 1f);
        if (accessoryId == KnicksShirtAccessoryId) return new Color(0.08f, 0.31f, 0.67f, 1f);
        if (accessoryId == NirvanaShirtAccessoryId) return new Color(0.08f, 0.08f, 0.08f, 1f);
        if (accessoryId == MetallicaShirtAccessoryId) return new Color(0.14f, 0.14f, 0.16f, 1f);
        return new Color(0.57f, 0.48f, 0.93f, 1f);
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
            RemoveEquippedAccessoriesInCategory(GetAccessoryCategory(accessoryId));
            SaveManager.Data.equippedAccessoryIds.Add(accessoryId);
        }

        SaveManager.Data.equippedAccessoryId = SaveManager.Data.equippedAccessoryIds.Count > 0 ? SaveManager.Data.equippedAccessoryIds[0] : "";
        SaveManager.Save();
        RefreshAfterSave(CustomizationSceneName);
    }

    private bool IsAccessoryEquipped(string accessoryId)
    {
        return SaveManager.Data.equippedAccessoryIds != null && SaveManager.Data.equippedAccessoryIds.Contains(accessoryId);
    }

    private bool IsAccessoryOwned(string accessoryId)
    {
        return SaveManager.Data.ownedAccessoryIds != null && SaveManager.Data.ownedAccessoryIds.Contains(accessoryId);
    }

    private void RemoveEquippedAccessoriesInCategory(string category)
    {
        if (string.IsNullOrEmpty(category) || SaveManager.Data.equippedAccessoryIds == null)
        {
            return;
        }

        for (int index = SaveManager.Data.equippedAccessoryIds.Count - 1; index >= 0; index--)
        {
            if (GetAccessoryCategory(SaveManager.Data.equippedAccessoryIds[index]) == category)
            {
                SaveManager.Data.equippedAccessoryIds.RemoveAt(index);
            }
        }
    }

    public void BuildTasksRewardsContent(RectTransform mainContent)
    {
        ClearChildren(mainContent);

        if (!compactDrawerMode)
        {
            CreateTitle(mainContent, "Tasks & Rewards");
        }

        GameObject tasksPanel = CreatePanel("TasksPanel", mainContent, new Color(0.86f, 0.95f, 1f, 0.84f), 860f);
        ConfigurePanelLayout(tasksPanel, 28, 28, 34, 34, 22);

        if (!compactDrawerMode)
        {
            TMP_Text tasksTitle = CreateText("TasksTitle", tasksPanel.transform, "Tasks", 34, FontStyles.Bold, TextAlignmentOptions.Center);
            tasksTitle.color = new Color(0.08f, 0.18f, 0.22f);

            TMP_Text tasksSubtitle = CreateText("TasksSubtitle", tasksPanel.transform, "Complete care actions, then claim coins here.", 24, FontStyles.Normal, TextAlignmentOptions.Center);
            tasksSubtitle.color = new Color(0.16f, 0.28f, 0.32f);
        }

        CreateTaskCard(tasksPanel.transform, FeedTaskId, "Feed your panda", "Feed your panda from the PocketPal screen.", 10, SaveManager.Data.snacksFed > 0);
        CreateTaskCard(tasksPanel.transform, CheckInTaskId, "Check in today", "Open the game and visit your sanctuary.", 5, true);
        CreateTaskCard(tasksPanel.transform, AccessorizeTaskId, "Accessorize your panda", "Equip sunglasses, a hat, or a t-shirt.", 15, SaveManager.Data.equippedAccessoryIds != null && SaveManager.Data.equippedAccessoryIds.Count > 0);
        CreateTaskCard(tasksPanel.transform, VisitShopTaskId, "Visit the shop", "Browse the panda accessory shop.", 5, SaveManager.Data.hasVisitedShop);

        GameObject rewardsPanel = CreatePanel("RewardsPanel", mainContent, new Color(0.83f, 0.93f, 0.78f, 0.88f), 680f);
        ConfigurePanelLayout(rewardsPanel, 28, 28, 30, 30, 18);

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
        CreateRewardCard(rewardsPanel.transform, SanctuaryRewardId, "Sanctuary growth bonus", "A larger bonus for steady progress.", 50, 250);

        GameObject levelPanel = CreatePanel("LevelRewardsPanel", mainContent, new Color(0.9f, 0.86f, 1f, 0.88f), 620f);
        ConfigurePanelLayout(levelPanel, 28, 28, 30, 30, 18);

        if (!compactDrawerMode)
        {
            TMP_Text levelTitle = CreateText("LevelRewardsTitle", levelPanel.transform, "Level Rewards", 34, FontStyles.Bold, TextAlignmentOptions.Center);
            levelTitle.color = new Color(0.08f, 0.18f, 0.22f);
        }

        TMP_Text levelBody = CreateText("LevelRewardsBody", levelPanel.transform, CreateLevelProgressText(), 24, FontStyles.Normal, TextAlignmentOptions.Left);
        levelBody.color = new Color(0.12f, 0.24f, 0.28f);
        float palProgress = SaveManager.Data.pocketPalLevel >= LevelingSystem.MaxLevel ? 1f : SaveManager.Data.pocketPalXp / (float)LevelingSystem.GetXpForNextLevel(SaveManager.Data.pocketPalLevel);
        CreateProgressBar("PocketPalLevelProgressBar", levelPanel.transform, palProgress, new Color(0.52f, 0.8f, 1f, 1f));

        CreateLevelRewardCard(levelPanel.transform, "pal_level_2_treat", "PocketPal Treat Bundle", "Reach PocketPal level 2 to earn bonus coins.", 2, 1, 75, "", "");
        CreateLevelRewardCard(levelPanel.transform, "habitat_level_2_bamboo", "Bamboo Habitat Kit", "Reach habitat level 2 to unlock a bamboo habitat reward.", 1, 2, 0, "bamboo_grove", "");
        CreateLevelRewardCard(levelPanel.transform, "pal_level_3_hat", "Level 3 Hat Reward", "Reach PocketPal level 3 to unlock the green cap.", 3, 1, 0, "", HatAccessoryId);
    }

    private void CreateTaskCard(Transform parent, string taskId, string title, string description, int coinReward, bool isReady)
    {
        bool isComplete = IsTaskComplete(taskId);
        Color cardColor = isComplete ? new Color(0.79f, 0.88f, 0.8f, 0.92f) : new Color(0.9f, 0.97f, 1f, 0.98f);

        if (!isComplete && !isReady)
        {
            cardColor = new Color(0.91f, 0.93f, 0.92f, 0.9f);
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
        LevelingSystem.GrantPocketPalXp(coinReward * 2);
        LevelingSystem.GrantHabitatXp(coinReward);
        SaveManager.Save();
        RefreshAfterSave(TasksRewardsSceneName);
    }

    private bool IsTaskComplete(string taskId)
    {
        return SaveManager.Data.completedTaskIds != null && SaveManager.Data.completedTaskIds.Contains(taskId);
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

        GameObject accessoryShop = CreatePanel("AccessoryShopSection", mainContent, new Color(1f, 1f, 1f, 0.42f), 760f);
        ConfigurePanelLayout(accessoryShop, 28, 28, 34, 34, 24);

        if (!compactDrawerMode)
        {
            TMP_Text shopTitle = CreateText("AccessoryShopTitle", accessoryShop.transform, "Panda Accessories", 34, FontStyles.Bold, TextAlignmentOptions.Center);
            shopTitle.color = new Color(0.08f, 0.18f, 0.22f);

            TMP_Text shopSubtitle = CreateText("AccessoryShopSubtitle", accessoryShop.transform, "Buy items here, then equip them in Customization.", 24, FontStyles.Normal, TextAlignmentOptions.Center);
            shopSubtitle.color = new Color(0.16f, 0.28f, 0.32f);
        }

        for (int index = 0; index < AccessoryCatalog.Length; index++)
        {
            AccessoryDefinition accessory = AccessoryCatalog[index];
            CreateShopItemCard(accessory.id + "ShopCard", accessoryShop.transform, accessory.id, accessory.name, accessory.price, accessory.description);
        }
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

        CreateAccessoryPreview(card.transform, accessoryId, true);

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
        LevelingSystem.GrantPocketPalXp(15);
        SaveManager.Save();
        RefreshAfterSave(ShopSceneName);
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
        TMP_Text statsBody = CreateText("PocketPalStatsBody", statsPanel.transform, "Favorite food: Berry bites\nAccessory: " + equippedAccessory + "\nPocketPal level: " + saveData.pocketPalLevel + "\nHabitat level: " + saveData.habitatLevel + "\nSnacks fed: " + saveData.snacksFed + "\nNext reward: level milestones", 28, FontStyles.Normal, TextAlignmentOptions.Left);
        statsBody.color = new Color(0.12f, 0.24f, 0.28f);
    }

    public void BuildPocketPalOptionsContent(RectTransform mainContent)
    {
        ClearChildren(mainContent);

        GameSaveData saveData = SaveManager.Data;
        string palNameText = string.IsNullOrWhiteSpace(saveData.selectedPocketPalName) ? "Panda" : saveData.selectedPocketPalName;

        TMP_Text title = CreateText("PocketPalOptionsTitle", mainContent, palNameText, 30, FontStyles.Bold, TextAlignmentOptions.Center);
        title.color = new Color(0.08f, 0.18f, 0.22f);

        pocketPalFeedCount = saveData.snacksFed;
        pocketPalBodyText = CreateText("PocketPalOptionsStatus", mainContent, CreatePocketPalStatusText("Curious", "Ready for a snack"), 22, FontStyles.Normal, TextAlignmentOptions.Center);
        pocketPalBodyText.color = new Color(0.12f, 0.24f, 0.28f);

        CreateButton("AccessorizeButton", mainContent, "Accessorize", GoToCustomization, new Color(0.69f, 0.88f, 0.66f, 1f));
        CreateButton("FeedButton", mainContent, "Feed", FeedPocketPal, new Color(1f, 0.9f, 0.52f, 1f));

        string equippedAccessory = string.IsNullOrEmpty(saveData.equippedAccessoryId) ? "None" : saveData.equippedAccessoryId;
        TMP_Text stats = CreateText("PocketPalOptionsStats", mainContent, "PocketPal level: " + saveData.pocketPalLevel + "\nHabitat level: " + saveData.habitatLevel + "\nAccessory: " + equippedAccessory, 21, FontStyles.Normal, TextAlignmentOptions.Left);
        stats.color = new Color(0.12f, 0.24f, 0.28f);
    }

    private string CreateLevelProgressText()
    {
        int palNextXp = LevelingSystem.GetXpForNextLevel(SaveManager.Data.pocketPalLevel);
        int habitatNextXp = LevelingSystem.GetXpForNextLevel(SaveManager.Data.habitatLevel);
        string palProgress = SaveManager.Data.pocketPalLevel >= LevelingSystem.MaxLevel ? "Max level" : SaveManager.Data.pocketPalXp + " / " + palNextXp + " XP";
        string habitatProgress = SaveManager.Data.habitatLevel >= LevelingSystem.MaxLevel ? "Max level" : SaveManager.Data.habitatXp + " / " + habitatNextXp + " XP";
        return "PocketPal level " + SaveManager.Data.pocketPalLevel + ": " + palProgress + "\nHabitat level " + SaveManager.Data.habitatLevel + ": " + habitatProgress + "\nTasks, feeding, shopping, and reward claims add XP.";
    }

    private void CreateLevelRewardCard(Transform parent, string rewardId, string title, string description, int requiredPocketPalLevel, int requiredHabitatLevel, int coinReward, string habitatRewardId, string accessoryRewardId)
    {
        bool isRedeemed = SaveManager.Data.claimedLevelRewardIds != null && SaveManager.Data.claimedLevelRewardIds.Contains(rewardId);
        bool isUnlocked = SaveManager.Data.pocketPalLevel >= requiredPocketPalLevel && SaveManager.Data.habitatLevel >= requiredHabitatLevel;
        Color cardColor = isRedeemed ? new Color(0.79f, 0.88f, 0.8f, 0.92f) : new Color(0.91f, 0.87f, 1f, 0.98f);

        if (!isRedeemed && !isUnlocked)
        {
            cardColor = new Color(0.91f, 0.93f, 0.92f, 0.9f);
        }

        GameObject card = CreatePanel(rewardId + "LevelRewardCard", parent, cardColor, 138f);

        if (!isRedeemed && isUnlocked)
        {
            Button button = card.AddComponent<Button>();
            Image image = card.GetComponent<Image>();
            image.raycastTarget = true;
            button.targetGraphic = image;
            button.onClick.AddListener(delegate { ClaimLevelReward(rewardId, requiredPocketPalLevel, requiredHabitatLevel, coinReward, habitatRewardId, accessoryRewardId); });
        }

        ConfigurePanelLayout(card, 24, 24, 18, 18, 8);

        string status = isRedeemed ? "Claimed" : isUnlocked ? "Tap to claim" : "Requires PocketPal level " + requiredPocketPalLevel + " and habitat level " + requiredHabitatLevel;
        TMP_Text label = CreateText(rewardId + "LevelRewardLabel", card.transform, title + "\n" + description + "\n" + status, 24, FontStyles.Normal, TextAlignmentOptions.Left);
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

        if (!string.IsNullOrEmpty(accessoryRewardId) && SaveManager.Data.ownedAccessoryIds != null && !SaveManager.Data.ownedAccessoryIds.Contains(accessoryRewardId))
        {
            SaveManager.Data.ownedAccessoryIds.Add(accessoryRewardId);
        }

        SaveManager.Save();
        RefreshAfterSave(TasksRewardsSceneName);
    }

    private string CreatePocketPalStatusText(string mood, string hunger)
    {
        return "Pal lives here\n\nMood: " + mood + "\nHunger: " + hunger + "\nSnacks eaten: " + pocketPalFeedCount;
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

    private void CreateButton(string objectName, Transform parent, string label, UnityEngine.Events.UnityAction onClick, Color color)
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

        pocketPalFeedCount++;
        SaveManager.Data.snacksFed = pocketPalFeedCount;
        LevelingSystem.GrantPocketPalXp(12);
        LevelingSystem.GrantHabitatXp(4);
        SaveManager.Save();

        if (pocketPalBodyText != null)
        {
            pocketPalBodyText.text = CreatePocketPalStatusText("Happy", "Full");
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
