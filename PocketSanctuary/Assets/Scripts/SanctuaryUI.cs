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
    private TMP_Text habitatStatusLabel;
    private Vector2 pocketPalTargetPosition;
    private string pocketPalTargetMessage = "Panda is exploring the habitat.";
    private float pocketPalPauseTimer;
    private float pocketPalAnimationTimer;
    private float pocketPalFacingDirection = 1f;
    private static Sprite pandaSprite;
    private static Sprite shadowSprite;
    private static Sprite sunglassesSprite;
    private static Sprite hatSprite;
    private static Sprite shirtSprite;
    private static Sprite customizationButtonIcon;
    private static Sprite rewardsButtonIcon;

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
    }

    public void OnCustomizationClicked()
    {
        SceneManager.LoadScene("CustomizationScene", LoadSceneMode.Single);
    }
    public void OnTasksRewardsClicked()
    {
        SceneManager.LoadScene("TasksRewardsScene", LoadSceneMode.Single);
    }
    public void OnAnimalClicked()
    {
        SceneManager.LoadScene("PocketPalScene", LoadSceneMode.Single);
    }

    private void BuildSanctuaryHome()
    {
        Transform background = FindCanvasChild("Background");
        if (background == null)
        {
            return;
        }

        Image backgroundImage = background.GetComponent<Image>();
        if (backgroundImage != null)
        {
            backgroundImage.color = new Color(0.53f, 0.82f, 0.95f, 1f);
            backgroundImage.raycastTarget = false;
        }

        if (background.Find(HabitatViewName) != null)
        {
            StyleBottomNavigationButtons();
            return;
        }

        Create3DHabitatView(background);
        CreateHomeLabel("HomeLabel", background, "PocketPal Home", new Vector2(0.5f, 0.68f), 42);
        habitatStatusLabel = CreateHomeLabel("HabitatStatusLabel", background, "Panda is exploring the habitat.", new Vector2(0.5f, 0.61f), 26);
        habitatStatusLabel.color = new Color(0.1f, 0.25f, 0.18f, 1f);
        StyleBottomNavigationButtons();
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
                habitatStatusLabel.text = pocketPalTargetMessage;
            }

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
                return;
            }

            if (targetIndex == 1)
            {
                pocketPalTargetPosition = new Vector2(0f, -120f);
                pocketPalTargetMessage = "Panda is playing in the pen.";
                return;
            }

            pocketPalTargetPosition = new Vector2(210f, -40f);
            pocketPalTargetMessage = "Panda is batting the toy ball.";
            return;
        }

        pocketPalTargetPosition = new Vector2(
            Random.Range(pocketPalAreaMin.x, pocketPalAreaMax.x),
            Random.Range(pocketPalAreaMin.y, pocketPalAreaMax.y));
        pocketPalTargetMessage = "Panda is exploring the habitat.";
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

    private void Create3DHabitatView(Transform background)
    {
        if (background.Find(HabitatViewName) != null)
        {
            return;
        }

        GameObject viewObject = new GameObject(HabitatViewName, typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage));
        viewObject.transform.SetParent(background, false);
        viewObject.transform.SetAsFirstSibling();

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

        CreateHabitatLight(parent, new Vector3(0f, 5f, -3f), 1.2f);
        CreateHabitatLight(parent, new Vector3(-3f, 3f, 2f), 0.45f);
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
        if (playBallTransform != null)
        {
            playBallTransform.Rotate(new Vector3(0f, 45f, 18f) * Time.deltaTime, Space.Self);
        }

        if (bambooMobileTransform != null)
        {
            bambooMobileTransform.localRotation = Quaternion.Euler(0f, 0f, 22f + Mathf.Sin(Time.time * 2f) * 10f);
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
        bool showSunglasses = HasEquippedAccessory("sunglasses");
        bool showHat = HasEquippedAccessory("hat");
        bool showShirt = HasEquippedAccessory("tshirt");

        CreateAccessoryLayer("SunglassesAccessory", visualParent, sunglassesSprite, new Vector2(0f, 48f), new Vector2(170f, 78f), showSunglasses);
        CreateAccessoryLayer("HatAccessory", visualParent, hatSprite, new Vector2(0f, 115f), new Vector2(180f, 106f), showHat);
        CreateAccessoryLayer("ShirtAccessory", visualParent, shirtSprite, new Vector2(0f, -65f), new Vector2(190f, 145f), showShirt);
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
        accessoryObject.SetActive(isVisible);

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
        Color lens = new Color(0.03f, 0.04f, 0.05f, 0.92f);
        Color shine = new Color(0.74f, 0.94f, 1f, 0.55f);
        Color frame = new Color(0.01f, 0.015f, 0.02f, 1f);

        DrawRoundedRect(texture, 54, 105, 61, 42, 16, frame);
        DrawRoundedRect(texture, 141, 105, 61, 42, 16, frame);
        DrawRoundedRect(texture, 59, 110, 51, 32, 13, lens);
        DrawRoundedRect(texture, 146, 110, 51, 32, 13, lens);
        DrawRoundedRect(texture, 112, 122, 32, 8, 4, frame);
        DrawEllipse(texture, 78, 132, 13, 5, shine);
        DrawEllipse(texture, 165, 132, 13, 5, shine);

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

    private static Texture2D CreateShirtTexture()
    {
        Texture2D texture = CreateTransparentTexture(AccessoryTextureSize, AccessoryTextureSize);
        Color shirt = new Color(0.26f, 0.56f, 0.88f, 0.96f);
        Color shadow = new Color(0.13f, 0.33f, 0.62f, 0.65f);
        Color star = new Color(1f, 0.93f, 0.42f, 1f);

        DrawEllipse(texture, 68, 115, 28, 29, shirt);
        DrawEllipse(texture, 188, 115, 28, 29, shirt);
        DrawRoundedRect(texture, 76, 78, 104, 104, 24, shirt);
        DrawRoundedRect(texture, 78, 78, 100, 24, 18, shadow);
        DrawEllipse(texture, 128, 111, 24, 24, star);

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
