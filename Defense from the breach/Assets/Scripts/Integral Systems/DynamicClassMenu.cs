using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicClassMenu : MonoBehaviour
{
    //Gets a reference to the nodes class
    DynamicClassNodes classNodeMenu;
    //Reference to the upgrade data object in the menu
    [SerializeField] RuntimeClassUpgradeData upgradeData;
    //Info panel for upgrades
    [SerializeField] GameObject infoPanel;
    //Upgrade description, the name, select and upgrade button text (For if the Upgrade has levels/hasn't been unlocked)
    [SerializeField] Text upgradeDesc, upgradeButtonText, nameText, selectButtonText;
    //Reference to the upgrade and select buttons, and a reference button size from the ability button
    [SerializeField] Button upgradeButton, selectButton, referenceButtonSize;

    //Stores a reference to the chosen upgrade (Used for the info panel)
    classUpgrades.SectionUpgrades chosenUpgrade;

    //Original button size is set to the reference button's size
    Vector2 originalButtonSize;
    //Transform size is the original size doubled
    Vector2 transformSize;

    //Stores the name of the currently selected node
    string selectedNode;
    // Start is called before the first frame update
    void Start()
    {
        //ClassNodeMenu is set to the nodes script attached to the menu object
        classNodeMenu = GetComponent<DynamicClassNodes>();

        //Set the original and transform button sizes
        originalButtonSize = referenceButtonSize.image.rectTransform.sizeDelta;
        transformSize = originalButtonSize * 2;

        //Set the default selected nodes for each section of nodes
        setDefaultNodes();
    }

    private void OnEnable()
    {
        classNodeMenu = GetComponent<DynamicClassNodes>();

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
                for (int j = 0; j < classNodeMenu.sections[i].nodes.Length; j++)
                {
                    if (classNodeMenu.sections[i].nodes[j].nodeName == NodeName)
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
            for (int j = 0; j < upgradeData.runtimeData[i].classSections.Length; j++)
            {
                //if the current section's name is found in the selected node, then check the upgrades found there
                if (nodeName.Contains(upgradeData.runtimeData[i].classSections[j].sectionName))
                {

                    for (int k = 0; k < upgradeData.runtimeData[i].classSections[j].Upgrades.Length; k++)
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
        //Set the name text to be the upgradeName parameter
        nameText.text = upgradeName;
        //Set the description text to be the description returned from the corresponding method
        upgradeDesc.text = upgradeDescription;
        //Set the panel to active
        infoPanel.SetActive(true);

        //Set the button colour, text and interactibility based on upgrade points compared to the cost of the selected upgrade
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

    //Used to get the ability's description for the info panel
    public string GetAbilityDesc(string abilityName)
    {
        //Cycle through the runtime data (The class systems)
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


    //Triggers and upgrade based on the value of the selectedNode string
    public void triggerUpgrade()
    {
        unlockUpgrade(selectedNode);
    }

    public void unlockUpgrade(string NodeName)
    {
        //if the node's name is equal to the chosen upgrade's name
        if (NodeName == chosenUpgrade.upgradeName)
        {

            //If it's not unlocked, reduce it by the upgrade cost, and set it to unlocked
            if (!chosenUpgrade.upgradeUnlocked)
            {
                GameManager.instance.upgradePoints -= chosenUpgrade.upgradeCost;
                chosenUpgrade.upgradeUnlocked = true;
            }
            //Otherwise, subtract cost from upgrade points, increase the upgrade's level and set the new cost to be the cost at the current level
            else
            {
                GameManager.instance.upgradePoints -= chosenUpgrade.upgradeCost;
                chosenUpgrade.upgradeLevel++;
                chosenUpgrade.upgradeCost = chosenUpgrade.levels[chosenUpgrade.upgradeLevel].levelCost;
            }


        }
        infoPanel.SetActive(false);
    }

    public void triggerSelect()
    {
        //Called from the info panel, selects the node based on the value in the string
        selectNode(selectedNode);
    }


    public void setDefaultNodes()
    {
        //Call the selected node method for the current node of each section
        for (int i = 0; i < classNodeMenu.sections.Length; i++)
        {
            selectNode(classNodeMenu.sections[i].currentNode);
        }
    }

}
