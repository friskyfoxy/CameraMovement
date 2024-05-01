using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.IO;

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

    private List<List<CameraStep>> recordings;

    private string recordingsPath;

    private void Awake()
    {
        currentRecording = new List<CameraStep>();
        recordings = new List<List<CameraStep>>();
        cameraTransform = transform;

        recordingsPath = Application.persistentDataPath + "/Recordings";
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
        return recordings[index];
    }

    public List<CameraStep> GetCurrentRecording()
    {
        return currentRecording;
    }

    public bool IsRecornigsExist()
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

    public List<List<CameraStep>> GetAllRecordings()
    {
        return recordings;
    }

    public void SaveCurrentRecordingToFile()
    {
        string json = JsonConvert.SerializeObject(currentRecording);
        if(!Directory.Exists(recordingsPath))
            Directory.CreateDirectory(recordingsPath);

        File.WriteAllText(Path.Combine(recordingsPath, "Recording_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".json"), json);
    }

    public void LoadRecordingFiles()
    {
        recordings.Clear();
        string[] files = Directory.GetFiles(recordingsPath, "*.json");

        foreach (var file in files)
        {
            string json = File.ReadAllText(file);
            List<CameraStep> recording = JsonConvert.DeserializeObject<List<CameraStep>>(json);
            recordings.Add(recording);
        }
    }
}
