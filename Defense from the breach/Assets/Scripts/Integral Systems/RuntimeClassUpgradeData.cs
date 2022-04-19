using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuntimeClassUpgradeData : MonoBehaviour
{
    //Reference to the classUpgradeData and the classSystems class within it
    public classUpgrades.ClassUpgradeData originalData;
    public classUpgrades.classSystems[] runtimeData;

    private void Awake()
    {
        //Set the runtime data to the classSystems found within the original data
        runtimeData = originalData.classSystems;

        //Set default upgrade data
        for (int i = 0; i < runtimeData.Length; i++)
        {
            for (int j = 0; j < runtimeData[i].classSections.Length; j++)
            {
                for (int k = 0; k < runtimeData[i].classSections[j].Upgrades.Length; k++)
                {
                    //If it's a starter upgrade, set to unlocked
                    if (runtimeData[i].classSections[j].Upgrades[k].starterUpgrade)
                    {
                        runtimeData[i].classSections[j].Upgrades[k].upgradeUnlocked = true;
                    }
                    else
                    {
                        //Set to false if not
                        runtimeData[i].classSections[j].Upgrades[k].upgradeUnlocked = false;
                    }

                    //Set upgrade levels to 0 if it has levels, and the level cost to the default
                    if (runtimeData[i].classSections[j].Upgrades[k].hasLevels)
                    {
                        runtimeData[i].classSections[j].Upgrades[k].upgradeLevel = 0;
                        runtimeData[i].classSections[j].Upgrades[k].upgradeCost = runtimeData[i].classSections[j].Upgrades[k].levels[0].levelCost;
                    }
                    
                }
            }

        }
        
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
