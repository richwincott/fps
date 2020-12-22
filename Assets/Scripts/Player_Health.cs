using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class Player_Health : MonoBehaviourPunCallbacks, IDamageable
{
    PhotonView PV;

    public int health = 100;

    public bool alive = true;
    private Text healthText;
    private PlayerUI playerUI;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        GameManager.RespawnClickEvent += RestoreHealth;
        healthText = GameObject.Find("Health Text").GetComponent<Text>();
        playerUI = GameObject.Find("PlayerUI").GetComponent<PlayerUI>();
        TransmitHealth(health);
        SetHealthText();
    }

    public void FixedUpdate()
    {
        if (PV.IsMine)
        {
            if (playerUI.damageScreenFlash.activeSelf)
            {
                playerUI.damageScreenFlash.SetActive(false);
            }
        }
    }

    private void SetHealthText()
    {
        if (PV.IsMine)
        {
            healthText.text = (health > 0 ? "+" : "") + health;
        }
    }

    private void OnHealthUpdate(int amount)
    {
        health = amount;
        SetHealthText();
    }

    public void TakeDamage(int amount)
    {
        health -= amount;

        if (PV.IsMine)
        {
            playerUI.damageScreenFlash.SetActive(true);
        }
    }

    public void InflictDamage(int dmg)
    {
        Hashtable hash = new Hashtable();
        hash.Add("inflictedDamage", dmg);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    private void TransmitHealth(int value)
    {
        if (PV.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("health", value);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    private void RestoreHealth()
    {
        health = 100;
        TransmitHealth(health);
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
    {
        if (!PV.IsMine && targetPlayer == PV.Owner)
        {
            if (changedProps.ContainsKey("health"))
                health = (int)changedProps["health"];

            if (changedProps.ContainsKey("inflictedDamage"))
                TakeDamage((int)changedProps["inflictedDamage"]);
        }
    }
}
