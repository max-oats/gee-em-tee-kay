using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleCanvas : MonoBehaviour
{
    [SerializeField] private float fadeTime = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        Global.dayManager.startPressed += FadeOut;
    }

    void FadeOut()
    {
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
}
