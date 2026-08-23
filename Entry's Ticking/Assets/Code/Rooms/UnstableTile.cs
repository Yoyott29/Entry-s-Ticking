using UnityEngine;
using System.Collections;

public class InstableTile : MonoBehaviour
{
    public Vector3Int tilePosition;
    public float fallDuration = 0.3f;
    public float fallDistance = 0.5f;

    [Header("Shake")]
    public float idleShakeAmplitude = 0.03f;
    public float idleShakeSpeed = 1.5f;
    public float activeShakeMultiplier = 2f;

    SpriteRenderer spriteRenderer;
    bool hasFallen = false;
    bool isPlayerOn = false;
    Vector3 basePosition;
    float timeOffsetX;
    float timeOffsetY;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        basePosition = transform.position;
        timeOffsetX = UnityEngine.Random.Range(0f, 100f);
        timeOffsetY = UnityEngine.Random.Range(0f, 100f);
    }

    void Update()
    {
        if (hasFallen)
            return;

        float amplitude = idleShakeAmplitude * (isPlayerOn ? activeShakeMultiplier : 1f);
        float speed = idleShakeSpeed * (isPlayerOn ? activeShakeMultiplier : 1f);

        float offsetX = Mathf.Sin((Time.time + timeOffsetX) * speed) * amplitude;
        float offsetY = Mathf.Sin((Time.time + timeOffsetY) * speed * 1.3f) * amplitude;

        transform.position = basePosition + new Vector3(offsetX, offsetY, 0f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasFallen)
            isPlayerOn = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (hasFallen || !other.CompareTag("Player"))
            return;

        isPlayerOn = false;
        hasFallen = true;

        var executor = other.GetComponent<PlayerExecutor>();
        if (executor != null)
            executor.RegisterFallenTile(tilePosition);

        StartCoroutine(FallAndDestroy());
    }

    IEnumerator FallAndDestroy()
    {
        spriteRenderer.sortingOrder = 0;
        Vector3 start = transform.position;
        Vector3 end = start + Vector3.down * fallDistance;
        Color startColor = spriteRenderer.color;
        float time = 0;

        while (time < fallDuration)
        {
            time += Time.deltaTime;
            float progress = time / fallDuration;
            transform.position = Vector3.Lerp(start, end, progress);
            spriteRenderer.color = Color.Lerp(startColor, Color.black, progress);
            yield return null;
        }

        Destroy(gameObject);
    }
}
