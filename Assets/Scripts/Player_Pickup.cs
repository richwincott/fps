using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player_Pickup : MonoBehaviourPunCallbacks
{
    PhotonView PV;

    [SerializeField]
    private WeaponHolderController weaponHolderController;

    private PlayerUI playerUI;
    private Camera myCamera;
    private GameObject availablePickup;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    public void Start()
    {
        playerUI = GameObject.Find("PlayerUI").GetComponent<PlayerUI>();
        myCamera = GetComponentInChildren<Camera>();
    }

    public void FixedUpdate()
    {
        if (PV.IsMine)
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(myCamera.transform.position, myCamera.transform.forward, out hitInfo, 2f))
            {

                if (hitInfo.collider.gameObject.layer != LayerMask.NameToLayer("Pickup"))
                    return;

                if (hitInfo.collider.gameObject.GetComponent<Status>().active)
                    availablePickup = hitInfo.collider.gameObject;
            }
            else
            {
                availablePickup = null;
                playerUI.statusText.GetComponent<Text>().text = "";
            }

            //foreach (GameObject pickup in GameObject.FindGameObjectsWithTag("Pickup"))
            //{
            //    float distance = Vector3.Distance(transform.position, pickup.transform.position);

            //    if (distance < 1f && pickup.GetComponent<Status>().active)
            //    {
            //        availablePickup = pickup;
            //    }
            //    if (distance > 1f && distance < 1.5f)
            //    {
            //        availablePickup = null;
            //        playerUI.statusText.GetComponent<Text>().text = "";
            //    }
            //}

            if (availablePickup)
            {
                if (availablePickup.name == "Chest(Clone)" || availablePickup.name == "Chest")
                    playerUI.statusText.GetComponent<Text>().text = "Press 'F' to open the chest";
                else
                    playerUI.statusText.GetComponent<Text>().text = "Press 'F' to pickup " + availablePickup.GetComponent<WeaponPickup>().weaponPrefab.name;

                if (Input.GetKeyUp(KeyCode.F))
                {
                    playerUI.statusText.GetComponent<Text>().text = "";

                    if (availablePickup.name == "Chest(Clone)" || availablePickup.name == "Chest")
                    {
                        availablePickup.GetComponent<Chest>().Open();
                    }
                    else // gun
                    {
                        weaponHolderController.PickUpWeapon(availablePickup.GetComponent<WeaponPickup>().weaponPrefab);
                        availablePickup.GetComponent<WeaponPickup>().Remove();
                    }

                    availablePickup.GetComponent<Status>().active = false;
                    availablePickup = null;
                }
            }
        }
    }
}
