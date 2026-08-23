using System.Collections;
using UnityEngine;
using System.Collections.Generic;


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
    List<string> currentRoomWords = new();
    List<string> lostWordsThisRoom = new();

    public static GameManager instance;
    private GameObject timerSoundObject;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

    }

    void Start()
    {

        roomManager = gameObject.GetComponent<RoomManager>();
        timerSoundObject = gameObject.transform.Find("Timer Sound").gameObject;

        recorder = gameObject.GetComponent<InputRecorder>();

        var playerObj = GameObject.Find("Player");
        if (playerObj != null)
            executor = playerObj.GetComponent<PlayerExecutor>();

        var canvasObj = GameObject.Find("Canvas");
        if (canvasObj != null)
            hud = canvasObj.GetComponent<HUDController>();

        StartCoroutine(RunLevel());
    }

    IEnumerator RunLevel()
    {

        while (true)
        {
            CurrentPhase = GamePhase.Loading;
            RoomData room;

            if (lastAttemptSuccess)
            {
                room = roomManager.LoadRandomRoom();

                currentRoomWords = WordPoolManager.instance.PickWordsForRoom(3);
                lostWordsThisRoom.Clear();
            }
            else
                room = roomManager.ReloadCurrentRoom();

            room.AssignWords(currentRoomWords, lostWordsThisRoom);

            executor.PlacePlayerAt(room, room.spawnTile);

            hud.UpdateLevelText(currentLevel);

            CurrentPhase = GamePhase.Recording;
            Manage_Sounds.PlaySFX("Timer", timerSoundObject, false, true);

            recorder.BeginRecording(recordTime);
            hud.StartTimerDisplay(recordTime);
            yield return new WaitUntil(() => recorder.isDone);

            Manage_Sounds.StopSFX(timerSoundObject);
            
            CurrentPhase = GamePhase.Executing;
            yield return executor.PlaybackMoves(recorder.Moves);

            bool success = executor.reachedExit && executor.hasKey && !executor.fellInHole;

            if (success)
            {
                foreach (var word in executor.TakenWords)
                {
                    Manage_Dictionary.addWord(word);
                    WordPoolManager.instance.RemoveWordPermanently(word);
                }
                currentLevel++;
            }
            else
            {
                lostWordsThisRoom.AddRange(executor.TakenWords);
            }

            lastAttemptSuccess = success;
            CurrentPhase = GamePhase.Result;

            if (success)
                yield return LaunchQuestionScene();
            else
                yield return new WaitForSeconds(3f);

        }
    }


    public static void ResetAndReturnToMenu()
    {
        Manage_Dictionary.fullDictionary = null;
        Manage_Dictionary.availableDictionary = null;
        Manage_Dictionary.questionAnsweredCorrectly = false;

        if (WordPoolManager.instance != null)
            Destroy(WordPoolManager.instance.gameObject);
        WordPoolManager.instance = null;

        SingleEventSystem.ResetInstance();
        
        if (instance != null)
            Destroy(instance.gameObject);
        instance = null;

        UnityEngine.SceneManagement.SceneManager.LoadScene("Main Menu", UnityEngine.SceneManagement.LoadSceneMode.Single);
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
