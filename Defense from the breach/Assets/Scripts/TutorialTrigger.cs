using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public bool hasTriggered;
    public bool triggerOnStart;

    public int tutorialIndex;
    public bool hasCondition;
    void Start()
    {
        if (triggerOnStart)
        {
            Debug.Log("TUTORIAL");
            hasTriggered = true;
            TutorialSystem.instance.triggerTutorial(tutorialIndex, hasCondition);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerBase>())
        {
            if (!hasTriggered)
            {
                hasTriggered = true;
                TutorialSystem.instance.triggerTutorial(tutorialIndex, hasCondition);
            }
        }
    }
}
