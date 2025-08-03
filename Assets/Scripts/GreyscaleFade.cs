using UnityEngine;

public class GreyscaleFade : MonoBehaviour
{
    public GrayscaleFadeFeature grayscaleFeature; // Drag your ScriptableRendererFeature here
    [Range(0f, 1f)]
    public float targetFade = 0f;

    public float fadeSpeed = 1f;
    private float currentFade = 0f;

    void Update()
    {
        if (grayscaleFeature == null) return;

        currentFade = Mathf.MoveTowards(currentFade, targetFade, Time.deltaTime * fadeSpeed);
        grayscaleFeature.SetFadeAmount(currentFade);
    }

    // Optional: expose a helper to fade in/out
    public void FadeToGrayscale(float duration)
    {
        targetFade = 1f;
        fadeSpeed = 1f / duration;
    }

    public void FadeToColor(float duration)
    {
        targetFade = 0f;
        fadeSpeed = 1f / duration;
    }
}
