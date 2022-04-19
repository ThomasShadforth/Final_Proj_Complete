using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ClassMenuHover : MonoBehaviour
{
    [SerializeField] GameObject tooltipBox;
    bool UIActive;
    public string nodeName;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI descText;
    [SerializeField] SimpleClassMenu simpleClass;
    [SerializeField] DynamicClassMenu dynamicClass;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        if (tooltipBox.activeInHierarchy) {
            Vector3 mouse = new Vector3(Input.mousePosition.x + 10f, Input.mousePosition.y, tooltipBox.transform.position.z);
            tooltipBox.transform.position = mouse;
        }
    }

    // Update is called once per frame
    public void activateUI()
    {
        tooltipBox.SetActive(true);
        Vector3 mouse = new Vector3(Input.mousePosition.x + 10f, Input.mousePosition.y, tooltipBox.transform.position.z);

        updateUIInfo();
        tooltipBox.transform.position = mouse;


    }

    public void deactivateUI()
    {
        tooltipBox.SetActive(false);
    }

    public void updateUIInfo()
    {
        if(simpleClass != null)
        {
            nameText.text = simpleClass.GetAbilityName(nodeName);
            descText.text = simpleClass.GetAbilityDesc(nodeName);
        }

        if(dynamicClass != null)
        {
            nameText.text = dynamicClass.GetAbilityName(nodeName);
            descText.text = dynamicClass.GetAbilityDesc(nodeName);
        }
    }

}
