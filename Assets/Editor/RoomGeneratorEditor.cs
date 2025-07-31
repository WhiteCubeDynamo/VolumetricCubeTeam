#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RoomGenerator))]
public class RoomGeneratorEditor : Editor
{
    SerializedProperty gridX;
    SerializedProperty GridY;

    SerializedProperty tileSize;
    SerializedProperty foundationSize;
    SerializedProperty wallSize; // now Vector2
    SerializedProperty tileOffset;
    SerializedProperty doorPos;
    SerializedProperty floorCount;
    SerializedProperty makeFloor;
    SerializedProperty makeWall;
    SerializedProperty makeFoundation;
    SerializedProperty notRotation;

    SerializedProperty tiles;
    SerializedProperty walls;
    SerializedProperty firstWalls;
    SerializedProperty wallCorners;
    SerializedProperty wallHalves;
    SerializedProperty doors;

    SerializedProperty floorFoundation;

    bool showRoomSettings = true;
    bool showPrefabLists = true;
    bool showFoundation = true;

    private void OnEnable()
    {
        gridX = serializedObject.FindProperty("gridX");
        GridY = serializedObject.FindProperty("GridY");

        tileSize = serializedObject.FindProperty("tileSize");
        foundationSize = serializedObject.FindProperty("foundationSize");
        wallSize = serializedObject.FindProperty("wallSize"); // Vector2
        tileOffset = serializedObject.FindProperty("tileOffset");
        doorPos = serializedObject.FindProperty("doorPos");
        floorCount = serializedObject.FindProperty("floorCount");
        makeFloor = serializedObject.FindProperty("makeFloor");
        makeWall = serializedObject.FindProperty("makeWall");
        makeFoundation = serializedObject.FindProperty("makeFoundation");
        notRotation = serializedObject.FindProperty("notRotation");

        tiles = serializedObject.FindProperty("tiles");
        walls = serializedObject.FindProperty("walls");
        firstWalls = serializedObject.FindProperty("firstWalls");
        wallCorners = serializedObject.FindProperty("wallCorners");
        wallHalves = serializedObject.FindProperty("wallHalves");
        doors = serializedObject.FindProperty("doors");

        floorFoundation = serializedObject.FindProperty("floorFoundation");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Room Dimensions", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(gridX);
        EditorGUILayout.PropertyField(GridY);

        EditorGUILayout.Space();

        showRoomSettings = EditorGUILayout.Foldout(showRoomSettings, "Room Settings");
        if (showRoomSettings)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(tileSize);
            EditorGUILayout.PropertyField(foundationSize);
            EditorGUILayout.PropertyField(wallSize); // Vector2 field
            EditorGUILayout.PropertyField(tileOffset);
            EditorGUILayout.PropertyField(doorPos);
            EditorGUILayout.PropertyField(floorCount);
            EditorGUILayout.PropertyField(makeFloor);
            EditorGUILayout.PropertyField(makeWall);
            EditorGUILayout.PropertyField(makeFoundation);
            EditorGUILayout.PropertyField(notRotation);
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();

        showPrefabLists = EditorGUILayout.Foldout(showPrefabLists, "Prefab Lists (Randomized)");
        if (showPrefabLists)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(tiles, true);
            EditorGUILayout.PropertyField(walls, true);
            EditorGUILayout.PropertyField(firstWalls, true);
            EditorGUILayout.PropertyField(wallCorners, true);
            EditorGUILayout.PropertyField(wallHalves, true);
            EditorGUILayout.PropertyField(doors, true);
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();

        showFoundation = EditorGUILayout.Foldout(showFoundation, "Foundation Prefab");
        if (showFoundation)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(floorFoundation);
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();

        RoomGenerator generator = (RoomGenerator)target;

        if (GUILayout.Button("Generate Room"))
        {
            generator.GenerateRoom();
        }

        if (GUILayout.Button("Clear Room"))
        {
            ClearChildren(generator.transform);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void ClearChildren(Transform parent)
    {
        while (parent.childCount > 0)
        {
            DestroyImmediate(parent.GetChild(0).gameObject);
        }
    }
}
#endif
