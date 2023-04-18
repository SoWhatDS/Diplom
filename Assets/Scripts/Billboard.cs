using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera _camera;

    private void Update()
    {
        if (_camera == null)
        {
            _camera = FindAnyObjectByType<Camera>();
        }

        if (_camera == null)
        {
            return;
        }

        transform.LookAt(_camera.transform);
        transform.Rotate(Vector3.up * 180);
    }
}
