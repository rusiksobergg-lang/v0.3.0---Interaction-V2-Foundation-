using UnityEngine;
using TMPro;
using System.Collections;

public class InteractionHUD : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject root;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemInfoText;
    public TextMeshProUGUI actionText;
    public UnityEngine.UI.Image itemIcon;

    [Header("Action Arrows")]
    public GameObject actionArrowLeft;
    public GameObject actionArrowRight;

    [Header("Arrow Highlight")]
    public Color arrowHighlightColor = new Color(1f, 0.75f, 0.15f, 1f);
    public float highlightDuration = 0.15f;

    private Coroutine highlightCoroutine;
    private Color leftArrowOriginalColor;
    private Color rightArrowOriginalColor;

    [Header("Fade")]
    public CanvasGroup canvasGroup;
    public float fadeInSpeed = 0.15f;
    public float fadeOutSpeed = 0.12f;


    private Coroutine fadeCoroutine;
    private bool isVisible;

    private void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = root.GetComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        isVisible = false;

        // На старті стрілки приховані
        SetActionArrows(false);

        if (actionArrowLeft != null)
        {
            UnityEngine.UI.Image image =
                actionArrowLeft.GetComponent<UnityEngine.UI.Image>();

            if (image != null)
                leftArrowOriginalColor = image.color;
        }

        if (actionArrowRight != null)
        {
            UnityEngine.UI.Image image =
                actionArrowRight.GetComponent<UnityEngine.UI.Image>();

            if (image != null)
                rightArrowOriginalColor = image.color;
        }
    }


    public void Show(
        string itemName,
        string itemInfo,
        string action,
        Sprite icon,
        bool hasMultipleActions)
    {
        itemNameText.text = itemName;
        itemInfoText.text = itemInfo;
        actionText.text = action;

        if (icon != null)
        {
            itemIcon.sprite = icon;
            itemIcon.enabled = true;
        }
        else
        {
            itemIcon.sprite = null;
            itemIcon.enabled = false;
        }

        // Стрілки тільки якщо є декілька дій
        SetActionArrows(hasMultipleActions);

        if (isVisible)
            return;

        isVisible = true;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(
            FadeTo(1f, fadeInSpeed)
        );
    }

    public void Hide()
    {
        if (!isVisible)
            return;

        isVisible = false;

        SetActionArrows(false);

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(
            FadeOut()
        );
    }

    private void SetActionArrows(bool visible)
    {
        if (actionArrowLeft != null)
            actionArrowLeft.SetActive(visible);

        if (actionArrowRight != null)
            actionArrowRight.SetActive(visible);
    }

    private IEnumerator FadeTo(
        float targetAlpha,
        float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / duration;

            canvasGroup.alpha =
                Mathf.Lerp(
                    startAlpha,
                    targetAlpha,
                    t
                );

            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }

    private IEnumerator FadeOut()
    {
        yield return StartCoroutine(
            FadeTo(0f, fadeOutSpeed)
        );

        canvasGroup.alpha = 0f;
    }
    public void HighlightArrow(bool right)
    {
        // Зупиняємо попередню анімацію
        if (highlightCoroutine != null)
            StopCoroutine(highlightCoroutine);

        // Завжди повертаємо ОБИДВІ стрілки
        // до нормального кольору перед новою підсвіткою
        ResetArrowColors();

        highlightCoroutine =
            StartCoroutine(
                HighlightArrowRoutine(right)
            );
    }

    private void ResetArrowColors()
    {
        if (actionArrowLeft != null)
        {
            UnityEngine.UI.Image image =
                actionArrowLeft.GetComponent<UnityEngine.UI.Image>();

            if (image != null)
                image.color = leftArrowOriginalColor;
        }

        if (actionArrowRight != null)
        {
            UnityEngine.UI.Image image =
                actionArrowRight.GetComponent<UnityEngine.UI.Image>();

            if (image != null)
                image.color = rightArrowOriginalColor;
        }
    }

    private IEnumerator HighlightArrowRoutine(bool right)
    {
        GameObject arrowObject =
            right
            ? actionArrowRight
            : actionArrowLeft;

        if (arrowObject == null)
            yield break;

        UnityEngine.UI.Image arrowImage =
            arrowObject.GetComponent<UnityEngine.UI.Image>();

        if (arrowImage == null)
            yield break;

        Color originalColor =
            right
            ? rightArrowOriginalColor
            : leftArrowOriginalColor;

        arrowImage.color = arrowHighlightColor;

        float elapsed = 0f;

        while (elapsed < highlightDuration)
        {
            elapsed += Time.deltaTime;

            float t =
                elapsed / highlightDuration;

            arrowImage.color =
                Color.Lerp(
                    arrowHighlightColor,
                    originalColor,
                    t
                );

            yield return null;
        }

        // Гарантовано повертаємо правильний колір
        arrowImage.color = originalColor;

        highlightCoroutine = null;
    }
}