using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ColoringTool : EditorWindow
{
    Color colorToUse;
    Material material;

    [MenuItem("Window/ColoringTool")]
    private static void ShowWindow()
    {
        var window = GetWindow<ColoringTool>();
        window.titleContent = new GUIContent("ColoringTool");
        window.Show();
    }
    private void OnGUI()
    {
        GUILayout.Label("Place objects to be colored");

        colorToUse = EditorGUILayout.ColorField("Color", colorToUse);
        material = (Material)EditorGUILayout.ObjectField("Material To Use", material, typeof(Material));

        if (GUILayout.Button("Recolor"))
        {
            material.color = colorToUse;
            foreach (GameObject selected in Selection.gameObjects)
            {
                Renderer renderer = selected.GetComponent<Renderer>();

                if (renderer != null)
                {
                    renderer.material = material;
                    renderer.sharedMaterial.color = colorToUse;
                }
            }
        }
    }
}