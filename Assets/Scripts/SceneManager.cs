using UnityEngine;
using System.Collections.Generic;

public class SceneManager : MonoBehaviour
{
    [SerializeField]
    private PlaybackService playbackService;
    [SerializeField]
    private RecordingService recordingService;

    public static SceneManager Instance;
    [HideInInspector]
    public List<CameraStep> CurrentRecording;
    [HideInInspector]
    public string CurrentRecordingPath;

    private readonly string recordingSceneName = "RecordingScene";
    private readonly string mainSceneName = "MainScene";

    private void Awake()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        CurrentRecording = new List<CameraStep>();

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        if (scene.name == recordingSceneName)
        {
            PlayRecording();
        }
    }

    public void PlayRecording()
    {
        if (string.IsNullOrWhiteSpace(CurrentRecordingPath))
            return;

        List<CameraStep> currentRecording = recordingService.LoadRecordingFromFile(CurrentRecordingPath);
        playbackService.StartPlayback(currentRecording, LoadMainScene);
    }

    public void OpenRecordingInNewScene(string recordingPath)
    {
        CurrentRecordingPath = recordingPath;
        UnityEngine.SceneManagement.SceneManager.LoadScene(recordingSceneName);
    }

    private void LoadMainScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(mainSceneName);
    }
}