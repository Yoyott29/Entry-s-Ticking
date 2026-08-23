using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerExecutor : MonoBehaviour
{
    public float stepDuration = 0.3f;
    public float originalPauseBetweenMoves = 0.05f;
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
    List<string> takenWords = new();

    private Animator playerAnimator;
    private GameObject actionSoundObject;

    private bool walking = false;

    public IReadOnlyList<string> TakenWords => takenWords;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerAnimator = GetComponent<Animator>();
        actionSoundObject = gameObject.transform.Find("Action Sound").gameObject;
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
        takenWords.Clear();

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
            float pauseBetweenMoves = originalPauseBetweenMoves;
            OnMoveExecuted?.Invoke(i);
            var action = moves[i];

            if (action.type == ActionType.Move)
            {
                facingDirection = action.direction;
                Vector3Int target = currentTile + new Vector3Int(action.direction.x, action.direction.y, 0);

                if (isWalkable(target))
                {
                    if (!walking)
                    {
                        Manage_Sounds.PlaySFX("Footsteps", gameObject, false, true);
                        walking = true;
                    }

                    yield return MoveTo(target);
                    currentTile = target;

                    if (fallenTiles.Contains(currentTile) && !fellInHole) {
                        walking = false;
                        Manage_Sounds.PlaySFX("Fall", gameObject, false);
                        yield return PlayerFall();
                        fellInHole = true;
                        yield break;
                    }
                }
                else
                {
                    walking = false;
                    Manage_Sounds.StopSFX(gameObject);
                    playerAnimator.SetFloat("X", facingDirection.x);
                    playerAnimator.SetFloat("Y", facingDirection.y);
                }
            } else if (action.type == ActionType.Attack)
            {
                walking = false;
                Manage_Sounds.StopSFX(gameObject);
                playerAnimator.SetBool("Swinging", true);
                Manage_Sounds.PlaySFX("Sword Swing", actionSoundObject, true);
                pauseBetweenMoves = 1.0f;
                Vector3Int target = currentTile + new Vector3Int(facingDirection.x, facingDirection.y, 0);

                if (barrels.TryGetValue(target, out Barrel barrel)) {
                    barrel.Break();
                    Manage_Sounds.PlaySFX("Wood Breaking", actionSoundObject, false);
                    barrels.Remove(target);
                }
            }

            yield return new WaitForSeconds(pauseBetweenMoves);
            playerAnimator.SetBool("Swinging", false);
        }
        reachedExit = currentTile == room.exitTile;
        if (reachedExit) {
            walking = false;
            if (hasKey)
            {
                Manage_Sounds.StopSFX(gameObject);
                Manage_Sounds.PlaySFX("Door", actionSoundObject, false);
            }
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            walking = false;
            Manage_Sounds.StopSFX(gameObject);
        }
        yield return null;
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
            Manage_Sounds.PlaySFX("Collect Key", actionSoundObject, false);
            hasKey = true;
            Destroy(other.gameObject);
        } else if (other.CompareTag("RecoverableWord"))
        {
            Manage_Sounds.PlaySFX("Collect Key", actionSoundObject, false);
            var word = other.GetComponent<RecoverableWord>();
            if (word != null) {
                takenWords.Add(word.originalWord);
                Destroy(other.gameObject);
            }
        }
    }

    bool isWalkable(Vector3Int tile) {
        return !room.collisionTilemap.HasTile(tile) && !barrels.ContainsKey(tile);
    }

    IEnumerator MoveTo(Vector3Int target)
    {
        playerAnimator.SetBool("Walking", true);

        Vector3 start = transform.position;
        Vector3 end = room.collisionTilemap.GetCellCenterWorld(target);
        float time = 0;
        playerAnimator.SetFloat("X", facingDirection.x);
        playerAnimator.SetFloat("Y", facingDirection.y);

        while (time < stepDuration) {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(start, end, time / stepDuration);
            yield return null;
        }

        transform.position = end;

        playerAnimator.SetBool("Walking", false);
    }
}



