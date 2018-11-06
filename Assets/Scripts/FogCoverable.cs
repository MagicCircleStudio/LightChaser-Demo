using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogCoverable : MonoBehaviour
{
    Renderer renderer;

    void Start()
    {
        renderer = GetComponent<Renderer>();
        // PlayerController.OnTargetsVisibilityChange += FieldOfViewOnTargetsVisibilityChange;
    }

    void OnDestroy()
    {
        // PlayerController.OnTargetsVisibilityChange -= FieldOfViewOnTargetsVisibilityChange;
    }

    void FieldOfViewOnTargetsVisibilityChange(List<Transform> newTargets)
    {
        renderer.enabled = newTargets.Contains(transform);
    }
}