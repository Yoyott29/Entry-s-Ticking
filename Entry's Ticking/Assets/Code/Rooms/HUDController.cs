using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Unity.VisualScripting;

public class HUDController : MonoBehaviour
{
    public InputRecorder recorder;
    public PlayerExecutor executor;
    public TMP_Text timerText;
    public TMP_Text levelText;
    public Transform moveQueueContainer;
    public GameObject moveIconPrefab;
    public int maxMoves = 15;

    public Color baseColor = Color.white;
    public Color executedColor = Color.yellow;

    float displayTimer;
    List<TMP_Text> slots = new();


    void Awake()
    {
        for (int i = 0; i < maxMoves; i++) {
            var slot = Instantiate(moveIconPrefab, moveQueueContainer, false);
            var text = slot.GetComponent<TMP_Text>();
            text.text = "_";
            text.color = baseColor;
            slots.Add(text);
        }

    }

    public void Initialize(InputRecorder rec, PlayerExecutor exec)
    {
        if (executor != null)
            executor.OnMoveExecuted -= HighlightMove;

        recorder = rec;
        executor = exec;

        if (executor != null)
            executor.OnMoveExecuted += HighlightMove;
    }

    void OnEnable()
    {
        if (executor != null)
            executor.OnMoveExecuted += HighlightMove;
    }

    void OnDisable()
    {
        if (executor != null)
            executor.OnMoveExecuted -= HighlightMove;
    }

    void ResetSlots()
    {
        foreach (var slot in slots) {
            slot.text = "_";
            slot.color = baseColor;
        }
    }

    void HighlightMove(int index)
    {
        if (index >= 0 && index < slots.Count)
            slots[index].color = executedColor;
    }


    public void StartTimerDisplay(float duration)
    {
        displayTimer = duration;
        ResetSlots();
    }

    public void UpdateLevelText(int level)
    {
        levelText.text = "Level " + level;
    }

    void Update()
    {
        if (recorder.isDone)
            return;

        displayTimer -= Time.deltaTime;
        timerText.text = Mathf.Ceil(Mathf.Max(displayTimer, 0)).ToString() + "s";

        RefreshMoveIcons();
    }

    void RefreshMoveIcons()
    {
        for (int i = 0; i < slots.Count; i++)
            slots[i].text = i < recorder.Moves.Count ? DirectionSymbol(recorder.Moves[i]) : "_";
    }

    string DirectionSymbol(Vector2Int move)
    {
        if (move == Vector2Int.up) return "↑";
        else if (move == Vector2Int.down) return "↓";
        else if (move == Vector2Int.left) return "←";
        else if (move == Vector2Int.right) return "→";
        else return "?";
    }
}
