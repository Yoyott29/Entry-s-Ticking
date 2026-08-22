using UnityEngine;
using System.Collections.Generic;

public class InputRecorder : MonoBehaviour
{
    public List<Vector2Int> Moves { get; private set; } = new();
    public bool isDone { get; private set; }
    float timer;
    public int maxMoves = 15;

    static readonly Dictionary<KeyCode, Vector2Int> Keymap = new()
    {
        { KeyCode.W, Vector2Int.up },
        { KeyCode.S, Vector2Int.down },
        { KeyCode.A, Vector2Int.left },
        { KeyCode.D, Vector2Int.right }
    };

    public void BeginRecording(float duration)
    {
        Moves.Clear();
        timer = duration;
        isDone = false;
    }

    void Update()
    {
        if (isDone)
            return;

        foreach(var keyValue in Keymap)
            if (Input.GetKeyDown(keyValue.Key) && Moves.Count < maxMoves)
                Moves.Add(keyValue.Value);

        if (Input.GetKeyDown(KeyCode.Backspace) && Moves.Count > 0)
            Moves.RemoveAt(Moves.Count - 1);

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
            isDone = true;
            timer = 0f;
            return;
        }

        timer -= Time.deltaTime;
        if (timer <= 0f)
            isDone = true;
    }
}
