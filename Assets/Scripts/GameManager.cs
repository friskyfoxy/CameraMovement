using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
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

    private void Awake()
    {
        recordingButton.onClick.AddListener(StartStopRecording);
        previewCurrentRecordingButton.onClick.AddListener(PreviewCurrentRecording);
        saveCurrentRecordingButton.onClick.AddListener(SaveCurrentRecording);
        loadAllRecordingsButton.onClick.AddListener(LoadAllRecordings);
        playSelectedRecordingButton.onClick.AddListener(StartRecordingPlayback);
    }

    private void StartStopRecording()
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
        if (recordingService.IsRecornigsExist() && !recordingService.IsRecording)
            playbackService.StartPlayback(recordingService.GetRecordingByIndex(recordingsDropdown.value));
    }

    private void Update()
    {
        if (joystick.Horizontal != 0 || joystick.Vertical != 0)
            cameraRotationService.RotateCamera(joystick.Horizontal, joystick.Vertical);
    }

    private void UpdateRecordingDropdown()
    {
        recordingsDropdown.options.Clear();
        var recordings = recordingService.GetAllRecordings();
        for (int i = 0, count = recordings.Count; i < count; i++)
            recordingsDropdown.options.Add(new TMP_Dropdown.OptionData() { text = i.ToString() });

        recordingsDropdown.value = recordingsDropdown.options.Count;
    }

    private void OnDestroy()
    {
        recordingButton.onClick.RemoveAllListeners();
        playSelectedRecordingButton.onClick.RemoveAllListeners();
    }
}
