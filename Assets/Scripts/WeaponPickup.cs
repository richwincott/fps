using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public GameObject weaponModel;
    public GameObject weaponPrefab;
    private bool grounded = false;

    public void Update()
    {
        RaycastHit hitInfo;

        if (!grounded && Physics.Raycast(transform.position, Vector3.down, out hitInfo, 100f))
        {
            Vector3 newLocation = transform.position;
            newLocation.y = hitInfo.point.y + 0.5f;
            transform.SetPositionAndRotation(newLocation, transform.rotation);
            grounded = true;
        }
    }

    public void Spawn()
    {
        Instantiate(weaponModel, transform);
    }

    public void Remove()
    {
        gameObject.SetActive(false);
    }
}
