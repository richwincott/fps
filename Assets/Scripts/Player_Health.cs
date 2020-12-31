using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Player_Health : MonoBehaviourPunCallbacks, IDamageable
{
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

    public void TakeDamage(int amount, Photon.Realtime.Player player)
    {
        PV.RPC("RPC_TakeDamage", RpcTarget.All, amount, player);
    }

    [PunRPC]
    void RPC_TakeDamage(int amount, Photon.Realtime.Player player)
    {
        if (!PV.IsMine)
            return;
        
        health -= amount;
        SetHealthText();
        playerUI.damageScreenFlash.SetActive(true);

        if (health <= 0)
        {
            Die(player);
        }
    }

    void Die(Photon.Realtime.Player player)
    {
        IncreaseKillsOrDeathsPlayerCustomProps(player, "kills");
        IncreaseKillsOrDeathsPlayerCustomProps(PhotonNetwork.LocalPlayer, "deaths");
        PV.RPC("RPC_BroadcastDeath", RpcTarget.All, player, PhotonNetwork.LocalPlayer);
        playerManager.Respawn();
    }

    [PunRPC]
    void RPC_BroadcastDeath(Photon.Realtime.Player player1, Photon.Realtime.Player player2)
    {
        GameObject go = Instantiate(playerUI.killFeedListItem, playerUI.killFeedPanel.transform);
        go.GetComponent<TMP_Text>().text = player1.NickName + " killed " + player2.NickName;
        Destroy(go, 3f);
    }

    void IncreaseKillsOrDeathsPlayerCustomProps(Photon.Realtime.Player player, string key)
    {
        Hashtable current = player.CustomProperties;
        Hashtable hash = new Hashtable();
        foreach(string _key in current.Keys)
        {
            if (_key != key)
                hash.Add(_key, current[_key]);
        }
        object currentKillsValue = current[key];
        hash.Add(key, currentKillsValue != null ? (int)currentKillsValue + 1 : 1);
        player.SetCustomProperties(hash);
    }
}
