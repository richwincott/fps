using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;
    GameObject player;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (PV.IsMine)
        {
            CreateController();
        }
    }

    void CreateController()
    {
        Debug.Log("Instantiated Player Controller");
        player = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), new Vector3(0, 100, 0), Quaternion.identity, 0, new object[] { PV.ViewID });
        player.GetComponent<Player>().SetUp(PhotonNetwork.NickName);
    }

    public void Die()
    {
        PhotonNetwork.Destroy(player);
        CreateController();
    }
}
