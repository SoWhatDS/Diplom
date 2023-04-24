using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class SingleShotGun : Gun
{
    [SerializeField] private Camera _camera;
    [SerializeField] private ParticleSystem _muzzleFlash;
    [SerializeField] private TMP_Text _reloadText;
    

    private float fireTimer;
    private AudioSource _AudioSource;

    private bool _isReloading;
    private float _timeReload = 3f;
    private float _reloadingTimer;

    PhotonView PV;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        _AudioSource = GetComponent<AudioSource>();
        _reloadText.gameObject.SetActive(false);
        
    }

    private void Start()
    {
        ((GunInfo)ItemInfo).currentBullets = ((GunInfo)ItemInfo).bulletsPerMag;
    }

    private void Update()
    {
        if (fireTimer < ((GunInfo)ItemInfo).fireRate)
        {
            fireTimer += Time.deltaTime;
        }

        if (_isReloading)
        {
            _reloadText.gameObject.SetActive(true);
            _reloadText.text = "Reloading";
            _timeReload -= Time.deltaTime;
            if (_timeReload <= 0)
            {
                _reloadText.gameObject.SetActive(false);
                _isReloading = false;
                _timeReload = 3f;
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }

    public override void Use()
    {
        if (fireTimer < ((GunInfo)ItemInfo).fireRate || _isReloading)
        {
            return;
        }
        if (((GunInfo)ItemInfo).currentBullets <= 0)
        {
            Reload();
            return;
        }
        Shoot();
        ((GunInfo)ItemInfo).currentBullets--;
        _muzzleFlash.Play();
        PlayShootSound();
        fireTimer = 0.0f;
    }

    private void Shoot()
    {
        Ray ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        ray.origin = _camera.transform.position;
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)ItemInfo).damage);
            PV.RPC("RPC_Shoot", RpcTarget.All, hit.point,hit.normal);
        }
    }

    private void PlayShootSound()
    {
        _AudioSource.clip = ((GunInfo)ItemInfo).shootSound;
        _AudioSource.Play();
    }

    private void Reload()
    {
        AudioManager.Instance.PlaySFX("Reloaded");
        _isReloading = true;
        if (((GunInfo)ItemInfo).bulletsLeft <= 0)
        {
            return;
        }
        int bulletsToLoad = ((GunInfo)ItemInfo).bulletsPerMag - ((GunInfo)ItemInfo).currentBullets;
        int bulletsToDeduct = (((GunInfo)ItemInfo).bulletsLeft >= bulletsToLoad) ? bulletsToLoad : ((GunInfo)ItemInfo).bulletsLeft;
        ((GunInfo)ItemInfo).bulletsLeft -= bulletsToDeduct;
        ((GunInfo)ItemInfo).currentBullets += bulletsToDeduct;
    }

    [PunRPC]
    void RPC_Shoot(Vector3 hitPosition,Vector3 hitNormal)
    {
        Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.3f);
        if (colliders.Length != 0)
        {
            GameObject bulletImpactObj = Instantiate(bulletImpactPrefab, hitPosition + hitNormal * 0.001f,
                Quaternion.LookRotation(hitNormal, Vector3.up) * bulletImpactPrefab.transform.rotation);
            Destroy(bulletImpactObj, 10f);
            bulletImpactObj.transform.SetParent(colliders[0].transform);
        }        
    }
}
