using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public static class ConditionalInspectorUtils
{

    /** Draw the structure containing all the room scripts */
    public static bool DrawConditionalInspector(Object obj, ref ConditionalListWrapper conditionals, string searchField = "")
    {
        bool isDirty = false;
        List<int> itemsToRemove = new List<int>();
        StyleWrapper boxStyle = new StyleWrapper();
        boxStyle.style = GUI.skin.GetStyle("HelpBox");

        boxStyle.Indent(); /** indent */

        int scriptLength = conditionals.list.Count;
        if (scriptLength == 0)
        {
            conditionals.list.Add(new ConditionalInspectorObject());
            isDirty = true;
        }
        else
        {
            for (int i = 0; i < scriptLength; ++i)
            {
                if (searchField.Length == 0 || conditionals.list[i].condition.Contains(searchField))
                {
                    Rect rect = EditorGUILayout.BeginHorizontal();
                    // Draw the actual room
                    DrawRoomScript(obj, i, ref conditionals.list, ref isDirty, boxStyle);

                    EditorGUILayout.EndHorizontal();
                    
                    float value = 1f - ((boxStyle.indent-1)/9f);
                    float padding = 1f;
                    EditorGUI.DrawRect(new Rect(rect.x+padding - (7f+padding), rect.y+padding, 5f, rect.height-(padding*2f)), new Color(value, value, value, 1f));
                }
            }
        }

        int count = 0;
        foreach (int i in itemsToRemove)
        {
            conditionals.list.RemoveAt(i - count);
            count++;
        }
        itemsToRemove.Clear();   
        
        boxStyle.Unindent();

        return isDirty;
    }

    private static void DrawRoomScript(Object obj, int index, ref List<ConditionalInspectorObject> list, ref bool isDirty, StyleWrapper boxStyle)
    {
        GUIStyle textFieldStyle = GUI.skin.GetStyle("TextField");
        GUIStyle labelFieldStyle = new GUIStyle();

        GUILayout.BeginVertical(boxStyle);

        if (list[index].condition.Length == 0)
        {
            labelFieldStyle.normal.textColor = Color.gray;
        }
        else
        {
            labelFieldStyle.normal.textColor = Color.black;
        }
        /** Draw the conditions string */
		EditorGUILayout.LabelField("Conditional:", labelFieldStyle);

        textFieldStyle.fontStyle = FontStyle.Bold;
        InspectorUtils.ListenForChange();
        string newConditions = EditorGUILayout.TextField(list[index].condition, textFieldStyle);
        if (InspectorUtils.WasChanged(obj, ref isDirty, "Updated script conditions"))
        {
            list[index].condition = newConditions;
        }

        if (list[index].script.Length == 0)
        {
            labelFieldStyle.normal.textColor = Color.gray;
        }
        else
        {
            labelFieldStyle.normal.textColor = Color.black;
        }
        /** Draw the events string */
		EditorGUILayout.LabelField("Resolved:", labelFieldStyle);

        textFieldStyle.fontStyle = FontStyle.Normal;
        InspectorUtils.ListenForChange();
		string newScript = EditorGUILayout.TextArea(list[index].script);
		if (InspectorUtils.WasChanged(obj, ref isDirty, "Updated script events")) 
        {
			list[index].script = newScript;
		}

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("+", GUILayout.MaxWidth(30f), GUILayout.MinWidth(10f)))
        {
            Undo.RecordObject(obj, "Inserted new");
            list.Insert(index+1, new ConditionalInspectorObject());
            
            GUI.FocusControl(null);
            isDirty = true;
        }

        if (GUILayout.Button("-", GUILayout.MaxWidth(30f), GUILayout.MinWidth(10f)))
        {
            Undo.RecordObject(obj, "delete thing");
            list.RemoveAt(index);
            isDirty = true;
            GUI.FocusControl(null);
        }

        if (index != 0)
        {
            if (GUILayout.Button("↑", GUILayout.MaxWidth(30f), GUILayout.MinWidth(10f)))
            {
                Undo.RecordObject(obj, "Move up");

                ConditionalInspectorObject temp = list[index-1];
                list[index-1] = list[index];
                list[index] = temp;

                isDirty = true;
                GUI.FocusControl(null);
            }
        }
        else
        {
            GUILayout.Space(30);
        }

        if (index != list.Count - 1 && list.Count != 1)
        {
            if (GUILayout.Button("↓", GUILayout.MaxWidth(30f), GUILayout.MinWidth(10f)))
            {
                Undo.RecordObject(obj, "Move down");

                ConditionalInspectorObject temp = list[index+1];
                list[index+1] = list[index];
                list[index] = temp;

                isDirty = true;
                GUI.FocusControl(null);
            }
        }
        else
        {
            GUILayout.Space(30);
        }

        GUILayout.EndHorizontal();

        if (list != null && index >= 0 && index < list.Count && list[index].nested != null)
        {
            if (list[index].nested.Count == 0)
            {
                // draw button
                if (GUILayout.Button("Add nested...", GUILayout.MaxWidth(100f)))
                {
                    ConditionalInspectorObject newFriend = new ConditionalInspectorObject();
                    newFriend.showNested = true;
                    list[index].nested.Add(newFriend);
                    isDirty = true;
                }
            }
            else
            {
                if (list[index].showNested)
                {
                    // draw button
                    if (GUILayout.Button("<", GUILayout.MaxWidth(30f)))
                    {
                        Event e = Event.current;
                        if (e.alt)
                        {
                            UpdateShowHide(ref list, index, false);
                        }

                        list[index].showNested = false;
                        isDirty = true;
                    }

                    for (int i = 0; i < list[index].nested.Count; ++i)
                    {
                        boxStyle.Indent();

                        Rect rect = EditorGUILayout.BeginHorizontal();

                        DrawRoomScript(obj, i, ref list[index].nested, ref isDirty, boxStyle);
                        
                        float value = 1f - ((boxStyle.indent-1)/9f);
                        float padding = 1f;
                        EditorGUI.DrawRect(new Rect(rect.x+padding - (7f+padding), rect.y+padding, 5f, rect.height-(padding*2f)), new Color(value, value, value, 1f));

                        EditorGUILayout.EndHorizontal();

                        boxStyle.Unindent();
                    }
                }
                else
                {
                    int no = GetNestedNumber(list);

                    Rect rect = EditorGUILayout.BeginHorizontal();

                    for (int i = 0; i < no; ++i)
                    {
                        GUILayout.Space(i+1 * 10f);
                    }

                    // draw button
                    if (GUILayout.Button(">", GUILayout.MaxWidth(30f)))
                    {
                        Event e = Event.current;
                        if (e.alt)
                        {
                            UpdateShowHide(ref list, index, true);
                        }
                        
                        list[index].showNested = true;
                        isDirty = true;
                    }
   
                    EditorGUILayout.EndHorizontal();

                    for (int i = 0; i < no; ++i)
                    {
                        float value = 1f - ((i + boxStyle.indent)/9f);
                        float padding = 1f;
                        EditorGUI.DrawRect(new Rect(rect.x+padding+(i*10f), rect.y+padding, 5f, rect.height-(padding*2f)), new Color(value, value, value, 1f));
                    }
                }
            }
        }
        
        GUILayout.EndVertical();
    }

    private static void UpdateShowHide(ref List<ConditionalInspectorObject> list, int index, bool value)
    {
        list[index].showNested = value;

        for (int i = 0; i < list[index].nested.Count; ++i)
        {
            UpdateShowHide(ref list[index].nested, i, value);
        }
    }

    private static int GetNestedNumber(List<ConditionalInspectorObject> list, int no = 0)
    {
        foreach (ConditionalInspectorObject obj in list)
        {
            if (obj.nested.Count > 0)
            {
                int newNo = GetNestedNumber(obj.nested, no+1);

                if (newNo > no)
                {
                    no = newNo;
                }
            }
        }

        return no;
    }

}