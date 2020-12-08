using Photon.Pun;
using UnityEngine;

public class MenuUI : MonoBehaviourPunCallbacks
{
    public static MenuUI Instance;

    private bool menuToggle = false;

    void Awake()
    {
        Instance = this;
    }

    public void LeaveRoom()
    {
        RoomManager.Instance.LeaveRoom();
    }

    public void ToggleMenu()
    {
        menuToggle = !menuToggle;

        if (menuToggle)
        {
            GetComponent<Canvas>().enabled = true;
            GameObject player = GameObject.Find(PhotonNetwork.NickName);
            player.GetComponent<Player>().ccc.enabled = false;
            player.GetComponent<Player>().cmc.enabled = false;
            player.GetComponent<Player_Shoot>().enabled = false;
            player.GetComponent<Player>().weaponHolder.GetComponent<WeaponHolderController>().GetCurrentWeapon().GetComponent<WeaponSwayScript>().enabled = false;
        }
        else
        {
            GetComponent<Canvas>().enabled = false;
            GameObject player = GameObject.Find(PhotonNetwork.NickName);
            player.GetComponent<Player>().ccc.enabled = true;
            player.GetComponent<Player>().cmc.enabled = true;
            player.GetComponent<Player_Shoot>().enabled = true;
            player.GetComponent<Player>().weaponHolder.GetComponent<WeaponHolderController>().GetCurrentWeapon().GetComponent<WeaponSwayScript>().enabled = true;
        }
    }
}
