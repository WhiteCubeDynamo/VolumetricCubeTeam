using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class ObjectRepeater : MonoBehaviour
{
    [Header("Repeat Settings")]
    public GameObject targetPrefab;
    public int count = 5;
    public Vector3 offset = new Vector3(1, 0, 0);

    public void Generate()
    {
        ClearPrevious();

        if (targetPrefab == null || count <= 0)
            return;

        for (int i = 0; i < count; i++)
        {
#if UNITY_EDITOR
            GameObject clone = (GameObject)PrefabUtility.InstantiatePrefab(targetPrefab, transform);
#else
            GameObject clone = Instantiate(targetPrefab, transform);
#endif
            clone.transform.localPosition = offset * i;
            clone.name = $"{targetPrefab.name}_{i}";
        }
    }

    public void ClearPrevious()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
#if UNITY_EDITOR
            Undo.DestroyObjectImmediate(child.gameObject);
#else
            DestroyImmediate(child.gameObject);
#endif
        }
    }
}
