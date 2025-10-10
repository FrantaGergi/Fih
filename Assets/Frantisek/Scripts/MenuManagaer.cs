using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MenuManagaer : MonoBehaviour
{
    [SerializeField] private Image startButton;
    [SerializeField] private Image settingButton;
    [SerializeField] private Image quitButton;
    [SerializeField] private Image containerOfButtons;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ShowMenu(true);
    }
    void ShowMenu(bool enabled)
    {
        if(!enabled)
        {
            StartCoroutine(FadeIn(containerOfButtons, 3f));
            StartCoroutine(FadeIn(startButton));
            StartCoroutine(FadeIn(settingButton, 1.5f));
            StartCoroutine(FadeIn(quitButton, 2f));
            return;
        }
        startButton.enabled = true;
        settingButton.enabled = true;
        quitButton.enabled = true;
        containerOfButtons.enabled = true;

        StartCoroutine(FadeOut(containerOfButtons, 2f));
        StartCoroutine(FadeOut(startButton));
        StartCoroutine(FadeOut(settingButton, 1.5f));
        StartCoroutine(FadeOut(quitButton, 2f));
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        
    }
    public void QuitGame()
    {
        ShowMenu(false);
        Application.Quit();
    }
    public void OpenSetting()
    {
        Debug.Log("Open Setting Menu");
    }


    public IEnumerator FadeIn(Image fadeImage, float fadeDuration = 1f)
    {
        float elapsed = 0f;
        Color c = fadeImage.color;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = 1f - (elapsed / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }
        c.a = 0f;
        fadeImage.color = c;
    }

    public IEnumerator FadeOut(Image fadeImage, float fadeDuration = 1f)
    {
        float elapsed = 0f;
        Color c = fadeImage.color;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = elapsed / fadeDuration;
            fadeImage.color = c;
            yield return null;
        }
        c.a = 1f;
        fadeImage.color = c;
    }
}
