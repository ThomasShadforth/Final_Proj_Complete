using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public bool isHealthPickup;
    public bool isAmmoPickup;

    public float healthRecovery;
    public int AmmoToGain;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerBase>())
        {
            if (isHealthPickup)
            {
                PlayerBase.instance.AddHealth(healthRecovery);
            } else if (isAmmoPickup)
            {
                PlayerBase.instance.AddAmmo(AmmoToGain);
            }

            Destroy(gameObject);
        }
    }
}
