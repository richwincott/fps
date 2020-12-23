using UnityEngine;
using UnityEngine.UI;

public class WeaponController : MonoBehaviour
{
    [SerializeField]
    public int damage = 25;
    [SerializeField]
    public float rateOfFire = 0.5f;
    [SerializeField]
    public Transform bulletSpawnOffset;
    [SerializeField]
    public Vector3 aimPosition;
    [SerializeField]
    public ParticleSystem flash;
    [SerializeField]
    public Animator animator;
    [SerializeField]
    public GameObject model;
    [SerializeField]
    GameObject bulletPrefab;
    [SerializeField]
    AudioClip gunSound;

    public int maxAmmo = 200;
    public int magSize = 30;
    public int leftInMag = 30;
    public bool reloading = false;
    float lastShootTime;

    private void Start()
    {
        lastShootTime = Time.time;
    }

    public bool CanShoot()
    {
        return leftInMag > 0 && !reloading;
    }

    public void RemoveBulletFromMag()
    {
        leftInMag -= 1;
    }

    public void Shoot(bool isMine)
    {
        if (Time.time > lastShootTime + rateOfFire && CanShoot())
        {
            if (isMine)
            {
                UpdateAmmoText();
            }

            RemoveBulletFromMag();
            SpawnBullet();
            flash.Play();
            lastShootTime = Time.time;
        }
    }

    public void Reload()
    {
        if (maxAmmo > 0)
        {
            if (maxAmmo > magSize)
            {
                if (leftInMag > 0)
                {
                    int amountToLoad = magSize - leftInMag;
                    maxAmmo -= amountToLoad;
                }
                else
                {
                    maxAmmo -= magSize;
                }
                leftInMag = magSize;
            }
            else
            {
                if (leftInMag > 0)
                {
                    leftInMag += maxAmmo;
                }
                else
                {
                    leftInMag = maxAmmo;
                }
                maxAmmo = 0;
            }
        }
        UpdateAmmoText();
    }

    public void SpawnBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab,
            bulletSpawnOffset.position,
            gameObject.transform.rotation);
        bullet.GetComponent<AudioSource>().clip = gunSound;
        bullet.GetComponent<AudioSource>().Play();

    }

    public void UpdateAmmoText()
    {
        GameObject.Find("PlayerUI").GetComponent<PlayerUI>().ammoText.GetComponent<Text>().text = leftInMag + " / " + maxAmmo;
    }
}
