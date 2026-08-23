using UnityEngine;

public class Demon_Hover : MonoBehaviour
{
    public float amplitude = 10f;   // how far up/down it moves, in UI pixels
    public float speed = 2f;  // how fast it oscillates

    private RectTransform rectTransform;
    private Vector2 basePosition;
    private float timeOffset;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        basePosition = rectTransform.anchoredPosition;
        timeOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        float offsetY = Mathf.Sin((Time.time + timeOffset) * speed) * amplitude;
        rectTransform.anchoredPosition = basePosition + new Vector2(0f, offsetY);
    }
}

