using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerExecutor : MonoBehaviour
{
    public float stepDuration = 0.3f;
    public float pauseBetweenMoves = 0.05f;
    public float fallDuration = 0.3f;
    public float fallDistance = 0.5f;

    Vector3Int currentTile;
    RoomData room;
    SpriteRenderer spriteRenderer;
    List<Vector3Int> fallenTiles = new();
    Dictionary<Vector3Int, Barrel> barrels = new();
    Vector2Int facingDirection = Vector2Int.up;

    public bool hasKey {get; private set;}
    public bool reachedExit {get; private set;}
    public bool fellInHole {get; private set;}
    public event Action<int> OnMoveExecuted;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void PlacePlayerAt(RoomData room, Vector3Int tile)
    {
        this.room = room;
        currentTile = tile;
        transform.position = room.collisionTilemap.GetCellCenterWorld(tile);

        hasKey = false;
        reachedExit = false;
        fellInHole = false;
        fallenTiles.Clear();
        facingDirection = Vector2Int.up;

        barrels.Clear();
        foreach(var barrel in room.GetComponentsInChildren<Barrel>())
            barrels[barrel.tilePosition] = barrel;

        spriteRenderer.sortingOrder = 5;
        spriteRenderer.color = Color.white;
        spriteRenderer.enabled = true;
    }

    public void RegisterFallenTile(Vector3Int tile)
    {
        if (!fallenTiles.Contains(tile))
            fallenTiles.Add(tile);
    }

    public IEnumerator PlaybackMoves(List<QueuedAction> moves)
    {
        for (int i = 0; i < moves.Count; i++) {
            OnMoveExecuted?.Invoke(i);
            var action = moves[i];

            if (action.type == ActionType.Move)
            {
                facingDirection = action.direction;
                Vector3Int target = currentTile + new Vector3Int(action.direction.x, action.direction.y, 0);

                if (isWalkable(target))
                {
                    yield return MoveTo(target);
                    currentTile = target;

                    if (fallenTiles.Contains(currentTile)) {
                        yield return PlayerFall();
                        fellInHole = true;
                        yield break;
                    }
                }
            } else if (action.type == ActionType.Attack)
            {
                Vector3Int target = currentTile + new Vector3Int(facingDirection.x, facingDirection.y, 0);

                if (barrels.TryGetValue(target, out Barrel barrel)) {
                    barrel.Break();
                    barrels.Remove(target);
                }
            }

            yield return new WaitForSeconds(pauseBetweenMoves);
        }
        reachedExit = currentTile == room.exitTile;
    }

    IEnumerator PlayerFall()
    {
        Vector3 start = transform.position;
        Vector3 end = start + Vector3.down * fallDistance;
        Color startColor = spriteRenderer.color;
        float time = 0;

        while (time < fallDuration)
        {
            time += Time.deltaTime;
            if (time >= 0.4 * fallDuration)
                spriteRenderer.sortingOrder = 0;
            float progress = time / fallDuration;
            transform.position = Vector3.Lerp(start, end, progress);
            spriteRenderer.color = Color.Lerp(startColor, Color.black, progress);
            yield return null;
        }
        spriteRenderer.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Key")) {
            hasKey = true;
            Destroy(other.gameObject);
        }
    }

    bool isWalkable(Vector3Int tile) {
        return !room.collisionTilemap.HasTile(tile) && !barrels.ContainsKey(tile);
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
