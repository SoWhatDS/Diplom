using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class PlayerNameManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField _usernameInput;

    private void Start()
    {
        if (PlayerPrefs.HasKey("username"))
        {
            _usernameInput.text = PlayerPrefs.GetString("username");
            PhotonNetwork.NickName = PlayerPrefs.GetString("username");
        }
        else
        {
            _usernameInput.text = "Player: " + Random.Range(0, 1000).ToString("0000");
            OnUsernameInputChanged();
        }
    }

    public void OnUsernameInputChanged()
    {
        PhotonNetwork.NickName = _usernameInput.text;
        PlayerPrefs.SetString("username", _usernameInput.text);
    }
}
