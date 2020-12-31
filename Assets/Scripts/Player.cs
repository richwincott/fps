using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

public class Player : MonoBehaviourPunCallbacks
{
    [SerializeField]
    GameObject partyHat;
    [SerializeField]
    GameObject glasses;
    [SerializeField]
    WeaponHolderController weaponHolderController;

    PhotonView PV;
    public bool aiming = false;
    public int weaponId = 1;
    public int previousWeaponId = -1;
    public bool reloading = false;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (PV.IsMine)
        {
            //transform.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            partyHat.layer = LayerMask.NameToLayer("DontDraw");
            glasses.layer = LayerMask.NameToLayer("DontDraw");

            Enviroment enviroment = GameObject.Find("Enviroment").GetComponent<Enviroment>();
            List<GameObject> spawnPoints = enviroment.spawnPoints;
            int randomNo = Random.Range(0, spawnPoints.Count - 1);
            transform.SetPositionAndRotation(spawnPoints[randomNo].transform.position, spawnPoints[randomNo].transform.rotation);
            //int x = Random.Range(-24, 24);
            //int z = Random.Range(-24, 23);
            //Vector3 randomLocation = new Vector3(x, 70, z);
            //transform.SetPositionAndRotation(randomLocation, new Quaternion(0, 0, 0, 0));
        }
    }

    private void Update()
    {
        if (!PV.IsMine)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MenuUI.Instance.ToggleMenu();
            GetComponent<PlayerController>().enabled = !MenuUI.Instance.menuToggle;
            GetComponent<Player_Shoot>().enabled = !MenuUI.Instance.menuToggle;
            weaponHolderController.GetCurrentWeapon().GetComponent<WeaponSwayScript>().enabled = !MenuUI.Instance.menuToggle;
        }
    }

    public void TransmitAimState(bool value)
    {
        if (PV.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("aimState", value);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }


    public void TransmitReloadingState(bool value)
    {
        if (PV.IsMine)
        {     
            Hashtable hash = new Hashtable();
            hash.Add("reloadingState", value);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    public void TransmitWeaponId(int value)
    {
        if (PV.IsMine)
        {
            Debug.Log("Transmit WeaponId");
            Hashtable hash = new Hashtable();
            hash.Add("weaponId", value);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
    {
        if (!PV.IsMine && targetPlayer == PV.Owner)
        {
            if (changedProps.ContainsKey("reloadingState"))
                reloading = (bool)changedProps["reloadingState"];

            if (changedProps.ContainsKey("aimState"))
                aiming = (bool)changedProps["aimState"];

            if (changedProps.ContainsKey("weaponId"))
                weaponId = (int)changedProps["weaponId"];
        }
    }

    [PunRPC]
    void RPC_PickedUpWeapon(string weaponPrefabName)
    {
        if (!PV.IsMine)
        {
            weaponHolderController.PickUpWeapon(weaponPrefabName);
        }
    }
}
