
using UnityEngine;
using Photon.Pun;
using System.IO;
using Photon.Realtime;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;
    [SerializeField] private List<EnemyAi> _enemies = new List<EnemyAi>();


    private GameObject _controller;
    private GameObject[] _enemyControllers;

    private int _kills;
    private int _deaths;
    private int _totalExp;
    private int _expForKill = 10;


    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        _enemyControllers = new GameObject[_enemies.Count]; 
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

        _deaths++;
        _totalExp -= _expForKill;
        if (_totalExp < 0)
        {
            _totalExp = 0;
        }
        PlayFabManager.Instance.SendLeaderScore(_totalExp);
        Hashtable hash = new Hashtable();
        hash.Add("Deaths", _deaths);
        hash.Add("TotalExp", _totalExp);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public void GetKill()
    {
        PV.RPC(nameof(RPC_GetKill), PV.Owner);
    }

    [PunRPC]
    private void RPC_GetKill()
    {
        _kills++;
        _totalExp += _expForKill;
        PlayFabManager.Instance.SendLeaderScore(_totalExp);
        Hashtable hash = new Hashtable();
        hash.Add("Kills", _kills);
        hash.Add("TotalExp", _totalExp);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public static PlayerManager Find(Player player)
    {
        return FindObjectsOfType<PlayerManager>().SingleOrDefault(x => x.PV.Owner == player);
    }
}
