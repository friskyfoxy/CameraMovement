using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaybackService : MonoBehaviour
{
    [SerializeField]
    private float playbackSpeed = 1000f;

    public bool IsPlaying { get; private set; }

    private Transform cameraTransform;

    public void StartPlayback(List<CameraStep> currentRecording, Action callback = null)
    {
        IsPlaying = true;
        cameraTransform = Camera.main.transform;
        StartCoroutine(Play(currentRecording, callback));
    }

    private IEnumerator Play(List<CameraStep> currentRecording, Action callback = null)
    {
        if (IsPlaying)
        {
            for (int i = 0, count = currentRecording.Count; i < count; i++)
            {
                CameraStep cameraStep = currentRecording[i];
                float distanceToStep = Vector3.Distance(cameraTransform.position, cameraStep.Position);
                float playbackDuration = distanceToStep / (playbackSpeed * Time.deltaTime);
                float playbackStartTime = Time.time;

                while (Time.time < playbackStartTime + playbackDuration)
                {
                    float t = (Time.time - playbackStartTime) / playbackDuration;
                    cameraTransform.SetPositionAndRotation(Vector3.Lerp(cameraTransform.position, cameraStep.Position, t), Quaternion.Slerp(cameraTransform.rotation, cameraStep.Rotation, t));
                    yield return null;
                }
                cameraTransform.SetPositionAndRotation(cameraStep.Position, cameraStep.Rotation);
            }
            IsPlaying = false;
            yield return new WaitForSeconds(1);
            callback?.Invoke();
        }
    }
}
