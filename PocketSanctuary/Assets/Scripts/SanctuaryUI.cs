using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SanctuaryUI : MonoBehaviour
{
    private const string PocketPalButtonName = "PocketPalButton";
    private const string HabitatRootName = "Procedural3DHabitat";
    private const string HabitatViewName = "Habitat3DView";
    private const int PandaTextureSize = 256;
    private const int ButtonIconTextureSize = 128;
    private const int HabitatBackgroundWidth = 1080;
    private const int HabitatBackgroundHeight = 1920;
    private const string PandaBaseSpriteResourcePath = "Sprites/Accessories/panda_base_real";
    private const string WhiteTigerBaseSpriteResourcePath = "Sprites/Accessories/white_tiger_base_real";
    private const string OtterBaseSpriteResourcePath = "Sprites/Accessories/otter_base_real";
    private const string PandaNightSleepSpriteResourcePath = "Sprites/Sleeping/panda_sleep";
    private const string MoonlitHabitatId = "moonlit_pond";
    private const float HabitatPhaseDurationSeconds = 480f;
    private const int SleepSessionPocketPalXpReward = 30;
    private const int SleepSessionAccountXpReward = 10;
    private const float NightSleepSpriteGlobalYOffset = -120f;
    private const float DrawerTopClearance = 122f;
    private const float DrawerBottomClearance = 138f;
    private const float DrawerSideMargin = 10f;
    private const float DrawerScreenWidthRatio = 0.68f;
    private const float CustomizationDrawerTopPadding = 4f;
    private const float CustomizationDrawerBottomPadding = 6f;

    [SerializeField] private float pocketPalMoveSpeed = 120f;
    [SerializeField] private float pocketPalPauseTime = 0.8f;
    [SerializeField] private Vector2 pocketPalAreaMin = new Vector2(-390f, -520f);
    [SerializeField] private Vector2 pocketPalAreaMax = new Vector2(390f, -70f);

    private RectTransform pocketPalTransform;
    private RectTransform pocketPalVisualTransform;
    private RectTransform pocketPalSpriteTransform;
    private RectTransform pocketPalShadowTransform;
    private RectTransform pocketPalNightStillTransform;
    private Image pocketPalNightStillImage;
    private Transform playBallTransform;
    private Transform bambooMobileTransform;
    private Transform habitatPandaRoot;
    private Transform habitatPandaHead;
    private Transform habitatPandaBody;
    private Transform habitatPandaLeftPaw;
    private Transform habitatPandaRightPaw;
    private Transform habitatActionBamboo;
    private Transform habitatActionSpark;
    private Transform habitatExplorerButterfly;
    private TMP_Text habitatStatusLabel;
    private TMP_Text sleepMeterLabel;
    private Vector2 pocketPalTargetPosition;
    private string pocketPalTargetMessage = "Panda is exploring the habitat.";
    private HabitatActivity habitatActivity = HabitatActivity.Exploring;
    private float pocketPalPauseTimer;
    private float pocketPalAnimationTimer;
    private float pocketPalFacingDirection = 1f;
    private float habitatPhaseTimer;
    private bool isNightHabitatActive;
    private static Sprite pandaSprite;
    private static Sprite tigerSprite;
    private static Sprite otterSprite;
    private static Sprite shadowSprite;
    private static Sprite pandaNightSleepSprite;
    private static readonly string[] EyewearAccessoryIds = new string[]
    {
        "eyewear_round_glasses",
        "eyewear_black_sunglasses",
        "eyewear_pixel_shades",
        "eyewear_heart_glasses",
        "eyewear_blue_round_glasses",
        "eyewear_star_glasses"
    };
    private static readonly string[] HeadwearAccessoryIds = new string[]
    {
        "headwear_blue_beanie",
        "headwear_chef_hat",
        "headwear_dino_hood",
        "headwear_crown",
        "headwear_straw_hat",
        "headwear_red_cap",
        "headwear_aviator_hat"
    };
    private static readonly string[] ShirtAccessoryIds = new string[]
    {
        "shirt_green_bamboo",
        "shirt_blue_paw",
        "shirt_yellow_panda",
        "shirt_blue_hawaiian",
        "shirt_black_heart",
        "shirt_red_hoodie"
    };
    private static readonly string[] HandheldAccessoryIds = new string[]
    {
        "prop_blue_paw_flag",
        "prop_rainbow_flag",
        "prop_red_heart_flag",
        "prop_checkered_flag",
        "prop_green_leaf_flag",
        "prop_pirate_flag",
        "prop_panda_flag"
    };
    private static readonly System.Collections.Generic.Dictionary<string, string> AccessorySpriteResourceNames = new System.Collections.Generic.Dictionary<string, string>
    {
        { "eyewear_round_glasses", "eyewear_round_glasses" },
        { "eyewear_black_sunglasses", "eyewear_black_sunglasses" },
        { "eyewear_pixel_shades", "eyewear_pixel_shades" },
        { "eyewear_heart_glasses", "eyewear_heart_glasses" },
        { "eyewear_blue_round_glasses", "eyewear_blue_round_glasses" },
        { "eyewear_star_glasses", "eyewear_star_glasses" },
        { "headwear_blue_beanie", "headwear_blue_beanie" },
        { "headwear_chef_hat", "headwear_chef_hat" },
        { "headwear_dino_hood", "headwear_dino_hood" },
        { "headwear_crown", "headwear_crown" },
        { "headwear_straw_hat", "headwear_straw_hat" },
        { "headwear_red_cap", "headwear_red_cap" },
        { "headwear_aviator_hat", "headwear_aviator_hat" },
        { "shirt_green_bamboo", "shirt_green_bamboo" },
        { "shirt_blue_paw", "shirt_blue_paw" },
        { "shirt_yellow_panda", "shirt_yellow_panda" },
        { "shirt_blue_hawaiian", "shirt_blue_hawaiian" },
        { "shirt_black_heart", "shirt_black_heart" },
        { "shirt_red_hoodie", "shirt_red_hoodie" },
        { "prop_blue_paw_flag", "prop_blue_paw_flag" },
        { "prop_rainbow_flag", "prop_rainbow_flag" },
        { "prop_red_heart_flag", "prop_red_heart_flag" },
        { "prop_checkered_flag", "prop_checkered_flag" },
        { "prop_green_leaf_flag", "prop_green_leaf_flag" },
        { "prop_pirate_flag", "prop_pirate_flag" },
        { "prop_panda_flag", "prop_panda_flag" }
    };
    private static readonly System.Collections.Generic.Dictionary<string, Sprite> habitatBackgroundSprites = new System.Collections.Generic.Dictionary<string, Sprite>();
    private static readonly System.Collections.Generic.Dictionary<string, Sprite> accessorySpriteCache = new System.Collections.Generic.Dictionary<string, Sprite>();
    private static Sprite customizationButtonIcon;
    private static Sprite rewardsButtonIcon;
    private const int PandaSpriteVersion = 2;
    private static int pandaSpriteBuildVersion = -1;
    private const int WhiteTigerSpriteVersion = 5;
    private static int whiteTigerSpriteBuildVersion = -1;
    private RectTransform drawerRoot;
    private RectTransform activeDrawer;
    private RectTransform activeDrawerContent;
    private CustomizationSceneManager drawerContentBuilder;
    private PanelKind openPanel = PanelKind.None;
    private PanelKind targetPanel = PanelKind.None;
    private float drawerAnimationTarget;

    private enum HabitatActivity
    {
        Exploring,
        NibblingBamboo,
        PlayingInPen,
        BattingBall,
        SleepingInHammock
    }

    private enum PanelKind
    {
        None,
        Customization,
        TasksRewards,
        PocketPal,
        Shop
    }

    private void Start()
    {
        SaveManager.EnsureLoaded();
        habitatPhaseTimer = 0f;
        isNightHabitatActive = false;
        BuildSanctuaryHome();
        DisableTextRaycasts();
        CreatePocketPalButton();
        ChooseNewPocketPalTarget();
        ApplyNighttimePresentation(true);
    }

    private void Update()
    {
        UpdateDayNightCycle();
        if (!isNightHabitatActive)
        {
            MovePocketPal();
        }
        AnimateHabitatProps();
        AnimateDrawer();
        UpdateSleepMeterUI();
    }

    public void OnCustomizationClicked()
    {
        TogglePanel(PanelKind.Customization);
    }
    public void OnTasksRewardsClicked()
    {
        TogglePanel(PanelKind.TasksRewards);
    }
    public void OnShopClicked()
    {
        TogglePanel(PanelKind.Shop);
    }
    public void OnAnimalClicked()
    {
        TogglePanel(PanelKind.PocketPal);
    }

    public void ToggleNightModeForTesting()
    {
        habitatPhaseTimer = 0f;
        isNightHabitatActive = !isNightHabitatActive;
        OnHabitatPhaseChanged();
    }

    public void RefreshOpenPanel()
    {
        if (openPanel == PanelKind.None)
        {
            return;
        }

        bool preserveDrawerScroll = openPanel == PanelKind.Customization || openPanel == PanelKind.TasksRewards;
        float preservedVerticalScroll = preserveDrawerScroll ? GetActiveDrawerVerticalScrollPosition() : 1f;
        RefreshHabitatBackground();
        BuildDrawerContent(openPanel, preserveDrawerScroll, preservedVerticalScroll);
        RefreshTopBar();
        RefreshPocketPalVisual();
        ApplyNighttimePresentation(true);
        PositionPocketPalPopup();
    }

    private void BuildSanctuaryHome()
    {
        Transform background = FindCanvasChild("Background");
        if (background == null)
        {
            return;
        }

        ApplyHabitatBackground(background);
        Remove3DHabitatView(background);
        EnsureHomeLabels(background);
        EnsureSleepMeterLabel(background);
        StyleBottomNavigationButtons();
    }

    private void RefreshHabitatBackground()
    {
        Transform background = FindCanvasChild("Background");
        if (background == null)
        {
            return;
        }

        ApplyHabitatBackground(background);
    }

    private void ApplyHabitatBackground(Transform background)
    {
        Image backgroundImage = background.GetComponent<Image>();
        if (backgroundImage != null)
        {
            backgroundImage.sprite = GetHabitatBackgroundSprite(GetActiveHabitatId());
            backgroundImage.color = Color.white;
            backgroundImage.type = Image.Type.Simple;
            backgroundImage.preserveAspect = false;
            backgroundImage.raycastTarget = false;
        }
    }

    private string GetDaytimeHabitatId()
    {
        string selectedHabitatId = SaveManager.Data != null ? SaveManager.Data.selectedHabitatId : "sunny_garden";
        if (string.IsNullOrEmpty(selectedHabitatId) || selectedHabitatId == MoonlitHabitatId)
        {
            return "sunny_garden";
        }

        return selectedHabitatId;
    }

    private string GetActiveHabitatId()
    {
        return isNightHabitatActive ? MoonlitHabitatId : GetDaytimeHabitatId();
    }

    private void UpdateDayNightCycle()
    {
        if (isNightHabitatActive)
        {
            ProcessNightSleep(Time.deltaTime);
        }

        habitatPhaseTimer += Time.deltaTime;
        if (habitatPhaseTimer < HabitatPhaseDurationSeconds)
        {
            return;
        }

        habitatPhaseTimer = 0f;
        isNightHabitatActive = !isNightHabitatActive;
        OnHabitatPhaseChanged();
    }

    private void OnHabitatPhaseChanged()
    {
        RefreshHabitatBackground();
        ApplyNighttimePresentation(true);
        UpdateSleepMeterUI();
        if (isNightHabitatActive)
        {
            pocketPalTargetPosition = new Vector2(0f, -210f);
            pocketPalTargetMessage = GetPocketPalDisplayName() + " is sleeping.";
            SetHabitatActivity(HabitatActivity.SleepingInHammock);
            pocketPalPauseTimer = pocketPalPauseTime;
            return;
        }

        pocketPalPauseTimer = 0f;
        ChooseNewPocketPalTarget();
    }

    private void ProcessNightSleep(float deltaSeconds)
    {
        int completedSessions = SaveManager.AddSelectedPocketPalSleepTime(deltaSeconds, HabitatPhaseDurationSeconds);
        if (completedSessions <= 0)
        {
            return;
        }

        LevelingSystem.GrantPocketPalXp(SleepSessionPocketPalXpReward * completedSessions);
        LevelingSystem.GrantAccountXp(SleepSessionAccountXpReward * completedSessions);
        SaveManager.Save();

        if (openPanel == PanelKind.TasksRewards || openPanel == PanelKind.PocketPal)
        {
            RefreshOpenPanel();
        }
    }

    private void ApplyNighttimePresentation(bool refreshStillSprite)
    {
        if (pocketPalTransform == null)
        {
            return;
        }

        if (isNightHabitatActive)
        {
            // During night mode, keep the sleeping still in the center of the screen.
            pocketPalTransform.anchoredPosition = Vector2.zero;
        }

        EnsureNighttimeStillLayer();
        if (refreshStillSprite && pocketPalNightStillImage != null)
        {
            Sprite stillSprite = GetNighttimeStillSprite();
            pocketPalNightStillImage.sprite = stillSprite;
            pocketPalNightStillImage.enabled = stillSprite != null;
            ConfigureNighttimeStillLayout(stillSprite);
        }

        Transform visualTransform = pocketPalTransform.Find("PandaVisual");
        Transform shadowTransform = pocketPalTransform.Find("PocketPalShadow");
        if (visualTransform != null)
        {
            visualTransform.gameObject.SetActive(!isNightHabitatActive);
        }

        if (shadowTransform != null)
        {
            shadowTransform.gameObject.SetActive(!isNightHabitatActive);
        }

        if (pocketPalNightStillTransform != null)
        {
            bool canShowStill = isNightHabitatActive && pocketPalNightStillImage != null && pocketPalNightStillImage.sprite != null;
            pocketPalNightStillTransform.gameObject.SetActive(canShowStill);
        }
    }

    private void EnsureNighttimeStillLayer()
    {
        if (pocketPalTransform == null)
        {
            return;
        }

        if (pocketPalNightStillTransform != null && pocketPalNightStillImage != null)
        {
            return;
        }

        Transform existingStill = pocketPalTransform.Find("NighttimeStill");
        if (existingStill != null)
        {
            pocketPalNightStillTransform = existingStill.GetComponent<RectTransform>();
            pocketPalNightStillImage = existingStill.GetComponent<Image>();
            return;
        }

        GameObject stillObject = new GameObject("NighttimeStill", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        stillObject.transform.SetParent(pocketPalTransform, false);
        stillObject.transform.SetAsLastSibling();

        pocketPalNightStillTransform = stillObject.GetComponent<RectTransform>();
        pocketPalNightStillTransform.anchorMin = new Vector2(0.5f, 0.5f);
        pocketPalNightStillTransform.anchorMax = new Vector2(0.5f, 0.5f);
        pocketPalNightStillTransform.pivot = new Vector2(0.5f, 0.5f);
        pocketPalNightStillTransform.sizeDelta = new Vector2(540f, 540f);
        pocketPalNightStillTransform.anchoredPosition = new Vector2(0f, -8f);

        pocketPalNightStillImage = stillObject.GetComponent<Image>();
        pocketPalNightStillImage.preserveAspect = true;
        pocketPalNightStillImage.raycastTarget = false;
        stillObject.SetActive(false);
    }

    private Sprite GetNighttimeStillSprite()
    {
        if (SaveManager.Data == null)
        {
            return pandaNightSleepSprite;
        }

        if (SaveManager.Data.selectedPocketPalId == "panda")
        {
            if (pandaNightSleepSprite == null)
            {
                pandaNightSleepSprite = TryCreateSpriteFromResource(PandaNightSleepSpriteResourcePath);
            }

            if (pandaNightSleepSprite != null)
            {
                return pandaNightSleepSprite;
            }
        }

        return GetSelectedPocketPalSprite();
    }

    private void ConfigureNighttimeStillLayout(Sprite stillSprite)
    {
        if (pocketPalNightStillTransform == null)
        {
            return;
        }

        bool isPandaSleepingSprite = SaveManager.Data != null && SaveManager.Data.selectedPocketPalId == "panda" && stillSprite == pandaNightSleepSprite;
        if (isPandaSleepingSprite)
        {
            pocketPalNightStillTransform.sizeDelta = new Vector2(300f, 300f);
            pocketPalNightStillTransform.anchoredPosition = new Vector2(0f, NightSleepSpriteGlobalYOffset);
            return;
        }

        pocketPalNightStillTransform.sizeDelta = new Vector2(230f, 230f);
        pocketPalNightStillTransform.anchoredPosition = new Vector2(0f, NightSleepSpriteGlobalYOffset);
    }

    private void EnsureHomeLabels(Transform background)
    {
        Transform labelParent = background.parent != null ? background.parent : background;
        Transform homeTransform = labelParent.Find("HomeLabel");
        if (homeTransform == null)
        {
            homeTransform = background.Find("HomeLabel");
        }

        if (homeTransform != null && homeTransform.parent != labelParent)
        {
            homeTransform.SetParent(labelParent, false);
        }

        if (homeTransform != null)
        {
            homeTransform.gameObject.SetActive(false);
        }

        Transform statusTransform = labelParent.Find("HabitatStatusLabel");
        if (statusTransform == null)
        {
            statusTransform = background.Find("HabitatStatusLabel");
        }

        if (statusTransform == null)
        {
            habitatStatusLabel = CreateHomeLabel("HabitatStatusLabel", labelParent, string.Empty, new Vector2(0.5f, 0.61f), 26);
        }
        else
        {
            if (statusTransform.parent != labelParent)
            {
                statusTransform.SetParent(labelParent, false);
            }

            habitatStatusLabel = statusTransform.GetComponent<TMP_Text>();
        }

        if (habitatStatusLabel != null)
        {
            habitatStatusLabel.text = string.Empty;
            habitatStatusLabel.color = new Color(0.1f, 0.25f, 0.18f, 1f);
            habitatStatusLabel.gameObject.SetActive(false);
        }
    }

    private void EnsureSleepMeterLabel(Transform background)
    {
        Transform labelParent = background.parent != null ? background.parent : background;
        Transform sleepTransform = labelParent.Find("SleepMeterLabel");
        if (sleepTransform == null)
        {
            sleepTransform = background.Find("SleepMeterLabel");
        }

        if (sleepTransform == null)
        {
            sleepMeterLabel = CreateHomeLabel("SleepMeterLabel", labelParent, string.Empty, new Vector2(0.5f, 0.86f), 20f);
        }
        else
        {
            if (sleepTransform.parent != labelParent)
            {
                sleepTransform.SetParent(labelParent, false);
            }

            sleepMeterLabel = sleepTransform.GetComponent<TMP_Text>();
        }

        if (sleepMeterLabel == null)
        {
            return;
        }

        RectTransform sleepRect = sleepMeterLabel.GetComponent<RectTransform>();
        sleepRect.anchorMin = new Vector2(0.5f, 0.86f);
        sleepRect.anchorMax = new Vector2(0.5f, 0.86f);
        sleepRect.pivot = new Vector2(0.5f, 0.5f);
        sleepRect.sizeDelta = new Vector2(420f, 54f);
        sleepRect.anchoredPosition = Vector2.zero;

        sleepMeterLabel.color = new Color(0.12f, 0.2f, 0.28f, 1f);
        sleepMeterLabel.fontSize = 20f;
        sleepMeterLabel.alignment = TextAlignmentOptions.Center;
        sleepMeterLabel.text = string.Empty;
        sleepMeterLabel.gameObject.SetActive(true);
    }

    private void UpdateSleepMeterUI()
    {
        if (sleepMeterLabel == null)
        {
            Transform background = FindCanvasChild("Background");
            if (background != null)
            {
                EnsureSleepMeterLabel(background);
            }
        }

        if (sleepMeterLabel == null)
        {
            return;
        }

        float sleepSeconds = SaveManager.GetSelectedPocketPalSleepSeconds();
        float sleepProgress = SaveManager.GetSelectedPocketPalSleepProgress01(HabitatPhaseDurationSeconds) * 100f;
        sleepMeterLabel.text = "Sleep " + FormatDurationMmSs(sleepSeconds) + " / " + FormatDurationMmSs(HabitatPhaseDurationSeconds) + " (" + Mathf.FloorToInt(sleepProgress) + "%)";
    }

    private static string FormatDurationMmSs(float totalSeconds)
    {
        int clampedSeconds = Mathf.Clamp(Mathf.FloorToInt(totalSeconds), 0, 5999);
        int minutes = clampedSeconds / 60;
        int seconds = clampedSeconds % 60;
        return minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    private void CreatePocketPalButton()
    {
        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            return;
        }

        Transform existingPal = canvas.transform.Find(PocketPalButtonName);
        if (existingPal != null)
        {
            pocketPalTransform = existingPal.GetComponent<RectTransform>();
            if (existingPal.Find("PandaVisual") == null)
            {
                CreatePocketPalSprite(existingPal);
            }
            else
            {
                RefreshPocketPalVisual();
            }

            Transform nameTransform = existingPal.Find("PocketPalName");
            if (nameTransform == null)
            {
                CreateHomeLabel("PocketPalName", existingPal, GetPocketPalDisplayName(), new Vector2(0.5f, -0.36f), 30);
            }
            else
            {
                TMP_Text nameLabel = nameTransform.GetComponent<TMP_Text>();
                if (nameLabel != null)
                {
                    nameLabel.text = GetPocketPalDisplayName();
                }
            }

            ApplyNighttimePresentation(true);

            return;
        }

        GameObject pocketPal = new GameObject(PocketPalButtonName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        pocketPal.transform.SetParent(canvas.transform, false);
        pocketPal.transform.SetAsLastSibling();

        pocketPalTransform = pocketPal.GetComponent<RectTransform>();
        pocketPalTransform.anchorMin = new Vector2(0.5f, 0.5f);
        pocketPalTransform.anchorMax = new Vector2(0.5f, 0.5f);
        pocketPalTransform.pivot = new Vector2(0.5f, 0.5f);
        pocketPalTransform.sizeDelta = new Vector2(330f, 360f);
        pocketPalTransform.anchoredPosition = Vector2.zero;

        Image image = pocketPal.GetComponent<Image>();
        image.color = new Color(1f, 1f, 1f, 0.01f);
        image.raycastTarget = true;
        image.type = Image.Type.Simple;

        Button button = pocketPal.GetComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(OnAnimalClicked);

        CreatePocketPalSprite(pocketPal.transform);
        CreateHomeLabel("PocketPalName", pocketPal.transform, GetPocketPalDisplayName(), new Vector2(0.5f, -0.36f), 30);
    }

    private void DisableTextRaycasts()
    {
        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            return;
        }

        TMP_Text[] labels = canvas.GetComponentsInChildren<TMP_Text>(true);
        foreach (TMP_Text label in labels)
        {
            label.raycastTarget = false;
        }
    }

    private void MovePocketPal()
    {
        if (pocketPalTransform == null)
        {
            return;
        }

        // Keep movement constrained to the ground region of the habitat.
        pocketPalTargetPosition = ClampPocketPalToGroundBounds(pocketPalTargetPosition);
        pocketPalTransform.anchoredPosition = ClampPocketPalToGroundBounds(pocketPalTransform.anchoredPosition);

        if (pocketPalPauseTimer > 0f)
        {
            pocketPalPauseTimer -= Time.deltaTime;
            UpdatePocketPalAnimation(false);
            if (pocketPalPauseTimer <= 0f)
            {
                ChooseNewPocketPalTarget();
            }

            return;
        }

        Vector2 positionBeforeMove = pocketPalTransform.anchoredPosition;
        pocketPalTransform.anchoredPosition = Vector2.MoveTowards(
            pocketPalTransform.anchoredPosition,
            pocketPalTargetPosition,
            pocketPalMoveSpeed * Time.deltaTime);

        UpdateFacingDirection(positionBeforeMove, pocketPalTransform.anchoredPosition);
        UpdatePocketPalAnimation(true);

        if (Vector2.Distance(pocketPalTransform.anchoredPosition, pocketPalTargetPosition) <= 4f)
        {
            if (habitatStatusLabel != null)
            {
                habitatStatusLabel.text = string.Empty;
            }

            SetHabitatActivityFromMessage(pocketPalTargetMessage);
            pocketPalPauseTimer = pocketPalPauseTime;
        }
    }

    private Vector2 ClampPocketPalToGroundBounds(Vector2 position)
    {
        float minX = Mathf.Min(pocketPalAreaMin.x, pocketPalAreaMax.x);
        float maxX = Mathf.Max(pocketPalAreaMin.x, pocketPalAreaMax.x);
        float minY = Mathf.Min(pocketPalAreaMin.y, pocketPalAreaMax.y);
        float maxY = Mathf.Max(pocketPalAreaMin.y, pocketPalAreaMax.y);

        // Never allow the pet to drift above the grass line (into sky/fence area).
        maxY = Mathf.Min(maxY, -70f);

        return new Vector2(
            Mathf.Clamp(position.x, minX, maxX),
            Mathf.Clamp(position.y, minY, maxY));
    }

    private void ChooseNewPocketPalTarget()
    {
        if (isNightHabitatActive)
        {
            pocketPalTargetPosition = new Vector2(0f, -210f);
            pocketPalTargetMessage = GetPocketPalDisplayName() + " is sleeping.";
            SetHabitatActivity(HabitatActivity.SleepingInHammock);
            return;
        }

        if (Random.value < 0.62f)
        {
            int targetIndex = Random.Range(0, 4);
            if (targetIndex == 0)
            {
                pocketPalTargetPosition = new Vector2(-260f, -70f);
                pocketPalTargetMessage = "Panda is nibbling bamboo.";
                SetHabitatActivity(HabitatActivity.NibblingBamboo);
                return;
            }

            if (targetIndex == 1)
            {
                pocketPalTargetPosition = new Vector2(0f, -120f);
                pocketPalTargetMessage = "Panda is playing in the pen.";
                SetHabitatActivity(HabitatActivity.PlayingInPen);
                return;
            }

            if (targetIndex == 2)
            {
                pocketPalTargetPosition = new Vector2(225f, -310f);
                pocketPalTargetMessage = "Panda is batting the toy ball.";
                SetHabitatActivity(HabitatActivity.BattingBall);
                return;
            }

            pocketPalTargetPosition = new Vector2(0f, -120f);
            pocketPalTargetMessage = "Panda is exploring the habitat.";
            SetHabitatActivity(HabitatActivity.Exploring);
            return;
        }

        if (Random.value < 0.28f)
        {
            pocketPalTargetPosition = new Vector2(225f, -310f);
            pocketPalTargetMessage = "Panda is batting the toy ball.";
            SetHabitatActivity(HabitatActivity.BattingBall);
            return;
        }

        pocketPalTargetPosition = new Vector2(
            Random.Range(pocketPalAreaMin.x, pocketPalAreaMax.x),
            Random.Range(pocketPalAreaMin.y, pocketPalAreaMax.y));
        pocketPalTargetMessage = "Panda is exploring the habitat.";
        SetHabitatActivity(HabitatActivity.Exploring);
    }

    private void TogglePanel(PanelKind panelKind)
    {
        EnsureDrawerRoot();

        if (openPanel == panelKind)
        {
            ClosePanel();
            return;
        }

        openPanel = panelKind;
        targetPanel = panelKind;
        BuildDrawerContent(panelKind);

        if (activeDrawer != null)
        {
            activeDrawer.gameObject.SetActive(true);
            activeDrawer.SetAsLastSibling();
        }

        drawerAnimationTarget = 1f;
    }

    private void ClosePanel()
    {
        openPanel = PanelKind.None;
        drawerAnimationTarget = 0f;
    }

    private void EnsureDrawerRoot()
    {
        if (drawerRoot != null)
        {
            return;
        }

        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            return;
        }

        GameObject rootObject = new GameObject("SanctuaryDrawerRoot", typeof(RectTransform));
        rootObject.transform.SetParent(canvas.transform, false);
        rootObject.transform.SetAsLastSibling();

        drawerRoot = rootObject.GetComponent<RectTransform>();
        drawerRoot.anchorMin = Vector2.zero;
        drawerRoot.anchorMax = Vector2.one;
        drawerRoot.offsetMin = Vector2.zero;
        drawerRoot.offsetMax = Vector2.zero;
    }

    private void CreateBackgroundCloseButton()
    {
        if (drawerRoot == null)
        {
            return;
        }

        GameObject closeCatcher = new GameObject("DrawerBackgroundClose", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        closeCatcher.transform.SetParent(drawerRoot, false);

        RectTransform closeTransform = closeCatcher.GetComponent<RectTransform>();
        closeTransform.anchorMin = Vector2.zero;
        closeTransform.anchorMax = Vector2.one;
        closeTransform.offsetMin = Vector2.zero;
        closeTransform.offsetMax = Vector2.zero;

        Image closeImage = closeCatcher.GetComponent<Image>();
        closeImage.color = new Color(1f, 1f, 1f, 0.001f);
        closeImage.raycastTarget = true;

        Button closeButton = closeCatcher.GetComponent<Button>();
        closeButton.targetGraphic = closeImage;
        closeButton.onClick.AddListener(ClosePanel);
    }

    private void BuildDrawerContent(PanelKind panelKind, bool preserveVerticalScroll = false, float preservedVerticalScroll = 1f)
    {
        EnsureDrawerRoot();

        if (drawerRoot == null)
        {
            return;
        }

        ClearDrawer();
        CreateBackgroundCloseButton();

        bool isShop = panelKind == PanelKind.Shop;
        bool isPocketPal = panelKind == PanelKind.PocketPal;
        GameObject panelObject = new GameObject(GetDrawerName(panelKind), typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panelObject.transform.SetParent(drawerRoot, false);
        panelObject.transform.SetAsLastSibling();

        activeDrawer = panelObject.GetComponent<RectTransform>();
        ConfigureDrawerTransform(activeDrawer, panelKind);
        activeDrawer.localScale = panelKind == PanelKind.Shop || panelKind == PanelKind.PocketPal ? new Vector3(1f, 0f, 1f) : Vector3.one;

        Image panelImage = panelObject.GetComponent<Image>();
        panelImage.color = isShop ? new Color(0.86f, 0.96f, 0.86f, 0.97f) : isPocketPal ? new Color(0.9f, 0.88f, 1f, 0.97f) : new Color(0.84f, 0.94f, 1f, 0.97f);
        panelImage.raycastTarget = true;

        GameObject closeButton = CreateDrawerButton("CloseDrawerButton", panelObject.transform, "X", new Vector2(1f, 1f), new Vector2(-34f, -34f), new Vector2(48f, 48f), new Color(0.92f, 0.36f, 0.34f, 1f));
        closeButton.GetComponent<Button>().onClick.AddListener(ClosePanel);

        TMP_Text heading = CreateHomeLabel("DrawerTitle", panelObject.transform, GetDrawerTitle(panelKind), new Vector2(0.5f, 1f), isPocketPal ? 24 : isShop ? 26 : 28);
        heading.color = new Color(0.08f, 0.18f, 0.22f, 1f);
        RectTransform headingRect = heading.GetComponent<RectTransform>();
        headingRect.anchorMin = new Vector2(0f, 1f);
        headingRect.anchorMax = new Vector2(1f, 1f);
        headingRect.pivot = new Vector2(0.5f, 1f);
        headingRect.sizeDelta = new Vector2(-96f, 54f);
        headingRect.anchoredPosition = new Vector2(0f, -16f);

        ScrollRect scrollRect = CreateDrawerScroll(panelObject.transform, isShop);
        activeDrawerContent = scrollRect.content;

        drawerContentBuilder = panelObject.AddComponent<CustomizationSceneManager>();
        drawerContentBuilder.enabled = false;
        drawerContentBuilder.SetCompactDrawerMode(true);

        if (panelKind == PanelKind.Customization)
        {
            drawerContentBuilder.BuildCustomizationContent(activeDrawerContent);
        }
        else if (panelKind == PanelKind.TasksRewards)
        {
            drawerContentBuilder.BuildTasksRewardsContent(activeDrawerContent);
        }
        else if (panelKind == PanelKind.PocketPal)
        {
            drawerContentBuilder.BuildPocketPalOptionsContent(activeDrawerContent);
        }
        else if (panelKind == PanelKind.Shop)
        {
            drawerContentBuilder.BuildShopContent(activeDrawerContent);
        }

        Canvas.ForceUpdateCanvases();
        NormalizeDrawerLayout(scrollRect);
        LayoutRebuilder.ForceRebuildLayoutImmediate(activeDrawerContent);
        scrollRect.verticalNormalizedPosition = preserveVerticalScroll ? Mathf.Clamp01(preservedVerticalScroll) : 1f;
        scrollRect.horizontalNormalizedPosition = 0f;
        PositionPocketPalPopup();
        RefreshTopBar();
    }

    private float GetActiveDrawerVerticalScrollPosition()
    {
        if (activeDrawer == null)
        {
            return 1f;
        }

        ScrollRect scrollRect = activeDrawer.GetComponentInChildren<ScrollRect>();
        return scrollRect != null ? scrollRect.verticalNormalizedPosition : 1f;
    }

    private void ConfigureDrawerTransform(RectTransform drawerTransform, PanelKind panelKind)
    {
        if (panelKind == PanelKind.Shop)
        {
            drawerTransform.anchorMin = new Vector2(0f, 1f);
            drawerTransform.anchorMax = new Vector2(1f, 1f);
            drawerTransform.pivot = new Vector2(0.5f, 1f);
            drawerTransform.sizeDelta = new Vector2(-28f, 620f);
            drawerTransform.anchoredPosition = new Vector2(0f, -96f);
            return;
        }

        if (panelKind == PanelKind.PocketPal)
        {
            drawerTransform.anchorMin = new Vector2(0.5f, 0.5f);
            drawerTransform.anchorMax = new Vector2(0.5f, 0.5f);
            drawerTransform.pivot = new Vector2(0.5f, 1f);
            drawerTransform.sizeDelta = new Vector2(410f, 380f);
            PositionPocketPalPopup(drawerTransform);
            return;
        }

        if (panelKind == PanelKind.Customization || panelKind == PanelKind.TasksRewards)
        {
            ResolveCustomizationDrawerClearances(out float customizationTopClearance, out float customizationBottomClearance);

            float customizationCanvasWidth = drawerRoot != null ? drawerRoot.rect.width : 1080f;
            if (customizationCanvasWidth <= 1f && drawerRoot != null && drawerRoot.parent is RectTransform customizationParentRect)
            {
                customizationCanvasWidth = customizationParentRect.rect.width;
            }

            if (customizationCanvasWidth <= 1f)
            {
                customizationCanvasWidth = 1080f;
            }

            float customizationDrawerWidth = Mathf.Clamp(customizationCanvasWidth * 0.34f, 280f, 420f);
            bool isRightSideDrawer = panelKind == PanelKind.TasksRewards;
            float horizontalAnchor = isRightSideDrawer ? 1f : 0f;
            drawerTransform.anchorMin = new Vector2(horizontalAnchor, 0f);
            drawerTransform.anchorMax = new Vector2(horizontalAnchor, 1f);
            drawerTransform.pivot = new Vector2(isRightSideDrawer ? 1f : 0f, 0.5f);
            drawerTransform.sizeDelta = new Vector2(customizationDrawerWidth, -(customizationTopClearance + customizationBottomClearance));
            float horizontalOffset = isRightSideDrawer ? -DrawerSideMargin : DrawerSideMargin;
            drawerTransform.anchoredPosition = new Vector2(horizontalOffset, (customizationBottomClearance - customizationTopClearance) * 0.5f);
            return;
        }

        float canvasWidth = drawerRoot != null ? drawerRoot.rect.width : 1080f;
        if (canvasWidth <= 1f && drawerRoot != null && drawerRoot.parent is RectTransform parentRect)
        {
            canvasWidth = parentRect.rect.width;
        }

        if (canvasWidth <= 1f)
        {
            canvasWidth = 1080f;
        }

        float drawerWidth = Mathf.Min(canvasWidth - (DrawerSideMargin * 2f), Mathf.Max(300f, canvasWidth * DrawerScreenWidthRatio));
        drawerTransform.anchorMin = new Vector2(0.5f, 0f);
        drawerTransform.anchorMax = new Vector2(0.5f, 1f);
        drawerTransform.pivot = new Vector2(0.5f, 0.5f);
        drawerTransform.sizeDelta = new Vector2(drawerWidth, -(DrawerTopClearance + DrawerBottomClearance));
        drawerTransform.anchoredPosition = new Vector2(0f, (DrawerBottomClearance - DrawerTopClearance) * 0.5f);
    }

    private void ResolveCustomizationDrawerClearances(out float topClearance, out float bottomClearance)
    {
        topClearance = DrawerTopClearance;
        bottomClearance = DrawerBottomClearance;

        if (drawerRoot == null)
        {
            return;
        }

        RectTransform topBarRect = FindTopBarRect();
        if (topBarRect != null && TryGetRectVerticalBoundsInDrawerRoot(topBarRect, out float topBarMinY, out _))
        {
            topClearance = Mathf.Max(0f, drawerRoot.rect.yMax - topBarMinY + CustomizationDrawerTopPadding);
        }

        RectTransform customizationButtonRect = FindCustomizationButtonRect();
        if (customizationButtonRect != null && TryGetRectVerticalBoundsInDrawerRoot(customizationButtonRect, out _, out float customizationButtonMaxY))
        {
            bottomClearance = Mathf.Max(0f, customizationButtonMaxY - drawerRoot.rect.yMin + CustomizationDrawerBottomPadding);
        }

        float maxAllowedClearance = Mathf.Max(0f, drawerRoot.rect.height - 220f);
        topClearance = Mathf.Clamp(topClearance, 0f, maxAllowedClearance);
        bottomClearance = Mathf.Clamp(bottomClearance, 0f, maxAllowedClearance);
    }

    private RectTransform FindTopBarRect()
    {
        TopBarManager topBarManager = FindAnyObjectByType<TopBarManager>();
        if (topBarManager != null)
        {
            return topBarManager.GetComponent<RectTransform>();
        }

        Canvas canvas = FindAnyObjectByType<Canvas>();
        Transform topBarTransform = canvas != null ? FindDescendant(canvas.transform, "TopBar") : null;
        return topBarTransform as RectTransform;
    }

    private RectTransform FindCustomizationButtonRect()
    {
        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            return null;
        }

        Transform customizationButton = FindDescendant(canvas.transform, "CustomizationButton");
        return customizationButton as RectTransform;
    }

    private bool TryGetRectVerticalBoundsInDrawerRoot(RectTransform sourceRect, out float minY, out float maxY)
    {
        minY = 0f;
        maxY = 0f;

        if (sourceRect == null || drawerRoot == null)
        {
            return false;
        }

        Vector3[] worldCorners = new Vector3[4];
        sourceRect.GetWorldCorners(worldCorners);

        Vector2 bottomLeft = drawerRoot.InverseTransformPoint(worldCorners[0]);
        Vector2 topRight = drawerRoot.InverseTransformPoint(worldCorners[2]);
        minY = bottomLeft.y;
        maxY = topRight.y;
        return true;
    }

    private ScrollRect CreateDrawerScroll(Transform parent, bool horizontal)
    {
        GameObject scrollObject = new GameObject("DrawerScroll", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(ScrollRect));
        scrollObject.transform.SetParent(parent, false);

        RectTransform scrollTransform = scrollObject.GetComponent<RectTransform>();
        scrollTransform.anchorMin = Vector2.zero;
        scrollTransform.anchorMax = Vector2.one;
        scrollTransform.offsetMin = new Vector2(14f, 16f);
        scrollTransform.offsetMax = new Vector2(-14f, -74f);

        Image scrollImage = scrollObject.GetComponent<Image>();
        scrollImage.color = new Color(1f, 1f, 1f, 0.18f);
        scrollImage.raycastTarget = true;

        GameObject viewportObject = new GameObject("Viewport", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Mask));
        viewportObject.transform.SetParent(scrollObject.transform, false);
        RectTransform viewport = viewportObject.GetComponent<RectTransform>();
        viewport.anchorMin = Vector2.zero;
        viewport.anchorMax = Vector2.one;
        viewport.offsetMin = Vector2.zero;
        viewport.offsetMax = new Vector2(-16f, 0f);

        Image viewportImage = viewportObject.GetComponent<Image>();
        viewportImage.color = new Color(1f, 1f, 1f, 0.01f);
        viewportImage.raycastTarget = true;
        viewportObject.GetComponent<Mask>().showMaskGraphic = false;

        GameObject contentObject = new GameObject("MainContent", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
        contentObject.transform.SetParent(viewportObject.transform, false);
        RectTransform content = contentObject.GetComponent<RectTransform>();
        content.anchorMin = new Vector2(0f, 1f);
        content.anchorMax = new Vector2(1f, 1f);
        content.pivot = new Vector2(0.5f, 1f);
        content.anchoredPosition = Vector2.zero;
        content.sizeDelta = new Vector2(0f, 0f);

        VerticalLayoutGroup layoutGroup = contentObject.GetComponent<VerticalLayoutGroup>();
        layoutGroup.padding = new RectOffset(10, 10, 10, 18);
        layoutGroup.spacing = 18f;
        layoutGroup.childAlignment = TextAnchor.UpperCenter;
        layoutGroup.childControlWidth = true;
        layoutGroup.childControlHeight = true;
        layoutGroup.childForceExpandWidth = true;
        layoutGroup.childForceExpandHeight = false;

        ContentSizeFitter contentSizeFitter = contentObject.GetComponent<ContentSizeFitter>();
        contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        ScrollRect scrollRect = scrollObject.GetComponent<ScrollRect>();
        scrollRect.viewport = viewport;
        scrollRect.content = content;
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;
        scrollRect.inertia = true;
        scrollRect.verticalScrollbar = CreateDrawerScrollbar(scrollObject.transform);
        scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;
        scrollRect.verticalScrollbarSpacing = 4f;

        return scrollRect;
    }

    private Scrollbar CreateDrawerScrollbar(Transform parent)
    {
        GameObject scrollbarObject = new GameObject("VerticalScrollbar", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Scrollbar));
        scrollbarObject.transform.SetParent(parent, false);

        RectTransform scrollbarTransform = scrollbarObject.GetComponent<RectTransform>();
        scrollbarTransform.anchorMin = new Vector2(1f, 0f);
        scrollbarTransform.anchorMax = new Vector2(1f, 1f);
        scrollbarTransform.pivot = new Vector2(1f, 0.5f);
        scrollbarTransform.sizeDelta = new Vector2(10f, 0f);
        scrollbarTransform.anchoredPosition = new Vector2(-2f, 0f);

        Image trackImage = scrollbarObject.GetComponent<Image>();
        trackImage.color = new Color(0.12f, 0.22f, 0.24f, 0.2f);
        trackImage.raycastTarget = true;

        GameObject slidingArea = new GameObject("Sliding Area", typeof(RectTransform));
        slidingArea.transform.SetParent(scrollbarObject.transform, false);

        RectTransform slidingAreaTransform = slidingArea.GetComponent<RectTransform>();
        slidingAreaTransform.anchorMin = Vector2.zero;
        slidingAreaTransform.anchorMax = Vector2.one;
        slidingAreaTransform.offsetMin = new Vector2(1f, 1f);
        slidingAreaTransform.offsetMax = new Vector2(-1f, -1f);

        GameObject handle = new GameObject("Handle", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        handle.transform.SetParent(slidingArea.transform, false);

        RectTransform handleTransform = handle.GetComponent<RectTransform>();
        handleTransform.anchorMin = Vector2.zero;
        handleTransform.anchorMax = Vector2.one;
        handleTransform.offsetMin = Vector2.zero;
        handleTransform.offsetMax = Vector2.zero;

        Image handleImage = handle.GetComponent<Image>();
        handleImage.color = new Color(0.08f, 0.18f, 0.22f, 0.75f);
        handleImage.raycastTarget = true;

        Scrollbar scrollbar = scrollbarObject.GetComponent<Scrollbar>();
        scrollbar.direction = Scrollbar.Direction.BottomToTop;
        scrollbar.targetGraphic = handleImage;
        scrollbar.handleRect = handleTransform;
        scrollbar.size = 0.35f;

        return scrollbar;
    }

    private void NormalizeDrawerLayout(ScrollRect scrollRect)
    {
        if (scrollRect == null || scrollRect.viewport == null || scrollRect.content == null)
        {
            return;
        }

        Canvas.ForceUpdateCanvases();

        float viewportWidth = scrollRect.viewport.rect.width;
        if (viewportWidth <= 1f)
        {
            viewportWidth = Mathf.Max(1f, activeDrawer.rect.width - 28f);
        }

        RectTransform content = scrollRect.content;
        content.anchorMin = new Vector2(0f, 1f);
        content.anchorMax = new Vector2(1f, 1f);
        content.offsetMin = Vector2.zero;
        content.offsetMax = new Vector2(0f, content.offsetMax.y);
        content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, viewportWidth);

        float childWidth = Mathf.Max(1f, viewportWidth - 20f);
        for (int index = 0; index < content.childCount; index++)
        {
            RectTransform child = content.GetChild(index) as RectTransform;
            if (child == null)
            {
                continue;
            }

            child.anchorMin = new Vector2(0f, child.anchorMin.y);
            child.anchorMax = new Vector2(1f, child.anchorMax.y);
            child.offsetMin = new Vector2(0f, child.offsetMin.y);
            child.offsetMax = new Vector2(0f, child.offsetMax.y);
            child.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, childWidth);

            LayoutElement layoutElement = child.GetComponent<LayoutElement>();
            if (layoutElement != null)
            {
                layoutElement.minWidth = childWidth;
                layoutElement.preferredWidth = childWidth;
                layoutElement.flexibleWidth = 0f;
            }
        }
    }

    private GameObject CreateDrawerButton(string objectName, Transform parent, string label, Vector2 anchor, Vector2 position, Vector2 size, Color color)
    {
        GameObject buttonObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(parent, false);

        RectTransform buttonRect = buttonObject.GetComponent<RectTransform>();
        buttonRect.anchorMin = anchor;
        buttonRect.anchorMax = anchor;
        buttonRect.pivot = new Vector2(0.5f, 0.5f);
        buttonRect.sizeDelta = size;
        buttonRect.anchoredPosition = position;

        Image image = buttonObject.GetComponent<Image>();
        image.color = Color.Lerp(color, new Color(0.82f, 0.72f, 1f, 1f), 0.35f);

        TMP_Text buttonLabel = CreateHomeLabel(objectName + "Label", buttonObject.transform, label, new Vector2(0.5f, 0.5f), 24);
        buttonLabel.color = Color.white;
        RectTransform labelRect = buttonLabel.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        Button button = buttonObject.GetComponent<Button>();
        button.targetGraphic = image;

        return buttonObject;
    }

    private void ClearDrawer()
    {
        if (drawerRoot == null)
        {
            return;
        }

        for (int index = drawerRoot.childCount - 1; index >= 0; index--)
        {
            Destroy(drawerRoot.GetChild(index).gameObject);
        }

        activeDrawer = null;
        activeDrawerContent = null;
        drawerContentBuilder = null;
    }

    private void AnimateDrawer()
    {
        if (activeDrawer == null)
        {
            return;
        }

        if (targetPanel == PanelKind.PocketPal)
        {
            PositionPocketPalPopup();
        }

        bool growsVertically = targetPanel == PanelKind.Shop || targetPanel == PanelKind.PocketPal;
        if (!growsVertically)
        {
            if (drawerAnimationTarget <= 0f)
            {
                ClearDrawer();
                targetPanel = PanelKind.None;
            }

            return;
        }

        float currentScale = growsVertically ? activeDrawer.localScale.y : activeDrawer.localScale.x;
        float nextScale = Mathf.MoveTowards(currentScale, drawerAnimationTarget, Time.deltaTime * 7f);

        if (growsVertically)
        {
            activeDrawer.localScale = new Vector3(1f, nextScale, 1f);
        }
        else
        {
            activeDrawer.localScale = new Vector3(nextScale, 1f, 1f);
        }

        if (Mathf.Approximately(nextScale, 0f) && drawerAnimationTarget <= 0f)
        {
            ClearDrawer();
            targetPanel = PanelKind.None;
        }
    }

    private string GetDrawerTitle(PanelKind panelKind)
    {
        if (panelKind == PanelKind.Customization)
        {
            return "Customize";
        }

        if (panelKind == PanelKind.TasksRewards)
        {
            return "Tasks & Rewards";
        }

        if (panelKind == PanelKind.PocketPal)
        {
            return "PocketPal";
        }

        return "Shop";
    }

    private string GetDrawerName(PanelKind panelKind)
    {
        return GetDrawerTitle(panelKind).Replace(" ", "").Replace("&", "") + "Drawer";
    }

    private void RefreshTopBar()
    {
        TopBarManager topBarManager = FindAnyObjectByType<TopBarManager>();
        if (topBarManager != null)
        {
            topBarManager.RefreshCoinDisplay();
        }
    }

    private void PositionPocketPalPopup()
    {
        PositionPocketPalPopup(activeDrawer);
    }

    private void PositionPocketPalPopup(RectTransform popupTransform)
    {
        if (popupTransform == null || targetPanel != PanelKind.PocketPal || pocketPalTransform == null)
        {
            return;
        }

        Vector2 popupPosition = pocketPalTransform.anchoredPosition + new Vector2(0f, -175f);
        popupPosition.x = Mathf.Clamp(popupPosition.x, -320f, 320f);
        popupPosition.y = Mathf.Clamp(popupPosition.y, -490f, 280f);
        popupTransform.anchoredPosition = popupPosition;
    }

    private void RefreshPocketPalVisual()
    {
        if (pocketPalTransform == null)
        {
            return;
        }

        pocketPalNightStillTransform = null;
        pocketPalNightStillImage = null;

        Transform existingVisual = pocketPalTransform.Find("PandaVisual");
        if (existingVisual != null)
        {
            Destroy(existingVisual.gameObject);
        }

        Transform existingShadow = pocketPalTransform.Find("PocketPalShadow");
        if (existingShadow != null)
        {
            Destroy(existingShadow.gameObject);
        }

        Transform existingNightStill = pocketPalTransform.Find("NighttimeStill");
        if (existingNightStill != null)
        {
            Destroy(existingNightStill.gameObject);
        }

        CreatePocketPalSprite(pocketPalTransform);
        ApplyNighttimePresentation(true);
    }

    private Transform FindCanvasChild(string childName)
    {
        Canvas canvas = FindAnyObjectByType<Canvas>();
        return canvas != null ? canvas.transform.Find(childName) : null;
    }

    private GameObject CreateHomePanel(string objectName, Transform parent, Vector2 anchor, Vector2 size, Color color)
    {
        GameObject panel = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        panel.transform.SetParent(parent, false);

        RectTransform rectTransform = panel.GetComponent<RectTransform>();
        rectTransform.anchorMin = anchor;
        rectTransform.anchorMax = anchor;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = size;
        rectTransform.anchoredPosition = Vector2.zero;

        Image image = panel.GetComponent<Image>();
        image.color = color;
        image.raycastTarget = false;
        image.type = Image.Type.Simple;

        return panel;
    }

    private void Remove3DHabitatView(Transform background)
    {
        Transform canvasTransform = background.parent != null ? background.parent : background;
        Transform canvasView = canvasTransform.Find(HabitatViewName);
        if (canvasView != null)
        {
            Destroy(canvasView.gameObject);
        }

        Transform backgroundView = background.Find(HabitatViewName);
        if (backgroundView != null)
        {
            Destroy(backgroundView.gameObject);
        }

        GameObject existingHabitat = GameObject.Find(HabitatRootName);
        if (existingHabitat != null)
        {
            Destroy(existingHabitat);
        }
    }

    private void Create3DHabitatView(Transform background)
    {
        Transform canvasTransform = background.parent != null ? background.parent : background;
        Transform existingView = canvasTransform.Find(HabitatViewName);
        if (existingView == null)
        {
            existingView = background.Find(HabitatViewName);
        }

        if (existingView != null)
        {
            Destroy(existingView.gameObject);
        }

        GameObject viewObject = new GameObject(HabitatViewName, typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage));
        viewObject.transform.SetParent(canvasTransform, false);

        int backgroundIndex = background.GetSiblingIndex();
        viewObject.transform.SetSiblingIndex(backgroundIndex + 1);

        RectTransform viewTransform = viewObject.GetComponent<RectTransform>();
        viewTransform.anchorMin = Vector2.zero;
        viewTransform.anchorMax = Vector2.one;
        viewTransform.offsetMin = Vector2.zero;
        viewTransform.offsetMax = Vector2.zero;

        RenderTexture habitatTexture = new RenderTexture(1080, 1920, 24);
        habitatTexture.name = "PocketSanctuaryHabitatTexture";
        habitatTexture.antiAliasing = 4;
        habitatTexture.Create();

        RawImage habitatImage = viewObject.GetComponent<RawImage>();
        habitatImage.texture = habitatTexture;
        habitatImage.raycastTarget = false;

        GameObject existingHabitat = GameObject.Find(HabitatRootName);
        if (existingHabitat != null)
        {
            Destroy(existingHabitat);
        }

        GameObject habitatRoot = new GameObject(HabitatRootName);
        habitatRoot.transform.position = new Vector3(0f, 0f, 18f);

        Camera habitatCamera = CreateHabitatCamera(habitatRoot.transform, habitatTexture);
        BuildHabitatGeometry(habitatRoot.transform);
        habitatCamera.Render();
    }

    private Camera CreateHabitatCamera(Transform parent, RenderTexture targetTexture)
    {
        GameObject cameraObject = new GameObject("HabitatCamera", typeof(Camera));
        cameraObject.transform.SetParent(parent, false);
        cameraObject.transform.localPosition = new Vector3(0f, 5.8f, -8.6f);
        cameraObject.transform.localRotation = Quaternion.Euler(55f, 0f, 0f);

        Camera habitatCamera = cameraObject.GetComponent<Camera>();
        habitatCamera.clearFlags = CameraClearFlags.SolidColor;
        habitatCamera.backgroundColor = new Color(0.56f, 0.84f, 0.97f, 1f);
        habitatCamera.fieldOfView = 38f;
        habitatCamera.nearClipPlane = 0.1f;
        habitatCamera.farClipPlane = 60f;
        habitatCamera.targetTexture = targetTexture;

        return habitatCamera;
    }

    private void BuildHabitatGeometry(Transform parent)
    {
        Material grass = CreateMaterial(new Color(0.25f, 0.62f, 0.32f, 1f));
        Material meadow = CreateMaterial(new Color(0.39f, 0.78f, 0.42f, 1f));
        Material bamboo = CreateMaterial(new Color(0.22f, 0.64f, 0.29f, 1f));
        Material bambooRing = CreateMaterial(new Color(0.82f, 0.92f, 0.45f, 1f));
        Material wood = CreateMaterial(new Color(0.65f, 0.45f, 0.24f, 1f));
        Material leaf = CreateMaterial(new Color(0.13f, 0.48f, 0.22f, 1f));
        Material stone = CreateMaterial(new Color(0.56f, 0.61f, 0.59f, 1f));
        Material toy = CreateMaterial(new Color(0.92f, 0.28f, 0.24f, 1f));
        Material toyStripe = CreateMaterial(new Color(0.98f, 0.9f, 0.28f, 1f));
        Material pandaBlack = CreateMaterial(new Color(0.02f, 0.06f, 0.05f, 1f));
        Material pandaWhite = CreateMaterial(new Color(0.98f, 0.98f, 0.9f, 1f));
        Material pandaLavender = CreateMaterial(new Color(0.57f, 0.42f, 0.91f, 1f));
        Material sparkle = CreateMaterial(new Color(1f, 0.92f, 0.38f, 1f));

        CreatePrimitive("SanctuaryFloor", PrimitiveType.Cylinder, parent, new Vector3(0f, -0.18f, 0f), new Vector3(7.2f, 0.18f, 5.2f), grass);
        CreatePrimitive("CenterMeadow", PrimitiveType.Cylinder, parent, new Vector3(0f, -0.08f, 0.15f), new Vector3(5.8f, 0.08f, 3.8f), meadow);

        CreateBambooCluster(parent, new Vector3(-2.75f, 0f, 0.85f), bamboo, bambooRing, leaf);
        CreateBambooCluster(parent, new Vector3(2.65f, 0f, 0.9f), bamboo, bambooRing, leaf);
        CreateBambooCluster(parent, new Vector3(-2.25f, 0f, -1.35f), bamboo, bambooRing, leaf);

        CreatePlaypen(parent, wood);
        CreatePrimitive("SmoothStoneLeft", PrimitiveType.Sphere, parent, new Vector3(-1.85f, 0.12f, -1.35f), new Vector3(0.72f, 0.28f, 0.45f), stone);
        CreatePrimitive("SmoothStoneRight", PrimitiveType.Sphere, parent, new Vector3(2.0f, 0.12f, -1.22f), new Vector3(0.62f, 0.24f, 0.42f), stone);

        GameObject ball = CreatePrimitive("PlayBall", PrimitiveType.Sphere, parent, new Vector3(1.45f, 0.28f, 0.24f), new Vector3(0.42f, 0.42f, 0.42f), toy);
        playBallTransform = ball.transform;
        CreatePrimitive("PlayBallStripe", PrimitiveType.Cylinder, ball.transform, Vector3.zero, new Vector3(1.04f, 0.05f, 1.04f), toyStripe).transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

        GameObject mobile = CreatePrimitive("HangingBambooToy", PrimitiveType.Cylinder, parent, new Vector3(-0.75f, 1.18f, -0.8f), new Vector3(0.11f, 0.82f, 0.11f), bambooRing);
        mobile.transform.localRotation = Quaternion.Euler(0f, 0f, 22f);
        bambooMobileTransform = mobile.transform;

        CreateHabitatPanda(parent, pandaBlack, pandaWhite, pandaLavender, bamboo, bambooRing, sparkle);
        CreateHabitatLight(parent, new Vector3(0f, 5f, -3f), 1.2f);
        CreateHabitatLight(parent, new Vector3(-3f, 3f, 2f), 0.45f);
        SetHabitatActivity(habitatActivity);
    }

    private void CreateHabitatPanda(Transform parent, Material black, Material white, Material shirt, Material bamboo, Material bambooRing, Material sparkle)
    {
        GameObject pandaRoot = new GameObject("HabitatPanda");
        pandaRoot.transform.SetParent(parent, false);
        pandaRoot.transform.localPosition = Vector3.zero;
        habitatPandaRoot = pandaRoot.transform;

        habitatPandaBody = CreatePrimitive("HabitatPandaBody", PrimitiveType.Sphere, pandaRoot.transform, new Vector3(0f, 0.56f, 0f), new Vector3(0.72f, 0.82f, 0.52f), black).transform;
        CreatePrimitive("HabitatPandaShirt", PrimitiveType.Sphere, pandaRoot.transform, new Vector3(0f, 0.5f, -0.22f), new Vector3(0.56f, 0.58f, 0.15f), shirt);
        CreatePrimitive("HabitatPandaBellyButton", PrimitiveType.Sphere, pandaRoot.transform, new Vector3(0f, 0.52f, -0.36f), new Vector3(0.09f, 0.09f, 0.04f), sparkle);

        habitatPandaHead = CreatePrimitive("HabitatPandaHead", PrimitiveType.Sphere, pandaRoot.transform, new Vector3(0f, 1.23f, -0.03f), new Vector3(0.58f, 0.5f, 0.48f), white).transform;
        CreatePrimitive("HabitatPandaLeftEar", PrimitiveType.Sphere, pandaRoot.transform, new Vector3(-0.45f, 1.32f, 0f), new Vector3(0.22f, 0.26f, 0.18f), black);
        CreatePrimitive("HabitatPandaRightEar", PrimitiveType.Sphere, pandaRoot.transform, new Vector3(0.45f, 1.32f, 0f), new Vector3(0.22f, 0.26f, 0.18f), black);
        CreatePrimitive("HabitatPandaLeftPatch", PrimitiveType.Sphere, pandaRoot.transform, new Vector3(-0.2f, 1.25f, -0.42f), new Vector3(0.22f, 0.19f, 0.05f), black);
        CreatePrimitive("HabitatPandaRightPatch", PrimitiveType.Sphere, pandaRoot.transform, new Vector3(0.2f, 1.25f, -0.42f), new Vector3(0.22f, 0.19f, 0.05f), black);
        CreatePrimitive("HabitatPandaLeftEye", PrimitiveType.Sphere, pandaRoot.transform, new Vector3(-0.16f, 1.26f, -0.47f), new Vector3(0.055f, 0.055f, 0.02f), white);
        CreatePrimitive("HabitatPandaRightEye", PrimitiveType.Sphere, pandaRoot.transform, new Vector3(0.16f, 1.26f, -0.47f), new Vector3(0.055f, 0.055f, 0.02f), white);
        CreatePrimitive("HabitatPandaNose", PrimitiveType.Sphere, pandaRoot.transform, new Vector3(0f, 1.13f, -0.49f), new Vector3(0.09f, 0.07f, 0.045f), black);

        habitatPandaLeftPaw = CreatePrimitive("HabitatPandaLeftPaw", PrimitiveType.Sphere, pandaRoot.transform, new Vector3(-0.48f, 0.62f, -0.1f), new Vector3(0.22f, 0.32f, 0.18f), black).transform;
        habitatPandaRightPaw = CreatePrimitive("HabitatPandaRightPaw", PrimitiveType.Sphere, pandaRoot.transform, new Vector3(0.48f, 0.62f, -0.1f), new Vector3(0.22f, 0.32f, 0.18f), black).transform;
        CreatePrimitive("HabitatPandaLeftFoot", PrimitiveType.Sphere, pandaRoot.transform, new Vector3(-0.24f, 0.12f, -0.1f), new Vector3(0.25f, 0.16f, 0.22f), black);
        CreatePrimitive("HabitatPandaRightFoot", PrimitiveType.Sphere, pandaRoot.transform, new Vector3(0.24f, 0.12f, -0.1f), new Vector3(0.25f, 0.16f, 0.22f), black);

        GameObject snack = CreatePrimitive("HabitatSnackBamboo", PrimitiveType.Cylinder, parent, new Vector3(-2.2f, 0.95f, -0.3f), new Vector3(0.055f, 0.38f, 0.055f), bambooRing);
        snack.transform.localRotation = Quaternion.Euler(0f, 0f, 84f);
        habitatActionBamboo = snack.transform;
        CreatePrimitive("HabitatSnackLeaf", PrimitiveType.Cube, snack.transform, new Vector3(0f, 0.5f, 0f), new Vector3(0.12f, 0.03f, 0.28f), bamboo);

        habitatActionSpark = CreatePrimitive("HabitatPlaySpark", PrimitiveType.Sphere, parent, new Vector3(0f, 1.28f, -0.7f), new Vector3(0.12f, 0.12f, 0.12f), sparkle).transform;
        habitatExplorerButterfly = CreatePrimitive("HabitatExplorerButterfly", PrimitiveType.Cube, parent, new Vector3(0f, 1.8f, 0.35f), new Vector3(0.18f, 0.03f, 0.11f), sparkle).transform;
        CreatePrimitive("ButterflyWingLeft", PrimitiveType.Sphere, habitatExplorerButterfly, new Vector3(-0.18f, 0f, 0f), new Vector3(0.22f, 0.035f, 0.16f), sparkle);
        CreatePrimitive("ButterflyWingRight", PrimitiveType.Sphere, habitatExplorerButterfly, new Vector3(0.18f, 0f, 0f), new Vector3(0.22f, 0.035f, 0.16f), sparkle);
    }

    private void CreateBambooCluster(Transform parent, Vector3 basePosition, Material bamboo, Material bambooRing, Material leaf)
    {
        for (int index = 0; index < 3; index++)
        {
            float offset = (index - 1) * 0.24f;
            float height = 2.1f + index * 0.22f;
            Vector3 position = basePosition + new Vector3(offset, height * 0.5f, index * 0.12f);
            GameObject stalk = CreatePrimitive("BambooStalk", PrimitiveType.Cylinder, parent, position, new Vector3(0.12f, height * 0.5f, 0.12f), bamboo);
            stalk.transform.localRotation = Quaternion.Euler(0f, 0f, offset * 8f);

            for (int ring = 0; ring < 4; ring++)
            {
                CreatePrimitive("BambooRing", PrimitiveType.Cylinder, parent, position + new Vector3(0f, -0.72f + ring * 0.44f, 0f), new Vector3(0.13f, 0.018f, 0.13f), bambooRing);
            }

            GameObject leafObject = CreatePrimitive("BambooLeaf", PrimitiveType.Cube, parent, position + new Vector3(0.25f, height * 0.48f, 0f), new Vector3(0.12f, 0.04f, 0.68f), leaf);
            leafObject.transform.localRotation = Quaternion.Euler(18f, 36f, 24f);
        }
    }

    private void CreatePlaypen(Transform parent, Material wood)
    {
        Vector3 center = new Vector3(0f, 0.33f, -0.45f);
        CreatePrimitive("PlaypenBackRail", PrimitiveType.Cube, parent, center + new Vector3(0f, 0.5f, 1.15f), new Vector3(3.5f, 0.12f, 0.12f), wood);
        CreatePrimitive("PlaypenFrontRail", PrimitiveType.Cube, parent, center + new Vector3(0f, 0.36f, -1.15f), new Vector3(3.5f, 0.12f, 0.12f), wood);
        CreatePrimitive("PlaypenLeftRail", PrimitiveType.Cube, parent, center + new Vector3(-1.75f, 0.43f, 0f), new Vector3(0.12f, 0.12f, 2.35f), wood);
        CreatePrimitive("PlaypenRightRail", PrimitiveType.Cube, parent, center + new Vector3(1.75f, 0.43f, 0f), new Vector3(0.12f, 0.12f, 2.35f), wood);

        for (int postIndex = 0; postIndex < 5; postIndex++)
        {
            float x = -1.55f + postIndex * 0.78f;
            CreatePrimitive("PlaypenPost", PrimitiveType.Cube, parent, center + new Vector3(x, 0.24f, -1.15f), new Vector3(0.12f, 0.8f, 0.12f), wood);
            CreatePrimitive("PlaypenBackPost", PrimitiveType.Cube, parent, center + new Vector3(x, 0.33f, 1.15f), new Vector3(0.12f, 0.95f, 0.12f), wood);
        }
    }

    private GameObject CreatePrimitive(string objectName, PrimitiveType primitiveType, Transform parent, Vector3 localPosition, Vector3 localScale, Material material)
    {
        GameObject primitive = GameObject.CreatePrimitive(primitiveType);
        primitive.name = objectName;
        primitive.transform.SetParent(parent, false);
        primitive.transform.localPosition = localPosition;
        primitive.transform.localScale = localScale;

        Renderer renderer = primitive.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = material;
        }

        Collider collider = primitive.GetComponent<Collider>();
        if (collider != null)
        {
            Destroy(collider);
        }

        return primitive;
    }

    private Material CreateMaterial(Color color)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
        {
            shader = Shader.Find("Standard");
        }

        if (shader == null)
        {
            shader = Shader.Find("Sprites/Default");
        }

        if (shader == null)
        {
            shader = Shader.Find("UI/Default");
        }

        Material material = new Material(shader);
        material.color = color;
        return material;
    }

    private void CreateHabitatLight(Transform parent, Vector3 localPosition, float intensity)
    {
        GameObject lightObject = new GameObject("HabitatLight", typeof(Light));
        lightObject.transform.SetParent(parent, false);
        lightObject.transform.localPosition = localPosition;

        Light light = lightObject.GetComponent<Light>();
        light.type = LightType.Point;
        light.intensity = intensity;
        light.range = 8f;
        light.color = new Color(1f, 0.96f, 0.86f, 1f);
    }

    private void AnimateHabitatProps()
    {
        AnimateHabitatPanda();

        if (playBallTransform != null)
        {
            if (habitatActivity == HabitatActivity.BattingBall)
            {
                float roll = Mathf.Sin(Time.time * 3.8f);
                playBallTransform.localPosition = new Vector3(1.38f + roll * 0.48f, 0.28f + Mathf.Abs(roll) * 0.22f, 0.12f + Mathf.Cos(Time.time * 2.2f) * 0.12f);
                playBallTransform.Rotate(new Vector3(120f, 180f, 35f) * Time.deltaTime, Space.Self);
            }
            else
            {
                playBallTransform.localPosition = Vector3.Lerp(playBallTransform.localPosition, new Vector3(1.45f, 0.28f, 0.24f), Time.deltaTime * 2.5f);
                playBallTransform.Rotate(new Vector3(0f, 45f, 18f) * Time.deltaTime, Space.Self);
            }
        }

        if (bambooMobileTransform != null)
        {
            bambooMobileTransform.localRotation = Quaternion.Euler(0f, 0f, 22f + Mathf.Sin(Time.time * 2f) * 10f);
        }
    }

    private void SetHabitatActivityFromMessage(string activityMessage)
    {
        if (activityMessage.Contains("nibbling"))
        {
            SetHabitatActivity(HabitatActivity.NibblingBamboo);
            return;
        }

        if (activityMessage.Contains("playing"))
        {
            SetHabitatActivity(HabitatActivity.PlayingInPen);
            return;
        }

        if (activityMessage.Contains("batting"))
        {
            SetHabitatActivity(HabitatActivity.BattingBall);
            return;
        }

        if (activityMessage.Contains("napping") || activityMessage.Contains("hammock") || activityMessage.Contains("bed"))
        {
            SetHabitatActivity(HabitatActivity.SleepingInHammock);
            return;
        }

        SetHabitatActivity(HabitatActivity.Exploring);
    }

    private void SetHabitatActivity(HabitatActivity nextActivity)
    {
        habitatActivity = nextActivity;

        if (habitatActionBamboo != null)
        {
            habitatActionBamboo.gameObject.SetActive(habitatActivity == HabitatActivity.NibblingBamboo);
        }

        if (habitatActionSpark != null)
        {
            habitatActionSpark.gameObject.SetActive(habitatActivity == HabitatActivity.PlayingInPen);
        }

        if (habitatExplorerButterfly != null)
        {
            habitatExplorerButterfly.gameObject.SetActive(habitatActivity == HabitatActivity.Exploring);
        }
    }

    private void AnimateHabitatPanda()
    {
        if (habitatPandaRoot == null)
        {
            return;
        }

        float time = Time.time;
        Vector3 targetPosition = GetHabitatPandaTarget(time);
        habitatPandaRoot.localPosition = Vector3.Lerp(habitatPandaRoot.localPosition, targetPosition, Time.deltaTime * 2.8f);
        habitatPandaRoot.localRotation = Quaternion.Lerp(habitatPandaRoot.localRotation, GetHabitatPandaFacing(), Time.deltaTime * 4f);

        float idleBob = Mathf.Sin(time * 5.2f) * 0.045f;
        float bounce = habitatActivity == HabitatActivity.PlayingInPen ? Mathf.Abs(Mathf.Sin(time * 5.8f)) * 0.16f : idleBob;
        Vector3 rootPosition = habitatPandaRoot.localPosition;
        habitatPandaRoot.localPosition = new Vector3(rootPosition.x, bounce, rootPosition.z);

        if (habitatPandaHead != null)
        {
            float nod = habitatActivity == HabitatActivity.NibblingBamboo ? Mathf.Sin(time * 9f) * 10f : Mathf.Sin(time * 2.5f) * 4f;
            habitatPandaHead.localRotation = Quaternion.Euler(nod, 0f, 0f);
        }

        AnimateHabitatPaws(time);
        AnimateHabitatActionProps(time);
    }

    private Vector3 GetHabitatPandaTarget(float time)
    {
        if (habitatActivity == HabitatActivity.NibblingBamboo)
        {
            return new Vector3(-2.06f, 0f, -0.48f);
        }

        if (habitatActivity == HabitatActivity.PlayingInPen)
        {
            return new Vector3(0f, 0f, -0.52f);
        }

        if (habitatActivity == HabitatActivity.BattingBall)
        {
            return new Vector3(0.95f, 0f, -0.15f);
        }

        return new Vector3(Mathf.Sin(time * 0.42f) * 1.45f, 0f, -0.12f + Mathf.Cos(time * 0.34f) * 0.72f);
    }

    private Quaternion GetHabitatPandaFacing()
    {
        if (habitatActivity == HabitatActivity.NibblingBamboo)
        {
            return Quaternion.Euler(0f, 28f, 0f);
        }

        if (habitatActivity == HabitatActivity.BattingBall)
        {
            return Quaternion.Euler(0f, -35f, 0f);
        }

        return Quaternion.identity;
    }

    private void AnimateHabitatPaws(float time)
    {
        if (habitatPandaLeftPaw == null || habitatPandaRightPaw == null)
        {
            return;
        }

        if (habitatActivity == HabitatActivity.NibblingBamboo)
        {
            habitatPandaLeftPaw.localPosition = new Vector3(-0.36f, 0.84f, -0.35f);
            habitatPandaRightPaw.localPosition = new Vector3(0.34f, 0.72f, -0.32f);
            return;
        }

        if (habitatActivity == HabitatActivity.BattingBall)
        {
            float swipe = Mathf.Abs(Mathf.Sin(time * 4.2f));
            habitatPandaLeftPaw.localPosition = new Vector3(-0.46f, 0.62f, -0.1f);
            habitatPandaRightPaw.localPosition = new Vector3(0.36f + swipe * 0.28f, 0.68f + swipe * 0.2f, -0.34f);
            return;
        }

        if (habitatActivity == HabitatActivity.PlayingInPen)
        {
            float wave = Mathf.Sin(time * 6f) * 0.12f;
            habitatPandaLeftPaw.localPosition = new Vector3(-0.42f, 0.76f + wave, -0.28f);
            habitatPandaRightPaw.localPosition = new Vector3(0.42f, 0.76f - wave, -0.28f);
            return;
        }

        float walk = Mathf.Sin(time * 3.8f) * 0.08f;
        habitatPandaLeftPaw.localPosition = new Vector3(-0.48f, 0.62f + walk, -0.1f);
        habitatPandaRightPaw.localPosition = new Vector3(0.48f, 0.62f - walk, -0.1f);
    }

    private void AnimateHabitatActionProps(float time)
    {
        if (habitatActionBamboo != null && habitatActivity == HabitatActivity.NibblingBamboo)
        {
            habitatActionBamboo.localPosition = new Vector3(-2.18f, 1.03f + Mathf.Sin(time * 9f) * 0.035f, -0.78f);
            habitatActionBamboo.localRotation = Quaternion.Euler(0f, 0f, 82f + Mathf.Sin(time * 6f) * 6f);
        }

        if (habitatActionSpark != null && habitatActivity == HabitatActivity.PlayingInPen)
        {
            habitatActionSpark.localPosition = new Vector3(Mathf.Sin(time * 3.8f) * 0.5f, 1.25f + Mathf.Abs(Mathf.Sin(time * 4.6f)) * 0.32f, -0.78f);
            float scale = 0.11f + Mathf.Abs(Mathf.Sin(time * 5.8f)) * 0.08f;
            habitatActionSpark.localScale = new Vector3(scale, scale, scale);
        }

        if (habitatExplorerButterfly != null && habitatActivity == HabitatActivity.Exploring)
        {
            habitatExplorerButterfly.localPosition = new Vector3(Mathf.Sin(time * 1.2f) * 1.8f, 1.55f + Mathf.Sin(time * 2.4f) * 0.18f, 0.1f + Mathf.Cos(time * 1.1f) * 0.7f);
            habitatExplorerButterfly.localRotation = Quaternion.Euler(0f, time * 90f, Mathf.Sin(time * 8f) * 18f);
        }
    }

    private void StyleBottomNavigationButtons()
    {
        Canvas canvas = FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            return;
        }

        EnsureButtonIcons();
        StyleIconButton(FindDescendant(canvas.transform, "CustomizationButton"), "Customize", customizationButtonIcon, new Color(0.95f, 0.78f, 0.28f, 1f));
        StyleIconButton(FindDescendant(canvas.transform, "TasksRewardsButton"), "Rewards", rewardsButtonIcon, new Color(0.3f, 0.78f, 0.66f, 1f));
    }

    private void StyleIconButton(Transform buttonTransform, string labelText, Sprite icon, Color buttonColor)
    {
        if (buttonTransform == null)
        {
            return;
        }

        RectTransform buttonRect = buttonTransform.GetComponent<RectTransform>();
        if (buttonRect != null)
        {
            buttonRect.sizeDelta = new Vector2(180f, 118f);
        }

        Image buttonImage = buttonTransform.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = buttonColor;
            buttonImage.raycastTarget = true;
        }

        Button button = buttonTransform.GetComponent<Button>();
        if (button != null && buttonImage != null)
        {
            button.targetGraphic = buttonImage;
        }

        TMP_Text label = buttonTransform.GetComponentInChildren<TMP_Text>(true);
        if (label != null)
        {
            label.text = labelText;
            label.fontSize = 24f;
            label.fontStyle = FontStyles.Bold;
            label.alignment = TextAlignmentOptions.Center;
            label.color = new Color(0.08f, 0.18f, 0.22f, 1f);
            label.raycastTarget = false;

            RectTransform labelRect = label.GetComponent<RectTransform>();
            if (labelRect != null)
            {
                labelRect.anchorMin = new Vector2(0f, 0f);
                labelRect.anchorMax = new Vector2(1f, 0f);
                labelRect.pivot = new Vector2(0.5f, 0f);
                labelRect.sizeDelta = new Vector2(0f, 36f);
                labelRect.anchoredPosition = new Vector2(0f, 12f);
            }
        }

        Transform existingIcon = buttonTransform.Find("ButtonIcon");
        GameObject iconObject = existingIcon != null ? existingIcon.gameObject : new GameObject("ButtonIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        iconObject.transform.SetParent(buttonTransform, false);
        iconObject.transform.SetAsFirstSibling();

        RectTransform iconRect = iconObject.GetComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0.5f, 1f);
        iconRect.anchorMax = new Vector2(0.5f, 1f);
        iconRect.pivot = new Vector2(0.5f, 1f);
        iconRect.sizeDelta = new Vector2(64f, 64f);
        iconRect.anchoredPosition = new Vector2(0f, -14f);

        Image iconImage = iconObject.GetComponent<Image>();
        iconImage.sprite = icon;
        iconImage.preserveAspect = true;
        iconImage.raycastTarget = false;
    }

    private Transform FindDescendant(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
            {
                return child;
            }

            Transform match = FindDescendant(child, childName);
            if (match != null)
            {
                return match;
            }
        }

        return null;
    }

    private void CreatePocketPalSprite(Transform parent)
    {
        EnsurePocketPalSprites();

        GameObject shadowObject = new GameObject("PocketPalShadow", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        shadowObject.transform.SetParent(parent, false);

        pocketPalShadowTransform = shadowObject.GetComponent<RectTransform>();
        pocketPalShadowTransform.anchorMin = new Vector2(0.5f, 0.5f);
        pocketPalShadowTransform.anchorMax = new Vector2(0.5f, 0.5f);
        pocketPalShadowTransform.pivot = new Vector2(0.5f, 0.5f);
        pocketPalShadowTransform.sizeDelta = new Vector2(230f, 58f);
        pocketPalShadowTransform.anchoredPosition = new Vector2(0f, -114f);

        Image shadowImage = shadowObject.GetComponent<Image>();
        shadowImage.sprite = shadowSprite;
        shadowImage.color = new Color(0.05f, 0.13f, 0.12f, 0.24f);
        shadowImage.raycastTarget = false;

        GameObject visualObject = new GameObject("PandaVisual", typeof(RectTransform));
        visualObject.transform.SetParent(parent, false);

        pocketPalVisualTransform = visualObject.GetComponent<RectTransform>();
        pocketPalVisualTransform.anchorMin = new Vector2(0.5f, 0.5f);
        pocketPalVisualTransform.anchorMax = new Vector2(0.5f, 0.5f);
        pocketPalVisualTransform.pivot = new Vector2(0.5f, 0.5f);
        pocketPalVisualTransform.sizeDelta = new Vector2(290f, 290f);
        pocketPalVisualTransform.anchoredPosition = new Vector2(0f, 0f);

        GameObject spriteObject = new GameObject("PandaBody", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        spriteObject.transform.SetParent(visualObject.transform, false);

        pocketPalSpriteTransform = spriteObject.GetComponent<RectTransform>();
        pocketPalSpriteTransform.anchorMin = new Vector2(0.5f, 0.5f);
        pocketPalSpriteTransform.anchorMax = new Vector2(0.5f, 0.5f);
        pocketPalSpriteTransform.pivot = new Vector2(0.5f, 0.5f);
        pocketPalSpriteTransform.sizeDelta = new Vector2(285f, 285f);
        pocketPalSpriteTransform.anchoredPosition = GetPocketPalBodyOffset();

        Image pandaImage = spriteObject.GetComponent<Image>();
        pandaImage.sprite = GetSelectedPocketPalSprite();
        pandaImage.preserveAspect = true;
        pandaImage.raycastTarget = false;

        CreateAccessoryLayers(visualObject.transform);
    }

    private void CreateAccessoryLayers(Transform visualParent)
    {
        string pocketPalId = SaveManager.Data != null ? SaveManager.Data.selectedPocketPalId : "panda";
        string eyewearId = GetEquippedAccessoryInCategory(EyewearAccessoryIds);
        string headwearId = GetEquippedAccessoryInCategory(HeadwearAccessoryIds);
        string shirtId = GetEquippedAccessoryInCategory(ShirtAccessoryIds);
        string handheldId = GetEquippedAccessoryInCategory(HandheldAccessoryIds);

        Sprite shirtSprite = GetAccessorySprite(shirtId, pocketPalId);
        Sprite handheldSprite = GetAccessorySprite(handheldId, pocketPalId);
        Sprite headwearSprite = GetAccessorySprite(headwearId, pocketPalId);
        Sprite eyewearSprite = GetAccessorySprite(eyewearId, pocketPalId);

        float textureToDisplayScale = GetAccessoryTextureToDisplayScale();
        float offsetScale = GetAccessoryOffsetScale(textureToDisplayScale);
        float shirtOffsetX = 0f;
        float shirtOffsetY = -52f;
        float shirtMultiplier = 0.90f;
        Vector2 shirtFallbackSize = new Vector2(186f, 122f);
        float handheldOffsetX = 112f;
        float handheldOffsetY = -22f;
        float headwearOffsetX = 0f;
        float headwearOffsetY = 114f;
        float headwearMultiplier = 1f;
        Vector2 headwearFallbackSize = new Vector2(190f, 130f);
        float eyewearOffsetX = 7f;
        float eyewearOffsetY = 56f;
        float eyewearMultiplier = 0.84f;
        Vector2 eyewearFallbackSize = new Vector2(126f, 66f);

        if (headwearId == "headwear_chef_hat" || headwearId == "headwear_crown" || headwearId == "headwear_straw_hat" || headwearId == "headwear_red_cap")
        {
          //  headwearOffsetX = 2f;
            headwearOffsetY = 160f;

        }
        else if (headwearId == "headwear_blue_benie")
        {
            headwearOffsetY = 160f;
        }
        else if (headwearId == "headwear_dino_hood" || headwearId == "headwear_aviator_hat")
        {
            headwearOffsetY = 100f;
        }

    
        // Some wider glasses styles need extra scale but should keep the same Y alignment.
        if (eyewearId == "eyewear_pixel_shades")
        {
            eyewearMultiplier *= 1.16f;
            eyewearFallbackSize = new Vector2(eyewearFallbackSize.x * 1.16f, eyewearFallbackSize.y * 1.16f);
        }
        else if (eyewearId == "eyewear_black_sunglasses")
        {
            eyewearMultiplier *= 1.10f;
            eyewearFallbackSize = new Vector2(eyewearFallbackSize.x * 1.10f, eyewearFallbackSize.y * 1.10f);
        }

        // Shirt-specific fit tuning.
        if (shirtId == "shirt_green_bamboo" || shirtId == "shirt_blue_paw" || shirtId == "shirt_black_heart" || shirtId == "shirt_yellow_panda")
        {
            shirtOffsetY -= 25f;
        }
        else if (shirtId == "shirt_red_hoodie")
        {
            shirtOffsetY -= 22f;
            shirtMultiplier *= 1.02f; // +5%

        }
        else if (shirtId == "shirt_blue_hawaiian")
        {   
            shirtMultiplier *= 1.02f;
            shirtOffsetY -= 25f;
        }
  

        if (handheldId == "prop_blue_paw_flag" || handheldId ==
        "prop_rainbow_flag" || handheldId ==
        "prop_red_heart_flag" || handheldId ==
        "prop_checkered_flag" || handheldId ==
        "prop_green_leaf_flag" || handheldId ==
        "prop_pirate_flag"|| handheldId ==
        "prop_panda_flag")
        {
            handheldOffsetX = 140f;
            handheldOffsetY = -30f;
        }

        // Layered accessories from the provided panda customization sheet.
        CreateAccessoryLayer(
            "ShirtAccessory",
            visualParent,
            shirtSprite,
            new Vector2(shirtOffsetX * offsetScale, shirtOffsetY * offsetScale),
            GetAccessoryLayerSize(shirtSprite, textureToDisplayScale, shirtMultiplier, shirtFallbackSize),
            !string.IsNullOrEmpty(shirtId));

        CreateAccessoryLayer(
            "HandheldAccessory",
            visualParent,
            handheldSprite,
            new Vector2(handheldOffsetX * offsetScale, handheldOffsetY * offsetScale),
            GetAccessoryLayerSize(handheldSprite, textureToDisplayScale, 1.06f, new Vector2(172f, 172f)),
            !string.IsNullOrEmpty(handheldId));

        CreateAccessoryLayer(
            "HeadwearAccessory",
            visualParent,
            headwearSprite,
            new Vector2(headwearOffsetX * offsetScale, headwearOffsetY * offsetScale),
            GetAccessoryLayerSize(headwearSprite, textureToDisplayScale, headwearMultiplier, headwearFallbackSize),
            !string.IsNullOrEmpty(headwearId));

        CreateAccessoryLayer(
            "EyewearAccessory",
            visualParent,
            eyewearSprite,
            new Vector2(eyewearOffsetX * offsetScale, eyewearOffsetY * offsetScale),
            GetAccessoryLayerSize(eyewearSprite, textureToDisplayScale, eyewearMultiplier, eyewearFallbackSize),
            !string.IsNullOrEmpty(eyewearId));
    }

    private string GetEquippedAccessoryInCategory(string[] accessoryIds)
    {
        for (int index = 0; index < accessoryIds.Length; index++)
        {
            if (HasEquippedAccessory(accessoryIds[index]))
            {
                return accessoryIds[index];
            }
        }

        return "";
    }

    private bool HasEquippedAccessory(string accessoryId)
    {
        return SaveManager.IsAccessoryEquippedForSelectedPocketPal(accessoryId);
    }

    private void CreateAccessoryLayer(string objectName, Transform parent, Sprite sprite, Vector2 anchoredPosition, Vector2 size, bool isVisible)
    {
        GameObject accessoryObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        accessoryObject.transform.SetParent(parent, false);
        accessoryObject.SetActive(isVisible && sprite != null);

        RectTransform accessoryTransform = accessoryObject.GetComponent<RectTransform>();
        accessoryTransform.anchorMin = new Vector2(0.5f, 0.5f);
        accessoryTransform.anchorMax = new Vector2(0.5f, 0.5f);
        accessoryTransform.pivot = new Vector2(0.5f, 0.5f);
        accessoryTransform.sizeDelta = size;
        accessoryTransform.anchoredPosition = anchoredPosition;

        Image accessoryImage = accessoryObject.GetComponent<Image>();
        accessoryImage.sprite = sprite;
        accessoryImage.preserveAspect = true;
        accessoryImage.raycastTarget = false;
    }

    private float GetAccessoryTextureToDisplayScale()
    {
        if (pocketPalSpriteTransform == null)
        {
            return 1f;
        }

        Sprite baseSprite = GetSelectedPocketPalSprite();
        if (baseSprite == null || baseSprite.rect.height <= 0.01f)
        {
            return 1f;
        }

        Vector2 fittedBaseSize = GetPreservedSize(baseSprite, pocketPalSpriteTransform.sizeDelta);
        return fittedBaseSize.y / baseSprite.rect.height;
    }

    private static float GetAccessoryOffsetScale(float textureToDisplayScale)
    {
        const float referenceScale = 285f / 234f;
        if (textureToDisplayScale <= 0.01f)
        {
            return 1f;
        }

        return Mathf.Clamp(textureToDisplayScale / referenceScale, 0.8f, 1.35f);
    }

    private static Vector2 GetAccessoryLayerSize(Sprite sprite, float textureToDisplayScale, float categoryMultiplier, Vector2 fallbackSize)
    {
        if (sprite == null || textureToDisplayScale <= 0.01f)
        {
            return fallbackSize;
        }

        float width = sprite.rect.width * textureToDisplayScale * categoryMultiplier;
        float height = sprite.rect.height * textureToDisplayScale * categoryMultiplier;
        return new Vector2(width, height);
    }

    private static Vector2 GetPreservedSize(Sprite sprite, Vector2 containerSize)
    {
        if (sprite == null || sprite.rect.height <= 0.01f || sprite.rect.width <= 0.01f)
        {
            return containerSize;
        }

        float spriteAspect = sprite.rect.width / sprite.rect.height;
        float containerAspect = Mathf.Abs(containerSize.y) > 0.01f ? containerSize.x / containerSize.y : spriteAspect;
        if (spriteAspect > containerAspect)
        {
            float width = containerSize.x;
            return new Vector2(width, width / spriteAspect);
        }

        float height = containerSize.y;
        return new Vector2(height * spriteAspect, height);
    }

    private static Sprite GetAccessorySprite(string accessoryId, string pocketPalId)
    {
        if (string.IsNullOrEmpty(accessoryId))
        {
            return null;
        }

        if (!AccessorySpriteResourceNames.ContainsKey(accessoryId))
        {
            return null;
        }

        string speciesPath = GetAccessoryResourcePath(accessoryId, pocketPalId);
        Sprite speciesSprite = GetCachedAccessorySprite(speciesPath);
        if (speciesSprite != null)
        {
            return speciesSprite;
        }

        if (pocketPalId != "panda")
        {
            string pandaFallbackPath = GetAccessoryResourcePath(accessoryId, "panda");
            Sprite fallbackSprite = GetCachedAccessorySprite(pandaFallbackPath);
            if (fallbackSprite != null)
            {
                return fallbackSprite;
            }
        }

        return null;
    }

    private static Sprite GetCachedAccessorySprite(string resourcePath)
    {
        if (string.IsNullOrEmpty(resourcePath))
        {
            return null;
        }

        if (accessorySpriteCache.ContainsKey(resourcePath))
        {
            return accessorySpriteCache[resourcePath];
        }

        Sprite sprite = TryCreateSpriteFromResource(resourcePath);
        if (sprite != null)
        {
            accessorySpriteCache[resourcePath] = sprite;
        }

        return sprite;
    }

    private static string GetAccessoryResourcePath(string accessoryId, string pocketPalId)
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

    private string GetPocketPalDisplayName()
    {
        if (!string.IsNullOrWhiteSpace(SaveManager.Data.selectedPocketPalName))
        {
            return SaveManager.Data.selectedPocketPalName;
        }

        string pocketPalId = SaveManager.Data.selectedPocketPalId;
        if (pocketPalId == "white_tiger")
        {
            return "White Tiger";
        }

        if (pocketPalId == "otter")
        {
            return "Otter";
        }

        return "Panda";
    }

    private void UpdateFacingDirection(Vector2 previousPosition, Vector2 nextPosition)
    {
        float horizontalMovement = nextPosition.x - previousPosition.x;
        if (Mathf.Abs(horizontalMovement) > 0.01f)
        {
            pocketPalFacingDirection = horizontalMovement >= 0f ? 1f : -1f;
        }
    }

    private void UpdatePocketPalAnimation(bool isMoving)
    {
        if (pocketPalVisualTransform == null)
        {
            return;
        }

        float speciesVisualScale = GetPocketPalVisualScale();
        pocketPalAnimationTimer += Time.deltaTime * (isMoving ? 7f : 3f);
        float bob = Mathf.Sin(pocketPalAnimationTimer) * (isMoving ? 10f : 4f);
        float squash = 1f + Mathf.Sin(pocketPalAnimationTimer * 2f) * (isMoving ? 0.035f : 0.015f);
        float stretch = 1f - Mathf.Sin(pocketPalAnimationTimer * 2f) * (isMoving ? 0.025f : 0.01f);

        if (!isMoving && habitatActivity == HabitatActivity.SleepingInHammock)
        {
            float sleepBob = Mathf.Sin(pocketPalAnimationTimer * 0.8f) * 2f;
            pocketPalVisualTransform.anchoredPosition = new Vector2(0f, 6f + sleepBob);
            pocketPalVisualTransform.localRotation = Quaternion.Euler(0f, 0f, -9f);
            pocketPalVisualTransform.localScale = new Vector3(pocketPalFacingDirection * 0.86f * speciesVisualScale, 0.82f * speciesVisualScale, 1f);

            if (pocketPalShadowTransform != null)
            {
                pocketPalShadowTransform.localScale = new Vector3(0.78f, 0.72f, 1f);
            }

            return;
        }

        pocketPalVisualTransform.anchoredPosition = new Vector2(0f, 12f + bob);
        pocketPalVisualTransform.localRotation = Quaternion.identity;
        pocketPalVisualTransform.localScale = new Vector3(pocketPalFacingDirection * squash * speciesVisualScale, stretch * speciesVisualScale, 1f);

        if (pocketPalShadowTransform != null)
        {
            float shadowScale = Mathf.Lerp(1.05f, 0.88f, Mathf.InverseLerp(-10f, 10f, bob));
            pocketPalShadowTransform.localScale = new Vector3(shadowScale, 1f, 1f);
        }
    }

    private float GetPocketPalVisualScale()
    {
        if (SaveManager.Data != null && SaveManager.Data.selectedPocketPalId == "white_tiger")
        {
            return 1.2f;
        }

        if (SaveManager.Data != null && SaveManager.Data.selectedPocketPalId == "otter")
        {
            return 1.3f;
        }

        return 1f;
    }

    private Vector2 GetPocketPalBodyOffset()
    {
        if (SaveManager.Data != null && SaveManager.Data.selectedPocketPalId == "white_tiger")
        {
            return new Vector2(-6f, 0f);
        }

        if (SaveManager.Data != null && SaveManager.Data.selectedPocketPalId == "otter")
        {
            return new Vector2(-10f, -13f);
        }

        return Vector2.zero;
    }

    private static void EnsurePocketPalSprites()
    {
        if (pandaSprite == null || pandaSpriteBuildVersion != PandaSpriteVersion)
        {
            if (pandaSprite != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(pandaSprite);
                }
                else
                {
                    DestroyImmediate(pandaSprite);
                }
            }

            pandaSprite = TryCreateSpriteFromResource(PandaBaseSpriteResourcePath);
            if (pandaSprite == null)
            {
                pandaSprite = Sprite.Create(CreatePandaTexture(), new Rect(0f, 0f, PandaTextureSize, PandaTextureSize), new Vector2(0.5f, 0.5f), 100f);
            }

            pandaSpriteBuildVersion = PandaSpriteVersion;
        }

        if (tigerSprite == null || whiteTigerSpriteBuildVersion != WhiteTigerSpriteVersion)
        {
            if (tigerSprite != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(tigerSprite);
                }
                else
                {
                    DestroyImmediate(tigerSprite);
                }
            }

            tigerSprite = TryCreateSpriteFromResource(WhiteTigerBaseSpriteResourcePath);
            if (tigerSprite == null)
            {
                tigerSprite = Sprite.Create(CreateWhiteTigerTexture(), new Rect(0f, 0f, PandaTextureSize, PandaTextureSize), new Vector2(0.5f, 0.5f), 100f);
            }

            whiteTigerSpriteBuildVersion = WhiteTigerSpriteVersion;
        }

        if (otterSprite == null)
        {
            otterSprite = TryCreateSpriteFromResource(OtterBaseSpriteResourcePath);
            if (otterSprite == null)
            {
                otterSprite = Sprite.Create(CreateOtterTexture(), new Rect(0f, 0f, PandaTextureSize, PandaTextureSize), new Vector2(0.5f, 0.5f), 100f);
            }
        }

        if (shadowSprite == null)
        {
            shadowSprite = Sprite.Create(CreateShadowTexture(), new Rect(0f, 0f, 128f, 48f), new Vector2(0.5f, 0.5f), 100f);
        }
    }

    private static Sprite TryCreateSpriteFromResource(string resourcePath)
    {
        Texture2D texture = Resources.Load<Texture2D>(resourcePath);
        if (texture == null)
        {
            return null;
        }

        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Bilinear;
        Rect rect = new Rect(0f, 0f, texture.width, texture.height);
        return Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), 100f);
    }

    private Sprite GetSelectedPocketPalSprite()
    {
        string pocketPalId = SaveManager.Data.selectedPocketPalId;
        if (pocketPalId == "white_tiger")
        {
            return tigerSprite;
        }

        if (pocketPalId == "otter")
        {
            return otterSprite;
        }

        return pandaSprite;
    }

    private static void EnsureButtonIcons()
    {
        if (customizationButtonIcon == null)
        {
            customizationButtonIcon = Sprite.Create(CreateCustomizationIconTexture(), new Rect(0f, 0f, ButtonIconTextureSize, ButtonIconTextureSize), new Vector2(0.5f, 0.5f), 100f);
        }

        if (rewardsButtonIcon == null)
        {
            rewardsButtonIcon = Sprite.Create(CreateRewardsIconTexture(), new Rect(0f, 0f, ButtonIconTextureSize, ButtonIconTextureSize), new Vector2(0.5f, 0.5f), 100f);
        }
    }

    private static Texture2D CreatePandaTexture()
    {
        Texture2D texture = new Texture2D(PandaTextureSize, PandaTextureSize, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[PandaTextureSize * PandaTextureSize];
        for (int index = 0; index < pixels.Length; index++)
        {
            pixels[index] = Color.clear;
        }

        texture.SetPixels(pixels);

        Color black = new Color(0.08f, 0.09f, 0.11f, 1f);
        Color blackSoft = new Color(0.15f, 0.16f, 0.2f, 1f);
        Color white = new Color(0.985f, 0.985f, 0.975f, 1f);
        Color whiteShadow = new Color(0.84f, 0.88f, 0.93f, 1f);
        Color muzzle = new Color(0.99f, 0.965f, 0.93f, 1f);
        Color blush = new Color(1f, 0.79f, 0.81f, 0.58f);
        Color nose = new Color(0.24f, 0.18f, 0.21f, 1f);
        Color mouth = new Color(0.3f, 0.2f, 0.24f, 1f);
        Color tongue = new Color(0.95f, 0.36f, 0.42f, 1f);

        // Body.
        DrawEllipse(texture, 128, 92, 62, 72, black);
        DrawEllipse(texture, 128, 97, 52, 64, whiteShadow);
        DrawEllipse(texture, 128, 104, 48, 56, white);
        DrawEllipse(texture, 128, 88, 34, 44, muzzle);

        // Feet.
        DrawEllipse(texture, 97, 34, 21, 14, black);
        DrawEllipse(texture, 159, 34, 21, 14, black);
        DrawEllipse(texture, 97, 34, 16, 10, white);
        DrawEllipse(texture, 159, 34, 16, 10, white);

        // Head and ears.
        DrawEllipse(texture, 128, 172, 78, 68, whiteShadow);
        DrawEllipse(texture, 128, 178, 70, 60, white);
        DrawEllipse(texture, 86, 212, 23, 21, black);
        DrawEllipse(texture, 170, 212, 23, 21, black);
        DrawEllipse(texture, 86, 212, 14, 13, blackSoft);
        DrawEllipse(texture, 170, 212, 14, 13, blackSoft);
        DrawEllipse(texture, 128, 224, 10, 7, white);
        DrawEllipse(texture, 120, 221, 5, 5, white);
        DrawEllipse(texture, 136, 221, 5, 5, white);

        // Front paws (short and rounded).
        DrawRoundedRect(texture, 78, 56, 24, 56, 12, black);
        DrawRoundedRect(texture, 154, 56, 24, 56, 12, black);
        DrawEllipse(texture, 90, 57, 8, 7, blackSoft);
        DrawEllipse(texture, 166, 57, 8, 7, blackSoft);

        // Eye patches.
        DrawEllipse(texture, 101, 168, 23, 25, black);
        DrawEllipse(texture, 155, 168, 23, 25, black);

        // Eyes.
        DrawEllipse(texture, 102, 168, 12, 13, Color.white);
        DrawEllipse(texture, 154, 168, 12, 13, Color.white);
        DrawEllipse(texture, 102, 167, 7, 8, black);
        DrawEllipse(texture, 154, 167, 7, 8, black);
        DrawEllipse(texture, 106, 171, 3, 3, Color.white);
        DrawEllipse(texture, 158, 171, 3, 3, Color.white);

        // Muzzle, nose, smile.
        DrawEllipse(texture, 128, 150, 33, 23, muzzle);
        DrawEllipse(texture, 112, 148, 13, 11, white);
        DrawEllipse(texture, 144, 148, 13, 11, white);
        DrawEllipse(texture, 128, 152, 9, 6, nose);
        DrawRoundedRect(texture, 126, 145, 4, 7, 2, mouth);
        DrawEllipse(texture, 121, 145, 5, 3, mouth);
        DrawEllipse(texture, 135, 145, 5, 3, mouth);
        DrawEllipse(texture, 128, 141, 5, 4, tongue);

        // Cheeks and soft highlight.
        DrawEllipse(texture, 97, 158, 10, 7, blush);
        DrawEllipse(texture, 159, 158, 10, 7, blush);
        DrawEllipse(texture, 108, 204, 18, 10, new Color(1f, 1f, 1f, 0.24f));

        texture.Apply();
        return texture;
    }

    private static Texture2D CreateWhiteTigerTexture()
    {
        Texture2D texture = new Texture2D(PandaTextureSize, PandaTextureSize, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[PandaTextureSize * PandaTextureSize];
        for (int index = 0; index < pixels.Length; index++)
        {
            pixels[index] = Color.clear;
        }

        texture.SetPixels(pixels);

        Color furWhite = new Color(0.985f, 0.985f, 0.975f, 1f);
        Color furShadow = new Color(0.82f, 0.87f, 0.93f, 1f);
        Color cream = new Color(0.99f, 0.95f, 0.86f, 1f);
        Color stripe = new Color(0.23f, 0.27f, 0.34f, 0.96f);
        Color innerEar = new Color(0.96f, 0.77f, 0.82f, 1f);
        Color eyeRing = new Color(0.72f, 0.76f, 0.84f, 0.92f);
        Color eyeWhite = new Color(0.995f, 0.995f, 1f, 1f);
        Color pupil = new Color(0.08f, 0.08f, 0.1f, 1f);
        Color nose = new Color(0.72f, 0.54f, 0.59f, 1f);
        Color blush = new Color(1f, 0.8f, 0.8f, 0.55f);
        Color mouth = new Color(0.31f, 0.21f, 0.24f, 1f);
        Color pawDark = new Color(0.27f, 0.31f, 0.39f, 1f);

        // Tail behind body.
        DrawEllipse(texture, 199, 104, 20, 50, furShadow);
        DrawEllipse(texture, 199, 100, 16, 44, furWhite);
        DrawEllipse(texture, 194, 126, 5, 8, stripe);
        DrawEllipse(texture, 194, 104, 5, 8, stripe);
        DrawEllipse(texture, 194, 82, 5, 8, stripe);

        // Body base.
        DrawEllipse(texture, 128, 90, 64, 72, furShadow);
        DrawEllipse(texture, 128, 98, 56, 63, furWhite);
        DrawEllipse(texture, 128, 86, 34, 47, cream);

        // Back feet.
        DrawEllipse(texture, 99, 34, 22, 14, furShadow);
        DrawEllipse(texture, 157, 34, 22, 14, furShadow);
        DrawEllipse(texture, 99, 34, 16, 10, furWhite);
        DrawEllipse(texture, 157, 34, 16, 10, furWhite);

        // Head and ears.
        DrawEllipse(texture, 128, 171, 76, 67, furShadow);
        DrawEllipse(texture, 128, 178, 68, 58, furWhite);
        DrawEllipse(texture, 86, 212, 23, 21, furShadow);
        DrawEllipse(texture, 170, 212, 23, 21, furShadow);
        DrawEllipse(texture, 86, 212, 17, 15, furWhite);
        DrawEllipse(texture, 170, 212, 17, 15, furWhite);
        DrawEllipse(texture, 86, 210, 9, 7, innerEar);
        DrawEllipse(texture, 170, 210, 9, 7, innerEar);

        // Front paws (shorter and lower to avoid face overlap).
        DrawRoundedRect(texture, 79, 56, 27, 60, 12, pawDark);
        DrawRoundedRect(texture, 150, 56, 27, 60, 12, pawDark);
        DrawEllipse(texture, 92, 58, 10, 8, furWhite);
        DrawEllipse(texture, 164, 58, 10, 8, furWhite);

        // Muzzle and cheeks.
        DrawEllipse(texture, 128, 154, 37, 26, cream);
        DrawEllipse(texture, 111, 151, 15, 13, furWhite);
        DrawEllipse(texture, 145, 151, 15, 13, furWhite);
        DrawEllipse(texture, 97, 159, 9, 6, blush);
        DrawEllipse(texture, 159, 159, 9, 6, blush);

        // Eyes.
        DrawEllipse(texture, 104, 170, 14, 15, eyeRing);
        DrawEllipse(texture, 152, 170, 14, 15, eyeRing);
        DrawEllipse(texture, 104, 169, 11, 12, eyeWhite);
        DrawEllipse(texture, 152, 169, 11, 12, eyeWhite);
        DrawEllipse(texture, 104, 167, 5, 6, pupil);
        DrawEllipse(texture, 152, 167, 5, 6, pupil);
        DrawEllipse(texture, 107, 172, 2, 2, Color.white);
        DrawEllipse(texture, 155, 172, 2, 2, Color.white);

        // Nose and mouth.
        DrawEllipse(texture, 128, 154, 9, 6, nose);
        DrawRoundedRect(texture, 126, 146, 4, 8, 2, mouth);
        DrawEllipse(texture, 121, 145, 5, 3, mouth);
        DrawEllipse(texture, 135, 145, 5, 3, mouth);

        // Head stripes (kept above/around eyes, never through them).
        DrawRoundedRect(texture, 123, 197, 10, 13, 5, stripe);
        DrawRoundedRect(texture, 111, 189, 8, 13, 4, stripe);
        DrawRoundedRect(texture, 137, 189, 8, 13, 4, stripe);
        DrawRoundedRect(texture, 84, 164, 7, 18, 3, stripe);
        DrawRoundedRect(texture, 165, 164, 7, 18, 3, stripe);

        // Body spots.
        DrawEllipse(texture, 75, 129, 6, 9, stripe);
        DrawEllipse(texture, 82, 108, 5, 7, stripe);
        DrawEllipse(texture, 181, 129, 6, 9, stripe);
        DrawEllipse(texture, 174, 108, 5, 7, stripe);

        texture.Apply();
        return texture;
    }

    private static Texture2D CreateOtterTexture()
    {
        Texture2D texture = new Texture2D(PandaTextureSize, PandaTextureSize, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[PandaTextureSize * PandaTextureSize];
        for (int index = 0; index < pixels.Length; index++)
        {
            pixels[index] = Color.clear;
        }

        texture.SetPixels(pixels);

        Color brown = new Color(0.55f, 0.36f, 0.22f, 1f);
        Color darkBrown = new Color(0.34f, 0.21f, 0.12f, 1f);
        Color cream = new Color(0.9f, 0.82f, 0.68f, 1f);
        Color nose = new Color(0.16f, 0.1f, 0.08f, 1f);

        DrawEllipse(texture, 128, 98, 54, 76, brown);
        DrawEllipse(texture, 128, 110, 46, 62, cream);
        DrawEllipse(texture, 128, 170, 64, 66, brown);
        DrawEllipse(texture, 128, 178, 54, 54, cream);
        DrawEllipse(texture, 86, 174, 26, 36, brown);
        DrawEllipse(texture, 170, 174, 26, 36, brown);
        DrawEllipse(texture, 92, 92, 22, 42, darkBrown);
        DrawEllipse(texture, 164, 92, 22, 42, darkBrown);

        DrawEllipse(texture, 84, 48, 26, 22, brown);
        DrawEllipse(texture, 172, 48, 26, 22, brown);
        DrawEllipse(texture, 84, 48, 13, 11, cream);
        DrawEllipse(texture, 172, 48, 13, 11, cream);

        DrawEllipse(texture, 96, 166, 9, 13, Color.white);
        DrawEllipse(texture, 160, 166, 9, 13, Color.white);
        DrawEllipse(texture, 96, 166, 4, 5, nose);
        DrawEllipse(texture, 160, 166, 4, 5, nose);
        DrawEllipse(texture, 128, 146, 12, 8, nose);
        DrawEllipse(texture, 108, 146, 16, 18, cream);
        DrawEllipse(texture, 148, 146, 16, 18, cream);
        DrawRoundedRect(texture, 70, 30, 116, 16, 8, darkBrown);
        DrawEllipse(texture, 196, 110, 18, 48, brown);

        texture.Apply();
        return texture;
    }

    private static Texture2D CreateCustomizationIconTexture()
    {
        Texture2D texture = CreateTransparentTexture(ButtonIconTextureSize, ButtonIconTextureSize);
        Color palette = new Color(0.98f, 0.92f, 0.68f, 1f);
        Color outline = new Color(0.09f, 0.18f, 0.16f, 1f);
        Color red = new Color(0.9f, 0.2f, 0.22f, 1f);
        Color blue = new Color(0.18f, 0.44f, 0.9f, 1f);
        Color green = new Color(0.18f, 0.65f, 0.32f, 1f);
        Color brush = new Color(0.48f, 0.28f, 0.15f, 1f);

        DrawEllipse(texture, 60, 65, 42, 34, outline);
        DrawEllipse(texture, 60, 65, 36, 28, palette);
        DrawEllipse(texture, 47, 74, 8, 8, red);
        DrawEllipse(texture, 68, 76, 8, 8, blue);
        DrawEllipse(texture, 55, 55, 8, 8, green);
        DrawEllipse(texture, 79, 58, 9, 9, Color.clear);
        DrawRoundedRect(texture, 78, 30, 13, 58, 6, brush);
        DrawRoundedRect(texture, 72, 82, 25, 13, 6, outline);

        texture.Apply();
        return texture;
    }

    private static Texture2D CreateRewardsIconTexture()
    {
        Texture2D texture = CreateTransparentTexture(ButtonIconTextureSize, ButtonIconTextureSize);
        Color box = new Color(0.98f, 0.7f, 0.28f, 1f);
        Color ribbon = new Color(0.18f, 0.55f, 0.88f, 1f);
        Color outline = new Color(0.08f, 0.18f, 0.22f, 1f);
        Color coin = new Color(1f, 0.88f, 0.24f, 1f);

        DrawRoundedRect(texture, 28, 48, 72, 48, 8, outline);
        DrawRoundedRect(texture, 33, 53, 62, 38, 6, box);
        DrawRoundedRect(texture, 59, 48, 12, 48, 4, ribbon);
        DrawRoundedRect(texture, 28, 43, 72, 14, 5, ribbon);
        DrawEllipse(texture, 48, 37, 16, 10, ribbon);
        DrawEllipse(texture, 80, 37, 16, 10, ribbon);
        DrawEllipse(texture, 64, 102, 18, 18, outline);
        DrawEllipse(texture, 64, 102, 14, 14, coin);
        DrawRoundedRect(texture, 60, 91, 8, 22, 4, new Color(0.82f, 0.58f, 0.08f, 1f));

        texture.Apply();
        return texture;
    }

    private static Texture2D CreateShadowTexture()
    {
        Texture2D texture = new Texture2D(128, 48, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[128 * 48];
        for (int index = 0; index < pixels.Length; index++)
        {
            pixels[index] = Color.clear;
        }

        texture.SetPixels(pixels);
        DrawEllipse(texture, 64, 24, 58, 17, Color.white);
        texture.Apply();
        return texture;
    }

    private static Sprite GetHabitatBackgroundSprite(string habitatId)
    {
        string safeHabitatId = string.IsNullOrEmpty(habitatId) ? "sunny_garden" : habitatId;
        if (!habitatBackgroundSprites.ContainsKey(safeHabitatId))
        {
            Texture2D texture = CreateHabitatBackgroundTexture(safeHabitatId);
            habitatBackgroundSprites[safeHabitatId] = Sprite.Create(texture, new Rect(0f, 0f, HabitatBackgroundWidth, HabitatBackgroundHeight), new Vector2(0.5f, 0.5f), 100f);
        }

        return habitatBackgroundSprites[safeHabitatId];
    }

    private static Texture2D CreateHabitatBackgroundTexture(string habitatId)
    {
        if (habitatId == "moonlit_pond")
        {
            return CreateMoonlitPondBackgroundTexture();
        }

        return CreateSunnyPlaypenBackgroundTexture();
    }

    private static Texture2D CreateSunnyPlaypenBackgroundTexture()
    {
        Texture2D texture = new Texture2D(HabitatBackgroundWidth, HabitatBackgroundHeight, TextureFormat.RGBA32, false);
        Color skyTop = new Color(0.62f, 0.85f, 0.96f, 1f);
        Color skyBottom = new Color(0.76f, 0.92f, 0.98f, 1f);
        Color habitatGrass = new Color(0.46f, 0.76f, 0.42f, 1f);
        Color meadow = new Color(0.58f, 0.84f, 0.46f, 1f);
        Color lavenderFence = new Color(0.61f, 0.47f, 0.7f, 1f);
        Color fenceShadow = new Color(0.43f, 0.32f, 0.52f, 1f);

        for (int y = 0; y < HabitatBackgroundHeight; y++)
        {
            float vertical = y / (float)(HabitatBackgroundHeight - 1);
            Color color = vertical > 0.46f
                ? Color.Lerp(skyBottom, skyTop, Mathf.InverseLerp(0.46f, 1f, vertical))
                : Color.Lerp(new Color(0.42f, 0.74f, 0.42f, 1f), habitatGrass, vertical / 0.46f);

            for (int x = 0; x < HabitatBackgroundWidth; x++)
            {
                texture.SetPixel(x, y, color);
            }
        }

        DrawSun(texture, 164, 1602, 66);
        DrawCloud(texture, 360, 1540, 0.9f);
        DrawCloud(texture, 748, 1478, 0.72f);
        DrawCloud(texture, 885, 1660, 0.5f);

        DrawEllipse(texture, 250, 950, 430, 90, new Color(0.38f, 0.69f, 0.38f, 1f));
        DrawEllipse(texture, 830, 940, 450, 95, new Color(0.36f, 0.66f, 0.37f, 1f));
        DrawRoundedRect(texture, 0, 980, HabitatBackgroundWidth, 120, 0, lavenderFence);
        DrawRoundedRect(texture, 0, 970, HabitatBackgroundWidth, 16, 0, fenceShadow);

        for (int post = -1; post < 7; post++)
        {
            int x = 80 + post * 185;
            DrawRoundedRect(texture, x, 965, 34, 210, 8, lavenderFence);
            DrawRoundedRect(texture, x + 24, 965, 10, 210, 4, fenceShadow);
        }

        DrawZooTree(texture, 104, 985, 0.92f);
        DrawZooTree(texture, 930, 1000, 1.05f);
        DrawZooTree(texture, 814, 850, 0.72f);
        DrawZooTree(texture, 142, 780, 0.72f);

        DrawEllipse(texture, 540, 705, 410, 155, meadow);
        DrawToyBall(texture, 760, 650, 52);

        DrawHabitatRock(texture, 218, 380, 1.05f);
        DrawHabitatRock(texture, 854, 430, 0.82f);
        DrawHabitatRock(texture, 734, 310, 0.55f);
        texture.Apply();
        return texture;
    }

    private static Texture2D CreateMoonlitPondBackgroundTexture()
    {
        Texture2D texture = new Texture2D(HabitatBackgroundWidth, HabitatBackgroundHeight, TextureFormat.RGBA32, false);
        Color skyTop = new Color(0.08f, 0.1f, 0.22f, 1f);
        Color skyBottom = new Color(0.18f, 0.23f, 0.38f, 1f);
        Color grass = new Color(0.22f, 0.45f, 0.34f, 1f);
        Color meadow = new Color(0.28f, 0.53f, 0.36f, 1f);
        Color pond = new Color(0.16f, 0.37f, 0.58f, 1f);
        Color moon = new Color(0.98f, 0.94f, 0.72f, 1f);
        Color fence = new Color(0.43f, 0.34f, 0.58f, 1f);

        for (int y = 0; y < HabitatBackgroundHeight; y++)
        {
            float vertical = y / (float)(HabitatBackgroundHeight - 1);
            Color color = vertical > 0.48f
                ? Color.Lerp(skyBottom, skyTop, Mathf.InverseLerp(0.48f, 1f, vertical))
                : Color.Lerp(new Color(0.15f, 0.32f, 0.29f, 1f), grass, vertical / 0.48f);

            for (int x = 0; x < HabitatBackgroundWidth; x++)
            {
                texture.SetPixel(x, y, color);
            }
        }

        DrawEllipse(texture, 820, 1585, 74, 74, moon);
        DrawEllipse(texture, 850, 1612, 70, 70, skyTop);
        DrawEllipse(texture, 820, 390, 76, 18, new Color(0.98f, 0.94f, 0.72f, 0.24f));
        DrawEllipse(texture, 800, 430, 128, 16, new Color(0.98f, 0.94f, 0.72f, 0.14f));
        DrawStar(texture, 170, 1570, 16);
        DrawStar(texture, 300, 1450, 10);
        DrawStar(texture, 600, 1640, 12);
        DrawStar(texture, 950, 1375, 9);

        DrawRoundedRect(texture, 0, 980, HabitatBackgroundWidth, 120, 0, fence);
        for (int post = -1; post < 7; post++)
        {
            int x = 80 + post * 185;
            DrawRoundedRect(texture, x, 965, 34, 210, 8, fence);
        }

        DrawZooTree(texture, 116, 980, 0.88f);
        DrawZooTree(texture, 935, 990, 1.02f);
        DrawZooTree(texture, 760, 820, 0.72f);
        DrawEllipse(texture, 540, 705, 410, 155, meadow);
        DrawEllipse(texture, 540, 472, 330, 92, pond);
        DrawRoundedRect(texture, 264, 486, 552, 18, 9, new Color(0.36f, 0.6f, 0.78f, 0.38f));
        DrawRoundedRect(texture, 376, 518, 328, 12, 6, new Color(0.98f, 0.94f, 0.72f, 0.18f));
        DrawRoundedRect(texture, 424, 452, 244, 10, 5, new Color(0.62f, 0.78f, 0.92f, 0.2f));
        DrawHabitatRock(texture, 218, 380, 1.05f);
        DrawHabitatRock(texture, 854, 430, 0.82f);

        texture.Apply();
        return texture;
    }

    private static void DrawHammock(Texture2D texture, int centerX, int centerY)
    {
        Color rope = new Color(0.82f, 0.72f, 0.48f, 1f);
        Color fabric = new Color(0.68f, 0.56f, 0.9f, 1f);
        Color fabricShadow = new Color(0.43f, 0.34f, 0.72f, 0.9f);
        Color fabricHighlight = new Color(0.9f, 0.84f, 1f, 0.92f);
        Color stitch = new Color(0.32f, 0.26f, 0.58f, 0.9f);

        DrawThickLine(texture, centerX - 310, centerY + 84, centerX - 160, centerY - 18, 10, rope);
        DrawThickLine(texture, centerX + 310, centerY + 84, centerX + 160, centerY - 18, 10, rope);
        DrawThickLine(texture, centerX - 282, centerY + 66, centerX - 146, centerY - 4, 4, new Color(0.96f, 0.88f, 0.58f, 0.95f));
        DrawThickLine(texture, centerX + 282, centerY + 66, centerX + 146, centerY - 4, 4, new Color(0.96f, 0.88f, 0.58f, 0.95f));
        DrawEllipse(texture, centerX, centerY - 44, 222, 68, fabricShadow);
        DrawEllipse(texture, centerX, centerY - 26, 214, 62, fabric);
        DrawRoundedRect(texture, centerX - 152, centerY - 36, 304, 22, 11, fabricHighlight);
        DrawRoundedRect(texture, centerX - 132, centerY - 62, 264, 10, 5, stitch);
        for (int stripe = 0; stripe < 5; stripe++)
        {
            int x = centerX - 96 + stripe * 48;
            DrawRoundedRect(texture, x, centerY - 72, 12, 80, 6, new Color(0.82f, 0.74f, 1f, 0.65f));
        }

        DrawRoundedRect(texture, centerX - 176, centerY - 10, 352, 8, 4, stitch);
        DrawEllipse(texture, centerX - 190, centerY - 8, 34, 22, rope);
        DrawEllipse(texture, centerX + 190, centerY - 8, 34, 22, rope);
    }

    private static void DrawSun(Texture2D texture, int centerX, int centerY, int radius)
    {
        Color ray = new Color(1f, 0.82f, 0.2f, 0.42f);
        Color sun = new Color(1f, 0.84f, 0.22f, 1f);
        Color glow = new Color(1f, 0.92f, 0.38f, 0.45f);

        DrawEllipse(texture, centerX, centerY, radius + 34, radius + 34, glow);
        for (int index = 0; index < 12; index++)
        {
            float angle = index * Mathf.PI * 2f / 12f;
            int innerX = centerX + Mathf.RoundToInt(Mathf.Cos(angle) * (radius + 8));
            int innerY = centerY + Mathf.RoundToInt(Mathf.Sin(angle) * (radius + 8));
            int outerX = centerX + Mathf.RoundToInt(Mathf.Cos(angle) * (radius + 42));
            int outerY = centerY + Mathf.RoundToInt(Mathf.Sin(angle) * (radius + 42));
            DrawThickLine(texture, innerX, innerY, outerX, outerY, 7, ray);
        }

        DrawEllipse(texture, centerX, centerY, radius, radius, sun);
        DrawEllipse(texture, centerX - radius / 3, centerY + radius / 3, radius / 4, radius / 6, new Color(1f, 0.97f, 0.7f, 0.75f));
    }

    private static void DrawCloud(Texture2D texture, int centerX, int centerY, float scale)
    {
        Color shadow = new Color(0.7f, 0.82f, 0.88f, 0.38f);
        Color cloud = new Color(1f, 1f, 1f, 0.86f);

        DrawEllipse(texture, centerX, centerY - Mathf.RoundToInt(12f * scale), Mathf.RoundToInt(112f * scale), Mathf.RoundToInt(32f * scale), shadow);
        DrawEllipse(texture, centerX - Mathf.RoundToInt(52f * scale), centerY, Mathf.RoundToInt(48f * scale), Mathf.RoundToInt(34f * scale), cloud);
        DrawEllipse(texture, centerX - Mathf.RoundToInt(8f * scale), centerY + Mathf.RoundToInt(22f * scale), Mathf.RoundToInt(58f * scale), Mathf.RoundToInt(44f * scale), cloud);
        DrawEllipse(texture, centerX + Mathf.RoundToInt(54f * scale), centerY + Mathf.RoundToInt(4f * scale), Mathf.RoundToInt(52f * scale), Mathf.RoundToInt(36f * scale), cloud);
        DrawRoundedRect(texture, centerX - Mathf.RoundToInt(88f * scale), centerY - Mathf.RoundToInt(18f * scale), Mathf.RoundToInt(176f * scale), Mathf.RoundToInt(36f * scale), Mathf.RoundToInt(18f * scale), cloud);
    }

    private static void DrawMoonlitBed(Texture2D texture, int centerX, int centerY)
    {
        Color shadow = new Color(0.04f, 0.08f, 0.12f, 0.28f);
        Color frame = new Color(0.45f, 0.31f, 0.2f, 1f);
        Color mattress = new Color(0.74f, 0.69f, 0.9f, 1f);
        Color blanket = new Color(0.5f, 0.42f, 0.78f, 1f);
        Color pillow = new Color(0.9f, 0.88f, 1f, 1f);

        DrawEllipse(texture, centerX + 6, centerY - 48, 162, 28, shadow);
        DrawRoundedRect(texture, centerX - 150, centerY - 38, 300, 42, 16, frame);
        DrawRoundedRect(texture, centerX - 132, centerY - 14, 264, 58, 20, mattress);
        DrawRoundedRect(texture, centerX - 118, centerY + 4, 172, 50, 16, blanket);
        DrawRoundedRect(texture, centerX + 52, centerY + 10, 70, 34, 15, pillow);
        DrawRoundedRect(texture, centerX - 148, centerY - 58, 24, 56, 8, frame);
        DrawRoundedRect(texture, centerX + 124, centerY - 58, 24, 56, 8, frame);
        DrawRoundedRect(texture, centerX - 92, centerY + 16, 108, 8, 4, new Color(0.82f, 0.76f, 1f, 0.8f));
    }

    private static void DrawToyBall(Texture2D texture, int centerX, int centerY, int radius)
    {
        Color ball = new Color(0.93f, 0.34f, 0.3f, 1f);
        Color stripe = new Color(0.98f, 0.9f, 0.28f, 1f);
        Color shine = new Color(1f, 0.94f, 0.84f, 0.75f);
        Color shadow = new Color(0.12f, 0.28f, 0.25f, 0.22f);

        DrawEllipse(texture, centerX, centerY - radius - 14, radius + 24, 18, shadow);
        DrawEllipse(texture, centerX, centerY, radius, radius, ball);
        DrawRoundedRect(texture, centerX - radius, centerY - 8, radius * 2, 16, 8, stripe);
        DrawEllipse(texture, centerX - radius / 3, centerY + radius / 3, radius / 4, radius / 6, shine);
    }

    private static void DrawThickLine(Texture2D texture, int startX, int startY, int endX, int endY, int radius, Color color)
    {
        int steps = Mathf.Max(Mathf.Abs(endX - startX), Mathf.Abs(endY - startY));
        for (int index = 0; index <= steps; index++)
        {
            float t = steps == 0 ? 0f : index / (float)steps;
            int x = Mathf.RoundToInt(Mathf.Lerp(startX, endX, t));
            int y = Mathf.RoundToInt(Mathf.Lerp(startY, endY, t));
            DrawEllipse(texture, x, y, radius, radius, color);
        }
    }

    private static void DrawStar(Texture2D texture, int centerX, int centerY, int radius)
    {
        Color star = new Color(0.98f, 0.94f, 0.72f, 0.9f);
        DrawEllipse(texture, centerX, centerY, radius, Mathf.Max(2, radius / 4), star);
        DrawEllipse(texture, centerX, centerY, Mathf.Max(2, radius / 4), radius, star);
    }

    private static void DrawZooTree(Texture2D texture, int baseX, int baseY, float scale)
    {
        Color trunk = new Color(0.45f, 0.32f, 0.2f, 1f);
        Color trunkShadow = new Color(0.3f, 0.21f, 0.13f, 1f);
        Color leafDark = new Color(0.2f, 0.52f, 0.27f, 1f);
        Color leafLight = new Color(0.36f, 0.68f, 0.33f, 1f);
        int trunkWidth = Mathf.RoundToInt(54f * scale);
        int trunkHeight = Mathf.RoundToInt(210f * scale);

        DrawRoundedRect(texture, baseX - trunkWidth / 2, baseY - trunkHeight, trunkWidth, trunkHeight, Mathf.RoundToInt(16f * scale), trunk);
        DrawRoundedRect(texture, baseX + trunkWidth / 5, baseY - trunkHeight, Mathf.Max(6, trunkWidth / 4), trunkHeight, Mathf.RoundToInt(8f * scale), trunkShadow);
        DrawEllipse(texture, baseX - Mathf.RoundToInt(54f * scale), baseY + Mathf.RoundToInt(8f * scale), Mathf.RoundToInt(102f * scale), Mathf.RoundToInt(86f * scale), leafDark);
        DrawEllipse(texture, baseX + Mathf.RoundToInt(52f * scale), baseY + Mathf.RoundToInt(20f * scale), Mathf.RoundToInt(112f * scale), Mathf.RoundToInt(92f * scale), leafDark);
        DrawEllipse(texture, baseX, baseY + Mathf.RoundToInt(72f * scale), Mathf.RoundToInt(132f * scale), Mathf.RoundToInt(108f * scale), leafLight);
        DrawEllipse(texture, baseX - Mathf.RoundToInt(38f * scale), baseY + Mathf.RoundToInt(106f * scale), Mathf.RoundToInt(74f * scale), Mathf.RoundToInt(48f * scale), new Color(0.53f, 0.78f, 0.4f, 0.9f));
    }

    private static void DrawHabitatRock(Texture2D texture, int centerX, int centerY, float scale)
    {
        Color rock = new Color(0.56f, 0.61f, 0.58f, 1f);
        Color shadow = new Color(0.39f, 0.45f, 0.43f, 1f);
        Color highlight = new Color(0.72f, 0.77f, 0.73f, 0.85f);

        DrawEllipse(texture, centerX, centerY, Mathf.RoundToInt(92f * scale), Mathf.RoundToInt(42f * scale), shadow);
        DrawEllipse(texture, centerX - Mathf.RoundToInt(28f * scale), centerY + Mathf.RoundToInt(24f * scale), Mathf.RoundToInt(70f * scale), Mathf.RoundToInt(52f * scale), rock);
        DrawEllipse(texture, centerX + Mathf.RoundToInt(34f * scale), centerY + Mathf.RoundToInt(18f * scale), Mathf.RoundToInt(62f * scale), Mathf.RoundToInt(44f * scale), rock);
        DrawEllipse(texture, centerX - Mathf.RoundToInt(48f * scale), centerY + Mathf.RoundToInt(46f * scale), Mathf.RoundToInt(22f * scale), Mathf.RoundToInt(12f * scale), highlight);
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
                float distance = Vector2.Distance(new Vector2(pixelX, pixelY), new Vector2(cornerX, cornerY));
                if (distance > radius)
                {
                    continue;
                }

                float edgeFade = Mathf.Clamp01(radius - distance + 1f);
                Color existingColor = texture.GetPixel(pixelX, pixelY);
                Color blendedColor = Color.Lerp(existingColor, color, color.a * edgeFade);
                blendedColor.a = Mathf.Clamp01(existingColor.a + color.a * edgeFade);
                texture.SetPixel(pixelX, pixelY, blendedColor);
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
                float distance = normalizedX * normalizedX + normalizedY * normalizedY;
                if (distance > 1f)
                {
                    continue;
                }

                float edgeFade = Mathf.Clamp01((1f - distance) * 16f);
                Color existingColor = texture.GetPixel(x, y);
                Color blendedColor = Color.Lerp(existingColor, color, color.a * edgeFade);
                blendedColor.a = Mathf.Clamp01(existingColor.a + color.a * edgeFade);
                texture.SetPixel(x, y, blendedColor);
            }
        }
    }

    private static void ClearEllipse(Texture2D texture, int centerX, int centerY, int radiusX, int radiusY)
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
                    texture.SetPixel(x, y, Color.clear);
                }
            }
        }
    }

    private TMP_Text CreateHomeLabel(string objectName, Transform parent, string text, Vector2 anchor, float fontSize)
    {
        GameObject labelObject = new GameObject(objectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        labelObject.transform.SetParent(parent, false);

        RectTransform rectTransform = labelObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = anchor;
        rectTransform.anchorMax = anchor;
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = new Vector2(500f, 80f);
        rectTransform.anchoredPosition = Vector2.zero;

        TextMeshProUGUI label = labelObject.GetComponent<TextMeshProUGUI>();
        label.text = text;
        label.fontSize = fontSize;
        label.fontStyle = FontStyles.Bold;
        label.alignment = TextAlignmentOptions.Center;
        label.color = new Color(0.08f, 0.18f, 0.22f, 1f);
        label.raycastTarget = false;

        return label;
    }
}
