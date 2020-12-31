using ExitGames.Client.Photon;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class MenuUI : MonoBehaviourPunCallbacks
{
    public static MenuUI Instance;

    public bool menuToggle = false;
    TMP_Text playerList;

    void Awake()
    {
        Instance = this;
        playerList = GetComponentInChildren<TMP_Text>();
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
        }
        else
        {
            GetComponent<Canvas>().enabled = false;
        }
    }

    void FixedUpdate()
    {
        if (menuToggle == false)
            return;

        Photon.Realtime.Player[] players = PhotonNetwork.PlayerList;

        playerList.text = "";
        for (int i = 0; i < players.Length; i++)
        {
            playerList.text += players[i].NickName + " ";

            Hashtable current = players[i].CustomProperties;

            if (current.ContainsKey("kills"))
                playerList.text += (int)current["kills"] + " / ";
            else
                playerList.text += "0 / ";

            if (current.ContainsKey("deaths"))
                playerList.text += (int)current["deaths"] + " ";
            else
                playerList.text += "0 ";
        }
    }
}
