using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class PlayerExecutor : MonoBehaviour
{
    public float stepDuration = 0.3f;
    public float pauseBetweenMoves = 0.05f;
    Vector3Int currentTile;
    RoomData room;

    public bool hasKey {get; private set;}
    public bool reachedExit {get; private set;}
    public event Action<int> OnMoveExecuted;

    public void PlacePlayerAt(RoomData room, Vector3Int tile)
    {
        this.room = room;
        currentTile = tile;
        transform.position = room.collisionTilemap.GetCellCenterWorld(tile);

        hasKey = false;
        reachedExit = false;
    }

    public IEnumerator PlaybackMoves(List<Vector2Int> moves)
    {
        for (int i = 0; i < moves.Count; i++) {
            OnMoveExecuted?.Invoke(i);

            Vector3Int target = currentTile + new Vector3Int(moves[i].x, moves[i].y, 0);
            if (isWalkable(target)) {
                yield return MoveTo(target);
                currentTile = target;
            }

            yield return new WaitForSeconds(pauseBetweenMoves);
        }
        reachedExit = currentTile == room.exitTile;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Key")) {
            hasKey = true;
            Destroy(other.gameObject);
        }
    }

    bool isWalkable(Vector3Int tile) {
        return !room.collisionTilemap.HasTile(tile);
    }

    IEnumerator MoveTo(Vector3Int target)
    {
        Vector3 start = transform.position;
        Vector3 end = room.collisionTilemap.GetCellCenterWorld(target);
        float time = 0;

        while (time < stepDuration) {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(start, end, time / stepDuration);
            yield return null;
        }

        transform.position = end;
    }
}
