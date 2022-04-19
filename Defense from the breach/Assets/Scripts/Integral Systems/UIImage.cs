using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIImage : MonoBehaviour
{
    //Script is attached to ability icon in the gameplay UI
    //Type of ability stored as string
    public string abilityType;
    //If the ability is a dynamic ability or not
    public bool isDynamic;
    //List of ability icons that are references
    public List<Sprite> AbilityIcons;
    // Start is called before the first frame update
    void Start()
    {
        //Updated on start (sets the default images)
        updateUIImage();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Called from start and also when setting a new ability
    public void updateUIImage()
    {
        //Initialise a blank string
        string abilityToCheck = "";

        //Check if the boolean is true. If it is, check the dynamic class ability based on the ability type
        if (isDynamic)
        {
            //Set the abilityToCheck string based on the string returned using the ability type
            switch (abilityType)
            {
                case "Concentrated Spec":
                    abilityToCheck = PlayerBase.instance.GetComponent<DynamicClassAbilities>().buffOrMissile;
                    break;
                case "Ordnance":
                    abilityToCheck = PlayerBase.instance.GetComponent<DynamicClassAbilities>().selectedProjectile;
                    break;
                
            }
        }
        else
        {
            //Same here, but for the simple class system
            switch (abilityType)
            {
                case "Jump":
                    abilityToCheck = PlayerBase.instance.GetComponent<SimpleClassAbilities>().selectedJump;
                    break;
                case "Dodge":
                    abilityToCheck = PlayerBase.instance.GetComponent<SimpleClassAbilities>().selectedDodge;
                    break;
                case "Buff":
                    abilityToCheck = PlayerBase.instance.GetComponent<SimpleClassAbilities>().selectedBuff;
                    break;
            }
        }

        
        //Set the sprite based on the ability to check string (And the icon's name)
        foreach (Sprite iconImage in AbilityIcons)
        {

            if (abilityToCheck.Contains(iconImage.name.ToString())){
                gameObject.GetComponent<Image>().sprite = iconImage;
            }
        }
    }

    
}
