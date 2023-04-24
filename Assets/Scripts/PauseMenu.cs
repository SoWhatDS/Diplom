using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{

    public static bool isOn = false;

    private void Start()
    {

    }

    public void LeaveRoom()
    {      
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0);
    }

}
