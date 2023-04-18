
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;

    [SerializeField] private ServerSettings _serverSettings;
    [SerializeField] private TMP_InputField _roomNameInputField;
    [SerializeField] private TMP_Text _errorText;
    [SerializeField] private TMP_Text _roomNameText;
    [SerializeField] private Transform _roomListContent;
    [SerializeField] private GameObject _roomListItemPrefab;
    [SerializeField] private Transform _playerListContent;
    [SerializeField] private GameObject _playerListItem;
    [SerializeField] private GameObject _startGameButton;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Debug.Log("Connecting to Master");
        PhotonNetwork.ConnectUsingSettings(_serverSettings.AppSettings);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        MenuManager.Instance.OpenMenu("Title");
        Debug.Log("Joined Lobby");
    }

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(_roomNameInputField.text))
        {
            return;
        }
        PhotonNetwork.CreateRoom(_roomNameInputField.text);
        MenuManager.Instance.OpenMenu("Loading");
    }

    public override void OnJoinedRoom()
    {
        MenuManager.Instance.OpenMenu("Room");
        _roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        Player[] players = PhotonNetwork.PlayerList;

        foreach (Transform child in _playerListContent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < players.Length; i++)
        {
            Instantiate(_playerListItem, _playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
        }

        _startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        _startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        _errorText.text = "Room Creation Failed: " + message;
        MenuManager.Instance.OpenMenu("Error");
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(1);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("Loading");
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.Instance.OpenMenu("Loading");
    }

    public override void OnLeftRoom()
    {
        MenuManager.Instance.OpenMenu("Title");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform trans in _roomListContent)
        {
            Destroy(trans.gameObject);
        }

        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
            {
                continue;
            }
            Instantiate(_roomListItemPrefab, _roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(_playerListItem, _playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }
}
