using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SanctuaryUI : MonoBehaviour
{
    private const string PocketPalButtonName = "PocketPalButton";

    [SerializeField] private float pocketPalMoveSpeed = 120f;
    [SerializeField] private float pocketPalPauseTime = 0.8f;
    [SerializeField] private Vector2 pocketPalAreaMin = new Vector2(-390f, -520f);
    [SerializeField] private Vector2 pocketPalAreaMax = new Vector2(390f, 520f);

    private RectTransform pocketPalTransform;
    private Vector2 pocketPalTargetPosition;
    private float pocketPalPauseTimer;

    private void Start()
    {
        BuildSanctuaryHome();
        DisableTextRaycasts();
        CreatePocketPalButton();
        ChooseNewPocketPalTarget();
    }

    private void Update()
    {
        MovePocketPal();
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

        if (background.Find("Ground") != null)
        {
            return;
        }

        CreateHomePanel("DistantHills", background, new Vector2(0.5f, 0.46f), new Vector2(920f, 380f), new Color(0.47f, 0.73f, 0.55f, 1f));
        CreateHomePanel("Ground", background, new Vector2(0.5f, 0.19f), new Vector2(1080f, 700f), new Color(0.36f, 0.7f, 0.39f, 1f));
        CreateHomePanel("CozyNest", background, new Vector2(0.5f, 0.38f), new Vector2(460f, 260f), new Color(0.97f, 0.86f, 0.58f, 1f));
        CreateHomePanel("GardenPatch", background, new Vector2(0.25f, 0.24f), new Vector2(240f, 160f), new Color(0.45f, 0.52f, 0.26f, 1f));
        CreateHomePanel("Pond", background, new Vector2(0.76f, 0.26f), new Vector2(260f, 150f), new Color(0.36f, 0.76f, 0.88f, 1f));
        CreateHomeLabel("HomeLabel", background, "PocketPal Home", new Vector2(0.5f, 0.68f), 42);
    }

    private void CreatePocketPalButton()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
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
        pocketPalTransform.sizeDelta = new Vector2(180f, 180f);
        pocketPalTransform.anchoredPosition = Vector2.zero;

        Image image = pocketPal.GetComponent<Image>();
        image.color = new Color(1f, 0.92f, 0.38f, 1f);
        image.raycastTarget = true;
        image.type = Image.Type.Simple;

        Button button = pocketPal.GetComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(OnAnimalClicked);

        CreateHomeLabel("PocketPalName", pocketPal.transform, "Sprout", new Vector2(0.5f, -0.26f), 28);
    }

    private void DisableTextRaycasts()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
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
            return;
        }

        pocketPalTransform.anchoredPosition = Vector2.MoveTowards(
            pocketPalTransform.anchoredPosition,
            pocketPalTargetPosition,
            pocketPalMoveSpeed * Time.deltaTime);

        if (Vector2.Distance(pocketPalTransform.anchoredPosition, pocketPalTargetPosition) <= 4f)
        {
            pocketPalPauseTimer = pocketPalPauseTime;
            ChooseNewPocketPalTarget();
        }
    }

    private void ChooseNewPocketPalTarget()
    {
        pocketPalTargetPosition = new Vector2(
            Random.Range(pocketPalAreaMin.x, pocketPalAreaMax.x),
            Random.Range(pocketPalAreaMin.y, pocketPalAreaMax.y));
    }

    private Transform FindCanvasChild(string childName)
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
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
