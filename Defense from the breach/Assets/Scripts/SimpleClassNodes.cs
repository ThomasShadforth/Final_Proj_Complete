using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SimpleClassNodes : MonoBehaviour
{
    
    public ClassSections[] sections;
}

[System.Serializable]
public class ClassSections
{
    public string SectionName;
    public string currentNode;
    public NodeNames[] nodes;
}

[System.Serializable]
public class NodeNames
{
    
    public string nodeName;
    public bool isSelected;
    public GameObject UIButton;
}

public class NodeButtons
{
    public GameObject buttonName;
    public string ButtonName;
}


