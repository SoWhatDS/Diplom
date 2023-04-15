
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class RoomListItem : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;

    public RoomInfo _info;

    public void SetUp(RoomInfo info)
    {
        _info = info;
        _text.text = _info.Name;
    }

    public void OnClick()
    {
        Launcher.Instance.JoinRoom(_info);
    }
}
