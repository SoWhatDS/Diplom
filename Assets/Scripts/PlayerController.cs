using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
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

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _PV = GetComponent<PhotonView>();
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

        for (int i = 0; i < _items.Length; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                EquipItem(i);
                break;
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
}
