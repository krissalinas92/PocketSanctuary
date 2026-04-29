using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TopBarManager : MonoBehaviour
{
    private const int IconTextureSize = 128;
    private const float AccountProgressPanelWidth = 430f;
    private const float AccountProgressPanelHeight = 68f;

    [Header("UI References")]
    public TMP_Text coinText;
    public GameObject settingsDropdown;

    [Header("Coin Settings")]
    public int currentCoins = 250;

    private static Sprite cogSprite;
    private static Sprite coinSprite;
    private RectTransform accountProgressPanel;
    private TextMeshProUGUI accountNameLabel;
    private TextMeshProUGUI accountLevelLabel;
    private TextMeshProUGUI accountProgressLabel;
    private RectTransform accountProgressFillRect;

    private void Awake()
    {
        SaveManager.EnsureLoaded();
        currentCoins = SaveManager.Data.coins;
        EnsureDropdownReference();
    }

    private void Start()
    {
        StyleTopBarImages();
        EnsureAccountProgressPanel();
        UpdateCoinDisplay();
        RefreshAccountProgressDisplay();
        EnsureNightModeTestButton();

        if (settingsDropdown != null)
        {
            settingsDropdown.SetActive(false);
        }
    }

    public void ToggleSettingsDropdown()
    {
        EnsureDropdownReference();
        EnsureNightModeTestButton();

        if (settingsDropdown == null)
        {
            Debug.LogWarning("SettingsDropdown reference is missing on " + gameObject.name);
            return;
        }

        bool shouldShow = !settingsDropdown.activeSelf;
        settingsDropdown.SetActive(shouldShow);

        if (shouldShow)
        {
            settingsDropdown.transform.SetAsLastSibling();
            transform.SetAsLastSibling();
        }

        Debug.Log("Settings dropdown " + (shouldShow ? "opened" : "closed"));
    }

    public void ToggleNightModeForTesting()
    {
        SanctuaryUI sanctuaryUI = FindAnyObjectByType<SanctuaryUI>();
        if (sanctuaryUI == null)
        {
            Debug.LogWarning("Night mode test is only available in SanctuaryScene.");
            if (settingsDropdown != null)
            {
                settingsDropdown.SetActive(false);
            }

            return;
        }

        sanctuaryUI.ToggleNightModeForTesting();
        if (settingsDropdown != null)
        {
            settingsDropdown.SetActive(false);
        }
    }

    public void GoToSettings()
    {
        SceneManager.LoadScene("SettingsScene", LoadSceneMode.Single);
    }

    public void GoToShop()
    {
        if (SceneManager.GetActiveScene().name == "SanctuaryScene")
        {
            SanctuaryUI sanctuaryUI = FindAnyObjectByType<SanctuaryUI>();
            if (sanctuaryUI != null)
            {
                sanctuaryUI.OnShopClicked();
                return;
            }
        }

        SceneManager.LoadScene("ShopScene", LoadSceneMode.Single);
    }

    public void Logout()
    {
        SceneManager.LoadScene("LoginScene", LoadSceneMode.Single);
    }
    public void AddCoins(int amount)
    {
        currentCoins += amount;
        SaveManager.Data.coins = currentCoins;
        SaveManager.Save();
        UpdateCoinDisplay();
    }

    public void SetCoins(int amount)
    {
        currentCoins = amount;
        SaveManager.Data.coins = currentCoins;
        SaveManager.Save();
        UpdateCoinDisplay();
    }

    public void RefreshCoinDisplay()
    {
        UpdateCoinDisplay();
        RefreshAccountProgressDisplay();
    }

    private void UpdateCoinDisplay()
    {
        SaveManager.EnsureLoaded();
        currentCoins = SaveManager.Data.coins;

        if (coinText != null)
        {
            coinText.text = currentCoins.ToString();
        }
    }

    private void StyleTopBarImages()
    {
        EnsureIconSprites();
        StyleSettingsCog();
        StyleCoinDisplay();
    }

    private void EnsureAccountProgressPanel()
    {
        if (accountProgressPanel != null && accountNameLabel != null && accountLevelLabel != null && accountProgressLabel != null && accountProgressFillRect != null)
        {
            return;
        }

        RectTransform topBarRect = transform as RectTransform;
        if (topBarRect == null)
        {
            return;
        }

        Transform existingPanel = transform.Find("AccountProgressPanel");
        GameObject panelObject = existingPanel != null
            ? existingPanel.gameObject
            : new GameObject("AccountProgressPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panelObject.transform.SetParent(transform, false);
        panelObject.transform.SetAsFirstSibling();

        accountProgressPanel = panelObject.GetComponent<RectTransform>();
        accountProgressPanel.anchorMin = new Vector2(0.5f, 0.5f);
        accountProgressPanel.anchorMax = new Vector2(0.5f, 0.5f);
        accountProgressPanel.pivot = new Vector2(0.5f, 0.5f);
        accountProgressPanel.sizeDelta = new Vector2(AccountProgressPanelWidth, AccountProgressPanelHeight);
        accountProgressPanel.anchoredPosition = new Vector2(0f, -2f);

        Image panelImage = panelObject.GetComponent<Image>();
        panelImage.color = new Color(0.9f, 0.86f, 1f, 0.5f);
        panelImage.raycastTarget = false;

        accountNameLabel = EnsureProgressText(panelObject.transform, "AccountNameLabel", new Vector2(0f, 0.52f), new Vector2(0.75f, 1f), new Vector2(10f, 0f), new Vector2(-6f, -4f), TextAlignmentOptions.Left, 20f, FontStyles.Bold, new Color(0.11f, 0.16f, 0.25f, 1f));
        accountLevelLabel = EnsureProgressText(panelObject.transform, "AccountLevelLabel", new Vector2(0.75f, 0.52f), new Vector2(1f, 1f), new Vector2(6f, 0f), new Vector2(-10f, -4f), TextAlignmentOptions.Right, 20f, FontStyles.Bold, new Color(0.11f, 0.16f, 0.25f, 1f));

        Transform existingTrack = panelObject.transform.Find("AccountProgressTrack");
        GameObject trackObject = existingTrack != null
            ? existingTrack.gameObject
            : new GameObject("AccountProgressTrack", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        trackObject.transform.SetParent(panelObject.transform, false);

        RectTransform trackRect = trackObject.GetComponent<RectTransform>();
        trackRect.anchorMin = new Vector2(0f, 0f);
        trackRect.anchorMax = new Vector2(1f, 0f);
        trackRect.offsetMin = new Vector2(10f, 8f);
        trackRect.offsetMax = new Vector2(-10f, 30f);

        Image trackImage = trackObject.GetComponent<Image>();
        trackImage.color = new Color(1f, 1f, 1f, 0.76f);
        trackImage.raycastTarget = false;

        Transform existingFill = trackObject.transform.Find("Fill");
        GameObject fillObject = existingFill != null
            ? existingFill.gameObject
            : new GameObject("Fill", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        fillObject.transform.SetParent(trackObject.transform, false);

        accountProgressFillRect = fillObject.GetComponent<RectTransform>();
        accountProgressFillRect.anchorMin = new Vector2(0f, 0f);
        accountProgressFillRect.anchorMax = new Vector2(0f, 1f);
        accountProgressFillRect.offsetMin = new Vector2(3f, 3f);
        accountProgressFillRect.offsetMax = new Vector2(-3f, -3f);

        Image fillImage = fillObject.GetComponent<Image>();
        fillImage.color = new Color(0.53f, 0.75f, 1f, 1f);
        fillImage.raycastTarget = false;

        accountProgressLabel = EnsureProgressText(trackObject.transform, "AccountProgressLabel", Vector2.zero, Vector2.one, new Vector2(0f, 0f), new Vector2(0f, 0f), TextAlignmentOptions.Center, 16f, FontStyles.Bold, new Color(0.09f, 0.18f, 0.22f, 1f));
    }

    private static TextMeshProUGUI EnsureProgressText(Transform parent, string objectName, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax, TextAlignmentOptions alignment, float size, FontStyles style, Color color)
    {
        Transform existing = parent.Find(objectName);
        GameObject textObject = existing != null ? existing.gameObject : new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(parent, false);

        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = anchorMin;
        textRect.anchorMax = anchorMax;
        textRect.offsetMin = offsetMin;
        textRect.offsetMax = offsetMax;

        TextMeshProUGUI label = textObject.GetComponent<TextMeshProUGUI>();
        label.alignment = alignment;
        label.fontSize = size;
        label.fontStyle = style;
        label.color = color;
        label.raycastTarget = false;
        label.textWrappingMode = TextWrappingModes.NoWrap;
        return label;
    }

    private void RefreshAccountProgressDisplay()
    {
        SaveManager.EnsureLoaded();
        EnsureAccountProgressPanel();

        if (accountProgressPanel == null || accountNameLabel == null || accountLevelLabel == null || accountProgressLabel == null || accountProgressFillRect == null)
        {
            return;
        }

        int accountLevel = SaveManager.GetAccountLevel();
        int accountXp = SaveManager.GetAccountXp();
        int requiredXp = LevelingSystem.GetXpForNextLevel(accountLevel);
        float progress = accountLevel >= LevelingSystem.MaxLevel ? 1f : Mathf.Clamp01(accountXp / (float)requiredXp);

        accountNameLabel.text = SaveManager.Data.username;
        accountLevelLabel.text = "Lv " + accountLevel;
        accountProgressLabel.text = accountLevel >= LevelingSystem.MaxLevel ? "MAX" : accountXp + " / " + requiredXp;

        accountProgressFillRect.anchorMax = new Vector2(progress, 1f);
    }

    private void StyleSettingsCog()
    {
        TMP_Text[] labels = GetComponentsInChildren<TMP_Text>(true);
        foreach (TMP_Text label in labels)
        {
            if (label.text != "Cog")
            {
                continue;
            }

            RectTransform parentTransform = label.transform.parent as RectTransform;
            if (parentTransform == null)
            {
                continue;
            }

            label.gameObject.SetActive(false);

            Transform existingIcon = parentTransform.Find("SettingsCogIcon");
            GameObject iconObject = existingIcon != null ? existingIcon.gameObject : new GameObject("SettingsCogIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            iconObject.transform.SetParent(parentTransform, false);
            iconObject.transform.SetAsLastSibling();

            RectTransform iconTransform = iconObject.GetComponent<RectTransform>();
            iconTransform.anchorMin = new Vector2(0.5f, 0.5f);
            iconTransform.anchorMax = new Vector2(0.5f, 0.5f);
            iconTransform.pivot = new Vector2(0.5f, 0.5f);
            iconTransform.sizeDelta = new Vector2(126f, 126f);
            iconTransform.anchoredPosition = Vector2.zero;

            Image buttonImage = parentTransform.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.color = new Color(1f, 1f, 1f, 0.001f);
            }

            Image iconImage = iconObject.GetComponent<Image>();
            iconImage.sprite = cogSprite;
            iconImage.color = Color.white;
            iconImage.preserveAspect = true;
            iconImage.raycastTarget = false;
            return;
        }
    }

    private void StyleCoinDisplay()
    {
        if (coinText == null)
        {
            return;
        }

        RectTransform coinDisplayTransform = coinText.transform.parent as RectTransform;
        if (coinDisplayTransform == null)
        {
            return;
        }

        Image coinButtonImage = coinDisplayTransform.GetComponent<Image>();
        if (coinButtonImage != null)
        {
            coinButtonImage.sprite = coinSprite;
            coinButtonImage.color = Color.white;
            coinButtonImage.preserveAspect = true;
        }

        RectTransform coinTextTransform = coinText.GetComponent<RectTransform>();
        if (coinTextTransform != null)
        {
            coinTextTransform.anchorMin = new Vector2(0.5f, 0.5f);
            coinTextTransform.anchorMax = new Vector2(0.5f, 0.5f);
            coinTextTransform.pivot = new Vector2(0f, 0.5f);
            coinTextTransform.sizeDelta = new Vector2(130f, 44f);
            coinTextTransform.anchoredPosition = new Vector2(74f, -1f);
        }

        coinText.alignment = TextAlignmentOptions.Left;
        coinText.fontStyle = FontStyles.Bold;
        coinText.fontSize = 28f;
        coinText.color = new Color(0.18f, 0.12f, 0.03f, 1f);
        coinText.raycastTarget = false;
    }

    private static void EnsureIconSprites()
    {
        if (cogSprite == null)
        {
            cogSprite = Sprite.Create(CreateCogTexture(), new Rect(0f, 0f, IconTextureSize, IconTextureSize), new Vector2(0.5f, 0.5f), 100f);
        }

        if (coinSprite == null)
        {
            coinSprite = Sprite.Create(CreateCoinTexture(), new Rect(0f, 0f, IconTextureSize, IconTextureSize), new Vector2(0.5f, 0.5f), 100f);
        }
    }

    private static Texture2D CreateCogTexture()
    {
        Texture2D texture = CreateTransparentTexture(IconTextureSize, IconTextureSize);
        Color outline = new Color(0.08f, 0.18f, 0.22f, 1f);
        Color metal = new Color(0.78f, 0.88f, 0.9f, 1f);
        Color highlight = new Color(1f, 1f, 1f, 0.52f);

        for (int tooth = 0; tooth < 8; tooth++)
        {
            float angle = tooth * Mathf.PI * 0.25f;
            int x = Mathf.RoundToInt(64f + Mathf.Cos(angle) * 37f) - 8;
            int y = Mathf.RoundToInt(64f + Mathf.Sin(angle) * 37f) - 8;
            DrawRoundedRect(texture, x - 2, y - 2, 20, 20, 5, outline);
            DrawRoundedRect(texture, x, y, 16, 16, 4, metal);
        }

        DrawEllipse(texture, 64, 64, 43, 43, outline);
        DrawEllipse(texture, 64, 64, 35, 35, metal);
        DrawEllipse(texture, 64, 64, 18, 18, outline);
        DrawEllipse(texture, 64, 64, 11, 11, Color.clear);
        DrawEllipse(texture, 52, 80, 12, 6, highlight);
        texture.Apply();
        return texture;
    }

    private static Texture2D CreateCoinTexture()
    {
        Texture2D texture = CreateTransparentTexture(IconTextureSize, IconTextureSize);
        Color shadow = new Color(0.62f, 0.39f, 0.04f, 1f);
        Color rim = new Color(0.96f, 0.68f, 0.09f, 1f);
        Color face = new Color(1f, 0.86f, 0.22f, 1f);
        Color shine = new Color(1f, 0.97f, 0.55f, 0.85f);
        Color mark = new Color(0.73f, 0.47f, 0.05f, 1f);

        DrawEllipse(texture, 64, 59, 49, 49, shadow);
        DrawEllipse(texture, 64, 65, 49, 49, rim);
        DrawEllipse(texture, 64, 65, 39, 39, face);
        DrawEllipse(texture, 48, 82, 13, 7, shine);
        DrawRoundedRect(texture, 59, 36, 10, 54, 5, mark);
        DrawRoundedRect(texture, 48, 48, 32, 8, 4, mark);
        DrawRoundedRect(texture, 48, 74, 32, 8, 4, mark);
        texture.Apply();
        return texture;
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
        for (int pixelX = x; pixelX < x + width; pixelX++)
        {
            for (int pixelY = y; pixelY < y + height; pixelY++)
            {
                if (pixelX < 0 || pixelY < 0 || pixelX >= texture.width || pixelY >= texture.height)
                {
                    continue;
                }

                int cornerX = pixelX < x + radius ? x + radius : pixelX >= x + width - radius ? x + width - radius - 1 : pixelX;
                int cornerY = pixelY < y + radius ? y + radius : pixelY >= y + height - radius ? y + height - radius - 1 : pixelY;
                float distance = Vector2.Distance(new Vector2(pixelX, pixelY), new Vector2(cornerX, cornerY));
                if (distance <= radius)
                {
                    texture.SetPixel(pixelX, pixelY, color);
                }
            }
        }
    }

    private static void DrawEllipse(Texture2D texture, int centerX, int centerY, int radiusX, int radiusY, Color color)
    {
        for (int x = centerX - radiusX; x <= centerX + radiusX; x++)
        {
            for (int y = centerY - radiusY; y <= centerY + radiusY; y++)
            {
                if (x < 0 || y < 0 || x >= texture.width || y >= texture.height)
                {
                    continue;
                }

                float normalizedX = (x - centerX) / (float)radiusX;
                float normalizedY = (y - centerY) / (float)radiusY;
                if ((normalizedX * normalizedX) + (normalizedY * normalizedY) <= 1f)
                {
                    texture.SetPixel(x, y, color);
                }
            }
        }
    }

    private void EnsureDropdownReference()
    {
        if (settingsDropdown != null)
        {
            return;
        }

        Transform dropdownTransform = FindChildByName(transform, "SettingsDropdown");
        if (dropdownTransform != null)
        {
            settingsDropdown = dropdownTransform.gameObject;
        }
    }

    private void EnsureNightModeTestButton()
    {
        EnsureDropdownReference();
        if (settingsDropdown == null)
        {
            return;
        }

        Transform existingButtonTransform = settingsDropdown.transform.Find("TestNightModeButton");
        if (existingButtonTransform != null)
        {
            WireNightModeTestButton(existingButtonTransform.GetComponent<Button>());
            return;
        }

        GameObject buttonObject = new GameObject("TestNightModeButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(LayoutElement));
        buttonObject.transform.SetParent(settingsDropdown.transform, false);
        buttonObject.transform.SetAsLastSibling();

        RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0f, 0.5f);
        buttonRect.anchorMax = new Vector2(1f, 0.5f);
        buttonRect.pivot = new Vector2(0.5f, 0.5f);
        buttonRect.sizeDelta = new Vector2(0f, 46f);

        LayoutElement layoutElement = buttonObject.GetComponent<LayoutElement>();
        layoutElement.preferredHeight = 46f;
        layoutElement.minHeight = 42f;
        layoutElement.flexibleHeight = 0f;

        Image buttonImage = buttonObject.GetComponent<Image>();
        buttonImage.color = new Color(0.58f, 0.67f, 0.92f, 1f);
        buttonImage.raycastTarget = true;

        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = buttonImage;
        WireNightModeTestButton(button);

        GameObject labelObject = new GameObject("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        labelObject.transform.SetParent(buttonObject.transform, false);

        RectTransform labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = new Vector2(8f, 4f);
        labelRect.offsetMax = new Vector2(-8f, -4f);

        TextMeshProUGUI label = labelObject.GetComponent<TextMeshProUGUI>();
        label.text = "Test Night Mode";
        label.alignment = TextAlignmentOptions.Center;
        label.fontSize = 24f;
        label.fontStyle = FontStyles.Bold;
        label.color = Color.white;
        label.raycastTarget = false;
    }

    private void WireNightModeTestButton(Button button)
    {
        if (button == null)
        {
            return;
        }

        button.onClick.RemoveListener(ToggleNightModeForTesting);
        button.onClick.AddListener(ToggleNightModeForTesting);
    }

    private Transform FindChildByName(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
            {
                return child;
            }

            Transform match = FindChildByName(child, childName);
            if (match != null)
            {
                return match;
            }
        }

        return null;
    }
}
