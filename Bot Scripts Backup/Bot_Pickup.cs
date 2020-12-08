using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Bot_Pickup : NetworkBehaviour
{
    [SerializeField]
    private WeaponHolderController weaponHolderController;

    private GameObject availablePickup;
    public bool hasWeapon = false;

    public void FixedUpdate()
    {
        foreach (GameObject pickup in GameObject.FindGameObjectsWithTag("Pickup"))
        {
            float distance = Vector3.Distance(transform.position, pickup.transform.position);       

            if (distance < 5f && pickup.GetComponent<Status>().active)
            {
                availablePickup = pickup;                    
            }
            else if (distance > 5f && distance < 5.5f)
            {
                availablePickup = null;
            }
        }

        if (availablePickup)
        {
            if (availablePickup.name == "Chest(Clone)")
            {
                availablePickup.GetComponent<Chest>().Open();
                availablePickup.GetComponent<Status>().active = false;
                availablePickup = null;
            }
            else
            {
                weaponHolderController.PickUpWeapon(availablePickup.GetComponent<WeaponPickup>().weaponPrefab);
                availablePickup.GetComponent<WeaponPickup>().Remove();
                availablePickup.GetComponent<Status>().active = false;
                availablePickup = null;
                hasWeapon = true;
            }
        }
    }
}
