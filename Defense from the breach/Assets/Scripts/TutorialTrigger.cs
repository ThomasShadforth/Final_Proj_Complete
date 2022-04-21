using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public bool hasTriggered;
    public bool triggerOnStart;

    public int tutorialIndex;
    public bool hasCondition;
    public float triggerTime;
    public bool timerActive;
    void Start()
    {
        if (triggerOnStart)
        {
            
            hasTriggered = true;
            TutorialSystem.instance.triggerTutorial(tutorialIndex, hasCondition);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (timerActive)
        {
            if (triggerTime > 0)
            {
                triggerTime -= GamePause.deltaTime;
            }
            else
            {
                timerActive = false;
                TutorialSystem.instance.triggerTutorial(tutorialIndex, hasCondition);
                hasTriggered = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerBase>())
        {
            if (!hasTriggered && !timerActive)
            {
                timerActive = true;
                
            }
        }
    }
}
