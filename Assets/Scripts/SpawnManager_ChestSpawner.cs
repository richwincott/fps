using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;

public class SpawnManager_ChestSpawner : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private int numberToSpawn = 50;

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.buildIndex == 1) // We're in the game scene
        {
            for (var i = 0; i < numberToSpawn; i++)
            {
                SpawnChest();
            }
        }
    }

    private void SpawnChest()
    {
        int x = Random.Range(-190, 200);
        int z = Random.Range(-165, 230);
        Vector3 randomLocation = new Vector3(x, 50, z);

        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Chest"), randomLocation, Quaternion.identity);
    }
}
