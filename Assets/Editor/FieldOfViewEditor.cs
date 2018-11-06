using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FieldOfView))]
public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        FieldOfView fow = (FieldOfView)target;
        // Vector3 aimingPosition = fow.transform.position + fow.transform.up * fow.aimingHeight;

        Handles.color = Color.blue;
        // Handles.DrawWireArc(aimingPosition, Vector3.up, Vector3.forward, 360, 0.1f);
        Handles.DrawWireArc(fow.transform.position, Vector3.up, Vector3.forward, 360, fow.viewRadius);

        Vector3 viewAngleA = fow.DirFromAngle(-fow.viewAngle / 2, false);
        Vector3 viewAngleB = fow.DirFromAngle(fow.viewAngle / 2, false);
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleA * fow.viewRadius);
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleB * fow.viewRadius);

        Handles.color = Color.red;
        foreach (var visibleTarget in fow.visibleTargets)
        {
            Handles.DrawWireArc(visibleTarget.position, Vector3.up, Vector3.forward, 360, 0.1f);
            // Handles.DrawLine(aimingPosition, visibleTarget.position);
        }
    }
}
