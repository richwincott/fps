using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

public class Player : MonoBehaviourPunCallbacks
{
    PhotonView PV;

    [SerializeField]
    public PlayerController cmc;
    [SerializeField]
    public CharacterCameraController ccc;
    [SerializeField]
    private Player_Shoot ss;
    [SerializeField]
    public Camera myCam;
    [SerializeField]
    private AudioListener myAudio;
    [SerializeField]
    private GameObject playerGraphics;
    [SerializeField]
    private GameObject partyHat;
    [SerializeField]
    private GameObject glasses;
    [SerializeField]
    public GameObject weaponHolder;
    [SerializeField]
    private AudioSource bombSound;

    public bool aiming = false;
    public int weaponId = 1;
    public int previousWeaponId = -1;
    public bool reloading = false;

    private Camera mainCam;
    private Player_Health healthScript;
    private PlayerUI playerUI;
    private GameManager gameManager;
    private GameObject enviromentPrefab;

    public int ActorNumber { get; internal set; }

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    public void SetUp(string playerName)
    {
        if (PV.IsMine)
        {
            gameObject.name = playerName;

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
            gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
            enviromentPrefab = gameManager.enviromentPrefab;
            if (!GameObject.Find(enviromentPrefab.name))
            {
                gameManager.spawnLevel();
            }
            TransmitAimState(aiming);
            TransmitWeaponId(weaponId);
            TransmitReloadingState(reloading);
            mainCam = Camera.main;
            playerUI = GameObject.Find("PlayerUI").GetComponent<PlayerUI>();
            GameObject.Find("MenuUI").GetComponent<Canvas>().enabled = false;
            Enable();
            //transform.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            playerUI.gameObject.GetComponent<Canvas>().enabled = true;
            partyHat.layer = LayerMask.NameToLayer("DontDraw");
            glasses.layer = LayerMask.NameToLayer("DontDraw");
        }
    }

    private void Enable()
    {
        playerGraphics.SetActive(true);
        weaponHolder.GetComponent<WeaponHolderController>().enabled = true;

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
            mainCam.gameObject.SetActive(false);
            cmc.enabled = true;
            ccc.enabled = true;
            ss.enabled = true;
            myCam.enabled = true;
            myAudio.enabled = true;
            playerUI.respawnButton.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Disable()
    {
        playerGraphics.SetActive(false);
        weaponHolder.GetComponent<WeaponHolderController>().ResetWeapons();
        weaponHolder.GetComponent<WeaponHolderController>().enabled = false;

        if (PV.IsMine)
        {
            mainCam.gameObject.SetActive(true);
            cmc.enabled = false;
            ccc.enabled = false;
            ss.enabled = false;
            myCam.enabled = false;
            myAudio.enabled = false;
            playerUI.respawnButton.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void Update()
    {
        if (healthScript.health <= 0 && healthScript.alive)
        {
            Debug.Log(transform.name + " died!");
            Disable();
            healthScript.alive = false;
            bombSound.Play();
        }

        if (healthScript.health > 0 && !healthScript.alive)
        {
            Debug.Log(transform.name + " respawned!");
            Enable();
            healthScript.alive = true;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MenuUI.Instance.ToggleMenu();
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
