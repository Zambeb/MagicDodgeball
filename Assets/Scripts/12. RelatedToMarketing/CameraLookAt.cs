using System;
using UnityEngine;

public class CameraLookAt : MonoBehaviour
{
    [SerializeField] private Transform target;

    private void Update()
    {
        if (!target)
            return;

        transform.LookAt(target);
    }
}