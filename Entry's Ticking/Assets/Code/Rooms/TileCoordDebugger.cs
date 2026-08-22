using UnityEngine;
using UnityEngine.Tilemaps;

public class TileCoordDebugger : MonoBehaviour
{
    public Tilemap tilemap;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0;
            Vector3Int tile = tilemap.WorldToCell(mouseWorld);
            Debug.Log($"Clicked cell: {tile}");
        }
    }
}
