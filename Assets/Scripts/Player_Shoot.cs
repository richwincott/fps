using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

public class Player_Shoot : MonoBehaviourPunCallbacks
{
    [SerializeField]
    WeaponHolderController weaponHolderController;
    [SerializeField]
    GameObject hitEffectPrefab;
    [SerializeField]
    GameObject bulletPrefab;
    [SerializeField]
    AudioSource gunSound;

    PlayerUI playerUI;
    WeaponController currentWeaponController;
    float nextFireTime = 0f;
    PhotonView PV;
    bool shooting = false;
    bool firstLoop = false;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    public override void OnEnable()
    {
        BulletController.HitEvent += OnHit;

        base.OnEnable();
    }

    private void Start()
    {
        if (PV.IsMine)
            playerUI = GameObject.Find("PlayerUI").GetComponent<PlayerUI>();
    }

    private void FixedUpdate()
    {
        GameObject currentWeapon = weaponHolderController.GetCurrentWeapon();

        if (currentWeapon != null)
            currentWeaponController = currentWeapon.GetComponent<WeaponController>();

        if (currentWeaponController)
        {
            if (PV.IsMine)
            {
                if (currentWeaponController.rapidFire)
                {
                    if (Input.GetButton("Fire1") && nextFireTime < Time.time)
                    {
                        if (currentWeaponController.CanShoot() && !shooting)
                        {
                            nextFireTime = Time.time + currentWeaponController.rateOfFire;
                            shooting = true;
                            firstLoop = true;
                            Shoot();
                            gunSound.Play();
                            PV.RPC("RPC_Shoot", RpcTarget.All, new object[0]);
                            currentWeaponController.RemoveBulletFromMag();
                        }
                    }
                }
                else
                {
                    if (Input.GetButtonDown("Fire1"))
                    {
                        if (currentWeaponController.CanShoot() && !shooting)
                        {
                            shooting = true;
                            firstLoop = true;
                            Shoot();
                            gunSound.Play();
                            PV.RPC("RPC_Shoot", RpcTarget.All, new object[0]);
                            currentWeaponController.RemoveBulletFromMag();
                        }
                    }
                }

                if (playerUI.hitMarker.activeSelf)
                {
                    playerUI.hitMarker.SetActive(false);
                }
                if (shooting) {
                    if (!firstLoop)
                        shooting = false;
                    firstLoop = false;
                }
            }
        } 
    }

    private void OnHit(RaycastHit hitInfo)
    {
        if (PV.IsMine && currentWeaponController)
        {
            Instantiate(hitEffectPrefab, hitInfo.point + hitInfo.normal * 0.001f, Quaternion.FromToRotation(Vector3.up, hitInfo.normal));
            if (hitInfo.collider.gameObject.GetComponent<IDamageable>() != null)
            {
                playerUI.hitMarker.SetActive(true);
                hitInfo.collider.gameObject.GetComponent<IDamageable>().TakeDamage(currentWeaponController.damage);
            }
        }
    }

    private void Shoot()
    {
        SpawnBullet();
        currentWeaponController.flash.Play();
    }

    [PunRPC]
    void RPC_Shoot()
    {
        if (!PV.IsMine)
        {
            Shoot();
        }
    }

    private void SpawnBullet()
    {
        Instantiate(bulletPrefab,
            currentWeaponController.bulletSpawnOffset.position,
            currentWeaponController.gameObject.transform.rotation);
    }

    public override void OnDisable()
    {
        BulletController.HitEvent -= OnHit;

        base.OnDisable();
    }
}
