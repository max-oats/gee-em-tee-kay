using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TextObject))]
public class ObjectBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TextObject textObject = (TextObject)target;

        if(GUILayout.Button("Update UI String"))
        {
            textObject.UpdateUIString();
        }

        DrawDefaultInspector();
    }
}