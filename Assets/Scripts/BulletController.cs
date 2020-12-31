using System.Collections;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField]
    private int speed = 100;
    [SerializeField]
    private float lifeSpan = 5f;
    [SerializeField]
    private float collisionDetectionDistance = 1f;

    private RaycastHit hitInfo;
    Vector3 oldPos;
    Vector3 newPos;
    bool collision = false;

    public delegate void HitDelegate(RaycastHit hitInfo);
    public static event HitDelegate HitEvent;

    private void Start()
    {
        StartCoroutine(Coroutine_Destroy(lifeSpan));
        oldPos = transform.position;
        newPos = transform.position;
    }

    /*private void FixedUpdate()
    {
        CheckCollision();
        if (!hitInfo.collider)
        {
            Vector3 velocity = transform.forward * speed * Time.deltaTime;
            transform.localRotation = transform.localRotation * Quaternion.Euler(0.03f, 0, 0);
            transform.position = transform.position + velocity;
        }
        else
        {
            HitEvent(hitInfo);
            Destroy(gameObject);
        }
    }*/

    void Update()
    {
        if (collision)
            return;

        newPos += speed * transform.forward * Time.deltaTime;

        Vector3 direction = newPos - oldPos;
        float distance = direction.magnitude;
        RaycastHit hit;

        if (Physics.Raycast(oldPos, direction, out hit, distance))
        {
            //Debug.Log(hit.transform.name);
            /*GameObject bulletHole = Instantiate(bulletHolePrefab, hit.point + hit.normal * 0.001f, Quaternion.FromToRotation(Vector3.up, hit.normal));
            bulletHole.transform.parent = hit.collider.gameObject.transform;
            Destroy(bulletHole, 5f);
            Instantiate(bulletImpactPrefab, hit.point + hit.normal * 0.001f, Quaternion.FromToRotation(Vector3.up, hit.normal));
            Rigidbody hitRb = hit.transform.gameObject.GetComponent<Rigidbody>();
            if (hitRb)
                hitRb.AddForceAtPosition(transform.forward * bulletWeight, hit.point);*/
            HitEvent(hit);
            collision = true;
            GetComponentInChildren<MeshRenderer>().gameObject.SetActive(false);
            StartCoroutine(Coroutine_Destroy(0.4f));
        }
        else
        {
            oldPos = transform.position;
            transform.position = newPos;
        }
    }

    private void CheckCollision()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, collisionDetectionDistance))
        {
            //Debug.Log(hitInfo.collider.tag + ":" + hitInfo.collider.name);
        }
    }

    public IEnumerator Coroutine_Destroy(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
