using System;
using UnityEngine;

[Serializable]
public struct CameraStep
{
    public Vector3 Position;
    public Quaternion Rotation;

    public CameraStep(Vector3 position, Quaternion rotation)
    {
        Position = position;
        Rotation = rotation;
    }
}