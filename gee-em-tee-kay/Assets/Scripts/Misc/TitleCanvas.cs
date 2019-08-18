using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleCanvas : MonoBehaviour
{
    [SerializeField] private bool shouldFade = true;
    [SerializeField] private float fadeTime = 1.0f;
    [SerializeField] private TextObject text;

    // Start is called before the first frame update
    void Start()
    {
        Global.dayManager.startPressed += FadeOut;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(SetText("\\c008\\b\\jhe's not."));
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(SetText("\\c008\\b\\jit wasn't."));
        }
    }

    void FadeOut()
    {
        if (shouldFade)
            StartCoroutine(FadeOutRenderers());
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

    IEnumerator SetText(string newText)
    {
        text.SetText(newText);

        foreach (LetterObject lo in text.GetLetterObjects())
        {
            float delay = 0.05f;

            // Show letter object
            lo.Show(true);

            yield return new WaitForSeconds(delay);
        }

    }
}
