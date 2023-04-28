using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TimerGame : MonoBehaviour
{
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private GameObject _gameOver;

    public bool count;
    public int Time;
    ExitGames.Client.Photon.Hashtable setTime = new ExitGames.Client.Photon.Hashtable();

    private void Start()
    {
        count = true;
    }

    private void Update()
    {

        Time = (int)PhotonNetwork.CurrentRoom.CustomProperties["Time"];
        float minutes = Mathf.FloorToInt((int)PhotonNetwork.CurrentRoom.CustomProperties["Time"] / 60);
        float seconds = Mathf.FloorToInt((int)PhotonNetwork.CurrentRoom.CustomProperties["Time"] % 60);

        _timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        if (PhotonNetwork.IsMasterClient)
        {
            if (count)
            {
                count = false;
                StartCoroutine(Timer());
            }
        }

        if (Time <= 0)
        {
            _gameOver.SetActive(true);
            StartCoroutine(EndGame());
        }

    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(1);
        int nextTime = Time -= 1;
        setTime["Time"] = nextTime;
        PhotonNetwork.CurrentRoom.SetCustomProperties(setTime);
        //PhotonNetwork.CurrentRoom.CustomProperties["Time"] = nextTime;
        count = true;
    }

    IEnumerator EndGame()
    {
        yield return new WaitForSeconds(5);
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0);
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
     
    }
}
