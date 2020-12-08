using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Bot_Attack : NetworkBehaviour {

	private float attackRate  = 3;
	private float nextAttack;
	private float minDistance = 40;
	private float currentDistance;
	private Transform myTransform;
	private Bot_Target targetScript;
    private WeaponController currentWeaponController;

    [SerializeField] private Material botGreen;
	[SerializeField] private Material botRed;

    [SerializeField]
    private WeaponHolderController weaponHolderController;
    [SerializeField]
    private ParticleSystem hitEffectPrefab;
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private AudioSource gunSound;

    // Use this for initialization
    void Start () 
	{
		myTransform = transform;
		targetScript = GetComponent<Bot_Target>();

		if(isServer)
		{
			StartCoroutine(Attack());
		}

	}

	void CheckIfTargetInRange()
	{
		if(targetScript.targetTransform != null)
		{
            currentWeaponController = weaponHolderController.GetCurrentWeapon().GetComponent<WeaponController>();

            currentDistance = Vector3.Distance(targetScript.targetTransform.position, myTransform.position);

			if(currentDistance < minDistance && Time.time > nextAttack)
			{
				nextAttack = Time.time + attackRate;

                if (GetComponent<Bot_Pickup>().hasWeapon)
                    Shoot();

				//targetScript.targetTransform.GetComponent<Player_Health>().TakeDamage(damage);
				//StartCoroutine(ChangeBotMat()); //For the host player.
				//RpcChangeBotAppearance();
			}
		}
	}

    private void Shoot()
    {
        SpawnBullet();
        //gunSound.Play();
        //currentWeaponController.flash.Play();
    }

    private void SpawnBullet()
    {
        Instantiate(bulletPrefab,
            currentWeaponController.bulletSpawnOffset.position,
            currentWeaponController.animator.transform.rotation);
    }

    IEnumerator ChangeBotMat()
	{
		GetComponent<Renderer>().material = botRed;
		yield return new WaitForSeconds(attackRate/2);
		GetComponent<Renderer>().material = botGreen;
	}

	[ClientRpc]
	void RpcChangeBotAppearance()
	{
		StartCoroutine(ChangeBotMat());
	}
	
	IEnumerator Attack()
	{
		for(;;)
		{
			yield return new WaitForSeconds(0.2f);
			CheckIfTargetInRange();
		}
	}
}
