using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CustomizationSceneManager : MonoBehaviour
{
    [SerializeField] private float scrollBottomPadding = 160f;

    private void Start()
    {
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

    private void PreserveExistingChildHeights(RectTransform parent)
    {
        foreach (RectTransform child in parent)
        {
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
}
