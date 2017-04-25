using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(Lift))]
public class LiftInspector : Editor
{
    Lift lift;
    void OnSceneGUI()
    {
        lift = target as Lift;

        if (Tools.current == Tool.Move)
        {
            Tools.hidden = EditorApplication.isPlaying;
        }
        else
        {
            Tools.hidden = false;
        }

        //停止中はオブジェクトの座標と同期するので描画しない
        if (EditorApplication.isPlaying)
        {
            ShowHandle(ref lift.startPoint);
        }
        else
        {
            if (IsChangeStartPoint()) ChangeStartPoint();
        }
        
        ShowHandle(ref lift.endPoint);
        DrawLine();
    }

    void ShowHandle(ref Vector3 point)
    {
        EditorGUI.BeginChangeCheck();
        Vector3 handlePoint;
        handlePoint = Handles.DoPositionHandle(point, Quaternion.identity);

        if (!EditorGUI.EndChangeCheck()) return;

        //変更
        Undo.RecordObject(lift, "Move Point");
        EditorUtility.SetDirty(lift);
        point = handlePoint;

#pragma warning disable CS0618
            lift.MoveRestart();
#pragma warning restore CS0618
    }

    bool IsChangeStartPoint()
    {
        return lift.startPoint != lift.transform.position;
    }

    void ChangeStartPoint()
    {
        Undo.RecordObject(lift, "Move Point");
        EditorUtility.SetDirty(lift);
        lift.startPoint = lift.transform.localPosition;
    }

    void DrawLine()
    {
        Handles.color = Color.white;
        Handles.DrawLine(lift.startPoint, lift.endPoint);

        Handles.color = new Color(1.0f, 0.0f, 0.0f, 0.5f); //red
        Handles.SphereHandleCap(0, lift.startPoint, Quaternion.identity, 0.5f, EventType.Repaint);

        Handles.color = new Color(0.0f, 1.0f, 0.0f, 0.5f); //green
        Handles.SphereHandleCap(0, lift.endPoint,   Quaternion.identity, 0.5f, EventType.Repaint);
    }
}
