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
    private const int AccessoryTextureSize = 256;
    private const int ButtonIconTextureSize = 128;
    private const int HabitatBackgroundWidth = 1080;
    private const int HabitatBackgroundHeight = 1920;
    private const float DrawerTopClearance = 122f;
    private const float DrawerBottomClearance = 138f;
    private const float DrawerSideMargin = 10f;
    private const float DrawerScreenWidthRatio = 0.68f;

    [SerializeField] private float pocketPalMoveSpeed = 120f;
    [SerializeField] private float pocketPalPauseTime = 0.8f;
    [SerializeField] private Vector2 pocketPalAreaMin = new Vector2(-390f, -520f);
    [SerializeField] private Vector2 pocketPalAreaMax = new Vector2(390f, 520f);

    private RectTransform pocketPalTransform;
    private RectTransform pocketPalVisualTransform;
    private RectTransform pocketPalSpriteTransform;
    private RectTransform pocketPalShadowTransform;
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
    private Vector2 pocketPalTargetPosition;
    private string pocketPalTargetMessage = "Panda is exploring the habitat.";
    private HabitatActivity habitatActivity = HabitatActivity.Exploring;
    private float pocketPalPauseTimer;
    private float pocketPalAnimationTimer;
    private float pocketPalFacingDirection = 1f;
    private static Sprite pandaSprite;
    private static Sprite shadowSprite;
    private static Sprite sunglassesSprite;
    private static Sprite hatSprite;
    private static Sprite shirtSprite;
    private static readonly System.Collections.Generic.Dictionary<string, Sprite> habitatBackgroundSprites = new System.Collections.Generic.Dictionary<string, Sprite>();
    private static readonly System.Collections.Generic.Dictionary<string, Sprite> accessorySpriteCache = new System.Collections.Generic.Dictionary<string, Sprite>();
    private static Sprite customizationButtonIcon;
    private static Sprite rewardsButtonIcon;
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
        BattingBall
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
        BuildSanctuaryHome();
        DisableTextRaycasts();
        CreatePocketPalButton();
        ChooseNewPocketPalTarget();
    }

    private void Update()
    {
        MovePocketPal();
        AnimateHabitatProps();
        AnimateDrawer();
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

    public void RefreshOpenPanel()
    {
        if (openPanel == PanelKind.None)
        {
            return;
        }

        RefreshHabitatBackground();
        BuildDrawerContent(openPanel);
        RefreshTopBar();
        RefreshPocketPalVisual();
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
            backgroundImage.sprite = GetHabitatBackgroundSprite(SaveManager.Data.selectedHabitatId);
            backgroundImage.color = Color.white;
            backgroundImage.type = Image.Type.Simple;
            backgroundImage.preserveAspect = false;
            backgroundImage.raycastTarget = false;
        }
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
                CreatePandaSprite(existingPal);
            }

            if (existingPal.Find("PocketPalName") == null)
            {
                CreateHomeLabel("PocketPalName", existingPal, GetPocketPalDisplayName(), new Vector2(0.5f, -0.36f), 30);
            }

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

        CreatePandaSprite(pocketPal.transform);
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

        if (pocketPalPauseTimer > 0f)
        {
            pocketPalPauseTimer -= Time.deltaTime;
            UpdatePocketPalAnimation(false);
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
            ChooseNewPocketPalTarget();
        }
    }

    private void ChooseNewPocketPalTarget()
    {
        if (Random.value < 0.62f)
        {
            int targetIndex = Random.Range(0, 3);
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

            pocketPalTargetPosition = new Vector2(225f, -165f);
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

    private void BuildDrawerContent(PanelKind panelKind)
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
        scrollRect.verticalNormalizedPosition = 1f;
        scrollRect.horizontalNormalizedPosition = 0f;
        PositionPocketPalPopup();
        RefreshTopBar();
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

        CreatePandaSprite(pocketPalTransform);
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

    private void CreatePandaSprite(Transform parent)
    {
        EnsurePandaSprites();

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
        pocketPalSpriteTransform.anchoredPosition = Vector2.zero;

        Image pandaImage = spriteObject.GetComponent<Image>();
        pandaImage.sprite = pandaSprite;
        pandaImage.preserveAspect = true;
        pandaImage.raycastTarget = false;

        CreateAccessoryLayers(visualObject.transform);
    }

    private void CreateAccessoryLayers(Transform visualParent)
    {
        string eyewearId = GetEquippedAccessoryInCategory(new string[] { "sunglasses", "monocle", "aviators" });
        string headwearId = GetEquippedAccessoryInCategory(new string[] { "hat", "beanie", "headband", "crown" });
        string shirtId = GetEquippedAccessoryInCategory(new string[] { "tshirt", "shirt_lakers", "shirt_bulls", "shirt_celtics", "shirt_knicks", "shirt_nirvana", "shirt_metallica" });

        CreateAccessoryLayer("EyewearAccessory", visualParent, GetAccessorySprite(eyewearId), new Vector2(0f, GetEyewearY(eyewearId)), GetEyewearSize(eyewearId), !string.IsNullOrEmpty(eyewearId));
        CreateAccessoryLayer("HeadwearAccessory", visualParent, GetAccessorySprite(headwearId), new Vector2(0f, 115f), new Vector2(180f, 106f), !string.IsNullOrEmpty(headwearId));
        CreateAccessoryLayer("ShirtAccessory", visualParent, GetAccessorySprite(shirtId), new Vector2(0f, -52f), new Vector2(238f, 184f), !string.IsNullOrEmpty(shirtId));
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

    private float GetEyewearY(string accessoryId)
    {
        return accessoryId == "monocle" ? 50f : 43f;
    }

    private Vector2 GetEyewearSize(string accessoryId)
    {
        return accessoryId == "monocle" ? new Vector2(150f, 108f) : accessoryId == "aviators" ? new Vector2(230f, 104f) : new Vector2(230f, 112f);
    }

    private bool HasEquippedAccessory(string accessoryId)
    {
        if (SaveManager.Data.equippedAccessoryId == accessoryId)
        {
            return true;
        }

        return SaveManager.Data.equippedAccessoryIds != null && SaveManager.Data.equippedAccessoryIds.Contains(accessoryId);
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

    private static Sprite GetAccessorySprite(string accessoryId)
    {
        if (string.IsNullOrEmpty(accessoryId))
        {
            return null;
        }

        if (accessoryId == "sunglasses") return sunglassesSprite;
        if (accessoryId == "hat") return hatSprite;
        if (accessoryId == "tshirt") return shirtSprite;

        if (accessorySpriteCache.ContainsKey(accessoryId))
        {
            return accessorySpriteCache[accessoryId];
        }

        Texture2D texture;
        if (accessoryId == "monocle" || accessoryId == "aviators")
        {
            texture = CreateEyewearTexture(accessoryId);
        }
        else if (accessoryId == "beanie" || accessoryId == "headband" || accessoryId == "crown")
        {
            texture = CreateHeadwearTexture(accessoryId);
        }
        else
        {
            texture = CreateShirtTexture(accessoryId);
        }

        Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, AccessoryTextureSize, AccessoryTextureSize), new Vector2(0.5f, 0.5f), 100f);
        accessorySpriteCache[accessoryId] = sprite;
        return sprite;
    }

    private string GetPocketPalDisplayName()
    {
        string savedName = SaveManager.Data.selectedPocketPalName;
        return string.IsNullOrWhiteSpace(savedName) ? "Panda" : savedName;
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

        pocketPalAnimationTimer += Time.deltaTime * (isMoving ? 7f : 3f);
        float bob = Mathf.Sin(pocketPalAnimationTimer) * (isMoving ? 10f : 4f);
        float squash = 1f + Mathf.Sin(pocketPalAnimationTimer * 2f) * (isMoving ? 0.035f : 0.015f);
        float stretch = 1f - Mathf.Sin(pocketPalAnimationTimer * 2f) * (isMoving ? 0.025f : 0.01f);

        pocketPalVisualTransform.anchoredPosition = new Vector2(0f, 12f + bob);
        pocketPalVisualTransform.localScale = new Vector3(pocketPalFacingDirection * squash, stretch, 1f);

        if (pocketPalShadowTransform != null)
        {
            float shadowScale = Mathf.Lerp(1.05f, 0.88f, Mathf.InverseLerp(-10f, 10f, bob));
            pocketPalShadowTransform.localScale = new Vector3(shadowScale, 1f, 1f);
        }
    }

    private static void EnsurePandaSprites()
    {
        if (pandaSprite == null)
        {
            pandaSprite = Sprite.Create(CreatePandaTexture(), new Rect(0f, 0f, PandaTextureSize, PandaTextureSize), new Vector2(0.5f, 0.5f), 100f);
        }

        if (shadowSprite == null)
        {
            shadowSprite = Sprite.Create(CreateShadowTexture(), new Rect(0f, 0f, 128f, 48f), new Vector2(0.5f, 0.5f), 100f);
        }

        if (sunglassesSprite == null)
        {
            sunglassesSprite = Sprite.Create(CreateSunglassesTexture(), new Rect(0f, 0f, AccessoryTextureSize, AccessoryTextureSize), new Vector2(0.5f, 0.5f), 100f);
        }

        if (hatSprite == null)
        {
            hatSprite = Sprite.Create(CreateHatTexture(), new Rect(0f, 0f, AccessoryTextureSize, AccessoryTextureSize), new Vector2(0.5f, 0.5f), 100f);
        }

        if (shirtSprite == null)
        {
            shirtSprite = Sprite.Create(CreateShirtTexture(), new Rect(0f, 0f, AccessoryTextureSize, AccessoryTextureSize), new Vector2(0.5f, 0.5f), 100f);
        }
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

        Color black = new Color(0.05f, 0.06f, 0.055f, 1f);
        Color blackHighlight = new Color(0.13f, 0.14f, 0.13f, 1f);
        Color white = new Color(0.96f, 0.97f, 0.9f, 1f);
        Color whiteShadow = new Color(0.78f, 0.84f, 0.82f, 1f);

        DrawEllipse(texture, 72, 178, 38, 42, black);
        DrawEllipse(texture, 184, 178, 38, 42, black);
        DrawEllipse(texture, 128, 104, 64, 72, black);
        DrawEllipse(texture, 128, 116, 55, 64, whiteShadow);
        DrawEllipse(texture, 128, 124, 50, 58, white);

        DrawEllipse(texture, 68, 94, 26, 48, black);
        DrawEllipse(texture, 188, 94, 26, 48, black);
        DrawEllipse(texture, 88, 46, 34, 24, black);
        DrawEllipse(texture, 168, 46, 34, 24, black);

        DrawEllipse(texture, 128, 162, 76, 70, whiteShadow);
        DrawEllipse(texture, 128, 170, 72, 66, white);
        DrawEllipse(texture, 95, 170, 28, 34, black);
        DrawEllipse(texture, 161, 170, 28, 34, black);
        DrawEllipse(texture, 91, 178, 9, 12, blackHighlight);
        DrawEllipse(texture, 157, 178, 9, 12, blackHighlight);

        DrawEllipse(texture, 97, 168, 8, 10, Color.white);
        DrawEllipse(texture, 159, 168, 8, 10, Color.white);
        DrawEllipse(texture, 99, 168, 4, 5, black);
        DrawEllipse(texture, 157, 168, 4, 5, black);

        DrawEllipse(texture, 128, 147, 13, 9, black);
        DrawEllipse(texture, 108, 206, 20, 12, new Color(1f, 1f, 1f, 0.22f));

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

    private static Texture2D CreateSunglassesTexture()
    {
        Texture2D texture = CreateTransparentTexture(AccessoryTextureSize, AccessoryTextureSize);
        Color lens = new Color(0.22f, 0.12f, 0.05f, 0.9f);
        Color shine = new Color(1f, 0.78f, 0.42f, 0.45f);
        Color frame = new Color(0.42f, 0.24f, 0.1f, 1f);
        Color tortoise = new Color(0.72f, 0.45f, 0.18f, 0.95f);

        DrawEllipse(texture, 77, 126, 43, 36, frame);
        DrawEllipse(texture, 179, 126, 43, 36, frame);
        DrawEllipse(texture, 68, 134, 14, 10, tortoise);
        DrawEllipse(texture, 188, 118, 14, 10, tortoise);
        DrawEllipse(texture, 77, 126, 34, 28, lens);
        DrawEllipse(texture, 179, 126, 34, 28, lens);
        DrawRoundedRect(texture, 112, 121, 32, 10, 5, frame);
        DrawEllipse(texture, 64, 137, 11, 6, shine);
        DrawEllipse(texture, 166, 137, 11, 6, shine);

        texture.Apply();
        return texture;
    }

    private static Texture2D CreateHatTexture()
    {
        Texture2D texture = CreateTransparentTexture(AccessoryTextureSize, AccessoryTextureSize);
        Color brim = new Color(0.12f, 0.42f, 0.32f, 1f);
        Color crown = new Color(0.2f, 0.62f, 0.47f, 1f);
        Color band = new Color(0.95f, 0.78f, 0.25f, 1f);
        Color highlight = new Color(1f, 1f, 1f, 0.25f);

        DrawEllipse(texture, 128, 96, 78, 18, brim);
        DrawRoundedRect(texture, 75, 96, 106, 62, 18, crown);
        DrawRoundedRect(texture, 75, 101, 106, 16, 7, band);
        DrawEllipse(texture, 112, 140, 22, 8, highlight);

        texture.Apply();
        return texture;
    }

    private static Texture2D CreateHeadwearTexture(string accessoryId)
    {
        Texture2D texture = CreateTransparentTexture(AccessoryTextureSize, AccessoryTextureSize);

        if (accessoryId == "crown")
        {
            Color gold = new Color(0.96f, 0.72f, 0.16f, 1f);
            Color shadow = new Color(0.62f, 0.42f, 0.06f, 1f);
            DrawRoundedRect(texture, 72, 86, 112, 38, 12, shadow);
            DrawRoundedRect(texture, 78, 92, 100, 30, 10, gold);
            DrawEllipse(texture, 88, 132, 18, 32, gold);
            DrawEllipse(texture, 128, 142, 18, 42, gold);
            DrawEllipse(texture, 168, 132, 18, 32, gold);
            texture.Apply();
            return texture;
        }

        if (accessoryId == "headband")
        {
            DrawRoundedRect(texture, 58, 101, 140, 26, 12, new Color(0.57f, 0.48f, 0.93f, 1f));
            DrawRoundedRect(texture, 78, 106, 100, 8, 4, new Color(0.35f, 0.62f, 0.34f, 1f));
            texture.Apply();
            return texture;
        }

        Color beanie = new Color(0.57f, 0.48f, 0.93f, 1f);
        DrawRoundedRect(texture, 74, 90, 108, 58, 22, beanie);
        DrawEllipse(texture, 128, 145, 52, 15, beanie);
        DrawEllipse(texture, 128, 156, 14, 14, new Color(0.86f, 0.95f, 1f, 1f));
        texture.Apply();
        return texture;
    }

    private static Texture2D CreateEyewearTexture(string accessoryId)
    {
        if (accessoryId == "aviators")
        {
            Texture2D texture = CreateTransparentTexture(AccessoryTextureSize, AccessoryTextureSize);
            Color frame = new Color(0.18f, 0.28f, 0.36f, 1f);
            Color lens = new Color(0.62f, 0.84f, 0.96f, 0.72f);
            DrawEllipse(texture, 78, 124, 42, 34, frame);
            DrawEllipse(texture, 178, 124, 42, 34, frame);
            DrawEllipse(texture, 78, 124, 33, 27, lens);
            DrawEllipse(texture, 178, 124, 33, 27, lens);
            DrawRoundedRect(texture, 112, 120, 32, 8, 4, frame);
            texture.Apply();
            return texture;
        }

        Texture2D monocle = CreateTransparentTexture(AccessoryTextureSize, AccessoryTextureSize);
        Color gold = new Color(0.96f, 0.72f, 0.16f, 1f);
        Color glass = new Color(0.62f, 0.84f, 0.96f, 0.4f);
        DrawEllipse(monocle, 160, 126, 36, 36, gold);
        DrawEllipse(monocle, 160, 126, 27, 27, glass);
        DrawRoundedRect(monocle, 194, 118, 24, 7, 4, gold);
        DrawEllipse(monocle, 206, 87, 5, 28, gold);
        monocle.Apply();
        return monocle;
    }

    private static Texture2D CreateShirtTexture()
    {
        Texture2D texture = CreateTransparentTexture(AccessoryTextureSize, AccessoryTextureSize);
        Color shirt = new Color(0.57f, 0.48f, 0.93f, 0.98f);
        Color shadow = new Color(0.34f, 0.28f, 0.68f, 0.72f);
        Color trim = new Color(0.86f, 0.95f, 1f, 0.98f);
        Color badge = new Color(0.95f, 0.84f, 0.28f, 1f);

        DrawRoundedRect(texture, 24, 10, 208, 154, 40, shadow);
        DrawRoundedRect(texture, 34, 24, 188, 138, 38, shirt);
        ClearEllipse(texture, 128, 172, 50, 36);
        ClearEllipse(texture, 128, 152, 34, 22);
        ClearEllipse(texture, 20, 146, 24, 46);
        ClearEllipse(texture, 236, 146, 24, 46);
        DrawRoundedRect(texture, 70, 26, 116, 16, 8, trim);
        DrawEllipse(texture, 128, 86, 15, 15, badge);

        texture.Apply();
        return texture;
    }

    private static Texture2D CreateShirtTexture(string accessoryId)
    {
        Texture2D texture = CreateTransparentTexture(AccessoryTextureSize, AccessoryTextureSize);
        Color shirt = GetShirtColor(accessoryId);
        Color shadow = Color.Lerp(shirt, Color.black, 0.35f);
        Color trim = GetShirtTrimColor(accessoryId);
        Color badge = new Color(0.95f, 0.84f, 0.28f, 1f);

        DrawRoundedRect(texture, 24, 10, 208, 154, 40, shadow);
        DrawRoundedRect(texture, 34, 24, 188, 138, 38, shirt);
        ClearEllipse(texture, 128, 172, 50, 36);
        ClearEllipse(texture, 128, 152, 34, 22);
        ClearEllipse(texture, 20, 146, 24, 46);
        ClearEllipse(texture, 236, 146, 24, 46);
        DrawRoundedRect(texture, 70, 26, 116, 16, 8, trim);
        DrawEllipse(texture, 128, 86, 15, 15, badge);

        if (accessoryId == "shirt_lakers")
        {
            DrawRoundedRect(texture, 84, 58, 88, 8, 4, new Color(0.96f, 0.72f, 0.16f, 1f));
        }
        else if (accessoryId == "shirt_bulls")
        {
            DrawRoundedRect(texture, 84, 58, 88, 8, 4, Color.black);
        }
        else if (accessoryId == "shirt_celtics")
        {
            DrawEllipse(texture, 128, 116, 22, 22, Color.white);
        }
        else if (accessoryId == "shirt_knicks")
        {
            DrawRoundedRect(texture, 84, 58, 88, 8, 4, new Color(0.95f, 0.36f, 0.08f, 1f));
        }
        else if (accessoryId == "shirt_nirvana")
        {
            DrawEllipse(texture, 128, 114, 24, 24, new Color(0.95f, 0.84f, 0.18f, 1f));
        }
        else if (accessoryId == "shirt_metallica")
        {
            DrawRoundedRect(texture, 88, 88, 80, 12, 6, new Color(0.75f, 0.78f, 0.82f, 1f));
        }

        texture.Apply();
        return texture;
    }

    private static Color GetShirtColor(string accessoryId)
    {
        if (accessoryId == "shirt_lakers") return new Color(0.36f, 0.19f, 0.63f, 0.98f);
        if (accessoryId == "shirt_bulls") return new Color(0.72f, 0.05f, 0.08f, 0.98f);
        if (accessoryId == "shirt_celtics") return new Color(0.02f, 0.48f, 0.23f, 0.98f);
        if (accessoryId == "shirt_knicks") return new Color(0.08f, 0.31f, 0.67f, 0.98f);
        if (accessoryId == "shirt_nirvana") return new Color(0.08f, 0.08f, 0.08f, 0.98f);
        if (accessoryId == "shirt_metallica") return new Color(0.14f, 0.14f, 0.16f, 0.98f);
        return new Color(0.57f, 0.48f, 0.93f, 0.98f);
    }

    private static Color GetShirtTrimColor(string accessoryId)
    {
        if (accessoryId == "shirt_lakers") return new Color(0.96f, 0.72f, 0.16f, 1f);
        if (accessoryId == "shirt_bulls") return Color.black;
        if (accessoryId == "shirt_celtics") return Color.white;
        if (accessoryId == "shirt_knicks") return new Color(0.95f, 0.36f, 0.08f, 1f);
        if (accessoryId == "shirt_nirvana") return new Color(0.95f, 0.84f, 0.18f, 1f);
        if (accessoryId == "shirt_metallica") return new Color(0.75f, 0.78f, 0.82f, 1f);
        return new Color(0.86f, 0.95f, 1f, 0.98f);
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
        DrawHammock(texture, 540, 790);
        DrawToyBall(texture, 760, 395, 52);

        DrawHabitatRock(texture, 218, 380, 1.05f);
        DrawHabitatRock(texture, 854, 430, 0.82f);
        DrawHabitatRock(texture, 734, 310, 0.55f);

        DrawRoundedRect(texture, 108, 250, 864, 18, 9, new Color(0.37f, 0.68f, 0.36f, 0.55f));
        DrawRoundedRect(texture, 190, 218, 700, 12, 6, new Color(0.66f, 0.86f, 0.51f, 0.75f));
        DrawRoundedRect(texture, 320, 1135, 440, 24, 12, new Color(0.52f, 0.8f, 0.48f, 1f));

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
        DrawEllipse(texture, 540, 498, 292, 58, new Color(0.36f, 0.6f, 0.78f, 0.65f));
        DrawEllipse(texture, 520, 506, 110, 18, new Color(0.98f, 0.94f, 0.72f, 0.18f));
        DrawEllipse(texture, 395, 530, 78, 20, new Color(0.16f, 0.38f, 0.27f, 1f));
        DrawEllipse(texture, 688, 525, 94, 24, new Color(0.16f, 0.38f, 0.27f, 1f));
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

        DrawThickLine(texture, centerX - 310, centerY + 84, centerX - 160, centerY - 18, 10, rope);
        DrawThickLine(texture, centerX + 310, centerY + 84, centerX + 160, centerY - 18, 10, rope);
        DrawEllipse(texture, centerX, centerY - 44, 222, 68, fabricShadow);
        DrawEllipse(texture, centerX, centerY - 26, 214, 62, fabric);
        DrawRoundedRect(texture, centerX - 152, centerY - 36, 304, 22, 11, fabricHighlight);
        DrawEllipse(texture, centerX - 190, centerY - 8, 34, 22, rope);
        DrawEllipse(texture, centerX + 190, centerY - 8, 34, 22, rope);
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
