using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class UserNameDisplay : MonoBehaviour
{
    [SerializeField] private PhotonView _playerPV;
    [SerializeField] TMP_Text _text;

    private void Start()
    {
        if (_playerPV.IsMine)
        {
            gameObject.SetActive(false);
        }
        _text.text = _playerPV.Owner.NickName;
    }
}
