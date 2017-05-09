using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(Lift))]
public class LiftInspector : Editor
{
    private Lift lift;
    private Quaternion handleRotate = new Quaternion(0, 0, 0, 1);

    private Color lineColor       = new Color(1.0f, 1.0f, 1.0f, 1.0f); //white
    private Color lineColorPoint1 = new Color(1.0f, 0.0f, 0.0f, 0.5f); //red
    private Color lineColorPoint2 = new Color(0.0f, 1.0f, 0.0f, 0.5f); //green 

    void OnSceneGUI()
    {
        lift = target as Lift;
        
        bool isPlaying = EditorApplication.isPlaying;
        Tools.hidden = (Tools.current == Tool.Move) ? isPlaying : false;

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
        Vector3 handlePoint = Handles.DoPositionHandle(point, handleRotate);

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
        Handles.color = lineColor;
        Handles.DrawLine(lift.startPoint, lift.endPoint);

        Handles.color = lineColorPoint1;
        Handles.SphereHandleCap(0, lift.startPoint, handleRotate, 0.5f, EventType.Repaint);

        Handles.color = lineColorPoint2;
        Handles.SphereHandleCap(0, lift.endPoint,   handleRotate, 0.5f, EventType.Repaint);
    }
}
