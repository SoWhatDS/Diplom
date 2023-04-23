using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MachineGunShoot : Gun
{
    [SerializeField] private Camera _camera;

    PhotonView PV;

    private float fireTimer;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (fireTimer < ((GunInfo)ItemInfo).fireRate)
        {
            fireTimer += Time.deltaTime;
        }
    }

    public override void Use()
    {
        if (fireTimer < ((GunInfo)ItemInfo).fireRate)
        {
            return;
        }
        Shoot();

        fireTimer = 0.0f;
    }

    private void Shoot()
    {
        Ray ray = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        ray.origin = _camera.transform.position;
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)ItemInfo).damage);
            PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
        }
    }

    [PunRPC]
    void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal)
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
