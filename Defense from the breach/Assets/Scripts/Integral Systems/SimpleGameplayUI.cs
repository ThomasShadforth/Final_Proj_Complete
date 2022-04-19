using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleGameplayUI : MonoBehaviour
{
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
        foreach(UIImage UI in AbilityUI)
        {
            UI.updateUIImage();
        }
    }
}
