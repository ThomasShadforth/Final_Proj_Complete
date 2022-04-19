using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicClassNodes : MonoBehaviour
{
    //Stores an array of the sections within the menu
    public ClassDynamSections[] sections;
}


//Stores the section name, the current node of the section, and an array of the nodes within the section
[System.Serializable]

public class ClassDynamSections
{
    public string SectionName;
    public string currentNode;

    public NodeDynamNames[] nodes;
}

//Stores the node name, whether it is selected, and the corresponding button
[System.Serializable]
public class NodeDynamNames
{
    public string nodeName;
    public bool isSelected;
    public GameObject UIButton;
}
