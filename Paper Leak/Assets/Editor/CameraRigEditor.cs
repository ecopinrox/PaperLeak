using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CameraRig))]
public class CameraRigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        CameraRig rig = (CameraRig)target;
        if(GUILayout.Button("Recalibrate confiner bounds"))
        {
            rig.SetConfinerBounds();
        }
    }
}
