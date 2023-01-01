using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Flags] public enum Weapon_Type
{
    AutoShot = 0,
    BurstShot = 1,
    SingleShot = 2,
    Burst_AutoShot = 4
}

public class WeaponTypes : MonoBehaviour
{
    public Weapon_Type type;
    public double RoundsPerMin;
    //Only necessary if the weapon type is a burst weapon
    public int burstRounds;
    [SerializeField] GameObject bulletPrefab;
    public Transform muzzleEnd;
    public float bulletSpeed = 30;


    public int AmmoHeld;
    public int MaxAmmoCount;
    public int AmmoCount;

    public float reloadSpeed;
    public readonly float maxReloadSpeed = 100;

    bool reloading;

    [SerializeField] float rateOfReload;

    [SerializeField] double RoundsPerSec;
    [SerializeField] double fireRate;
    
    double burstFireRate;
    
    double chosenFireRate;
    [SerializeField] float recoilTime;
    [SerializeField] float stability;
    [SerializeField] Vector3 deviationVal;

    [SerializeField] Vector3 recoil;
    Vector3 recoilStart, recoilOrigin;
    [SerializeField] Vector3 recoilPeak;

    CamController cam;
    [SerializeField] Transform player;
    [SerializeField] BulletObject[] bulletTypes;

    DynamicClassAbilities dynamicAbilities;
    public float damageModifier = 1f;
    // Start is called before the first frame update
    void Start()
    {
        RoundsPerSec = System.Math.Round((RoundsPerMin / 60), 2);
        fireRate = (1 / RoundsPerSec);
        fireRate = System.Math.Round(fireRate, 2);
        burstFireRate = fireRate + .2f;
        //burstFireRateVal = burstFireRate;
        //fireRateVal = fireRate;
        AmmoCount = MaxAmmoCount;
        rateOfReload = (reloadSpeed / maxReloadSpeed) * 5;
        recoilOrigin = Vector3.zero;
        cam = FindObjectOfType<CamController>();
    }

    private void OnEnable()
    {
        reloading = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (GamePause.paused)
        {
            return;
        }

        if(TutorialSystem.instance != null && (TutorialSystem.instance.isTutorialActive && !TutorialSystem.instance.doesTutHaveConditions && !TutorialSystem.instance.uiDisappeared))
        {
            
            return;
        } else if(TutorialSystem.instance != null && (TutorialSystem.instance.isTutorialActive && TutorialSystem.instance.doesTutHaveConditions && !TutorialSystem.instance.uiDisappeared))
        {
            return;
        }

        if(chosenFireRate > 0)
        {
            chosenFireRate -= Time.deltaTime;
        }

        if (!GetComponentInParent<DynamicClassAbilities>().concentratedShotActive && !reloading)
        {
            checkForFireInput();
            checkForReloadInput();
        }
    }

