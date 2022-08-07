using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
    }

    public void CameraLookAr(Transform transfrom)
    {
        transform.LookAt(transform);
    }
}
