using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FlowerSection))]
public class FlowerSectionEditor : Editor
{
    private void OnSceneViewGUI(SceneView sv)
    {
        FlowerSection fs = target as FlowerSection;

        fs.startPoint = Handles.PositionHandle(fs.startPoint, Quaternion.identity);
        fs.endPoint = Handles.PositionHandle(fs.endPoint, Quaternion.identity);
        fs.startTangent = Handles.PositionHandle(fs.startTangent, Quaternion.identity);
        fs.endTangent = Handles.PositionHandle(fs.endTangent, Quaternion.identity);

        Handles.DrawBezier(fs.startPoint, fs.endPoint, fs.startTangent, fs.endTangent, Color.red, null, 20f);
    }

    void OnEnable()
    {
        Debug.Log("OnEnable");
        SceneView.onSceneGUIDelegate += OnSceneViewGUI;
    }

    void OnDisable()
    {
        Debug.Log("OnDisable");
        SceneView.onSceneGUIDelegate -= OnSceneViewGUI;
    }
}
