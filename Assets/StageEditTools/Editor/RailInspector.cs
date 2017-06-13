using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RailCreator))]
public class RailInspector : Editor
{
    private RailCreator rail;

    private Quaternion handleRotate = Quaternion.identity;

    private Color lineColor = Color.white;

    private void OnSceneGUI()
    {
        rail = target as RailCreator;

        //線を描画
        for (int i = 0; i < rail.points.Length - 1; i++)
        {
            Vector3 p0 = ShowPoint(i);
            Vector3 p1 = ShowPoint(i+1);

            Handles.color = lineColor;
            Handles.DrawLine(p0, p1);
        }
    }

    private Vector3 ShowPoint(int index)
    {
        Vector3 point = rail.points[index];
        EditorGUI.BeginChangeCheck();
        point = Handles.DoPositionHandle(point, handleRotate);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(rail, "Move Point");
            EditorUtility.SetDirty(rail);
            rail.points[index] = point;
        }
        return point;
    }

    public override void OnInspectorGUI()
    {
        rail = target as RailCreator;

        DrawPointsInspector();

        //ポイントを追加
        if (GUILayout.Button("Add Rail"))
        {
            Undo.RecordObject(rail, "Add Rail");
            rail.AddPoint();
            EditorUtility.SetDirty(rail);
        }
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        rail.createObject = EditorGUILayout.ObjectField ("Rail Object", rail.createObject, typeof(GameObject), false) as GameObject;

        EditorGUILayout.Space();
        GUILayout.Label("Railの名前");
        rail.railName = EditorGUILayout.TextField("Rail Name", rail.railName);

        EditorGUILayout.Space();
        GUILayout.Label("GenerateはUndo出来ません");
        if (GUILayout.Button("Generate"))
        {
            rail.Generate();
        }
    }

    //削除ボタン用
    GUILayoutOption[] option = new GUILayoutOption[]
    {
        GUILayout.Width(15),
        GUILayout.Height(15)
    };

    private void DrawPointsInspector()
    {
        GUILayout.Label("Selected Point");
        GUILayout.Label("Rails Number : " + rail.points.Length);

        for (int i = 0; i < rail.points.Length; i++)
        {
            EditorGUILayout.BeginHorizontal();

                //削除ボタン
                if (GUILayout.Button("-", option))
                {
                    Undo.RecordObject(rail, "Add Rail");
                    rail.RemovePoint(i);
                    EditorUtility.SetDirty(rail);
                }

                Vector3 pos = rail.points[i];
                pos = EditorGUILayout.Vector3Field("Position", pos);
                if (pos != rail.points[i])
                {
                    Undo.RecordObject(rail, "Move Point");
                    EditorUtility.SetDirty(rail);
                    rail.points[i] = pos;
                }

            EditorGUILayout.EndHorizontal();
        }
    }
}
