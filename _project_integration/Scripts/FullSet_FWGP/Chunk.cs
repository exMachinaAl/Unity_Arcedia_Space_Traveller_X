using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public Vector2Int coord;
    public GameObject chunkObject;
    public MeshRenderer renderer;
    public MeshFilter filter;
    public List<GameObject> spawnedObjects = new List<GameObject>();


    public Chunk(Vector2Int coord, GameObject prefab, Transform parent)
    {
        this.coord = coord;

        chunkObject = GameObject.Instantiate(prefab, parent);
        chunkObject.name = $"Chunk {coord.x} {coord.y}";

        renderer = chunkObject.GetComponent<MeshRenderer>();
        filter = chunkObject.GetComponent<MeshFilter>();
    }

    public void SetPosition(Vector3 pos)
    {
        chunkObject.transform.position = pos;
    }
}
