using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    private const string LoginTitleObjectName = "GameTitle";
    private const string LoginPanelObjectName = "LoginPanel";
    private const string LoginTitleLogoObjectName = "GameTitleLogo";
    private const string LoginTitleLogoResourcePath = "Sprites/UI/logo";
    private const float LogoWidthRelativeToPanel = 0.72f;
    private const float LogoMinWidth = 360f;
    private const float LogoMaxWidth = 640f;
    private const float LogoVerticalGap = 22f;
    private const float LogoTopPadding = 20f;

    [Header("UI References")]
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TMP_Text statusText;

    //[Header("Scene Settings")]
    //public string nextSceneName = "SanctuaryScene";

    private void Start()
    {
        if (statusText != null)
        {
            statusText.text = "Welcome back to Pocket Sanctuary";
        }
        ReplaceWelcomeTitleWithLogo();
    }

    private void ReplaceWelcomeTitleWithLogo()
    {
        GameObject titleObject = GameObject.Find(LoginTitleObjectName);
        if (titleObject == null)
        {
            return;
        }

        RectTransform titleRect = titleObject.GetComponent<RectTransform>();
        if (titleRect == null || titleRect.parent == null)
        {
            return;
        }

        Texture2D logoTexture = Resources.Load<Texture2D>(LoginTitleLogoResourcePath);
        if (logoTexture == null)
        {
            Debug.LogWarning("Login logo not found at Resources/" + LoginTitleLogoResourcePath);
            return;
        }

        TMP_Text titleText = titleObject.GetComponent<TMP_Text>();
        if (titleText != null)
        {
            titleText.enabled = false;
        }

        Transform existingLogo = titleRect.parent.Find(LoginTitleLogoObjectName);
        GameObject logoObject;
        if (existingLogo != null)
        {
            logoObject = existingLogo.gameObject;
        }
        else
        {
            logoObject = new GameObject(LoginTitleLogoObjectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            logoObject.transform.SetParent(titleRect.parent, false);
        }

        RectTransform logoRect = logoObject.GetComponent<RectTransform>();
        logoRect.anchorMin = new Vector2(0.5f, 0.5f);
        logoRect.anchorMax = new Vector2(0.5f, 0.5f);
        logoRect.pivot = new Vector2(0.5f, 0.5f);

        RectTransform panelRect = null;
        GameObject panelObject = GameObject.Find(LoginPanelObjectName);
        if (panelObject != null)
        {
            panelRect = panelObject.GetComponent<RectTransform>();
        }

        RectTransform parentRect = titleRect.parent as RectTransform;
        float aspect = logoTexture.height > 0 ? logoTexture.width / (float)logoTexture.height : 1f;
        float targetWidth = panelRect != null
            ? Mathf.Clamp(panelRect.rect.width * LogoWidthRelativeToPanel, LogoMinWidth, LogoMaxWidth)
            : 500f;
        float targetHeight = targetWidth / Mathf.Max(0.01f, aspect);
        logoRect.sizeDelta = new Vector2(targetWidth, targetHeight);

        float targetX = panelRect != null ? panelRect.anchoredPosition.x : titleRect.anchoredPosition.x;
        float targetY = panelRect != null
            ? panelRect.anchoredPosition.y + (panelRect.rect.height * 0.5f) + LogoVerticalGap + (targetHeight * 0.5f)
            : titleRect.anchoredPosition.y;

        if (parentRect != null)
        {
            float maxY = (parentRect.rect.height * 0.5f) - (targetHeight * 0.5f) - LogoTopPadding;
            targetY = Mathf.Min(targetY, maxY);
        }

        logoRect.anchoredPosition = new Vector2(targetX, targetY);

        Image logoImage = logoObject.GetComponent<Image>();
        logoImage.raycastTarget = false;
        logoImage.preserveAspect = true;
        logoImage.color = Color.white;

        Sprite logoSprite = Sprite.Create(
            logoTexture,
            new Rect(0f, 0f, logoTexture.width, logoTexture.height),
            new Vector2(0.5f, 0.5f),
            100f);
        logoImage.sprite = logoSprite;
    }

    public void OnLoginPressed() 
    {
        string username = usernameInput.text.Trim();
        string password = passwordInput.text.Trim();

        if (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(password))
        {
            statusText.text = "Please enter username and password";
            return;
        }

        if (string.IsNullOrEmpty(username))
        {
            statusText.text = "Please enter username";
            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            statusText.text = "Please enter password";
            return;
        }

        statusText.text = "Logging in...";
        SaveManager.SwitchToProfile(username);
        SceneManager.LoadScene("SanctuaryScene", LoadSceneMode.Single);
    }
}
