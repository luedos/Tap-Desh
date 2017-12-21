using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class U_EQS_Test : MonoBehaviour {

    [HideInInspector]
    public U_EQS_Generator MyGenerator;

    [Tooltip("If true, test will always give 1")]
    public bool TurnOffTest = false;

    [Range(0f, 1f)]
    public float Multiplier = 1f;

    [Tooltip("This value tast will gave if it fails")]
    [Range(0f, 1f)]
    public float MinMultiplier = 0f;

    public U_EQS_Query MyQuery;

    public virtual float CulcPoint(Vector3 InPoint) { return 0f; }

}
