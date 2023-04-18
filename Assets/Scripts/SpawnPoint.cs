using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private GameObject _graphics;

    private void Awake()
    {
        _graphics.SetActive(false);
    }
}
