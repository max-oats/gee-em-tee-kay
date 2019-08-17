using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/** The script that handles the data within the Canvas itself */
public class SpeechBubbleHandler : MonoBehaviour
{
    // Speech bubble prefab
    [SerializeField] private GameObject _speechBubble;

    // Dialogue option bubbles.
    public List<SpeechBubble> buttons;

    // Entity associated with the speech bubble
    private Entity entity;

    void Start()
    {
        // Grab entity
        entity = GetComponentInParent<Entity>();
    }

    /**
     * CreateSpeechBubble
     * - Creates (and returns) a speech bubble.
     */
    public SpeechBubble CreateSpeechBubble()
    {
        GameObject go = Instantiate(_speechBubble, transform);
        SpeechBubble sb = go.GetComponent<SpeechBubble>();

        return sb;
    }
}
