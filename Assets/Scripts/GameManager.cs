using UnityEngine;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    public GameObject enviromentPrefab;

    private GameObject respawnButton;

    public delegate void RespawnClickDelegate();
    public static event RespawnClickDelegate RespawnClickEvent;

    public void Start()
    {
        respawnButton = GameObject.Find("PlayerUI").GetComponent<PlayerUI>().respawnButton;
        name = "Game Manager";
        Application.targetFrameRate = 60;
    }

    public void spawnLevel()
    {
        GameObject enviroment = Instantiate(enviromentPrefab);
        enviroment.name = enviromentPrefab.name;
    }

    public void RespawnButton_Click()
    {
        RespawnClickEvent();
    }

    public void FixedUpdate()
    {
        if (respawnButton.activeSelf)
        {
            if (Input.GetButtonUp("Fire"))
            {
                RespawnClickEvent();
            }
        }
    }
}
