using System.Collections;
using UnityEngine;


public enum GamePhase { Loading, Recording, Executing, Result }

public class GameManager : MonoBehaviour
{
    public GamePhase CurrentPhase { get; private set; }
    public RoomManager roomManager;
    public InputRecorder recorder;
    public PlayerExecutor executor;
    public HUDController hud;
    public float recordTime = 10f;
    public int currentLevel = 1;
    bool lastAttemptSuccess = true;

    void Start() {
        StartCoroutine(RunLevel());
    }

    IEnumerator RunLevel()
    {
        while (true)
        {
            CurrentPhase = GamePhase.Loading;
            var room = lastAttemptSuccess ? roomManager.LoadRandomRoom() : roomManager.ReloadCurrentRoom();
            executor.PlacePlayerAt(room, room.spawnTile);
            hud.UpdateLevelText(currentLevel);

            CurrentPhase = GamePhase.Recording;
            recorder.BeginRecording(recordTime);
            hud.StartTimerDisplay(recordTime);
            yield return new WaitUntil(() => recorder.isDone);

            CurrentPhase = GamePhase.Executing;
            yield return executor.PlaybackMoves(recorder.Moves);

            bool success = executor.reachedExit && executor.hasKey;
            if (success)
                currentLevel++;            
            lastAttemptSuccess = success;

            CurrentPhase = GamePhase.Result;
            Debug.Log(success ? "Level completed!" : "Failed, retrying same room.");
            yield return new WaitForSeconds(3f);
        }
    }
}
