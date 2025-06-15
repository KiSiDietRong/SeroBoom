using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Control : MonoBehaviour
{
    public Image fadeImage; // Gán image fade vào đây từ Inspector
    public float fadeDuration = 1f; // Thời gian mờ

    private void Start()
    {
        // Đảm bảo hình đen ban đầu trong suốt
        if (fadeImage != null)
            fadeImage.color = new Color(0, 0, 0, 0);
    }

    public void LevelScene()
    {
        if (fadeImage != null)
        {
            // Fade in -> Load scene
            fadeImage.DOFade(1f, fadeDuration).OnComplete(() =>
            {
                SceneManager.LoadScene("Level");
            });
        }
        else
        {
            // Nếu không có fadeImage, load thẳng
            SceneManager.LoadScene("Level");
        }
    }

    public void HomeScene()
    {
        SceneManager.LoadScene("Menu");
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
