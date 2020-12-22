using Photon.Pun;
using UnityEngine;

public class MenuUI : MonoBehaviourPunCallbacks
{
    public static MenuUI Instance;

    public bool menuToggle = false;

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
        }
        else
        {
            GetComponent<Canvas>().enabled = false;
        }
    }
}
