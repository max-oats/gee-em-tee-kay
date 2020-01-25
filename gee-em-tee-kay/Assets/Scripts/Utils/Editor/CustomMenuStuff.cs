using UnityEditor;
using UnityEngine;

public class CustomMenuStuff : MonoBehaviour
{
    [MenuItem("GameObject/Laundry/Create Bézier Spline", false, 0)]
    static void CreateBezier(MenuCommand command)
    {
        GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<Object>("Assets/Prefabs/Misc/BezierSpline.prefab"));
        GameObjectUtility.SetParentAndAlign(go, (GameObject)command.context);
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;
    }

    [MenuItem("GameObject/Laundry/Create Interest Point", false, 0)]
    static void CreateInterestPoint(MenuCommand command)
    {
        GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<Object>("Assets/Prefabs/Components/InterestPoint.prefab"));
        GameObjectUtility.SetParentAndAlign(go, (GameObject)command.context);
        go.layer = 13;
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;
    }
}