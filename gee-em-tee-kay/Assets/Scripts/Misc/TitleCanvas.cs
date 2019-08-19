using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleCanvas : MonoBehaviour
{
    [SerializeField] private bool shouldFade = true;
    [SerializeField] private float fadeTime = 1.0f;
    [SerializeField] private TextObject text;

    private Coroutine coroutine;

    // Start is called before the first frame update
    void Start()
    {
        //Global.dayManager.startPressed += FadeOut;
    }

    public void FadeOut()
    {
        if (shouldFade)
        {
            coroutine = StartCoroutine(FadeOutRenderers());
        }
    }

    IEnumerator FadeOutRenderers()
    {
        float timeCounter = 0f;

        Text[] textObjects = GetComponentsInChildren<Text>();

        while (timeCounter < fadeTime)
        {
            timeCounter += Time.deltaTime;

            foreach (Text textObject in textObjects)
            {
                textObject.color = new Color(textObject.color.r, textObject.color.g, textObject.color.b, (fadeTime - timeCounter) / fadeTime);
            }

            yield return null;
        }

        gameObject.SetActive(false);
    }

    public List<LetterObject> SetText(string newText)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);

            coroutine = null;
        }

        Text[] textObjects = GetComponentsInChildren<Text>();
        text.SetText(newText);

        foreach (Text textObject in textObjects)
        {
            textObject.color = new Color(textObject.color.r, textObject.color.g, textObject.color.b, 1.0f);
        }

        return text.GetLetterObjects();
    }
}
