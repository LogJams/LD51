using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(OverworldManager))]
public class OverworldEditorTools : Editor {

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        if (GUILayout.Button("Regenerate World")) {
            ((OverworldManager)target).Generate();
        }
    }
}
