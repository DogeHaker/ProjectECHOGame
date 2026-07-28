using UnityEngine;
using System.Collections;

public class SceneFadeIn : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    public float fadeDuration = 2.5f; // How many seconds the fade takes

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f; // Force pure black instantly on scene boot
        }
    }

    IEnumerator Start()
    {
        if (canvasGroup == null) yield break;

        // Optional tiny pause on black for suspense
        yield return new WaitForSeconds(0.5f);

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            // Smoothly lerp alpha from 1 (black) to 0 (transparent)
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        gameObject.SetActive(false); // Turn off panel so it doesn't block mouse clicks
    }
}