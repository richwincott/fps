using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.AI;

public struct GameObjectDistanceDictionaryItem
{
    public float distance;
    public GameObject obj;
}

public class Bot_Target : NetworkBehaviour {

    [SerializeField]
    private GameObject head;
    [SerializeField]
    private float radius = 100;

    private NavMeshAgent agent;
	public Transform targetTransform;
	private LayerMask raycastLayer;
    private bool ready = false;

	// Use this for initialization
	void Start () 
	{
		agent = GetComponent<NavMeshAgent>();

		if(isServer)
		{
			StartCoroutine(DoCheck());
        }
	}

	void SearchForTarget()
	{
		if(!isServer)
		{
			return;
		}

		if(targetTransform == null)
		{
            if (GetComponent<Bot_Pickup>().hasWeapon)
            {
                raycastLayer = 1 << LayerMask.NameToLayer("Player");
            }
            else
            {
                raycastLayer = 1 << LayerMask.NameToLayer("Pickup");
            }

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius, raycastLayer);

            if (hitColliders.Length > 0)
            {
            	int randomint = Random.Range(0, hitColliders.Length);
                
                if (GetComponent<Bot_Pickup>().hasWeapon)
                {
                    if (hitColliders[randomint].gameObject.name != gameObject.name)
                        targetTransform = hitColliders[randomint].transform;
                }
                else
                {
                    if (hitColliders[randomint].gameObject.gameObject.GetComponent<Status>().active)
                        targetTransform = hitColliders[randomint].transform;
                }

                StartCoroutine(Reset(60f));
            }
            else
            {
                targetTransform = GameObject.Find("Center").transform;
                StartCoroutine(Reset(8f));
            }
        }

        if (targetTransform != null && targetTransform.gameObject.activeSelf == false)
		{
			targetTransform = null;
            StopCoroutine("Reset");
		}
	}

	void MoveToTarget()
	{
		if(targetTransform != null && isServer)
		{
			SetNavDestination(targetTransform);
        }
	}

    void Update()
    {
        if (Physics.Raycast(transform.position, Vector3.down, 0.5f))
        {
            StartCoroutine(Ready());
        }

        if (targetTransform != null && isServer)
        {
            Vector3 pos = targetTransform.position - head.transform.position;
            Quaternion newRot = Quaternion.LookRotation(pos);
            head.transform.rotation = Quaternion.Lerp(head.transform.rotation, newRot, 10 * Time.deltaTime);
        }
    }

    void SetNavDestination(Transform dest)
	{
        if (agent.enabled)
		    agent.SetDestination(dest.position);
	}

	IEnumerator DoCheck()
	{
		for(;;)
		{
            SearchForTarget();
            MoveToTarget();
			yield return new WaitForSeconds(0.2f);
		}
	}

    IEnumerator Ready()
    {
        yield return new WaitForSeconds(1f);
        agent.enabled = true;
    }

    IEnumerator Reset(float time)
    {
        yield return new WaitForSeconds(time);
        targetTransform = null;
    }
}
