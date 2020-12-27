using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PlayerManager : MonoBehaviour
{
    AudioSource bombSound;
    PhotonView PV;
    GameObject player;
    AudioSource boomSound;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        bombSound = GetComponent<AudioSource>();
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
    }

    public void Respawn()
    {
        Die();
        CreateController();
    }

    public void Die()
    {
        bombSound.Play();
        PhotonNetwork.Destroy(player);
    }
}
