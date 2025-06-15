using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFade : MonoBehaviour
{
    public Image fadeImage;

    private void Awake()
    {
        if (fadeImage != null)
            fadeImage.color = new Color(0, 0, 0, 0);
    }

    public Tween FadeOut(float duration)
    {
        return fadeImage.DOFade(1f, duration);
    }

    public Tween FadeIn(float duration)
    {
        return fadeImage.DOFade(0f, duration);
    }
}
