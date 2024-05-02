using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainSceneView : MonoBehaviour
{
    private readonly string startRecordingText = "Start Recording";
    private readonly string stopRecordingText = "Stop Recording";

    [SerializeField]
    private CameraRotationService cameraRotationService;
    [SerializeField]
    private RecordingService recordingService;
    [SerializeField]
    private PlaybackService playbackService;
    [SerializeField]
    private SceneManager sceneManager;

    [SerializeField]
    private FixedJoystick joystick;

    [SerializeField]
    private Button recordingButton;
    [SerializeField]
    private Button previewCurrentRecordingButton;
    [SerializeField]
    private Button saveCurrentRecordingButton;
    [SerializeField]
    private Button loadAllRecordingsButton;
    [SerializeField]
    private TMP_Dropdown recordingsDropdown;
    [SerializeField]
    private TMP_Text startStopRecordingText;
    [SerializeField]
    private Button playSelectedRecordingButton;
    [SerializeField]
    private Button playInNewSceneButton;

    private void Awake()
    {
        recordingButton.onClick.AddListener(ToogleRecording);
        previewCurrentRecordingButton.onClick.AddListener(PreviewCurrentRecording);
        saveCurrentRecordingButton.onClick.AddListener(SaveCurrentRecording);
        loadAllRecordingsButton.onClick.AddListener(LoadAllRecordings);
        playSelectedRecordingButton.onClick.AddListener(StartRecordingPlayback);
        playInNewSceneButton.onClick.AddListener(PlayInNewScene);
        recordingsDropdown.onValueChanged.AddListener(OnDropDownValueChanged);
    }

    private void Start()
    {
        LoadAllRecordings();
    }

    private void OnDropDownValueChanged(int value)
    {
        recordingService.UpdateCurrentRecording(value);
    }

    private void ToogleRecording()
    {
        recordingService.IsRecording = !recordingService.IsRecording;
        startStopRecordingText.text = recordingService.IsRecording ? stopRecordingText : startRecordingText;

        if (recordingService.IsRecording && !playbackService.IsPlaying)
            recordingService.StartRecording();
        else
        {
            recordingService.StopRecording();
        }
    }

    private void PreviewCurrentRecording()
    {
        if(!recordingService.IsRecording)
            playbackService.StartPlayback(recordingService.GetCurrentRecording());
    }


    private void SaveCurrentRecording()
    {
        recordingService.SaveCurrentRecordingToFile();
    }

    private void LoadAllRecordings()
    {
        recordingService.LoadRecordingFiles();
        UpdateRecordingDropdown();
    }

    private void StartRecordingPlayback()
    {
        if (recordingService.IsRecordigsExist() && !recordingService.IsRecording)
            playbackService.StartPlayback(recordingService.GetRecordingByIndex(recordingsDropdown.value));
    }

    private void PlayInNewScene()
    {
        string currentRecordingName = recordingsDropdown.options[recordingsDropdown.value].text;
        string currentRecordingPath = Path.Combine(recordingService.RecordingsPath, currentRecordingName + ".json");
        sceneManager.OpenRecordingInNewScene(currentRecordingPath);
        
    }

    private void Update()
    {
        if (joystick.Horizontal != 0 || joystick.Vertical != 0)
            cameraRotationService.RotateCamera(joystick.Horizontal, joystick.Vertical);
    }

    private void UpdateRecordingDropdown()
    {
        if (recordingsDropdown == null)
            return;
        recordingsDropdown.options.Clear();
        var recordingNames = recordingService.GetAllRecordings().Keys;
        foreach (var recordingName in recordingNames)
            recordingsDropdown.options.Add(new TMP_Dropdown.OptionData() { text = recordingName });

        recordingsDropdown.value = recordingsDropdown.options.Count;
    }

    private void OnDestroy()
    {
        recordingButton.onClick.RemoveAllListeners();
        playSelectedRecordingButton.onClick.RemoveAllListeners();
    }
}
