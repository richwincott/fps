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
    GameObject hitEffectPlayerPrefab;

    PlayerUI playerUI;
    PhotonView PV;
    bool shooting = false;

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

    private void Update()
    {
        if (!PV.IsMine)
            return;

        if (Input.GetButtonDown("Fire1"))
        {
            shooting = true;
        }
        else if (Input.GetButtonUp("Fire1"))
        {
            shooting = false;
        }
    }

    private void FixedUpdate()
    {
        if (!PV.IsMine)
            return;

        if (shooting)
            PV.RPC("RPC_Shoot", RpcTarget.All);

        if (playerUI.hitMarker.activeSelf)
        {
            playerUI.hitMarker.SetActive(false);
        }
    }

    private void OnHit(RaycastHit hitInfo)
    {
        if (PV.IsMine)
        {
            Instantiate(hitInfo.collider.gameObject.CompareTag("Player") ? hitEffectPlayerPrefab : hitEffectPrefab, hitInfo.point + hitInfo.normal * 0.001f, Quaternion.FromToRotation(Vector3.up, hitInfo.normal));
            if (hitInfo.collider.gameObject.GetComponent<IDamageable>() != null)
            {
                WeaponController currentWeaponController = weaponHolderController.GetCurrentWeapon().GetComponent<WeaponController>();
                playerUI.hitMarker.SetActive(true);
                hitInfo.collider.gameObject.GetComponent<IDamageable>().TakeDamage(currentWeaponController.damage);
            }
        }
    }

    [PunRPC]
    void RPC_Shoot()
    {
        WeaponController currentWeaponController = weaponHolderController.GetCurrentWeapon().GetComponent<WeaponController>();
        currentWeaponController.Shoot(PV.IsMine);
    }

    public override void OnDisable()
    {
        BulletController.HitEvent -= OnHit;

        base.OnDisable();
    }
}
