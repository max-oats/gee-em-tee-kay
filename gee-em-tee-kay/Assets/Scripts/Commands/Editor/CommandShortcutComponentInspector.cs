using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(CommandShortcutComponent))]
public class CommandShortcutComponentInspector : Editor
{
    CommandShortcutComponent csc;

    private bool isDirty = false;
    private GUIStyle boldStyle;
    private GUIStyle buttonStyle;
    private GUIStyle textFieldStyle;

    private StyleWrapper boxStyle = new StyleWrapper();

    public override void OnInspectorGUI()
    {
        csc = (CommandShortcutComponent)target;

        /** Set up GUIStyle*/
        boxStyle.style = GUI.skin.GetStyle("HelpBox");
        textFieldStyle = GUI.skin.GetStyle("TextField");
        boldStyle = new GUIStyle();
        boldStyle.fontStyle = FontStyle.Bold;

        /** Reset DIRT checker */
        isDirty = false;

        if (csc.shortcuts.Count == 0)
        {
            csc.shortcuts.Add(new CommandShortcut());
        }

        /** Draw the default inspector*/
        DrawDefaultInspector();

        /** Separate */
        EditorGUILayout.Separator();

        EditorGUILayout.LabelField("Command shortuts:");

        DrawShortcuts();

        /** If is dirty at end of GUI, set object as dirty */
        if (isDirty)
        {
            EditorUtility.SetDirty(csc);
        }
    }

    private void DrawShortcuts()
    {
        GUILayout.BeginVertical(boxStyle);
        boxStyle.Indent();

        for (int i = 0; i < csc.shortcuts.Count; ++i)
        {
            GUILayout.BeginVertical(boxStyle);
            /** Draw the conditions string */
            EditorGUILayout.LabelField("Shortcut:");

            textFieldStyle.fontStyle = FontStyle.Bold;
            InspectorUtils.ListenForChange();
            string newConditions = EditorGUILayout.TextField(csc.shortcuts[i].name, textFieldStyle);
            if (InspectorUtils.WasChanged(csc, ref isDirty, "Updated script conditions"))
            {
                csc.shortcuts[i].name = newConditions;
            }

            /** Draw the events string */
            EditorGUILayout.LabelField("Commands:");

            textFieldStyle.fontStyle = FontStyle.Normal;
            InspectorUtils.ListenForChange();
            string newScript = EditorGUILayout.TextArea(csc.shortcuts[i].command);
            if (InspectorUtils.WasChanged(csc, ref isDirty, "Updated script events")) 
            {
                csc.shortcuts[i].command = newScript;
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+", GUILayout.MaxWidth(30f), GUILayout.MinWidth(10f)))
            {
                Undo.RecordObject(csc, "Inserted new");
                csc.shortcuts.Insert(i+1, new CommandShortcut());
                
                GUI.FocusControl(null);
                isDirty = true;
            }

            if (GUILayout.Button("-", GUILayout.MaxWidth(30f), GUILayout.MinWidth(10f)))
            {
                Undo.RecordObject(csc, "delete thing");
                csc.shortcuts.RemoveAt(i);
                isDirty = true;
                GUI.FocusControl(null);
            }
            GUILayout.EndHorizontal();
            
            GUILayout.EndVertical();
        }

        boxStyle.Unindent();
        GUILayout.EndVertical();
    }
}