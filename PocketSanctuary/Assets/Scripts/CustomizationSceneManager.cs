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
        ScrollRect scrollRect = FindFirstObjectByType<ScrollRect>();
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
        CreateSection(mainContent, "Habitat Backgrounds", "Side-scroller placeholder for home spaces.", 360f, new string[]
        {
            "Sunny Garden - calm starter habitat",
            "Moonlit Pond - locked decorative backdrop",
            "Cozy Burrow - future reward unlock"
        });

        CreateSection(mainContent, "PocketPals", "Side-scroller placeholder for choosing your active pal.", 360f, new string[]
        {
            "Sprout - selected",
            "Pebble - unlock later",
            "Nimbus - unlock later"
        });

        CreateSection(mainContent, "Accessories", "Small wearable items for your active pal.", 420f, new string[]
        {
            "Leaf Hat - owned",
            "Tiny Scarf - shop item placeholder",
            "Star Pin - reward placeholder"
        });
    }

    private void BuildTasksRewardsContent(RectTransform mainContent)
    {
        ClearChildren(mainContent);

        CreateTitle(mainContent, "Tasks & Rewards");
        CreateSection(mainContent, "Tasks", "Daily care goals and streak builders.", 760f, new string[]
        {
            "Feed your PocketPal - +10 coins",
            "Check in today - +5 coins",
            "Accessorize your PocketPal - +15 coins",
            "Visit the shop - +5 coins"
        });

        CreateSection(mainContent, "Rewards", "Redeem progress for coins, cosmetics, and habitat pieces.", 760f, new string[]
        {
            "Progress bar placeholder - 25 / 100",
            "Redeem 50 coins",
            "Redeem Leaf Hat",
            "Redeem Garden Background"
        });
    }

    private void BuildShopContent(RectTransform mainContent)
    {
        ClearChildren(mainContent);

        CreateTitle(mainContent, "Shop");
        CreateSection(mainContent, "Buy Coins", "Coin pack placeholders for future purchases.", 520f, new string[]
        {
            "Small pouch - 100 coins",
            "Garden jar - 500 coins",
            "Sanctuary chest - 1200 coins"
        });

        CreateSection(mainContent, "Decor", "Future habitat and pal cosmetic shop items.", 460f, new string[]
        {
            "Flower Arch - 250 coins",
            "Cloud Mobile - 300 coins",
            "Tiny Scarf - 150 coins"
        });
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
        label.enableWordWrapping = true;

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
        SaveManager.Data.coins += 10;
        SaveManager.Data.rewardProgress += 10;
        SaveManager.Save();

        TopBarManager topBarManager = FindFirstObjectByType<TopBarManager>();
        if (topBarManager != null)
        {
            topBarManager.RefreshCoinDisplay();
        }

        if (pocketPalBodyText != null)
        {
            pocketPalBodyText.text = CreatePocketPalStatusText("Happy", "Full");
        }

        Debug.Log("PocketPal fed.");
    }
}
