using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//[CustomEditor(typeof(TileManager))]
public class TileManagerEditor : Editor
{
    TileManager tileManager = null;

    private void OnEnable()
    {
        tileManager = (TileManager) target;

        ArrayLayout array = new ArrayLayout(tileManager.height);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        //EditorGUILayout.IntSlider(hpProperty, 0, 100);

        serializedObject.ApplyModifiedProperties();
    }
}
