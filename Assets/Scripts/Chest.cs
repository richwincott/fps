using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public GameObject weaponPickupPrefab;
    public GameObject[] weaponSpawns = new GameObject[3];
    private GameObject[] threeRandomWeapons = new GameObject[3];
    private bool grounded = false;

    [SerializeField]
    GameObject[] weaponPrefabs = new GameObject[2];

    void Start()
    {
        threeRandomWeapons[0] = PickRandomWeapon();
        threeRandomWeapons[1] = PickRandomWeapon();
        threeRandomWeapons[2] = PickRandomWeapon();
    }

    private GameObject PickRandomWeapon()
    {
        int randomNo = Random.Range(0, weaponPrefabs.Length);
        return weaponPrefabs[randomNo];
    }

    public void Update()
    {
        RaycastHit hitInfo;

        if (!grounded && Physics.Raycast(transform.position, Vector3.down, out hitInfo, 100f))
        {
            Vector3 newLocation = transform.position;
            newLocation.y = hitInfo.point.y + 0.5f;
            transform.SetPositionAndRotation(newLocation, transform.rotation);
            grounded = true;
        }
    }

    public void Open()
    {
        GameObject weaponPickup1 = Instantiate(weaponPickupPrefab, weaponSpawns[0].transform);
        GameObject weaponPickup2 = Instantiate(weaponPickupPrefab, weaponSpawns[1].transform);
        GameObject weaponPickup3 = Instantiate(weaponPickupPrefab, weaponSpawns[2].transform);

        // set weapon prefab 
        weaponPickup1.GetComponent<WeaponPickup>().weaponPrefab = threeRandomWeapons[0];
        weaponPickup2.GetComponent<WeaponPickup>().weaponPrefab = threeRandomWeapons[1];
        weaponPickup3.GetComponent<WeaponPickup>().weaponPrefab = threeRandomWeapons[2];

        // set model 
        weaponPickup1.GetComponent<WeaponPickup>().weaponModel = threeRandomWeapons[0].GetComponent<WeaponController>().model;
        weaponPickup2.GetComponent<WeaponPickup>().weaponModel = threeRandomWeapons[1].GetComponent<WeaponController>().model;
        weaponPickup3.GetComponent<WeaponPickup>().weaponModel = threeRandomWeapons[2].GetComponent<WeaponController>().model;

        // spawn weapon
        weaponPickup1.GetComponent<WeaponPickup>().Spawn();
        weaponPickup2.GetComponent<WeaponPickup>().Spawn();
        weaponPickup3.GetComponent<WeaponPickup>().Spawn();
    }
}
