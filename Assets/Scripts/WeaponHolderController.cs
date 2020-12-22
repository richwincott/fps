using Photon.Pun;
using System.IO;
using UnityEngine;

public class WeaponHolderController : MonoBehaviour
{
    [SerializeField]
    Transform weaponHolder;
    [SerializeField]
    GameObject[] availableWeapons;
    [SerializeField]
    GameObject primaryWeaponPrefab;
    [SerializeField]
    GameObject secondaryWeaponPrefab;
    [SerializeField]
    float aimSpeed = 10f;
    [SerializeField]
    Player player;

    Vector3 originalPosition;
    GameObject primaryWeapon;
    GameObject secondaryWeapon;
    GameObject camera;

    private void Start()
    {
        originalPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
        camera = GetComponentInParent<Camera>().gameObject;

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

        if (player.GetComponent<PhotonView>().IsMine)
            player.GetComponent<PhotonView>().RPC("RPC_PickedUpWeapon", RpcTarget.All, weaponPrefab.gameObject.name);
    }

    public void PickUpWeapon(string weaponPrefabName)
    {
        foreach (GameObject weapon in availableWeapons)
        {
            if (weapon.name == weaponPrefabName)
                PickUpWeapon(weapon);
        }
    }

    public GameObject GetCurrentWeapon()
    {
        if (player == null)
            return primaryWeapon;

        if (primaryWeapon && primaryWeapon.activeSelf) return primaryWeapon;
        else if (secondaryWeapon && secondaryWeapon.activeSelf) return secondaryWeapon;
        else return null;
    }

    private void Update()
    {
        SwitchWeapons();
        PointGunAtTarget();
        AimDownSights();
    }

    private void SwitchWeapons()
    {
        if (player == null)
            return;

        if (player.GetComponent<PhotonView>().IsMine)
        {
            // Scroll wheel
            if (Input.mouseScrollDelta.y != 0)
            {
                if (player.weaponId == 2)
                {
                    player.previousWeaponId = player.weaponId;
                    player.weaponId = 1;
                    player.TransmitWeaponId(1);
                }
                else if (player.weaponId == 1)
                {
                    player.previousWeaponId = player.weaponId;
                    player.weaponId = 2;
                    player.TransmitWeaponId(2);
                }
            }

            // Buttons 1 and 2
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
                //GetCurrentWeapon().GetComponent<WeaponSwayScript>().isEnabled = false;
                player.TransmitAimState(true);
            }
            if (Input.GetButtonUp("Fire2"))
            {
                //GetCurrentWeapon().GetComponent<WeaponSwayScript>().isEnabled = true;
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

    void PointGunAtTarget()
    {
        RaycastHit hit;

        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, 100f))
        {
            transform.LookAt(hit.point);
        }
        else
        {
            transform.localRotation = Quaternion.identity;
        }
    }
}
