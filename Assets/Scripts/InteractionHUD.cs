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

    [Header("Fade")]
    public CanvasGroup canvasGroup;
    public float fadeInSpeed = 0.15f;
    public float fadeOutSpeed = 0.12f;

    private Coroutine fadeCoroutine;

    private void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = root.GetComponent<CanvasGroup>();

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;

        root.SetActive(false);
    }

    public void Show(
        string itemName,
        string itemInfo,
        string action)
    {
        itemNameText.text = itemName;
        itemInfoText.text = itemInfo;
        actionText.text = action;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        root.SetActive(true);

        fadeCoroutine = StartCoroutine(FadeTo(1f, fadeInSpeed));
    }

    public void Hide()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeOut());
    }

    private IEnumerator FadeTo(float targetAlpha, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / duration;

            canvasGroup.alpha =
                Mathf.Lerp(startAlpha, targetAlpha, t);

            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }

    private IEnumerator FadeOut()
    {
        yield return StartCoroutine(
            FadeTo(0f, fadeOutSpeed)
        );

        root.SetActive(false);
    }
}