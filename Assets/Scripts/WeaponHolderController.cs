using Photon.Pun;
using UnityEngine;

public class WeaponHolderController : MonoBehaviour
{
    [SerializeField]
    private Transform weaponHolder;

    [SerializeField]
    private GameObject primaryWeaponPrefab;
    [SerializeField]
    private GameObject secondaryWeaponPrefab;

    [SerializeField]
    private float aimSpeed = 10f;
    [SerializeField]
    private Player player;

    private Vector3 originalPosition;
    private GameObject primaryWeapon;
    private GameObject secondaryWeapon;

    private void Start()
    {
        originalPosition = transform.localPosition;

        SpawnWeapons();
        ActivateCurrentWeapon();
    }


    private void SpawnWeapons()
    {
        if (primaryWeaponPrefab != null)
            primaryWeapon = Instantiate(primaryWeaponPrefab, weaponHolder);
        if (secondaryWeaponPrefab != null)
            secondaryWeapon = Instantiate(secondaryWeaponPrefab, weaponHolder);
    }

    private void ActivateCurrentWeapon()
    {
        if (player == null)
        {
            bool isBot = primaryWeapon.GetComponent<WeaponController>().isBot;

            if (isBot)
                return;

            isBot = true;
            secondaryWeapon.SetActive(false);
            primaryWeapon.SetActive(true);
            return;
        }
        if (player.weaponId != player.previousWeaponId) {
            if (player.weaponId == 1)
            {
                secondaryWeapon.SetActive(false);
                primaryWeapon.SetActive(true);
            }
            if (player.weaponId == 2)
            {
                primaryWeapon.SetActive(false);
                secondaryWeapon.SetActive(true);
            }
        }
    }

    public void ResetWeapons()
    {
        Destroy(primaryWeapon);
        primaryWeapon = Instantiate(primaryWeaponPrefab, weaponHolder);
        primaryWeapon.SetActive(true);
        Destroy(secondaryWeapon);
        secondaryWeapon = Instantiate(secondaryWeaponPrefab, weaponHolder);
        secondaryWeapon.SetActive(false);
    }

    public void PickUpWeapon(GameObject weaponPrefab)
    {
        secondaryWeapon.SetActive(false);
        primaryWeapon.SetActive(false);

        if (player == null)
        {
            Destroy(primaryWeapon);
            primaryWeapon = Instantiate(weaponPrefab, weaponHolder);
            primaryWeapon.GetComponent<WeaponController>().isBot = true;
            primaryWeapon.SetActive(true);
            return;
        }

        if (player.weaponId == 1)
        {
            Destroy(primaryWeapon);
            primaryWeapon = Instantiate(weaponPrefab, weaponHolder);
            primaryWeapon.SetActive(true);
        }
        if (player.weaponId == 2)
        {
            Destroy(secondaryWeapon);
            secondaryWeapon = Instantiate(weaponPrefab, weaponHolder);
            secondaryWeapon.SetActive(true);
        }
    }

    public GameObject GetCurrentWeapon()
    {
        if (player == null)
            return primaryWeapon;

        if (player.weaponId == 1)
            return primaryWeapon;
        if (player.weaponId == 2)
            return secondaryWeapon;

        return null;
    }

    private void Update()
    {
        SwitchWeapons();
        AimDownSights();
    }

    private void SwitchWeapons()
    {
        if (player == null)
            return;

        if (player.GetComponent<PhotonView>().IsMine)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) && player.weaponId == 2)
            {
                player.previousWeaponId = player.weaponId;
                player.weaponId = 1;
                player.TransmitWeaponId(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) && player.weaponId == 1)
            {
                player.previousWeaponId = player.weaponId;
                player.weaponId = 2;
                player.TransmitWeaponId(2);
            }
            ActivateCurrentWeapon();
        }
        else
        {
            ActivateCurrentWeapon();
        }
    }

    public void AimDownSights()
    {
        if (player == null)
            return;

        if (player.GetComponent<PhotonView>().IsMine)
        {
            if (Input.GetButton("Fire2"))
                transform.localPosition = Vector3.Lerp(transform.localPosition, GetCurrentWeapon().GetComponent<WeaponController>().aimPosition, Time.deltaTime * aimSpeed);
            else
                transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, Time.deltaTime * aimSpeed);

            if (Input.GetButtonDown("Fire2"))
            {
                GetCurrentWeapon().GetComponent<WeaponSwayScript>().isEnabled = false;
                player.TransmitAimState(true);
            }
            if (Input.GetButtonUp("Fire2"))
            {
                GetCurrentWeapon().GetComponent<WeaponSwayScript>().isEnabled = true;
                player.TransmitAimState(false);
            }
        }
        else
        {
            if (player.aiming)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, GetCurrentWeapon().GetComponent<WeaponController>().aimPosition, Time.deltaTime * aimSpeed);
            }
            else
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, Time.deltaTime * aimSpeed);
            }
        }
    }
}
