using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Bot_Health : NetworkBehaviour {

	private int health = 50;
    private SpawnManager_BotSpawner botSpawner;

    public void Start()
    {
        botSpawner = GameObject.Find("Spawn Manager").GetComponent<SpawnManager_BotSpawner>();
    }

	public void TakeDamage(int dmg)
	{
		health -= dmg;
		CheckHealth();
	}

	void CheckHealth()
	{
		if(health <= 0)
		{
			Destroy(gameObject);
            botSpawner.BotDealth();
		}
	}

    public void InflictDamage(int dmg)
    {
        CmdInflictDamageOnServer(dmg);
    }

    [Command]
    private void CmdInflictDamageOnServer(int amount)
    {
        TakeDamage(amount);
    }
}
