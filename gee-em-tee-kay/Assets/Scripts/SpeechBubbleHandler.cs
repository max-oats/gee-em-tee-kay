using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/** The script that handles the data within the Canvas itself */
public class SpeechBubbleHandler : MonoBehaviour
{
    // Text prefab
    public GameObject textPrefab;

    // Main speech bubble. Can be used for any entity
    public SpeechBubbleImage mainBubble;

    // Dialogue option bubbles.
    public List<SpeechBubbleImage> buttons;

    // Entity associated with the speech bubble
    private Entity entity;

    void Start()
    {
        // Grab entity
        entity = GetComponentInParent<Entity>();
    }
}
