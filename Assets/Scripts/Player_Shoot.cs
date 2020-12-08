using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

public class Player_Shoot : MonoBehaviourPunCallbacks
{
    PhotonView PV;

    private bool shooting = false;
    private bool firstLoop = false;

    [SerializeField]
    private WeaponHolderController weaponHolderController;
    [SerializeField]
    private ParticleSystem hitEffectPrefab;
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private AudioSource gunSound;

    private PlayerUI playerUI;
    private WeaponController currentWeaponController;
    private float nextFireTime = 0f;
    private bool fireInput;

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
        if (!currentWeaponController)
        {
            GameObject currentWeapon = weaponHolderController.GetCurrentWeapon();
            if (currentWeapon)
                currentWeaponController = currentWeapon.GetComponent<WeaponController>();
        }

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
                            currentWeaponController.RemoveBulletFromMag();
                            TransmitShootValue(true);
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
                            currentWeaponController.RemoveBulletFromMag();
                            TransmitShootValue(true);
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
            Instantiate(hitEffectPrefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
            if (hitInfo.collider.tag == "Player")
            {
                playerUI.hitMarker.SetActive(true);
                GameObject playerHit = GameObject.Find(hitInfo.collider.name);
                playerHit.GetComponent<Player_Health>().InflictDamage(currentWeaponController.damage);
            }
            else if (hitInfo.collider.tag == "Bot")
            {
                playerUI.hitMarker.SetActive(true);
                GameObject botHit = GameObject.Find(hitInfo.collider.name);
                //botHit.GetComponent<Bot_Health>().InflictDamage(currentWeaponController.damage);
            }
        }
    }

    public void TransmitShootValue(bool value)
    {
        if (PV.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("playerShoot", value);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    private void Shoot()
    {
        SpawnBullet();
        gunSound.Play();
        currentWeaponController.flash.Play();
    }

    private void SpawnBullet()
    {
        Instantiate(bulletPrefab,
            currentWeaponController.bulletSpawnOffset.position,
            currentWeaponController.animator.transform.rotation);
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
    {
        if (!PV.IsMine && targetPlayer == PV.Owner)
        {
            if (changedProps.ContainsKey("playerShoot"))
            {
                GameObject playerShooting = GameObject.Find(targetPlayer.NickName);
                if (playerShooting && (bool)changedProps["playerShoot"])
                    playerShooting.GetComponent<Player_Shoot>().Shoot();
            }
        }
    }

    public override void OnDisable()
    {
        BulletController.HitEvent -= OnHit;

        base.OnDisable();
    }
}
