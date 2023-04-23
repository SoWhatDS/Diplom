using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerController : MonoBehaviourPunCallbacks,IDamageable
{
    [SerializeField] private Image _healthBarImage;
    [SerializeField] private GameObject _ui;
    [SerializeField] private float _mouseSensitivity;
    [SerializeField] private float _sprintSpeed;
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _smoothTime;

    [SerializeField] private GameObject _cameraHolder;

    [SerializeField] private Item[] _items;

    private int _itemIndex;
    private int _previousItemIndex = -1;

    private bool _isGrounded;
    private Vector3 _smoothMoveVelocity;
    private Vector3 _moveAmount;
    private float _verticalLookRotation;

    private Rigidbody _rb;
    private PhotonView _PV;

    private const float Max_Health = 100f;
    private float _currentHealth = Max_Health;

    PlayerManager _playerManager;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _PV = GetComponent<PhotonView>();

        _playerManager = PhotonView.Find((int)_PV.InstantiationData[0]).GetComponent<PlayerManager>();
    }

    private void Start()
    {
        if (_PV.IsMine)
        {
            EquipItem(0);
        }
        else
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(_rb);
            Destroy(_ui);
        }
    }

    private void Update()
    {
        if (!_PV.IsMine)
        {
            return;
        }

        Look();

        Move();

        Jump();

        ChangeWeapon();

        ScroolWheelChangeWeapon();

        Fire();

        DieForOutPerimetrMap();
       
    }

    private void DieForOutPerimetrMap()
    {
        if (transform.position.y <= -10f)
        {
            Die();
        }
    }

    private void Fire()
    {
        if (Input.GetMouseButton(0))
        {
            _items[_itemIndex].Use();
        }
    }

    private void ChangeWeapon()
    {
        for (int i = 0; i < _items.Length; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                EquipItem(i);
                break;
            }
        }
    }

    private void ScroolWheelChangeWeapon()
    {
        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {
            if (_itemIndex >= _items.Length - 1)
            {
                EquipItem(0);
            }
            else
            {
                EquipItem(_itemIndex + 1);
            }
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            if (_itemIndex <= 0)
            {
                EquipItem(_items.Length - 1);
            }
            else
            {
                EquipItem(_itemIndex - 1);
            }
        }
    }

    private void Look()
    {
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * _mouseSensitivity);

        _verticalLookRotation += Input.GetAxisRaw("Mouse Y") * _mouseSensitivity;
        _verticalLookRotation = Mathf.Clamp(_verticalLookRotation, -90f, 90f);

        _cameraHolder.transform.localEulerAngles = Vector3.left * _verticalLookRotation;
    }

    private void Move()
    {
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        _moveAmount = Vector3.SmoothDamp(_moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? _sprintSpeed : _walkSpeed), ref _smoothMoveVelocity, _smoothTime);
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
        {
            _rb.AddForce(transform.up * _jumpForce);
        }
    }

    void EquipItem(int index)
    {
        if (index == _previousItemIndex)
        {
            return;
        }
        _itemIndex = index;
        _items[_itemIndex].ItemGameObject.SetActive(true);

        if (_previousItemIndex != -1)
        {
            _items[_previousItemIndex].ItemGameObject.SetActive(false);
        }

        _previousItemIndex = _itemIndex;

        if (_PV.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("ItemIndex", _itemIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("ItemIndex") && !_PV.IsMine && targetPlayer == _PV.Owner)
        {
            EquipItem((int)changedProps["ItemIndex"]);
        }
    }

    public void SetGroundState(bool isGrounded)
    {
        _isGrounded = isGrounded;
    }

    private void FixedUpdate()
    {
        if (!_PV.IsMine)
        {
            return;
        }
        _rb.MovePosition(_rb.position + transform.TransformDirection(_moveAmount) * Time.fixedDeltaTime);
    }

    public void TakeDamage(float damage)
    {
        _PV.RPC(nameof(RPC_TakeDamage), _PV.Owner, damage);
    }

    [PunRPC]
    void RPC_TakeDamage(float damage,PhotonMessageInfo info)
    {
        _currentHealth -= damage;

        _healthBarImage.fillAmount = _currentHealth / Max_Health;

        if(_currentHealth <= 0)
        {
            Die();
            PlayerManager.Find(info.Sender).GetKill();
        }
    }

    private void Die()
    {
        _playerManager.Die();
    }
}
