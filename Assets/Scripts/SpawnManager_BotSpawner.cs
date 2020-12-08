using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;

public class SpawnManager_BotSpawner : MonoBehaviourPunCallbacks {

	[SerializeField] GameObject botPrefab;
	private GameObject[] botSpawns;
	private int counter;
	private int numberOfBots = 25;
	private int maxNumberOfBots = 50;
	private float waveRate = 1;
	private bool isSpawnActivated = true;
    private int numberAlive = 25;
    private PlayerUI playerUI;

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.buildIndex == 1) // We're in the game scene
        {
            botSpawns = GameObject.FindGameObjectsWithTag("BotSpawn");
            playerUI = GameObject.Find("PlayerUI").GetComponent<PlayerUI>();
            //StartCoroutine(BotSpawner());
            CommenceSpawn();
        }
    }

    IEnumerator BotSpawner()
	{
		for(;;)
		{
			yield return new WaitForSeconds(waveRate);
			GameObject[] bots = GameObject.FindGameObjectsWithTag("Bot");
			if(bots.Length < maxNumberOfBots)
			{
				CommenceSpawn();
			}
		}
	}

	void CommenceSpawn()
	{
		if(isSpawnActivated)
		{
			for(int i = 0; i < numberOfBots; i++)
			{
                //int randomIndex = Random.Range(0, botSpawns.Length);
                //SpawnBots(botSpawns[randomIndex].transform.position);

                int x = Random.Range(-190, 200);
                int z = Random.Range(-165, 230);
                Vector3 randomLocation = new Vector3(x, 100f, z);
                SpawnBot(randomLocation);
                numberAlive++;              
            }
            UpdatePlayerCounterText();
        }
    }

	void SpawnBot(Vector3 spawnPos)
	{
		counter++;
        GameObject go = PhotonNetwork.Instantiate(Path.Combine("Prefabs", "Bot"), spawnPos, Quaternion.identity);
        //go.GetComponent<Bot_ID>().botID = "Bot " + counter;
	}

    public void BotDealth()
    {
        numberAlive--;
        UpdatePlayerCounterText();
    }

    private void UpdatePlayerCounterText()
    {
        playerUI.playerCountText.GetComponent<Text>().text = numberAlive.ToString();
    }
}
