using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ObjectRepeater))]
public class ObjectRepeaterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ObjectRepeater repeater = (ObjectRepeater)target;

        EditorGUILayout.Space();
        if (GUILayout.Button("Generate"))
        {
            repeater.Generate();
        }

        if (GUILayout.Button("Clear"))
        {
            repeater.ClearPrevious();
        }
    }
}