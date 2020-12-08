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

    public delegate void HitDelegate(RaycastHit hitInfo);
    public static event HitDelegate HitEvent;

    private void Start()
    {
        StartCoroutine(Coroutine_Destroy());
    }

    private void FixedUpdate()
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
    }

    private void CheckCollision()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, collisionDetectionDistance))
        {
            //Debug.Log(hitInfo.collider.tag + ":" + hitInfo.collider.name);
        }
    }

    public IEnumerator Coroutine_Destroy()
    {
        yield return new WaitForSeconds(lifeSpan);
        Destroy(gameObject);
    }
}
