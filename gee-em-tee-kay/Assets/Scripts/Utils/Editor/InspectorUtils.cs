using UnityEngine;
using UnityEditor;

public class StyleWrapper
{
    public GUIStyle style;

    public int indent = 1;

    public void Indent()
    {
        indent++;

        style.margin = new RectOffset(10, 10, 10, 10);
    }

    public void Unindent()
    {
        indent--;

        style.margin = new RectOffset(10, 10, 10, 10);
    }
    
    public static implicit operator GUIStyle(StyleWrapper sw) => sw.style;
}

public static class InspectorUtils
{

    public static void DrawQuad(Rect position, Color color) 
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0,0,color);
        texture.Apply();
        GUI.skin.box.normal.background = texture;
        GUI.Box(position, GUIContent.none);
    }

    public static void ListenForChange()
    {
        EditorGUI.BeginChangeCheck();
    }

    public static bool WasChanged(Object affectedObject, string record = "Updated property")
    {
        bool wasChanged = EditorGUI.EndChangeCheck();

        if (wasChanged)
        {
            Undo.RecordObject(affectedObject, record);
        }

        return wasChanged;
    }

    public static bool WasChanged(Object affectedObject, ref bool isDirty, string record = "Updated property")
    {
        bool wasChanged = EditorGUI.EndChangeCheck();

        if (wasChanged)
        {
            isDirty = true;
            Undo.RecordObject(affectedObject, record);
        }

        return wasChanged;
    }

}