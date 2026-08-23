using UnityEngine;
using TMPro;

public class RecoverableWord : MonoBehaviour
{
    public string originalWord;
    public TMP_Text wordText;

    [Header("Bobbing")]
    public float bobAmplitude = 0.05f;
    public float bobSpeed = 2f;

    Vector3 basePosition;
    float timeOffset;

    void Awake()
    {
        basePosition = transform.position;
        timeOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        float offsetY = Mathf.Sin((Time.time + timeOffset) * bobSpeed) * bobAmplitude;
        transform.position = basePosition + new Vector3(0f, offsetY, 0f);
    }

    public void SetWord(string word)
    {
        originalWord = word;
        if (wordText != null)
            wordText.text = word;
    }
}
 