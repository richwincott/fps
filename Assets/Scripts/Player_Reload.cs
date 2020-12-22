using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player_Reload : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private WeaponHolderController weaponHolderController;

    PhotonView PV;
    WeaponController currentWeaponController;
    Player player;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    public void Start()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {
        GameObject currentWeapon = weaponHolderController.GetCurrentWeapon();

        if (!currentWeapon)
            return;

        currentWeaponController = currentWeapon.GetComponent<WeaponController>();

        if (PV.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                Reload();
                player.TransmitReloadingState(true);
            }
            else if (Input.GetKeyUp(KeyCode.R))
            {
                player.TransmitReloadingState(false);
            }
        }
        else
        {
            if (player.reloading)
            {
                Reload();
            }
        }
    }

    public void Reload()
    {
        currentWeaponController.reloading = true;
        currentWeaponController.animator.Play("Reload");
        StartCoroutine(ReloadTimer());
    }

    public IEnumerator ReloadTimer()
    {
        yield return new WaitForSeconds(1f);
        currentWeaponController.reloading = false;
        currentWeaponController.animator.Play("Idle");
        currentWeaponController.Reload();
    }
}
