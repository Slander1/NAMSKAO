using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }

    private Camera _camera;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
        _camera = Camera.main;
    }

    public void CameraLookAt(Transform transfrom)
    {
        transform.LookAt(transform);
    }
}
