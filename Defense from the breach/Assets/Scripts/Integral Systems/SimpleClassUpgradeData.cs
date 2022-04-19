using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace classUpgradeData
{
    [CreateAssetMenu(fileName = "Simple Class Upgrades", menuName = "ScriptableObjects/Simple Class Data")]
    public class SimpleClassUpgradeData : ScriptableObject
    {
        public static SimpleClassUpgradeData instance;

        public SimpleClassSections[] classSections;
    }

    [System.Serializable]
    public class SimpleClassSections
    {
        public string sectionName;

        public SimpleClassUpgrades[] sectionUpgrades;
    }

    [System.Serializable]
    public class SimpleClassUpgrades
    {
        public string upgradeName;
        public int upgradeCost;
        public string upgradeDescription;
        public bool isUnlocked;
        public bool isStarterUpgrade;
        public bool hasLevels;
        public ClassUpgradeLevels[] levels;
    }

    [System.Serializable]
    public class ClassUpgradeLevels
    {
        public int levelNum;
        public int LevelCost;
        public int LevelIncrease;
    }

}
