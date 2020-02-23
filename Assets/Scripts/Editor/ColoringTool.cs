using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ColoringTool : EditorWindow {
    [SerializeField] GameObject[] objectsToColor = null;
    [SerializeField] Material materialToUse = null;

    [MenuItem ("Window/ColoringTool")]
    private static void ShowWindow () {
        var window = GetWindow<ColoringTool> ();
        window.titleContent = new GUIContent ("ColoringTool");
        window.Show ();
    }
    private void OnGUI () {
        
    }
}