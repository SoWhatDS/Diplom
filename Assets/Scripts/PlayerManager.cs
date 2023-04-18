
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;

    private GameObject _controller;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (PV.IsMine)
        {
            CreateController();
        }
    }

    private void CreateController()
    {
        Transform spawnPoint = SpawnManager.Instance.GetSpawnPoint();
       _controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnPoint.position, spawnPoint.rotation,0,new object[] { PV.ViewID });
    }

    public void Die()
    {
        PhotonNetwork.Destroy(_controller);
        CreateController();
    }
}
