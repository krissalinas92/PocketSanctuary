using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(CanvasScaler))]
public sealed class PortraitViewport : MonoBehaviour
{
    private RectTransform content;

    private void Awake()
    {
        var scaler = GetComponent<CanvasScaler>();

        // The Canvas Scaler fits the reference resolution inside the current window.
        // Keep the layout at that resolution so folding only changes its pixel scale.
        var viewport = new GameObject("PortraitViewport", typeof(RectTransform), typeof(RectMask2D));
        viewport.layer = gameObject.layer;
        content = viewport.GetComponent<RectTransform>();
        content.SetParent(transform, false);
        content.anchorMin = content.anchorMax = content.pivot = new Vector2(0.5f, 0.5f);
        content.anchoredPosition = Vector2.zero;
        content.sizeDelta = scaler.referenceResolution;

        while (transform.childCount > 1)
        {
            transform.GetChild(0).SetParent(content, false);
        }
    }

    public static RectTransform FindContent()
    {
        var viewport = FindAnyObjectByType<PortraitViewport>();
        if (viewport != null && viewport.content != null)
        {
            return viewport.content;
        }

        var canvas = FindAnyObjectByType<Canvas>();
        return canvas != null ? (RectTransform)canvas.transform : null;
    }
}
