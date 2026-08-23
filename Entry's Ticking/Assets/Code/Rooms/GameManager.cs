using System.Collections;
using UnityEngine;


public enum GamePhase { Loading, Recording, Executing, Result }

public class GameManager : MonoBehaviour
{
    public GamePhase CurrentPhase { get; private set; }
    private RoomManager roomManager;
    private InputRecorder recorder;
    private PlayerExecutor executor;
    private HUDController hud;
    public float recordTime = 10f;
    public int currentLevel = 1;
    bool lastAttemptSuccess = true;

    public static GameManager instance;

    void Awake()
    {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else
            Destroy(gameObject);
    }

    void Start()
    {
        roomManager = gameObject.GetComponent<RoomManager>();
        recorder = gameObject.GetComponent<InputRecorder>();
        executor = GameObject.Find("Player").GetComponent<PlayerExecutor>();
        hud = GameObject.Find("Canvas").GetComponent<HUDController>();

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

            bool success = executor.reachedExit && executor.hasKey && !executor.fellInHole;
            if (success)
                currentLevel++;            
            lastAttemptSuccess = success;

            CurrentPhase = GamePhase.Result;
            if (success)
                yield return LaunchQuestionScene();
            else
                yield return new WaitForSeconds(3f);
        }
    }

    IEnumerator LaunchQuestionScene()
    {
        string previousSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        UnityEngine.SceneManagement.SceneManager.LoadScene("Question", UnityEngine.SceneManagement.LoadSceneMode.Additive);

        yield return null;

        yield return UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(previousSceneName);

        yield return new WaitUntil(() => Manage_Dictionary.questionAnsweredCorrectly);
        
        Manage_Dictionary.questionAnsweredCorrectly = false;

        yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(previousSceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive);

        yield return UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("Question");

        executor = GameObject.Find("Player").GetComponent<PlayerExecutor>();
        hud = GameObject.Find("Canvas").GetComponent<HUDController>();
        hud.Initialize(recorder, executor);
        
        UnityEngine.SceneManagement.SceneManager.SetActiveScene(
            UnityEngine.SceneManagement.SceneManager.GetSceneByName(previousSceneName)
        );
    }
}
