using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicGameplayUI : MonoBehaviour
{
    //Stores an array of object with the UIImage script attached.
    public UIImage[] AbilityUI;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateUI()
    {
        //For each entry in the array, call their respective updateUIImage method
        foreach (UIImage UI in AbilityUI)
        {
            UI.updateUIImage();
        }
    }
}
