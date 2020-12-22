using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class Player_Health : MonoBehaviourPunCallbacks, IDamageable
{
    [SerializeField]
    private AudioSource bombSound;

    PhotonView PV;
    public int health = 100;
    public bool alive = true;
    Text healthText;
    PlayerUI playerUI;
    PlayerManager playerManager;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
    }

    void Start()
    {
        healthText = GameObject.Find("Health Text").GetComponent<Text>();
        playerUI = GameObject.Find("PlayerUI").GetComponent<PlayerUI>();
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

    public void TakeDamage(int amount)
    {
        PV.RPC("RPC_TakeDamage", RpcTarget.All, amount);
    }

    [PunRPC]
    void RPC_TakeDamage(int amount)
    {
        if (!PV.IsMine)
            return;
        
        health -= amount;
        SetHealthText();
        playerUI.damageScreenFlash.SetActive(true);

        if (health <= 0)
        {
            Die();
            //bombSound.Play();
        }
    }

    void Die()
    {
        playerManager.Respawn();
    }
}
