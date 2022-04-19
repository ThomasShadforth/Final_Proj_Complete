using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace classUpgrades
{

    [CreateAssetMenu(fileName = "Class Upgrade Data", menuName = "ScriptableObjects/ClassUpgradeData")]

    //Main class that stores an array of class systems (In the Scriptable object, stores 2 - one for the simple class and one for the dynamic class
    public class ClassUpgradeData : ScriptableObject
    {
        public static ClassUpgradeData instance;

        public classSystems[] classSystems;

        

    }

    //Class for the class systems.
    [System.Serializable]
    public class classSystems
    {
        //String stores the name of the class type ("dynamic", "simple")
        public string systemType;
        //Each class will have multiple sections, the properties of which are defined in the classSections class
        public classSections[] classSections;

    }


    //Class for the sections of the class systems
    [System.Serializable]
    public class classSections
    {
        //Stores the name of the section (e.g. "Jump", "Dodge", etc.)
        public string sectionName;
        //Each section will have multiple upgrades, the properties of which are defined in the sectionUpgrades class.
        public SectionUpgrades[] Upgrades;
    }


    //Store each upgrade/ability
    [System.Serializable]
    public class SectionUpgrades
    {
        //Name of the ability, and the basic description
        public string upgradeName;
        public string upgradeDescription;
        //Whether or not it has been unlocked
        public bool upgradeUnlocked;
        //Whether it is an upgrade the player has at the start of the game or not
        public bool starterUpgrade;
        //How much it costs to unlock/upgrade
        public int upgradeCost;
        //Whether or not it will have further upgrade levels (For future development)
        public bool hasLevels;
        //The current upgrade level
        public int upgradeLevel;
        //The properties for each level is defined in the upgradeLevels class
        public upgradeLevels[] levels;
    }


    //Stores properties for the levels
    [System.Serializable]
    public class upgradeLevels
    {
        //How much the level in question costs
        public int levelCost;
        //How much the level increases e.g. damage, jumpHeight, etc.
        public float levelIncrease;
    }
}
