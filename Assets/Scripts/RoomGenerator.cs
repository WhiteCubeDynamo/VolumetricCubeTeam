using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    [Header("Room Size")]
    [SerializeField] int gridX;
    [SerializeField] int GridY;

    [Header("Room Settings")]
    [SerializeField] float tileSize = 1f;
    [SerializeField] float foundationSize = 2f;
    [SerializeField] Vector2 wallSize = new Vector2(1f, 1f); // X = width, Y = height
    [SerializeField] Vector2 tileOffset;
    [SerializeField] Vector2Int doorPos;
    [SerializeField] int floorCount = 1;

    [SerializeField] bool makeFloor;
    [SerializeField] bool makeWall;
    [SerializeField] bool makeFoundation;
    [SerializeField] bool notRotation;

    [Header("Prefab Lists (Randomized)")]
    public List<GameObject> tiles;
    public List<GameObject> walls;
    public List<GameObject> firstWalls;
    public List<GameObject> wallCorners;
    public List<GameObject> wallHalves;
    public List<GameObject> doors;

    [Header("Foundation Prefab")]
    public GameObject floorFoundation;

    private Transform floorParent;
    private Transform wallsParent;
    private Transform foundationParent;
    private Vector3 originOffset;

    public void GenerateRoom()
    {
        originOffset = transform.position;
        ClearExistingRoom();
        CreateParentContainers();
        ClampDoorPosition();
        if (makeFloor) CreateFloor();
        if (makeWall) CreateWalls();
        if (makeFoundation) CreateFoundation();
    }

    void ClampDoorPosition()
    {
        doorPos.x = Mathf.Clamp(doorPos.x, 0, Mathf.FloorToInt(gridX / 2f) - 1);
        doorPos.y = Mathf.Clamp(doorPos.y, 0, Mathf.FloorToInt(GridY / 2f) - 1);
    }

    void CreateParentContainers()
    {
        if (makeFloor)
        {
            floorParent = new GameObject("Floor").transform;
            floorParent.SetParent(transform);
            floorParent.localPosition = Vector3.zero;
        }
        if (makeWall)
        {
            wallsParent = new GameObject("Walls").transform;
            wallsParent.SetParent(transform);
            wallsParent.localPosition = Vector3.zero;
        }
        if (makeFoundation)
        {
            foundationParent = new GameObject("Foundation").transform;
            foundationParent.SetParent(transform);
            foundationParent.localPosition = Vector3.zero;
        }
    }

    void ClearExistingRoom()
    {
        DestroyIfExists("Floor");
        DestroyIfExists("Walls");
        DestroyIfExists("Foundation");
    }

    void DestroyIfExists(string name)
    {
        Transform child = transform.Find(name);
        if (child != null) DestroyImmediate(child.gameObject);
    }

    void CreateFloor()
    {
        for (int y = 0; y < GridY; y++)
        {
            for (int x = 0; x < gridX; x++)
            {
                Vector3 localPos = new Vector3(x * tileSize + tileOffset.x, 0, -y * tileSize + tileOffset.y);
                Vector3 pos = transform.TransformPoint(localPos);
                Quaternion rot = !notRotation ? transform.rotation * GetRandomRotation() : Quaternion.identity;
                GameObject prefab = GetRandomPrefab(tiles);
                if (prefab != null)
                    Instantiate(prefab, pos, rot, floorParent).name = $"Tile_{x}_{y}";
            }
        }
    }

    private Quaternion GetRandomRotation()
    {
        int i = Random.Range(0, 4);
        return Quaternion.Euler(0, i * 90, 0);
    }

    void CreateWalls()
    {
        int halfX = Mathf.FloorToInt(gridX / (wallSize.x / tileSize));
        int halfY = Mathf.FloorToInt(GridY / (wallSize.x / tileSize));

        for (int j = 0; j < floorCount; j++)
        {
            float heightOffset = j * wallSize.y;

            Vector3 localPos = new Vector3(0, heightOffset, 0);
            Vector3 pos = transform.TransformPoint(localPos);
            Quaternion rot = transform.rotation;
            GameObject first = GetRandomPrefab(firstWalls);
            if (first != null)
                Instantiate(first, pos, rot, wallsParent);

            for (int i = 1; i < halfX; i++)
            {
                localPos = new Vector3(i * wallSize.x, heightOffset, 0);
                pos = transform.TransformPoint(localPos);
                GameObject prefab = (doorPos.x != 0 && j == 0 && i == doorPos.x)
                    ? GetRandomPrefab(doors)
                    : GetRandomPrefab(walls);
                if (prefab != null)
                    Instantiate(prefab, pos, rot, wallsParent);
            }

            localPos = new Vector3(halfX * wallSize.x, heightOffset, 0);
            pos = transform.TransformPoint(localPos);
            rot = transform.rotation * Quaternion.Euler(0, 180, 0);
            GameObject corner = GetRandomPrefab(wallCorners);
            if (corner != null)
                Instantiate(corner, pos, rot, wallsParent);

            for (int i = 1; i < halfY; i++)
            {
                localPos = new Vector3(halfX * wallSize.x, heightOffset, -i * wallSize.x);
                pos = transform.TransformPoint(localPos);
                rot = transform.rotation * Quaternion.Euler(0, 90, 0);
                GameObject prefab = (doorPos.y != 0 && j == 0 && i == doorPos.y)
                    ? GetRandomPrefab(doors)
                    : GetRandomPrefab(walls);
                if (prefab != null)
                    Instantiate(prefab, pos, rot, wallsParent);
            }

            localPos = new Vector3(halfX * wallSize.x, heightOffset, -halfY * wallSize.x);
            pos = transform.TransformPoint(localPos);
            rot = transform.rotation * Quaternion.Euler(0, -90, 0);
            GameObject half = GetRandomPrefab(wallHalves);
            if (half != null)
                Instantiate(half, pos, rot, wallsParent);
        }
    }

    void CreateFoundation()
    { 
        for (int y = 0; y < GridY / (foundationSize / tileSize); y++)
        {
            Vector3 localPos = new Vector3(tileOffset.x, -2f, -y * tileSize + tileOffset.y);
            Vector3 pos = transform.TransformPoint(localPos);
            if (floorFoundation != null)
                Instantiate(floorFoundation, pos, transform.rotation, foundationParent);
        }

        for (int x = 1; x < gridX / (foundationSize / tileSize); x++)
        {
            Vector3 localPos = new Vector3(x * tileSize + tileOffset.x, -2f, -(GridY - 1) * tileSize + tileOffset.y);
            Vector3 pos = transform.TransformPoint(localPos);
            if (floorFoundation != null)
                Instantiate(floorFoundation, pos, transform.rotation, foundationParent);
        }
    }

    GameObject GetRandomPrefab(List<GameObject> list)
    {
        if (list == null || list.Count == 0) return null;
        return list[Random.Range(0, list.Count)];
    }
}
