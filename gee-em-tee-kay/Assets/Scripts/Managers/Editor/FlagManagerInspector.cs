using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(FlagManager))]
public class FlagManagerInspector : Editor
{
    FlagManager fm;
    string searchField = "";
    private int indentLevel = 1;
    private GUIStyle boxStyle;
    private GUIStyle boldStyle;

    public override void OnInspectorGUI()
    {
        fm = (FlagManager)target;

        boldStyle = new GUIStyle();
        boldStyle.fontStyle = FontStyle.Bold;
        
        GUIStyle redStyle = new GUIStyle();
        redStyle.normal.textColor = Color.red;
        
        GUIStyle greenStyle = new GUIStyle();
        redStyle.normal.textColor = Color.green;
        
        /** Set up GUIStyle*/
        boxStyle = GUI.skin.GetStyle("HelpBox");

        EditorGUILayout.Separator();

        searchField = EditorGUILayout.TextField("Flag", searchField);
        if (GUILayout.Button("Add flag"))
        {
            if (searchField.Length > 0)
            {
                fm.SetFlag(searchField, true);
                searchField = "";
            }
        }

        DrawFlags();

        DrawDefaultInspector();
    }

    private void DrawFlags()
    {
        EditorGUILayout.BeginVertical(boxStyle);
        Indent();
        foreach (FlagList flagList in fm.GetFlagLists())
        {
            EditorGUILayout.BeginVertical(boxStyle);
            EditorGUILayout.LabelField(flagList.name, boldStyle);
            foreach (Flag flag in flagList.flags)
            {
                string flagName = flag.name;
                if (flag.type == FlagType.Int)
                {
                    flagName += ": " + flag.asInt;
                }
                else if (flag.type == FlagType.Float)
                {
                    flagName += ": " + flag.asFloat;
                }
                else if (flag.type == FlagType.Bool)
                {
                    if (flag.asBool)
                    {
                        flagName += ": ✔️";
                    }
                    else
                    {
                        flagName += ": ✖";
                    }
                }
                else
                {
                    flagName += ": " + flag.asString;
                }

                EditorGUILayout.LabelField(flagName);
            }
            EditorGUILayout.EndVertical();
        }
        Unindent();
        EditorGUILayout.EndVertical();
    }

    /** ------------ */
    /** Editor utils */
    /** ------------ */
    private void Indent()
    {
        indentLevel++;
        boxStyle.margin = new RectOffset(indentLevel * 5, indentLevel * 5, 10, 10);
    }

    private void Unindent()
    {
        indentLevel--;
        boxStyle.margin = new RectOffset(indentLevel * 5, indentLevel * 5, 10, 10);
    }

}