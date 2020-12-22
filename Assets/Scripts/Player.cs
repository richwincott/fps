using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

public class Player : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject partyHat;
    [SerializeField]
    private GameObject glasses;
    [SerializeField]
    public GameObject weaponHolder;
    [SerializeField]
    private AudioSource bombSound;

    PhotonView PV;
    public bool aiming = false;
    public int weaponId = 1;
    public int previousWeaponId = -1;
    public bool reloading = false;

    private Player_Health healthScript;

    PlayerManager playerManager;

    public int ActorNumber { get; internal set; }

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
    }

    public void SetUp(string playerName)
    {
        if (PV.IsMine)
        {
            //gameObject.name = playerName;

            // Sync to everyone else
            Hashtable hash = new Hashtable();
            hash.Add("nickName", playerName);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    private void Start()
    {
        healthScript = GetComponent<Player_Health>();

        if (PV.IsMine)
        {
            GameObject.Find("MenuUI").GetComponent<Canvas>().enabled = false;
            Enable();
            //transform.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            partyHat.layer = LayerMask.NameToLayer("DontDraw");
            glasses.layer = LayerMask.NameToLayer("DontDraw");
        }
    }

    private void Enable()
    {
        if (PV.IsMine)
        {
            //Enviroment enviroment = GameObject.Find(enviromentPrefab.name).GetComponent<Enviroment>();
            //List<GameObject> spawnPoints = enviroment.spawnPoints;
            //int randomNo = Random.Range(0, spawnPoints.Count - 1);
            //transform.SetPositionAndRotation(spawnPoints[randomNo].transform.position, spawnPoints[randomNo].transform.rotation);
            int x = Random.Range(-24, 24);
            int z = Random.Range(-24, 23);
            Vector3 randomLocation = new Vector3(x, 70, z);
            transform.SetPositionAndRotation(randomLocation, new Quaternion(0, 0, 0, 0));
        }
    }

    private void Update()
    {
        if (healthScript.health <= 0 && healthScript.alive)
        {
            Debug.Log(transform.name + " died!");
            playerManager.Die();
            bombSound.Play();
        }

        if (healthScript.health > 0 && !healthScript.alive)
        {
            Debug.Log(transform.name + " respawned!");
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MenuUI.Instance.ToggleMenu();
            GetComponent<PlayerController>().enabled = !MenuUI.Instance.menuToggle;
            GetComponent<Player_Shoot>().enabled = !MenuUI.Instance.menuToggle;
            GetComponent<Player>().weaponHolder.GetComponent<WeaponHolderController>().GetCurrentWeapon().GetComponent<WeaponSwayScript>().enabled = !MenuUI.Instance.menuToggle;
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

            if (changedProps.ContainsKey("nickName"))
                gameObject.name = (string)changedProps["nickName"];
        }
    }
}
