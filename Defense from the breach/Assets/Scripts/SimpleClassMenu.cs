using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleClassMenu : MonoBehaviour
{
    SimpleClassNodes classNodeMenu;
    [SerializeField]
    RuntimeClassUpgradeData upgradeData;
    [SerializeField]
    GameObject infoPanel;
    [SerializeField]
    Text upgradeDesc, upgradeButtonText, nameText, selectButtonText;
    [SerializeField]
    Button upgradeButton, selectButton, referenceButtonSize;

    classUpgrades.SectionUpgrades chosenUpgrade;

    Vector2 originalButtonSize;
    Vector2 transformSize;

    string selectedNode;
    void Start()
    {
        classNodeMenu = GetComponent<SimpleClassNodes>();

        originalButtonSize = referenceButtonSize.image.rectTransform.sizeDelta;
        transformSize = originalButtonSize * 2;

        setDefaultNodes();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void selectNode(string NodeName)
    {
        
        for (int i = 0; i < classNodeMenu.sections.Length; i++)
        {
            
            if (NodeName.Contains(classNodeMenu.sections[i].SectionName))
            {
                
                for (int j = 0; j < classNodeMenu.sections[i].nodes.Length; j++)
                {
                    if (classNodeMenu.sections[i].nodes[j].nodeName == classNodeMenu.sections[i].currentNode)
                    {
                        classNodeMenu.sections[i].nodes[j].isSelected = false;
                        classNodeMenu.sections[i].nodes[j].UIButton.GetComponent<Button>().interactable = true;
                        
                        
                        classNodeMenu.sections[i].nodes[j].UIButton.GetComponent<Button>().image.rectTransform.sizeDelta = originalButtonSize;
                        classNodeMenu.sections[i].currentNode = "";
                        break;
                    }
                }
            }
        }


        for (int i = 0; i < classNodeMenu.sections.Length; i++)
        {
            
            if (NodeName.Contains(classNodeMenu.sections[i].SectionName))
            {
                for(int j = 0; j < classNodeMenu.sections[i].nodes.Length; j++)
                {
                    if(classNodeMenu.sections[i].nodes[j].nodeName == NodeName)
                    {
                        
                        classNodeMenu.sections[i].currentNode = NodeName;
                        classNodeMenu.sections[i].nodes[j].isSelected = true;
                        classNodeMenu.sections[i].nodes[j].UIButton.GetComponent<Button>().interactable = false;
                        classNodeMenu.sections[i].nodes[j].UIButton.GetComponent<Button>().image.rectTransform.sizeDelta = transformSize;
                        //PlayerBase.instance.setJump(NodeName);
                        
                        PlayerBase.instance.setAbility(NodeName);
                        break;
                    }
                }
            }
        }
        
    }


    public void selectClassNode(string nodeName)
    {
        
        //Cycle through both class system arrays
        for (int i = 0; i < upgradeData.runtimeData.Length; i++)
        {
            
            //Cycle through the sections for the current class system
            for(int j = 0; j < upgradeData.runtimeData[i].classSections.Length; j++)
            {
                //if the current section's name is found in the selected node, then check the upgrades found there
                if (nodeName.Contains(upgradeData.runtimeData[i].classSections[j].sectionName))
                {
                    
                    for(int k = 0; k < upgradeData.runtimeData[i].classSections[j].Upgrades.Length; k++)
                    {
                        //if the selected upgrade matches the current name
                        if (upgradeData.runtimeData[i].classSections[j].Upgrades[k].upgradeName == nodeName)
                        {
                            //Then check if the upgrade is unlocked
                            if (upgradeData.runtimeData[i].classSections[j].Upgrades[k].upgradeUnlocked)
                            {
                                //if it is unlocked, then check if the upgrade has additional upgrade levels, if so, display the upgrade info panel
                                //Otherwise, simply change the selected upgrade
                                if (upgradeData.runtimeData[i].classSections[j].Upgrades[k].hasLevels)
                                {
                                    chosenUpgrade = upgradeData.runtimeData[i].classSections[j].Upgrades[k];
                                    selectedNode = nodeName;
                                    displayInfo(upgradeData.runtimeData[i].classSections[j].Upgrades[k].upgradeName, upgradeData.runtimeData[i].classSections[j].Upgrades[k].upgradeDescription, upgradeData.runtimeData[i].classSections[j].Upgrades[k].upgradeCost, upgradeData.runtimeData[i].classSections[j].Upgrades[k].upgradeUnlocked);
                                }
                                else
                                {
                                    
                                    selectNode(nodeName);
                                }
                            }
                            else
                            {
                                chosenUpgrade = upgradeData.runtimeData[i].classSections[j].Upgrades[k];
                                selectedNode = nodeName;
                                displayInfo(upgradeData.runtimeData[i].classSections[j].Upgrades[k].upgradeName, upgradeData.runtimeData[i].classSections[j].Upgrades[k].upgradeDescription, upgradeData.runtimeData[i].classSections[j].Upgrades[k].upgradeCost, upgradeData.runtimeData[i].classSections[j].Upgrades[k].upgradeUnlocked);
                                //display the upgrade info panel
                            }
                        }
                    }
                }
            }
        }
        //First Check if Node is currently unlocked
        //if not, display upgrade unlock panel, and return corresponding details from upgrade data

        //otherwise, continue as normal
    }


    public void displayInfo(string upgradeName, string upgradeDescription, int upgradeCost, bool isUnlocked)
    {
        nameText.text = upgradeName;
        upgradeDesc.text = upgradeDescription;
        infoPanel.SetActive(true);

        if (GameManager.instance.upgradePoints >= upgradeCost)
        {
            if (isUnlocked)
            {
                upgradeButtonText.text = "Upgrade";
                upgradeButton.interactable = true;
                upgradeButton.image.color = Color.white;
                
            }
            else
            {
                upgradeButtonText.text = "Unlock";
                upgradeButton.interactable = true;
                upgradeButton.image.color = Color.white;
            }
        }
        else
        {
            upgradeButtonText.text = "Not enough points!";
            upgradeButton.interactable = false;
            upgradeButton.image.color = Color.red;
        }

        if (isUnlocked)
        {
            selectButton.interactable = true;
            selectButton.image.color = Color.white;
            selectButtonText.text = "Select";
        }
        else
        {
            selectButton.interactable = false;
            selectButton.image.color = Color.red;
            selectButtonText.text = "Locked";
        }
    }

    public string GetAbilityName(string abilityName)
    {
        for (int i = 0; i < upgradeData.runtimeData.Length; i++)
        {

            //Cycle through the sections for the current class system
            for (int j = 0; j < upgradeData.runtimeData[i].classSections.Length; j++)
            {
                //if the current section's name is found in the selected node, then check the upgrades found there
                if (abilityName.Contains(upgradeData.runtimeData[i].classSections[j].sectionName))
                {

                    for (int k = 0; k < upgradeData.runtimeData[i].classSections[j].Upgrades.Length; k++)
                    {
                        //if the selected upgrade matches the current name
                        if (upgradeData.runtimeData[i].classSections[j].Upgrades[k].upgradeName == abilityName)
                        {
                            return abilityName;
                        }
                    }
                    
                }

            }
            
        }

        return null;
    }

    public string GetAbilityDesc(string abilityName)
    {
        for (int i = 0; i < upgradeData.runtimeData.Length; i++)
        {

            //Cycle through the sections for the current class system
            for (int j = 0; j < upgradeData.runtimeData[i].classSections.Length; j++)
            {
                //if the current section's name is found in the selected node, then check the upgrades found there
                if (abilityName.Contains(upgradeData.runtimeData[i].classSections[j].sectionName))
                {

                    for (int k = 0; k < upgradeData.runtimeData[i].classSections[j].Upgrades.Length; k++)
                    {
                        //if the selected upgrade matches the current name
                        if (upgradeData.runtimeData[i].classSections[j].Upgrades[k].upgradeName == abilityName)
                        {
                            return upgradeData.runtimeData[i].classSections[j].Upgrades[k].upgradeDescription;
                        }
                    }

                }

            }

        }

        return null;
    }

    public void triggerUpgrade()
    {
        unlockUpgrade(selectedNode);
    }

    public void unlockUpgrade(string NodeName)
    {
        if (NodeName == chosenUpgrade.upgradeName)
        {
            if (!chosenUpgrade.upgradeUnlocked)
            {
                GameManager.instance.upgradePoints -= chosenUpgrade.upgradeCost;
                chosenUpgrade.upgradeUnlocked = true;
            }
            else
            {
                GameManager.instance.upgradePoints -= chosenUpgrade.upgradeCost;
                chosenUpgrade.upgradeLevel++;
                chosenUpgrade.upgradeCost = chosenUpgrade.levels[chosenUpgrade.upgradeLevel].levelCost;
            }

            
        }
        infoPanel.SetActive(false);
    }

    /*
    public void selectUpgrade(string NodeName)
    {
        for (int i = 0; i < classNodeMenu.sections.Length; i++)
        {

            if (NodeName.Contains(classNodeMenu.sections[i].SectionName))
            {

                for (int j = 0; j < classNodeMenu.sections[i].nodes.Length; j++)
                {
                    if (classNodeMenu.sections[i].nodes[j].nodeName == classNodeMenu.sections[i].currentNode)
                    {
                        classNodeMenu.sections[i].nodes[j].isSelected = false;
                        classNodeMenu.sections[i].nodes[j].UIButton.GetComponent<Button>().interactable = true;
                        classNodeMenu.sections[i].currentNode = "";
                        break;
                    }
                }
            }
        }


        for (int i = 0; i < classNodeMenu.sections.Length; i++)
        {

            if (NodeName.Contains(classNodeMenu.sections[i].SectionName))
            {
                for (int j = 0; j < classNodeMenu.sections[i].nodes.Length; j++)
                {
                    if (classNodeMenu.sections[i].nodes[j].nodeName == NodeName)
                    {

                        classNodeMenu.sections[i].currentNode = NodeName;
                        classNodeMenu.sections[i].nodes[j].isSelected = true;
                        classNodeMenu.sections[i].nodes[j].UIButton.GetComponent<Button>().interactable = false;
                        PlayerBase.instance.setJump(NodeName);
                        break;
                    }
                }
            }
        }
    }*/

    public void triggerSelect()
    {
        selectNode(selectedNode);
    }

    public void setDefaultNodes()
    {
        for (int i = 0; i < classNodeMenu.sections.Length; i++)
        {
            selectNode(classNodeMenu.sections[i].currentNode);
        }
    }
}