    #region Input Checks
    private void checkForFireInput()
    {
        //Check the weapon types

        if(type == Weapon_Type.BurstShot || type == Weapon_Type.SingleShot)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                if (chosenFireRate <= 0)
                {
                    if (AmmoCount > 0)
                    {
                        fireWeapon(type);
                        if (type == Weapon_Type.SingleShot)
                        {
                            chosenFireRate = fireRate;
                        }
                        else
                        {
                            chosenFireRate = burstFireRate;
                        }
                    }
                    else
                    {
                        StartCoroutine(reloadWeapon());
                    }
                }
            }
            
        } else if(type == Weapon_Type.AutoShot || type == Weapon_Type.Burst_AutoShot)
        {
            
            if(Input.GetButton("Fire1"))
            {
                if (chosenFireRate <= 0)
                {
                    if (AmmoCount > 0)
                    {
                        fireWeapon(type);

                        if(type == Weapon_Type.AutoShot)
                        {
                            chosenFireRate = fireRate;
                        }
                        else
                        {
                            chosenFireRate = burstFireRate;
                        }

                        
                    }
                    else
                    {
                        StartCoroutine(reloadWeapon());
                    }
                }
            }
        }
    }

    void checkForReloadInput()
    {
        if (Input.GetButtonDown("Reload"))
        {
            if (AmmoHeld > 0 && AmmoCount < MaxAmmoCount)
            {
                reloading = true;
                StartCoroutine(reloadWeapon());
            }
        }
    }
    #endregion

    #region Firing Weapon

    void fireWeapon(Weapon_Type typing)
    {
        if(typing == Weapon_Type.BurstShot || typing == Weapon_Type.Burst_AutoShot)
        {
            double bulletRate = fireRate / burstRounds;
            float rate = Convert.ToSingle(bulletRate);
            StartCoroutine(burstFire(rate));
        }

        else if(typing == Weapon_Type.SingleShot || typing == Weapon_Type.AutoShot)
        {
            createBullet();
            AddRecoil();
        }
    }

    public IEnumerator burstFire(float rate)
    {
        for(int i = 0; i < burstRounds; i++)
        {
            createBullet();
            yield return new WaitForSeconds(rate);
        }
    }
    #endregion

    #region Recoil Methods

    void AddRecoil()
    {
        var recoilStart = cam.transform.localEulerAngles;
        
        var recoilProduct = cam.transform.localEulerAngles + recoil;

        var recoilMidpoint = recoilStart + (recoilProduct - recoilStart) / 2;

        /*var recoilEnd = new Vector3(
            Mathf.LerpAngle(recoilStart.x, recoilProduct.x, 3 * Time.deltaTime),
            Mathf.LerpAngle(recoilStart.y, recoilProduct.y, 3 * Time.deltaTime),
            recoilStart.z
            );*/

        StartCoroutine(recoilTransition(recoilStart, recoilProduct));

        

        //StartCoroutine(recoilResetTransition(recoilPeak, recoilMidpoint));

    }

    

    #endregion

    #region Utility Methods
    public IEnumerator reloadWeapon()
    {
        

        yield return new WaitForSeconds(rateOfReload);

        if(AmmoHeld > MaxAmmoCount)
        {
            if(AmmoCount == 0)
            {
                AmmoCount = MaxAmmoCount;
                AmmoHeld -= MaxAmmoCount;
            }
            else
            {
                var AmmoToAdd = MaxAmmoCount - AmmoCount;
                AmmoCount += AmmoToAdd;
                AmmoHeld -= AmmoToAdd;
            }
            
        }
        else
        {
            var AmmoToReload = AmmoHeld;
            if (AmmoCount == 0)
            {
                
                AmmoCount = AmmoToReload;
                AmmoHeld -= AmmoToReload;
            }
            else
            {
                AmmoCount += AmmoToReload;
                AmmoHeld -= AmmoToReload;
            }
        }

        reloading = false;

    }

    void createBullet()
    {
        GameObject shootBullet = Instantiate(bulletPrefab, muzzleEnd.position, muzzleEnd.rotation);
        shootBullet.GetComponent<Rigidbody>().velocity = transform.forward * bulletSpeed;
        shootBullet.GetComponent<BulletObject>().setDamage(20, damageModifier);
        shootBullet.GetComponent<BulletObject>().SetOwner(this.gameObject, true);
        AmmoCount -= 1;
    }

    public IEnumerator recoilTransition(Vector3 recoilStart, Vector3 recoilProduct)
    {
        

        float TimePercentage = 0f;
        while(TimePercentage < 1)
        {
            TimePercentage += GamePause.deltaTime / recoilTime;
            //cam.transform.localEulerAngles = Vector3.Lerp(recoilStart, recoilProduct, TimePercentage);
            cam.transform.localEulerAngles = new Vector3(Mathf.Lerp(recoilStart.x, recoilProduct.x, TimePercentage),
                Mathf.Lerp(recoilStart.y, recoilProduct.y, TimePercentage), cam.transform.localEulerAngles.z);
            
            recoilPeak = cam.transform.localEulerAngles;
            yield return null;
        }
        cam.xRot += recoil.x;
    }

    

    public IEnumerator recoilResetTransition(Vector3 currentRecoil, Vector3 recoilResetPos)
    {
        yield return new WaitForSeconds(.18f);
        
        float TimePercentage = 1f;
        while(TimePercentage > 0)
        {
            TimePercentage -= Time.deltaTime / recoilTime;
            //cam.transform.localEulerAngles = Vector3.Lerp(recoilResetPos, currentRecoil, TimePercentage);
            cam.transform.localEulerAngles = new Vector3(Mathf.Lerp(recoilResetPos.x, currentRecoil.x, TimePercentage), cam.transform.localEulerAngles.y, cam.transform.localEulerAngles.z);
            yield return null;
        }
    }

    #endregion
}
