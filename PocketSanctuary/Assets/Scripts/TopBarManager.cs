using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TopBarManager : MonoBehaviour
{
    private const int IconTextureSize = 128;

    [Header("UI References")]
    public TMP_Text coinText;
    public GameObject settingsDropdown;

    [Header("Coin Settings")]
    public int currentCoins = 250;

    private static Sprite cogSprite;
    private static Sprite coinSprite;

    private void Awake()
    {
        SaveManager.EnsureLoaded();
        currentCoins = SaveManager.Data.coins;
        EnsureDropdownReference();
    }

    private void Start()
    {
        StyleTopBarImages();
        UpdateCoinDisplay();

        if (settingsDropdown != null)
        {
            settingsDropdown.SetActive(false);
        }
    }

    public void ToggleSettingsDropdown()
    {
        EnsureDropdownReference();

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
