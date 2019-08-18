using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeechBubble : MonoBehaviour
{   
    public TextObject text;

    // Curves for rad tweening
    public AnimationCurve introCurve;
    public AnimationCurve outroCurve;
    public AnimationCurve interruptCurve;
    public AnimationCurve selectCurve;

    // If the image is an option button (used in dialogue)
    public bool isOption = false;
    
    // If the image is an option button (used in dialogue)
    public bool isDialogueOption = false;

    // The amount of time taken for resizing dialogue bubbles 
    public float resizeTime = 0.2f;

    // The alpha of the button
    float alpha;

    // The amount the alpha is multiplied by when not selected
    public float deselectedAlphaMultiplier = 0.5f;

    // The amount the alpha is multiplied by when not selected
    public float deselectedDialogueAlphaMultiplier = 0.7f;

    // (Optional tail element)
    public Image tail;

    [SerializeField] private float horizontalPadding = 10f;
    [SerializeField] private float verticalPadding = 10f;

    // Initial Scale-- allows resetting to it after juicing operations
    private Vector3 initialScale;

    // Image component (used to adjust alpha/colour on the fly)
    private Image imageElement;

    // The transform component
    private RectTransform rt;

    // Used only for resizing text bubbles
    private Vector2 previousSize = new Vector2();
    private Vector2 desiredSize = new Vector2();

    // Coroutine used for the animation
    private IEnumerator animationCoroutine;

    public void Awake()
    {
        // Grab elements
        imageElement = GetComponent<Image>();
        rt = GetComponent<RectTransform>();

        text.SetPosition(new Vector2(horizontalPadding, -verticalPadding));

        // Disable image by default
        imageElement.enabled = false;

        // Set initial scales
        alpha = imageElement.color.a;
        initialScale = transform.localScale;

        // Update keys to scale validly (based on initial scale)
        Keyframe[] tempKeys = introCurve.keys;
        tempKeys[introCurve.length-1].value = initialScale.x;
        introCurve.keys = tempKeys;

        tempKeys = outroCurve.keys;
        tempKeys[0].value = initialScale.x;
        outroCurve.keys = tempKeys;

        // If option button, set to disabled option
        if (isOption)
            imageElement.color = new Color(imageElement.color.r, imageElement.color.g, imageElement.color.b, alpha*deselectedAlphaMultiplier);

        // If option button, set to disabled option
        if (isDialogueOption)
            imageElement.color = new Color(imageElement.color.r, imageElement.color.g, imageElement.color.b, alpha*deselectedDialogueAlphaMultiplier);
    }

    public void SetContents(string newContents)
    {
        text.SetText(newContents);

        Vector2 newSize = text.GetSize();
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (2 * horizontalPadding) + newSize.x);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (2 * verticalPadding) + newSize.y);
    }

    public void SetHeight(float newHeight)
    {
        rt.anchoredPosition = new Vector2(0f, newHeight);
    }

    /**
     * ShowBubble
     * - Displays bubble and sets it to active
     */
    public void ShowBubble()
    {
        imageElement.enabled = true;

        if (tail != null)
            tail.enabled = true;
    }
    
    /**
     * HideBubble
     * - Hides bubble and sets it to inactive
     */
    public void HideBubble()
    {
        imageElement.enabled = false;

        if (tail != null)
            tail.enabled = false;
    }

    /**
     * GrowBubble
     * - Plays the growing animation 
     * - Used when showing a button for the first time
     */
    public void GrowBubble()
    {
        animationCoroutine = UpdateCoroutine(animationCoroutine, AnimateBubble(introCurve));
        StartCoroutine(animationCoroutine);
    }

    /**
     * InterruptCurve
     * - Interrupts any currently occuring curves
     * - Used for interrupting the shrink bubble when using the same speech bubble (probably a bit hacky)
     */
    public void InterruptCurve()
    {
        animationCoroutine = UpdateCoroutine(animationCoroutine, AnimateBubble(interruptCurve));
        StartCoroutine(animationCoroutine);
    }

    /**
     * ShrinkBubble
     * - Plays the shrink animation
     * - Used when hiding a button
     */
    public void ShrinkBubble()
    {
        animationCoroutine = UpdateCoroutine(animationCoroutine, AnimateBubble(outroCurve, 1f, true));
        StartCoroutine(animationCoroutine);
    }

    /**
     * SelectButton
     * - Highlight the button
     * - Play the select animation
     */
    public void SelectButton(bool resetTime = true)
    {
        animationCoroutine = UpdateCoroutine(animationCoroutine, AnimateBubble(selectCurve));
        StartCoroutine(animationCoroutine);

        imageElement.color = new Color(imageElement.color.r, imageElement.color.g, imageElement.color.b, alpha);
    }

    /**
     * DeselectButton
     * - Unhighlight the button
     */
    public void DeselectButton()
    {
        // Reset scale to what it should be
        transform.localScale = initialScale;
        
        imageElement.color = new Color(imageElement.color.r, imageElement.color.g, imageElement.color.b, alpha*deselectedDialogueAlphaMultiplier);
    }

    /**
     * AnimateBubble
     * - Coroutine to animate a bubble using a curve
     * - Animation length is the curve length
     */
    public IEnumerator AnimateBubble(AnimationCurve curve, float rate = 1f, bool postHide = false)
    {
        float accumulatedTime = 0f;
        while (accumulatedTime < curve[curve.length-1].time)
        {
            // Update scale based on graph
            transform.localScale = Vector3.one*curve.Evaluate(accumulatedTime);

            accumulatedTime += Time.deltaTime*rate;

            yield return null;
        }

        transform.localScale = initialScale;

        if (postHide)
        {
            Destroy(gameObject);
        }
    }

    /**
     * LerpSize
     * Coroutine used to lerp the size of the speech bubbles
     */
    public IEnumerator LerpSize(float lerpTime, float rate = 1f)
    {
        float accumulatedTime = 0f;
        while (accumulatedTime < lerpTime)
        {
            Vector2 currentSize = Vector2.Lerp(previousSize, desiredSize, accumulatedTime/lerpTime);
            SetSize(currentSize.x, currentSize.y);

            accumulatedTime += Time.deltaTime*rate;

            yield return null;
        }
        
        SetSize(desiredSize.x, desiredSize.y);
    }

    /**
     * UpdateCoroutine
     * - Utils function to null-check and update coroutines
     */
    public IEnumerator UpdateCoroutine(IEnumerator coroutine, IEnumerator update)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        coroutine = update;
        return coroutine;
    }

    /**
     * IsActive
     * - Returns whether the bubble is active
     */
    public bool isActive()
    {
        return imageElement.enabled;
    }
    
    /**
     * GetHalfHeight
     * - Returns the halved height of the bubble
     */
    public float GetHalfHeight()
    {
        return (rt.rect.height * rt.localScale.y * 0.5f);
    }

    /**
     * SetSize (normal)
     */
    public void SetSize(float w, float h)
    {
        previousSize = new Vector2(w, h);

        // Set size
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);

        rt.anchoredPosition = new Vector2(0f, rt.rect.height*0.5f);

        if (tail != null)
            tail.rectTransform.anchoredPosition = new Vector2(0, tail.rectTransform.anchoredPosition.y);
    }

    /**
     * SetSize (y-offset)
     */
    public void SetSizeAndOffset(float w, float h, float y)
    {
        SetSize(w, h);

        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, rt.rect.height*0.5f + y);
    }

    /**
     * SetSize (y-offset & x-offset)
     */
    public void SetSizeAndOffset(float w, float h, float x, float y)
    {
        SetSize(w, h);

        rt.anchoredPosition = new Vector2(x, rt.rect.height*0.5f + y);
    }

    /**
     * SetSize (blend from previous size)
     */
    public void SetSizeWithBlend(float w, float h)
    {
        SetSize(previousSize.x, previousSize.y);

        desiredSize = new Vector2(w, h);

        StopCoroutine(LerpSize(resizeTime));
        StartCoroutine(LerpSize(resizeTime));
    }
}
