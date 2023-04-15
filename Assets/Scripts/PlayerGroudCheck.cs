using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroudCheck : MonoBehaviour
{
    PlayerController _playerController;

    private void Awake()
    {
        _playerController = GetComponentInParent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        _playerController.SetGroundState(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject == _playerController.gameObject)
        {
            return;
        }
        _playerController.SetGroundState(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == _playerController.gameObject)
        {
            return;
        }
        _playerController.SetGroundState(true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        _playerController.SetGroundState(true);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == _playerController.gameObject)
        {
            return;
        }
        _playerController.SetGroundState(false);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject == _playerController.gameObject)
        {
            return;
        }
        _playerController.SetGroundState(true);
    }
}
