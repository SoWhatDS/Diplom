
using UnityEngine;
using Photon.Pun;
using System.IO;
using Photon.Realtime;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Collections.Generic;

public class AIManager : MonoBehaviour
{
    [SerializeField] private GameObject _enemy;
    [SerializeField] private int _countEnemies = 5; 

    private GameObject _controller;
    private GameObject _enemyController;

    private int _kills;
    private int _deaths;
    private int _totalExp;
    private int _expForKill = 10;


    private void Awake()
    {

    }

    private void Start()
    {
      
    }

    private void CreateEnemy()
    {
        Transform spawnpoint = SpawnManager.Instance.GetSpawnPoint();
        _enemyController = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "EnemyAI"), spawnpoint.position, spawnpoint.rotation, 0);
    }


    public void Die()
    {
        PhotonNetwork.Destroy(_controller);
        CreateEnemy();

        _deaths++;
        Hashtable hash = new Hashtable();
        hash.Add("Deaths", _deaths);
        hash.Add("TotalExp", _totalExp);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }


}
