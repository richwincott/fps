using UnityEngine;
using UnityEngine.UI;

public class WeaponController : MonoBehaviour
{
    [SerializeField]
    public int damage = 25;
    [SerializeField]
    public bool rapidFire = false;
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

    public int maxAmmo = 200;
    public int magSize = 30;
    public int leftInMag = 30;
    public bool reloading = false;
    public bool isBot = false;

    public void OnEnable()
    {
        if (!isBot)
            UpdateAmmoText();
    }

    public bool CanShoot()
    {
        return leftInMag > 0 && !reloading;
    }

    public void RemoveBulletFromMag()
    {
        leftInMag -= 1;
        UpdateAmmoText();
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

    public void UpdateAmmoText()
    {
        GameObject.Find("PlayerUI").GetComponent<PlayerUI>().ammoText.GetComponent<Text>().text = leftInMag + " / " + maxAmmo;
    }
}
