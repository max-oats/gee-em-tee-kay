using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Holder : MonoBehaviour
{
    /** The bone to use for position transforms */
    public GameObject bone;

    /** The default up offset to be added to any item when held */
    public float defaultUpOffset = -0.5f;

    /** The default forward offset to be added to any item when held */
    public float defaultForwardOffset = 1.5f;

    public Vector3 heldEuler;

    /** The object we are currently holding */
    public GameObject heldObject;

    void Update()
    {
        if (heldObject != null)
        {
            // Update transform:
            // Position
            Vector3 newPosition = bone.transform.position;

            // - add forward offset
            newPosition += (-bone.transform.up * (defaultForwardOffset));

            // - add upwards offset
            newPosition += (-bone.transform.right * (defaultUpOffset));

            // - set final position
            heldObject.transform.position = newPosition;

            // Rotation
            heldObject.transform.eulerAngles = bone.transform.eulerAngles;
            heldObject.transform.Rotate(heldEuler, Space.Self);
        }
    }

    public void Drop(Vector3 dropLocation)
    {
        Transform heldTransform = heldObject.transform;
        heldObject = null;

        heldTransform.position = dropLocation;
        heldTransform.rotation = Quaternion.identity;
    }
}