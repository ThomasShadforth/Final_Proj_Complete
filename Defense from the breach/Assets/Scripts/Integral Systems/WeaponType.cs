using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Flags] public enum Weapon_Types
{
    Auto = 0,
    Burst = 1,
    SingleShot = 2,
    BurstAuto = Auto | Burst
}

public class WeaponType : MonoBehaviour
{
    public Weapon_Types type;
    public int RoundsPerMin;
    public int burstRounds;
    [SerializeField] float RoundsPerSec;
    [SerializeField] double fireRate;
    double fireRateVal;
    [SerializeField] float recoilModifier;
    [SerializeField] float stability;
    [SerializeField] Vector3 deviationVal;

    // Start is called before the first frame update
    void Start()
    {
        RoundsPerSec = RoundsPerMin / 60;
        fireRate = (1 / RoundsPerSec);
        fireRate = System.Math.Round(fireRate, 2);
        fireRateVal = fireRate;
    }

    // Update is called once per frame
    void Update()
    {
        checkForFireInput();

    }

    void fireWeapon()
    {
        if(type == Weapon_Types.Burst || type == Weapon_Types.BurstAuto)
        {
            double bulletRate = fireRateVal / burstRounds;
            float rate = Convert.ToSingle(bulletRate);
            StartCoroutine(burstFire(rate));
        }
        else
        {
            //instantiate and set velocity
        }
    }
    void checkForFireInput()
    {
        if (type == Weapon_Types.Burst || type == Weapon_Types.SingleShot)
        {
            if (Input.GetMouseButtonDown(0) && fireRateVal <= 0)
            {
                fireWeapon();
                fireRateVal = fireRate;
            }
            else
            {
                fireRate -= 1 * Time.deltaTime;
            }
        } else if(type == Weapon_Types.Auto || type == Weapon_Types.BurstAuto)
        {
            if (Input.GetMouseButtonDown(0) && fireRateVal <= 0)
            {
                fireWeapon();
                fireRateVal = fireRate;
            }
            else
            {
                fireRate -= 1 * Time.deltaTime;
            }
        }
    }

    public IEnumerator burstFire(float ratePerBullet) { 
        for(int i = 0; i < burstRounds; i++)
        {
            //instantiate the bullet here
            //Set velocity to be equal to forward * speed
            yield return new WaitForSeconds(ratePerBullet);
        }
    }

    //Write method for applying recoil to weapon
    public void applyRecoil()
    {
        float appliedRecoil = stability / 100;
        appliedRecoil *= (recoilModifier * 10);
        //PlayerBase.instance.transform.Rotate()
    }
}
