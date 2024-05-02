using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

public class RecordingService : MonoBehaviour
{
    [SerializeField]
    private float positionThreshold = 0.1f;
    [SerializeField]
    private float rotationThreshold = 0.1f;

    public bool IsRecording { get; set; }

    private Coroutine recordStepsCoroutine;
    private List<CameraStep> currentRecording;
    private Transform cameraTransform;

    private Vector3 lastPosition;
    private Quaternion lastRotation;

    private Dictionary<string,List<CameraStep>> recordings;

    [HideInInspector]
    public string RecordingsPath;

    private void Awake()
    {
        currentRecording = new List<CameraStep>();
        recordings = new Dictionary<string, List<CameraStep>>();
        cameraTransform = transform;

        RecordingsPath = Application.persistentDataPath + "/Recordings";
    }

    public void StartRecording()
    {
        currentRecording.Clear();

        lastPosition = cameraTransform.position;
        lastRotation = cameraTransform.rotation;

        recordStepsCoroutine = StartCoroutine(RecordSteps());
    }

    public void StopRecording()
    {
        StopCoroutine(recordStepsCoroutine);
    }

    public List<CameraStep> GetRecordingByIndex(int index)
    {
        return recordings.ElementAt(index).Value;
    }

    public List<CameraStep> GetCurrentRecording()
    {
        return currentRecording;
    }

    public bool IsRecordigsExist()
    {
        return recordings != null && recordings.Count > 0;
    }

    private IEnumerator RecordSteps()
    {
        while (IsRecording)
        {
            if (Vector3.Distance(cameraTransform.position, lastPosition) > positionThreshold ||
                Quaternion.Angle(cameraTransform.rotation, lastRotation) > rotationThreshold)
            {
                currentRecording.Add(new CameraStep(cameraTransform.position, cameraTransform.rotation));
                lastPosition = cameraTransform.position;
                lastRotation = cameraTransform.rotation;
            }
            yield return null;
        }
    }

    public Dictionary<string, List<CameraStep>> GetAllRecordings()
    {
        return recordings;
    }

    public string SaveCurrentRecordingToFile()
    {
        string json = JsonConvert.SerializeObject(currentRecording);
        if(!Directory.Exists(RecordingsPath))
            Directory.CreateDirectory(RecordingsPath);

        var path = Path.Combine(RecordingsPath, "Recording_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".json");
        File.WriteAllText(path, json);
        return path;
    }

    public void LoadRecordingFiles()
    {
        recordings.Clear();
        string[] files = Directory.GetFiles(RecordingsPath, "*.json");

        foreach (var file in files)
        {
            var name = Path.GetFileNameWithoutExtension(file);
            string json = File.ReadAllText(file);
            List<CameraStep> recording = JsonConvert.DeserializeObject<List<CameraStep>>(json);
            recordings.Add(name, recording);
        }
    }

    public List<CameraStep> LoadRecordingFromFile(string path)
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<List<CameraStep>>(json);
        }
        return null;
    }

    public void UpdateCurrentRecording(int index)
    {
        currentRecording = GetRecordingByIndex(index);
    }
}
